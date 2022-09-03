using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Configs;
using Data;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UiLevelScene : MonoBehaviour, IDisposable
    {
        public struct Ctx
        {
            public ReactiveCommand onClickMenuButton;
            public ReactiveCommand onClickRandomButton;
            public GameSet gameSet;
            public Pool pool;
            public List<CardEntity> cards;
            public ReactiveCommand<List<CardEntity>> onRemoveCards;
            public ReactiveCommand onCompactCards;
        }

        private const float FADE_TIME = 0.3f;

        private Ctx _ctx;

        [SerializeField] private Button menuButton = null;
        [SerializeField] private Button randomChangeButton = null;
        [SerializeField] private RectTransform cardsParent = null;
        [SerializeField] private RectTransform spawnPoint = null;
        [SerializeField] private RectTransform lintPoint = null;

        private Vector3 _spawnPosition;
        private Vector3 _linePosition;

        private CompositeDisposable _disposables;

        public void SetCtx(Ctx ctx)
        {
            _ctx = ctx;
            _disposables = new CompositeDisposable();

            menuButton.onClick.AddListener(() => { _ctx.onClickMenuButton.Execute(); });
            randomChangeButton.onClick.AddListener(() => _ctx.onClickRandomButton.Execute());

            _spawnPosition = spawnPoint.position;
            _linePosition = lintPoint.position;

            AddCards();
            ArrangeCards();

            _ctx.onRemoveCards.Subscribe(OnRemoveCards).AddTo(_disposables);
            _ctx.onCompactCards.Subscribe(_ => OnCompactCards()).AddTo(_disposables);
        }

        private void AddCards()
        {
            foreach (var card in _ctx.cards)
            {
                var cView = Instantiate(_ctx.gameSet.cardPrefab, cardsParent);
                cView.transform.position = _spawnPosition;

                cView.SetCtx(new CardView.Ctx
                {
                    sprite = card.sprite,
                    values = card.parameters,
                    title = card.title,
                    description = card.description,
                    changeDuration = _ctx.gameSet.paramChangeDuration,
                });

                card.view = cView;
            }
        }

        private async void ArrangeCards()
        {
            var distance = Vector3.Distance(_linePosition, _spawnPosition);
            var amount = (float) _ctx.gameSet.GetCardsAmount();
            var step = 60 / amount;
            step = Mathf.Clamp(step, 0, 10);
            var half = (step * amount - step) / 2;

            for (var i = 0; i < _ctx.cards.Count; i++)
            {
                var card = _ctx.cards[i].view.transform;
                var direction = Quaternion.Euler(0, 0, half - step * i) * Vector3.up;

                var position = _spawnPosition + direction * distance;
                card.rotation = Quaternion.Euler(0, 0, half - step * i) * card.rotation;
                card.DOMove(position, _ctx.gameSet.cardAppearDuration);
                await Task.Delay((int) (_ctx.gameSet.cardAppearDuration / 5 * 1000));
            }
        }

        private void OnRemoveCards(List<CardEntity> cards)
        {
            for (var i = 0; i < cards.Count; i++)
            {
                var card = cards[i].view.transform;
                card.DOMove(_spawnPosition, _ctx.gameSet.cardRemoveDuration);
            }
        }

        private void OnCompactCards()
        {
            var distance = Vector3.Distance(_linePosition, _spawnPosition);
            var amount = (float) _ctx.cards.Count;
            var step = 60 / amount;
            step = Mathf.Clamp(step, 0, 10);
            var half = (step * amount - step) / 2;

            for (var i = 0; i < _ctx.cards.Count; i++)
            {
                
                var card = _ctx.cards[i].view.transform;
                var direction = Quaternion.Euler(0, 0, half - step * i) * Vector3.up;

                var position = _spawnPosition + direction * distance;
                var rotation = Quaternion.Euler(0, 0, half - step * i) * Quaternion.identity;
                card.DOMove(position, _ctx.gameSet.cardCompactDuration);
                card.DORotate(rotation.eulerAngles, _ctx.gameSet.cardCompactDuration);
            }
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
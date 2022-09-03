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
    public class UiLevelScene : MonoBehaviour
    {
        public struct Ctx
        {
            public ReactiveCommand onClickMenuButton;
            public GameSet gameSet;
            public Pool pool;
            public List<CardEntity> cards;
        }

        private const float FADE_TIME = 0.3f;

        private Ctx _ctx;

        [SerializeField] private Button menuButton = null;
        [SerializeField] private RectTransform cardsParent = null;
        [SerializeField] private RectTransform spawnPoint = null;
        [SerializeField] private RectTransform lintPoint = null;

        private Vector3 _spawnPosition;
        private Vector3 _linePosition;

        public void SetCtx(Ctx ctx)
        {
            _ctx = ctx;
            menuButton.onClick.AddListener(() => { _ctx.onClickMenuButton.Execute(); });

            _spawnPosition = spawnPoint.position;
            _linePosition = lintPoint.position;

            AddCards();
            ArrangeCards();
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
                });

                card.view = cView;
            }
        }

        private async void ArrangeCards()
        {
            var distance = Vector3.Distance(_linePosition, _spawnPosition);
            var amount = (float) _ctx.gameSet.GetCardsAmount();
            var step = 60 / amount;
            var half = step * (amount / 2) - step / 2;

            var cardSpeed = 0.4f;
            for (var i = 0; i < _ctx.cards.Count; i++)
            {
                var card = _ctx.cards[i].view.transform;
                var direction = Quaternion.Euler(0, 0, half - step * i) * Vector3.up;

                var position = _spawnPosition + direction * distance;
                card.rotation = Quaternion.Euler(0, 0, half - step * i) * card.rotation;
                card.DOMove(position, cardSpeed);
                await Task.Delay((int) (cardSpeed / 5 * 1000));
            }
        }
    }
}
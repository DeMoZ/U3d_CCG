using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Configs;
using Data;
using DG.Tweening;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelScenePm : IDisposable
{
    public struct Ctx
    {
        public ReactiveCommand<GameScenes> onSwitchScene;
        public ReactiveCommand onClickMenuButton;
        public GameSet gameSet;
        public List<Sprite> sprites;
        public List<CardEntity> cards;
        public ReactiveCommand onClickRandomButton;
        
        public ReactiveProperty<Vector3> spawnPosition;
        public ReactiveProperty<Vector3> linePosition;
        public ReactiveProperty<RectTransform> dropArea;
        public ReactiveProperty<Transform> cardsParent;
        public ReactiveCommand onInstantiateCards;
    }

    private Ctx _ctx;
    private CompositeDisposable _disposables;
    private bool isChanging;
    private ReactiveCommand<int> _onCardReturn;

    public LevelScenePm(Ctx ctx)
    {
        _ctx = ctx;
        _disposables = new CompositeDisposable();

        _ctx.onClickMenuButton.Subscribe(_ => { _ctx.onSwitchScene.Execute(GameScenes.Menu); }).AddTo(_disposables);
        _ctx.onClickRandomButton.Subscribe(_ => OnClickRandomButton()).AddTo(_disposables);
        _ctx.onInstantiateCards.Subscribe(_ => OnInstantiateCards()).AddTo(_disposables);

        _onCardReturn = new ReactiveCommand<int>();
        _onCardReturn.Subscribe(OnCardReturn).AddTo(_disposables);
        
        CreateCards();
        Debug.Log($"[{this}] constructor finished");
    }

    private void CreateCards()
    {
        for (var i = 0; i < _ctx.gameSet.GetCardsAmount(); i++)
        {
            var card = new CardEntity
            {
                sprite = _ctx.sprites[i],
                title = _ctx.gameSet.GetRandomTitle(),
                description = _ctx.gameSet.GetRandomDescription(),
                parameters = new List<CardValue>
                {
                    new() {type = ParamTypes.Mana, value = _ctx.gameSet.GetRandomParamValue()},
                    new() {type = ParamTypes.Power, value = _ctx.gameSet.GetRandomParamValue()},
                    new() {type = ParamTypes.Health, value = _ctx.gameSet.GetRandomParamValue()},
                },
                index = new ReactiveProperty<int>(i),
            };

            _ctx.cards.Add(card);
        }
    }

    private async void OnClickRandomButton()
    {
        if (isChanging) return;

        isChanging = true;
        var toRemove = new List<CardEntity>();

        for (var i = _ctx.cards.Count - 1; i >= 0; i--)
        {
            var card = _ctx.cards[i];
            var parameterId = Random.Range(0, card.parameters.Count);
            var parameterType = card.parameters[parameterId].type;
            var odlValue = card.parameters[parameterId].value;
            var change = -_ctx.gameSet.GetRandomChangeParameterValue();
            var newValue = odlValue + change;
            card.parameters[parameterId].value = newValue;
            card.view.ChangeCardParam(parameterType, odlValue, newValue);

            if (parameterType == ParamTypes.Health && newValue <= 0)
            {
                _ctx.cards.RemoveAt(i);
                toRemove.Add(card);
            }
        }

        for (var i = 0; i < _ctx.cards.Count; i++)
            _ctx.cards[i].index.Value = i;

        await Task.Delay((int) (_ctx.gameSet.paramChangeDuration * 1000));

        if (toRemove.Count > 0)
        {
            await RemoveCards(toRemove);
            await OnCompactCards();
        }

        isChanging = false;
    }

    private void OnInstantiateCards()
    {
        for (var i = 0; i < _ctx.cards.Count; i++)
        {
            var card = _ctx.cards[i];
            var cView = UnityEngine.MonoBehaviour.Instantiate(_ctx.gameSet.cardPrefab, _ctx.cardsParent.Value);
            cView.transform.position = _ctx.spawnPosition.Value;

            cView.SetCtx(new CardView.Ctx
            {
                sprite = card.sprite,
                values = card.parameters,
                title = card.title,
                description = card.description,
                changeDuration = _ctx.gameSet.paramChangeDuration,
                index = card.index,
                onCardReturn = _onCardReturn,
                dropArea = _ctx.dropArea.Value,
            });

            card.view = cView;
        }

        ArrangeCards();
    }

    private async void ArrangeCards()
    {
        var distance = Vector3.Distance(_ctx.linePosition.Value, _ctx.spawnPosition.Value);
        var amount = (float) _ctx.gameSet.GetCardsAmount();
        var step = 60 / amount;
        step = Mathf.Clamp(step, 0, 10);
        var half = (step * amount - step) / 2;

        for (var i = 0; i < _ctx.cards.Count; i++)
        {
            var card = _ctx.cards[i].view.transform;
            var direction = Quaternion.Euler(0, 0, half - step * i) * Vector3.up;

            var position = _ctx.spawnPosition.Value + direction * distance;
            _ctx.cards[i].position = position;
            card.rotation = Quaternion.Euler(0, 0, half - step * i) * card.rotation;
            card.DOMove(position, _ctx.gameSet.cardAppearDuration);
            await Task.Delay((int) (_ctx.gameSet.cardAppearDuration / 5 * 1000));
        }
    }

    private async Task RemoveCards(List<CardEntity> cards)
    {
        foreach (var card in cards)
        {
            var view = card.view.transform;
            view.DOMove(_ctx.spawnPosition.Value, _ctx.gameSet.cardRemoveDuration);
        }

        await Task.Delay((int) (_ctx.gameSet.cardRemoveDuration * 1000));
    }

    private async Task OnCompactCards()
    {
        var distance = Vector3.Distance(_ctx.linePosition.Value, _ctx.spawnPosition.Value);
        var amount = (float) _ctx.cards.Count;
        var step = 60 / amount;
        step = Mathf.Clamp(step, 0, 10);
        var half = (step * amount - step) / 2;

        for (var i = 0; i < _ctx.cards.Count; i++)
        {
            var card = _ctx.cards[i].view.transform;
            var direction = Quaternion.Euler(0, 0, half - step * i) * Vector3.up;

            var position = _ctx.spawnPosition.Value + direction * distance;
            _ctx.cards[i].position = position;
            var rotation = Quaternion.Euler(0, 0, half - step * i) * Quaternion.identity;
            card.DOMove(position, _ctx.gameSet.cardCompactDuration);
            card.DORotate(rotation.eulerAngles, _ctx.gameSet.cardCompactDuration);
        }

        await Task.Delay((int) (_ctx.gameSet.cardCompactDuration * 1000));
    }

    private void OnCardReturn(int index)
    {
        var card = _ctx.cards[index];
        card.view.transform.DOMove(card.position, _ctx.gameSet.cardReturnDuration);
    }
    
    public void Dispose()
    {
        _disposables.Dispose();
    }
}
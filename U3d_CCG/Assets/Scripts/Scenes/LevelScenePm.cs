using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Configs;
using Data;
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
        public ReactiveCommand<List<CardEntity>> onRemoveCards;
        public ReactiveCommand onCompactCards;
    }

    private Ctx _ctx;
    private CompositeDisposable _disposables;
    private bool isChanging;

    public LevelScenePm(Ctx ctx)
    {
        _ctx = ctx;
        _disposables = new CompositeDisposable();

        _ctx.onClickMenuButton.Subscribe(_ => { _ctx.onSwitchScene.Execute(GameScenes.Menu); }).AddTo(_disposables);
        _ctx.onClickRandomButton.Subscribe(_ => OnClickRandomButton()).AddTo(_disposables);

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
                }
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

        await Task.Delay((int) (_ctx.gameSet.paramChangeDuration * 1000));

        if (toRemove.Count > 0)
        {
            _ctx.onRemoveCards.Execute(toRemove);
            await Task.Delay((int) (_ctx.gameSet.cardRemoveDuration * 1000));
            _ctx.onCompactCards.Execute();
            await Task.Delay((int) (_ctx.gameSet.cardCompactDuration * 1000));
        }

        isChanging = false;
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}
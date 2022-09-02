using System;
using System.Collections.Generic;
using Configs;
using Data;
using UniRx;
using UnityEngine;

public class LevelScenePm : IDisposable
{
    public struct Ctx
    {
        public ReactiveCommand<GameScenes> onSwitchScene;
        public ReactiveCommand onClickMenuButton;
        public GameSet gameSet;
        public List<Sprite> sprites;
        public List<CardEntity> cards;
    }

    private Ctx _ctx;
    private CompositeDisposable _disposables;

    public LevelScenePm(Ctx ctx)
    {
        _ctx = ctx;
        _disposables = new CompositeDisposable();

        _ctx.onClickMenuButton.Subscribe(_ => { _ctx.onSwitchScene.Execute(GameScenes.Menu); }).AddTo(_disposables);
        
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

    public void Dispose()
    {
        _disposables.Dispose();
    }
}
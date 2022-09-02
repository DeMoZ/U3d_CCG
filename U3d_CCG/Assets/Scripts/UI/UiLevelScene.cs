using System.Collections.Generic;
using Configs;
using Data;
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

        private Vector3 spawnPosition;
        public void SetCtx(Ctx ctx)
        {
            _ctx = ctx;
            menuButton.onClick.AddListener(() => { _ctx.onClickMenuButton.Execute(); });

            spawnPosition = cardsParent.position;
            AddCards();
        }

        private async void AddCards()
        {
            foreach (var card in _ctx.cards)
            {
                var cView = Instantiate(_ctx.gameSet.cardPrefab, cardsParent);
                cView.transform.localPosition = Vector3.zero;
                
                cView.SetCtx(new CardView.Ctx
                {
                    sprite = card.sprite,
                    values = card.parameters,
                    title = card.title,
                    description = card.description,
                });

            }
        }
    }
}
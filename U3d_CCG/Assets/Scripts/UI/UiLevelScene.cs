using System;
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
            public ReactiveProperty<Vector3> spawnPosition;
            public ReactiveProperty<Vector3> linePosition;
            public ReactiveProperty<RectTransform> dropArea;
            public ReactiveProperty<Transform> cardsParent;
        }

        private const float FADE_TIME = 0.3f;

        private Ctx _ctx;

        [SerializeField] private Button menuButton = null;
        [SerializeField] private Button randomChangeButton = null;
        [SerializeField] private RectTransform cardsParent = null;
        [SerializeField] private RectTransform spawnPoint = null;
        [SerializeField] private RectTransform lintPoint = null;
        [SerializeField] private RectTransform dropArea = null;

        private CompositeDisposable _disposables;

        public void SetCtx(Ctx ctx)
        {
            _ctx = ctx;
            _disposables = new CompositeDisposable();

            menuButton.onClick.AddListener(() => { _ctx.onClickMenuButton.Execute(); });
            randomChangeButton.onClick.AddListener(() => _ctx.onClickRandomButton.Execute());

            _ctx.spawnPosition.Value = spawnPoint.position;
            _ctx.linePosition.Value = lintPoint.position;
            _ctx.cardsParent.Value = cardsParent;
            _ctx.dropArea.Value = dropArea;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
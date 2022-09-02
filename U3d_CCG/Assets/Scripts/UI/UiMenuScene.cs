using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UiMenuScene: MonoBehaviour
    {
        public struct Ctx
        {
            public ReactiveCommand<GameScenes> onSwitchScene { get; set; }
        }

        [SerializeField] private Button playBtn = default;

        private Ctx _ctx;

        public void SetCtx(Ctx ctx)
        {
            _ctx = ctx;
            playBtn.onClick.AddListener(OnClickPlay);
        }

        private void OnClickPlay()
        { 
            Debug.Log("[UiMenuScene] OnClickPlay");
            _ctx.onSwitchScene.Execute(GameScenes.Level1);
        }
    }

}
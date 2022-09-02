using UI;
using UniRx;

public class MenuSceneEntity : IGameScene
{
    public struct Ctx
    {
        public GameScenes scene;
        public ReactiveCommand<GameScenes> onSwitchScene;
    }

    private Ctx _ctx;
    private UiMenuScene _ui;
    private CompositeDisposable _disposable;

    public MenuSceneEntity(Ctx ctx)
    {
        _ctx = ctx;
        _disposable = new CompositeDisposable();
    }

    public void Enter()
    {
        var menuScenePm = new MenuScenePm(new MenuScenePm.Ctx
        {
           
        }).AddTo(_disposable);
        
        // Find UI or instantiate from Addressable
        // _ui = Addressable.Instantiate();
        _ui = UnityEngine.GameObject.FindObjectOfType<UiMenuScene>();
        
        _ui.SetCtx(new UiMenuScene.Ctx
        {
            onSwitchScene = _ctx.onSwitchScene,
        });
    }

    public void Exit()
    {
    }

    public void Dispose()
    {
        _disposable.Dispose();
    }
}
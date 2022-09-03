using System.Collections.Generic;
using System.Threading.Tasks;
using Configs;
using Data;
using UI;
using UniRx;
using UnityEngine;

public class LevelSceneEntity : IGameScene
{
    public struct Ctx
    {
        public Container<Task> constructorTask;
        public ReactiveCommand<GameScenes> onSwitchScene;
    }

    private Ctx _ctx;
    private UiLevelScene _ui;
    private CompositeDisposable _disposables;
    private List<Sprite> _sprites;
    private ImageLoader _imageLoader;
    private ImageHandler _imageHandler;
    private GameSet _gameSet;
    
    public LevelSceneEntity(Ctx ctx)
    {
        _ctx = ctx;
        _disposables = new CompositeDisposable();

        _imageLoader = new ImageLoader(@"https://picsum.photos/200");
        _imageHandler = new ImageHandler(_imageLoader);
        _gameSet = Resources.Load<GameSet>("GameSet");
        
        AsyncOperations();
    }

    private void AsyncOperations()
    {
        _ctx.constructorTask.Value = ConstructorTask();
    }

    private async Task ConstructorTask()
    {
        _sprites = await _imageHandler.GetImages(_gameSet.GetCardsAmount());
    }

    public void Enter()
    {
        Debug.Log("[LevelSceneEntity] Entered");

        // todo here would be nice to show in level loading screen till the start instantiation finished..
        // todo better load prefab from resources or addressables
        _ui = UnityEngine.GameObject.FindObjectOfType<UiLevelScene>();
        //var camera = _ui.GetCamera();
        var uiPool = new Pool(new GameObject("uiPool").transform);

        var onClickMenuButton = new ReactiveCommand().AddTo(_disposables);

        var cards = new List<CardEntity>();
        var scenePm = new LevelScenePm(new LevelScenePm.Ctx
        {
            onSwitchScene = _ctx.onSwitchScene,
            onClickMenuButton = onClickMenuButton,
            gameSet = _gameSet,
            sprites = _sprites,
            cards = cards
        }).AddTo(_disposables);

        _ui.SetCtx(new UiLevelScene.Ctx
        {
            onClickMenuButton = onClickMenuButton,
            pool = uiPool,
            cards = cards,
            gameSet = _gameSet,
        });

        
    }

    public void Exit()
    {
    }

    public void Dispose()
    {
        _disposables.Dispose();
        Resources.UnloadUnusedAssets();
    }
}
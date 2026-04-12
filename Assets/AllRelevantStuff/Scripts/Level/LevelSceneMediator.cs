using Cysharp.Threading.Tasks;


public interface ILevelSceneInputsHandler
{
    void Release();
}

public class LevelSceneMediator : BaseSceneMediator<LevelSceneView>, ILevelSceneInputsHandler
{
    protected override string SceneName => "Level_{0}";
    private readonly ILevelDataService _levelDataService;

    private readonly LevelSceneController _levelSceneController;

    public LevelSceneMediator(SceneMediatorDependenices dependenices, LevelSceneController levelSceneController) : base(dependenices)
    {
        _levelSceneController = levelSceneController;
    }

    protected override UniTask DoOnInit()
    {
        _levelSceneController.Init(_view, this);
        return base.DoOnInit();
    }

    protected override string GetSceneKey()
    {
        int levelNumber = _levelDataService.CurrentLevel;
        string sceneKey = string.Format(SceneName, levelNumber);
        return sceneKey;
    }
}

using Cysharp.Threading.Tasks;
using UI;


public interface ISceneMediator : IView
{
    UniTask Init(UnityEngine.Camera camera, int sortingOrder);
    void ChangeCanvasSorting(int sorting);
    UniTask ShowSceneAdditive();
    UniTask UnloadScene();
}

public abstract class BaseSceneMediator<TSceneView> : ISceneMediator where TSceneView : ISceneView
{
    protected abstract string SceneName { get; }

    protected readonly ISceneService _sceneService;
    protected readonly ISceneUIManager _sceneManager;

    protected TSceneView _view;

    protected BaseSceneMediator(SceneMediatorDependenices dependenices)
    {
        _sceneService = dependenices.SceneService;
        _sceneManager = dependenices.SceneManager;
    }

    public async UniTask ShowSceneAdditive()
    {
        await _sceneManager.ShowSceneAdditive(this);
    }

    public async UniTask UnloadScene()
    {
        await _sceneManager.ReleaseScene(this);
    }

    public async UniTask Init(UnityEngine.Camera camera, int sortingOrder)
    {
        string sceneKey = GetSceneKey();
        var view = await _sceneService.LoadSceneAsync(sceneKey, UnityEngine.SceneManagement.LoadSceneMode.Additive);
        view.Init(camera, sortingOrder);
        _view = (TSceneView)view;
        await DoOnInit();
    }

    public virtual void Show()
    {
        _view.Show();
    }

    public virtual void Hide()
    {
        _view.Hide();
    }

    public void Release()
    {
        DoOnRelease();
        _sceneService.UnloadSceneAsync(SceneName);
    }

    public void ChangeCanvasSorting(int sorting)
    {
        _view.ChangeCanvasSorting(sorting);
    }

    protected virtual string GetSceneKey()
    {
        return SceneName;
    }

    protected virtual async UniTask DoOnInit()
    {
    }

    protected virtual void DoOnRelease()
    {
    }
}

public class SceneMediatorDependenices
{
    public ISceneService SceneService { get; }
    public ISceneUIManager SceneManager { get; }
    public ILocalizationManager LocalizationManager { get; }
    public ILevelDataService LevelDataService { get; }

    public SceneMediatorDependenices(ISceneService sceneService, ISceneUIManager sceneManager,
        ILocalizationManager localizationManager, ILevelDataService levelDataService)
    {
        SceneService = sceneService;
        SceneManager = sceneManager;
        LocalizationManager = localizationManager;
        LevelDataService = levelDataService;
    }
}

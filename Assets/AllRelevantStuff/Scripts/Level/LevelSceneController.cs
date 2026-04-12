public class LevelSceneController
{
    private LevelSceneView _view;
    private LevelTrigger _trigger;

    private LevelCompletedPopupMediator _levelCompletedMediator;
    private ILevelSceneInputsHandler _inputsHandler;

    public LevelSceneController(LevelCompletedPopupMediator levelCompletedMediator)
    {
        _levelCompletedMediator = levelCompletedMediator;
    }

    public void Init(LevelSceneView view, ILevelSceneInputsHandler inputsHandler)
    {
        _view = view;
        _trigger = view.Trigger;
        _inputsHandler = inputsHandler;
        _trigger.OnPlayerExitLevel += OnLevelEnd;
    }

    private void OnLevelEnd()
    {
        _levelCompletedMediator.CreatePopup();
        _levelCompletedMediator.OnClosedPopup += DoOnClose;
        _trigger.OnPlayerExitLevel -= OnLevelEnd;
    }

    private void DoOnClose()
    {
        _levelCompletedMediator.OnClosedPopup -= DoOnClose;
        _inputsHandler.Release();
    }
}

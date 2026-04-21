using static UnityEngine.InputSystem.InputAction;

public class LevelSceneController
{
    private readonly IEventBus _eventBus;
    private LevelCompletedPopupMediator _levelCompletedMediator;
    private LevelPausePopupMediator _levelPauseMediator;
    private ILevelSceneInputsHandler _inputsHandler;
    private LevelSceneView _view;
    private LevelTrigger _trigger;

    public LevelSceneController(LevelCompletedPopupMediator levelCompletedMediator,
        LevelPausePopupMediator pausePopupMediator,
        IEventBus eventBus)
    {
        _levelCompletedMediator = levelCompletedMediator;
        _levelPauseMediator = pausePopupMediator;
        _eventBus = eventBus;
    }

    public void Init(LevelSceneView view, ILevelSceneInputsHandler inputsHandler)
    {
        _view = view;
        _trigger = view.Trigger;
        _inputsHandler = inputsHandler;

        _trigger.OnPlayerExitLevel += OnLevelEnd;
        InputReader.Actions.UI.Pause.performed += OnPauseButtonPressed;

        _eventBus.Subscribe<SendToMainMenuEventArgs>(OnSendToMainMenu);
        _eventBus.Subscribe<SendToLevelEventArgs>(OnSendToLevel);
    }

    private void OnLevelEnd()
    {
        _levelCompletedMediator.CreatePopup();
        _levelCompletedMediator.OnClosedPopup += DoOnLevelCompletedPopupClosed;

        PlayerControlInput inputActions = new PlayerControlInput();
        inputActions.InGame.Disable();

        InputReader.Actions.InGame.Disable();
        InputReader.Actions.UI.Disable();
        _trigger.OnPlayerExitLevel -= OnLevelEnd;
    }

    private void DoOnLevelCompletedPopupClosed()
    {
        _levelCompletedMediator.OnClosedPopup -= DoOnLevelCompletedPopupClosed;
        InputReader.Actions.UI.Pause.performed -= OnPauseButtonPressed;
    }

    private void OnPauseButtonPressed(CallbackContext callbackContext)
    {
        _levelPauseMediator.CreatePopup();
        _levelPauseMediator.OnClosedPopup += OnPauseLevelPopupClosed;

        InputReader.Actions.InGame.Disable();
        InputReader.Actions.UI.Pause.performed -= OnPauseButtonPressed;
    }

    private void OnPauseLevelPopupClosed()
    {
        InputReader.Actions.InGame.Enable();
        InputReader.Actions.UI.Pause.performed += OnPauseButtonPressed;

        _levelPauseMediator.OnClosedPopup -= OnPauseLevelPopupClosed;
    }

    private async void OnSendToMainMenu(SendToMainMenuEventArgs args)
    {
        _inputsHandler.Release();
        InputReader.Actions.UI.Pause.performed -= OnPauseButtonPressed;

        _eventBus.Unsubscribe<SendToMainMenuEventArgs>(OnSendToMainMenu);
    }

    private async void OnSendToLevel(SendToLevelEventArgs args)
    {
        _inputsHandler.Release();
        InputReader.Actions.UI.Pause.performed -= OnPauseButtonPressed;

        _eventBus.Unsubscribe<SendToLevelEventArgs>(OnSendToLevel);
    }
}

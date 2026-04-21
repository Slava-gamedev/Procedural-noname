using Zenject;

public class SendToSceneEventListener
{
    private readonly IEventBus _eventBus;
    private readonly MainMenuSceneMediator _mainMenuSceneMediator;
    private readonly DiContainer _container;

    public SendToSceneEventListener(IEventBus eventBus, MainMenuSceneMediator mainMenuSceneMediator,
        DiContainer container)
    {
        _eventBus = eventBus;
        _mainMenuSceneMediator = mainMenuSceneMediator;
        _container = container;
    }

    public void Init()
    {
        _eventBus.Unsubscribe<SendToMainMenuEventArgs>(OnSendToMainMenu);
        _eventBus.Subscribe<SendToMainMenuEventArgs>(OnSendToMainMenu);

        _eventBus.Unsubscribe<SendToLevelEventArgs>(OnSendToLevel);
        _eventBus.Subscribe<SendToLevelEventArgs>(OnSendToLevel);
    }

    private async void OnSendToMainMenu(SendToMainMenuEventArgs args)
    {
        InputReader.Actions.UI.Disable();
        InputReader.Actions.InGame.Disable();

        await _mainMenuSceneMediator.ShowSceneSingle();
    }

    private async void OnSendToLevel(SendToLevelEventArgs args)
    {
        InputReader.Actions.UI.Enable();
        InputReader.Actions.InGame.Enable();

        LevelSceneMediator levelSceneMediator = _container.Resolve<LevelSceneMediator>();

        await levelSceneMediator.ShowSceneSingle();
    }
}

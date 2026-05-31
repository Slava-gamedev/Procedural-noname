using Cysharp.Threading.Tasks;
using UI;
using UI.Settings;
using UnityEngine;


public class MainMenuSceneMediator : BaseSceneMediator<MainMenuSceneView>
{
    private const string kTable = "MainMenu";
    private const string kPlay = "Play";
    private const string kSettings = "Settings";
    private const string kExit = "Exit";
    private const string kBack = "Back";

    protected override string SceneName => "MainMenu";


    private readonly ILevelDataService _levelDataService;
    private readonly ILocalizationManager _localizationManager;
    private readonly IEventBus _eventBus;
    private readonly IKeyRebindingController _rebindingController;

    public MainMenuSceneMediator(SceneMediatorDependenices dependenices, ILevelDataService levelDataService,
        IEventBus eventBus, IKeyRebindingController rebindingController) : base(dependenices)
    {
        _levelDataService = levelDataService;
        _localizationManager = dependenices.LocalizationManager;
        _eventBus = eventBus;
        _rebindingController = rebindingController;
    }

    protected override async UniTask DoOnInit()
    {
        string playText = await _localizationManager.GetLocalizedStringAsync(kTable, kPlay);
        string settingsText = await _localizationManager.GetLocalizedStringAsync(kTable, kSettings);
        string exitText = await _localizationManager.GetLocalizedStringAsync(kTable, kExit);
        string backText = await _localizationManager.GetLocalizedStringAsync(kTable, kBack);

        _view.MainMenuView.Init(playText, settingsText, exitText);
        _view.SettingsMenuView.Init(backText);

        SubscribeInputs();
    }

    protected override void DoOnRelease()
    {
        UnsubscribeInputs();
        _view.Release();
    }

    private void SubscribeInputs()
    {
        _view.MainMenuView.OnSettingsPressed += OnSettingPressed;
        _view.MainMenuView.OnPlayPressed += OnPlayClicked;
        _view.MainMenuView.OnExitPressed += OnExitClicked;
        _view.SettingsMenuView.OnBackPressed += OnSettingClosed;
    }

    private void UnsubscribeInputs()
    {
        _view.MainMenuView.OnSettingsPressed -= OnSettingPressed;
        _view.MainMenuView.OnPlayPressed -= OnPlayClicked;
        _view.MainMenuView.OnExitPressed -= OnExitClicked;
        _view.SettingsMenuView.OnBackPressed -= OnSettingClosed;
    }

    private void OnSettingPressed()
    {
        _view.MainMenuView.Hide();
        _view.SettingsMenuView.Show();

        RebindOverlayMapper mapper = _view.SettingsMenuView.RebindOverlayMapper;
        _rebindingController.Init(InputReader.Actions.InGame, mapper);
    }

    private void OnSettingClosed()
    {
        _view.MainMenuView.Show();
        _view.SettingsMenuView.Hide();
        _rebindingController.Release();
    }

    private async void OnPlayClicked()
    {
        _levelDataService.SetLevel(1);

        Release();

        SendToLevelEventArgs args = new SendToLevelEventArgs();
        _eventBus.InvokeEvent<SendToLevelEventArgs>(args);
    }

    private void OnExitClicked()
    {
        Application.Quit();
    }
}

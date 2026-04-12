using Cysharp.Threading.Tasks;
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
    private readonly LevelSceneMediator _levelMediator;

    public MainMenuSceneMediator(SceneMediatorDependenices dependenices, ILevelDataService levelDataService,
        LevelSceneMediator levelMediator) : base(dependenices)
    {
        _levelDataService = levelDataService;
        _localizationManager = dependenices.LocalizationManager;
        _levelMediator = levelMediator;
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

        base.DoOnRelease();
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
    }

    private void OnSettingClosed()
    {
        _view.MainMenuView.Show();
        _view.SettingsMenuView.Hide();
    }

    private async void OnPlayClicked()
    {
        _levelDataService.SetLevel(1);

        await _sceneManager.ReleaseScene(this);
        await _levelMediator.ShowSceneAdditive();
    }

    private void OnExitClicked()
    {
        Application.Quit();
    }
}

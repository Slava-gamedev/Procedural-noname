using UI;
using UnityEngine;

public class SettingsMenuController : MonoBehaviour
{
    private const string kTable = "MainMenu";
    private const string kBack = "Back";

    [SerializeField] private SettingsMenuView _view;
    [SerializeField] private MainMenuController _mainMenu;
    private SettingsMenuModel _model;
    private LocalizationManager _localizationManager;

    public void Show()
    {
        _view.gameObject.SetActive(true);
    }

    public void Hide()
    {
        _view.gameObject.SetActive(false);
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _localizationManager = LocalizationManager.Instance;

        BuildModel();
        _view.Init(_model);

        UnsubscribeFromInputs();
        SubscribeToInputs();
    }

    private void BuildModel()
    {
        SettingsMenuModel model = new SettingsMenuModel();

        model.BackText = _localizationManager.GetLocalizedString(kTable, kBack);
        _model = model;
    }

    private void SubscribeToInputs()
    {
        _view.OnBackPressed += OnBackClicked;
    }

    private void UnsubscribeFromInputs()
    {
        _view.OnBackPressed -= OnBackClicked;
    }

    private void OnBackClicked()
    {
        Hide();
        _mainMenu.Show();
    }
}

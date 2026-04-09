using UI;
using UnityEngine;

public class SettingsMenuController : BasePopupController<SettingsMenuModel>
{
    protected override string kTable => "MainMenu";
    private const string kBack = "Back";

    [SerializeField] private SettingsMenuView _view;
    [SerializeField] private MainMenuController _mainMenu;

    public override void Show()
    {
        _view.Show();
    }

    public override void Hide()
    {
        _view.Hide();
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _model = BuildModel();
        _view.Init(_model);

        UnsubscribeFromInputs();
        SubscribeToInputs();
    }

    protected override SettingsMenuModel BuildModel()
    {
        SettingsMenuModel model = new SettingsMenuModel();

        model.BackText = _localizationManager.GetLocalizedString(kTable, kBack);
        return model;
    }

    protected override void SubscribeToInputs()
    {
        _view.OnBackPressed += OnBackClicked;
    }

    protected override void UnsubscribeFromInputs()
    {
        _view.OnBackPressed -= OnBackClicked;
    }

    private void OnBackClicked()
    {
        Hide();
        _mainMenu.Show();
    }
}

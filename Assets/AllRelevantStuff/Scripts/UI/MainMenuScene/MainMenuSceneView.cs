using UI;
using UnityEngine;

public class MainMenuSceneView : BaseSceneView
{
    [SerializeField] private MainMenuView _mainMenuView;
    [SerializeField] private SettingsMenuView _settingsMenuView;

    public MainMenuView MainMenuView => _mainMenuView;
    public SettingsMenuView SettingsMenuView => _settingsMenuView;


    public override void Release()
    {
        _mainMenuView.Release();
        _settingsMenuView.Release();
    }
}

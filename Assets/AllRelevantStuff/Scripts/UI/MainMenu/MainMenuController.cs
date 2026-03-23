using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainMenuController : MonoBehaviour
    {
        private const string kTable = "MainMenu";
        private const string kPlay = "Play";
        private const string kSettings = "Settings";
        private const string kExit = "Exit";
        private const string kScene = "Level_1";

        [SerializeField] private MainMenuView _view;
        [SerializeField] private SettingsMenuController _settingsMenu;
        private MainMenuModel _model;
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
            MainMenuModel model = new MainMenuModel();

            model.PlayText = _localizationManager.GetLocalizedString(kTable, kPlay);
            model.SettingsText = _localizationManager.GetLocalizedString(kTable, kSettings);
            model.ExitText = _localizationManager.GetLocalizedString(kTable, kExit);
            _model = model;
        }

        private void SubscribeToInputs()
        {
            _view.OnPlayPressed += OnPlayClicked;
            _view.OnSettingsPressed += OnSettingsClicked;
            _view.OnExitPressed += OnExitClick;
        }

        private void UnsubscribeFromInputs()
        {
            _view.OnPlayPressed -= OnPlayClicked;
            _view.OnSettingsPressed -= OnSettingsClicked;
            _view.OnExitPressed -= OnExitClick;
        }

        private void OnPlayClicked()
        {
            SceneManager.LoadScene(kScene);
        }

        private void OnSettingsClicked()
        {
            Hide();
            _settingsMenu.Show();
        }

        private void OnExitClick()
        {
            Application.Quit();
        }
    }
}

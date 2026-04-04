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

        [SerializeField] private MainMenuView _view;
        [SerializeField] private SettingsMenuController _settingsMenu;
        private MainMenuModel _model;
        private LocalizationManager _localizationManager;
        private LevelDataService _levelDataService;
        private const string kLevelSceneFormat = "Level_{0}";

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
            _levelDataService = LevelDataService.Instance;

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
            _levelDataService.SetLevel(1);

            int levelNumber = _levelDataService.CurrentLevel;
            string levelName = string.Format(kLevelSceneFormat, levelNumber);
            SceneManager.LoadScene(levelName);
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

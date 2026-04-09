using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MainMenuController : BasePopupController<MainMenuModel>
    {
        protected override string kTable => "MainMenu";
        private const string kPlay = "Play";
        private const string kSettings = "Settings";
        private const string kExit = "Exit";

        [SerializeField] private MainMenuView _view;
        [SerializeField] private SettingsMenuController _settingsMenu;
        private LevelDataService _levelDataService;
        private const string kLevelSceneFormat = "Level_{0}";

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
            _levelDataService = LevelDataService.Instance;

            _model = BuildModel();
            _view.Init(_model);

            UnsubscribeFromInputs();
            SubscribeToInputs();
        }

        protected override MainMenuModel BuildModel()
        {
            MainMenuModel model = new MainMenuModel();

            model.PlayText = _localizationManager.GetLocalizedString(kTable, kPlay);
            model.SettingsText = _localizationManager.GetLocalizedString(kTable, kSettings);
            model.ExitText = _localizationManager.GetLocalizedString(kTable, kExit);

            return model;
        }

        protected override void SubscribeToInputs()
        {
            _view.OnPlayPressed += OnPlayClicked;
            _view.OnSettingsPressed += OnSettingsClicked;
            _view.OnExitPressed += OnExitClick;
        }

        protected override void UnsubscribeFromInputs()
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

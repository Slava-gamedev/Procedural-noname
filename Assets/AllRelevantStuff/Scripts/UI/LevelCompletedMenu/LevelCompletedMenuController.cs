using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class LevelCompletedMenuController : BasePopupController<LevelCompletedMenuModel>
    {
        protected override string kTable => "LevelCompleted";
        private const string kMainMenu = "MainMenu";
        private const string kNextLevel = "NextLevel";
        private const string kCongratulations = "Congratulations";

        private const string kMainMenuScene = "MainMenu";
        private const string kLevelSceneFormat = "Level_{0}";

        [SerializeField] private LevelCompletedMenuView _view;

        private LevelDataService _levelDataService;

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

        protected override LevelCompletedMenuModel BuildModel()
        {
            LevelCompletedMenuModel model = new LevelCompletedMenuModel();

            model.NextLevelText = _localizationManager.GetLocalizedString(kTable, kNextLevel);
            model.MainMenuText = _localizationManager.GetLocalizedString(kTable, kMainMenu);
            model.CongratulationsText = _localizationManager.GetLocalizedString(kTable, kCongratulations);
            model.IsLastLevel = _levelDataService.IsLastLevel;

            return model;
        }

        protected override void SubscribeToInputs()
        {
            _view.OnNextLevelPressed += LoadNextLevel;
            _view.OnMainMenuPressed += LoadMainMenu;
        }

        protected override void UnsubscribeFromInputs()
        {
            _view.OnNextLevelPressed -= LoadNextLevel;
            _view.OnMainMenuPressed -= LoadMainMenu;
        }

        private void LoadNextLevel()
        {
            int nextLevelNumber = _levelDataService.CurrentLevel + 1;
            _levelDataService.SetLevel(nextLevelNumber);

            int levelNumber = _levelDataService.CurrentLevel;
            string levelName = string.Format(kLevelSceneFormat, levelNumber);
            SceneManager.LoadScene(levelName);
        }

        private void LoadMainMenu()
        {
            SceneManager.LoadScene(kMainMenuScene);
        }
    }
}

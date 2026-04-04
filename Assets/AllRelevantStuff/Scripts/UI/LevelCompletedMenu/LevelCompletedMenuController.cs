using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class LevelCompletedMenuController : MonoBehaviour
    {
        private const string kTable = "LevelCompleted";
        private const string kMainMenu = "MainMenu";
        private const string kNextLevel = "NextLevel";
        private const string kCongratulations = "Congratulations";

        private const string kMainMenuScene = "MainMenu";
        private const string kLevelSceneFormat = "Level_{0}";

        [SerializeField] private LevelCompletedMenuView _view;

        private LevelCompletedMenuModel _model;
        private LocalizationManager _localizationManager;
        private LevelDataService _levelDataService;

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
            LevelCompletedMenuModel model = new LevelCompletedMenuModel();

            model.NextLevelText = _localizationManager.GetLocalizedString(kTable, kNextLevel);
            model.MainMenuText = _localizationManager.GetLocalizedString(kTable, kMainMenu);
            model.CongratulationsText = _localizationManager.GetLocalizedString(kTable, kCongratulations);
            model.IsLastLevel = _levelDataService.IsLastLevel;
            _model = model;
        }

        private void SubscribeToInputs()
        {
            _view.OnNextLevelPressed += LoadNextLevel;
            _view.OnMainMenuPressed += LoadMainMenu;
        }

        private void UnsubscribeFromInputs()
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

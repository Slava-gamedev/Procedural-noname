using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace UI
{
    public class LevelCompletedPopupController : BasePopupController<LevelCompletedPopupView, LevelCompletedPopupModel, LevelCompletedPopupMediator>
    {
        protected override string kTable => "LevelCompleted";
        private const string kMainMenu = "MainMenu";
        private const string kNextLevel = "NextLevel";
        private const string kCongratulations = "Congratulations";

        private readonly ILevelDataService _levelDataService;
        private readonly MainMenuSceneMediator _mainMenuSceneMediator;
        private readonly LevelSceneMediator _levelSceneMediator;

        public LevelCompletedPopupController(PopupControllerDependenices dependenices,
            ILevelDataService levelDataService,
            MainMenuSceneMediator mainMenuSceneMediator,
            LevelSceneMediator levelSceneMediator) : base(dependenices)
        {
            _levelDataService = levelDataService;
            _mainMenuSceneMediator = mainMenuSceneMediator;
            _levelSceneMediator = levelSceneMediator;
        }

        public override void Show()
        {
            _view.Show();
        }

        public override void Hide()
        {
            _view.Hide();
        }

        protected override async UniTask DoOnInit()
        {
            _view.Init(_model);

            UnsubscribeFromInputs();
            SubscribeToInputs();
        }

        protected override async UniTask<LevelCompletedPopupModel> BuildModel()
        {
            LevelCompletedPopupModel model = new LevelCompletedPopupModel();

            model.NextLevelText = await _localizationManager.GetLocalizedStringAsync(kTable, kNextLevel);
            model.MainMenuText = await _localizationManager.GetLocalizedStringAsync(kTable, kMainMenu);
            model.CongratulationsText = await _localizationManager.GetLocalizedStringAsync(kTable, kCongratulations);
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

        private async void LoadNextLevel()
        {
            int nextLevelNumber = _levelDataService.CurrentLevel + 1;
            _levelDataService.SetLevel(nextLevelNumber);

            _mediator.ClosePopup();
            await _levelSceneMediator.ShowSceneAdditive();
        }

        private async void LoadMainMenu()
        {
            _mediator.ClosePopup();
            await _mainMenuSceneMediator.ShowSceneAdditive();
        }
    }
}

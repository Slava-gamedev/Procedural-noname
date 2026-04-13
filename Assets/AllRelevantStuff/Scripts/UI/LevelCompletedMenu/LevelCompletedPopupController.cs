using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
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
        private readonly IEventBus _eventBus;

        public LevelCompletedPopupController(PopupControllerDependenices dependenices,
            ILevelDataService levelDataService,
            IEventBus eventBus) : base(dependenices)
        {
            _levelDataService = levelDataService;
            _eventBus = eventBus;
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

            SendToLevelEventArgs args = new SendToLevelEventArgs();
            _eventBus.InvokeEvent<SendToLevelEventArgs>(args);
        }

        private async void LoadMainMenu()
        {
            _mediator.ClosePopup();

            SendToMainMenuEventArgs args = new SendToMainMenuEventArgs();
            _eventBus.InvokeEvent<SendToMainMenuEventArgs>(args);
        }
    }
}

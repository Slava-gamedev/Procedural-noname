using Cysharp.Threading.Tasks;

namespace UI
{
    public class LevelPausePopupController : BasePopupController<LevelPausePopupView, LevelPausePopupModel, LevelPausePopupMediator>
    {
        protected override string kTable => "LevelPause";
        private const string kMainMenu = "MainMenu";
        private const string kCloseMenu = "CloseMenu";

        private readonly IEventBus _eventBus;

        public LevelPausePopupController(PopupControllerDependenices dependenices,
            IEventBus eventBus) : base(dependenices)
        {
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

        protected override async UniTask<LevelPausePopupModel> BuildModel()
        {
            LevelPausePopupModel model = new LevelPausePopupModel();

            model.MainMenuText = await _localizationManager.GetLocalizedStringAsync(kTable, kMainMenu);
            model.CloseMenuText = await _localizationManager.GetLocalizedStringAsync(kTable, kCloseMenu);

            return model;
        }

        protected override void SubscribeToInputs()
        {
            _view.OnCloseMenuPressed += CloseMenu;
            _view.OnMainMenuPressed += LoadMainMenu;
        }

        protected override void UnsubscribeFromInputs()
        {
            _view.OnCloseMenuPressed -= CloseMenu;
            _view.OnMainMenuPressed -= LoadMainMenu;
        }

        private async void CloseMenu()
        {
            _mediator.ClosePopup();
        }

        private async void LoadMainMenu()
        {
            _mediator.ClosePopup();

            SendToMainMenuEventArgs args = new SendToMainMenuEventArgs();
            _eventBus.InvokeEvent<SendToMainMenuEventArgs>(args);
        }
    }
}

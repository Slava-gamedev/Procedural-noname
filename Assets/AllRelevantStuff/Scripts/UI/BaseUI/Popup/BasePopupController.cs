using Cysharp.Threading.Tasks;

namespace UI
{

    public interface IPopupController : IView
    {
        UniTask Init(IPopupView view, IPopupMediator mediator);
    }

    public abstract class BasePopupController<TPopupView, TPopupModel, TPopupMediator> : IPopupController
        where TPopupModel : IPopupModel
        where TPopupMediator : IPopupMediator
        where TPopupView : IPopupView
    {
        protected abstract string kTable { get; }

        protected readonly ILocalizationManager _localizationManager;

        protected TPopupModel _model;
        protected TPopupView _view;
        protected TPopupMediator _mediator;

        protected BasePopupController(PopupControllerDependenices dependenices)
        {
            _localizationManager = dependenices.LocalizationManager;
        }

        public async UniTask Init(IPopupView view, IPopupMediator mediator)
        {
            _view = (TPopupView)view;
            _mediator = (TPopupMediator)mediator;

            _model = await BuildModel();

            await DoOnInit();
        }

        public virtual void Show() { }
        public virtual void Hide() { }
        public virtual void Release() { }

        protected abstract UniTask<TPopupModel> BuildModel();
        protected abstract UniTask DoOnInit();

        protected virtual void SubscribeToInputs() { }
        protected virtual void UnsubscribeFromInputs() { }
    }

    public class PopupControllerDependenices
    {
        public ILocalizationManager LocalizationManager { get; }
        
        public PopupControllerDependenices(ILocalizationManager localizationManager)
        {
            LocalizationManager = localizationManager;
        }
    }
}

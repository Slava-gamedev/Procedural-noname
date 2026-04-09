using UI;
using UnityEngine;

namespace UI
{

    public abstract class BasePopupController<TPopupModel> : MonoBehaviour where TPopupModel : IPopupModel
    {
        protected abstract string kTable { get; }

        protected readonly LocalizationManager _localizationManager = LocalizationManager.Instance;

        protected TPopupModel _model;

        public virtual void Show() { }

        public virtual void Hide() { }

        protected abstract TPopupModel BuildModel();
        protected virtual void SubscribeToInputs() { }
        protected virtual void UnsubscribeFromInputs() { }
    }
}

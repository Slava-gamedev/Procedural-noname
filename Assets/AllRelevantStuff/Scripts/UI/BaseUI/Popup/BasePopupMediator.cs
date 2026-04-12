using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace UI
{

    public interface IPopupContext : IView
    {
        UniTask InitPopup(Camera camera, Transform parent, int orderLayer = 0);
        void SetCanvasOrder(int orderLayer);
    }

    public interface IPopupMediator
    {
        event Action OnClosedPopup;
        void CreatePopup();
        void ClosePopup();
    }

    public abstract class BasePopupMediator<TView> : IPopupMediator, IPopupContext where TView : IPopupView
    {
        protected abstract PopupType _popupType { get; }

        protected readonly IUIManager _UIManager;
        protected readonly IAddressablePopupProvider _popupProvider;
        protected TView _view;

        public event Action OnClosedPopup;

        protected BasePopupMediator(PopupMediatorDependenices dependenices)
        {
            _UIManager = dependenices.UIManager;
            _popupProvider = dependenices.PopupProvider;
        }

        public virtual async UniTask InitPopup(Camera camera, Transform parent, int orderLayer = 0)
        {
            _view = await _popupProvider.InstantiatePopupAsync<TView>(parent,_popupType);
            _view.Init(camera, orderLayer);
        }

        public void SetCanvasOrder(int orderLayer)
        {
            _view.SetCanvasOrder(orderLayer);
        }

        public void CreatePopup()
        {
            _UIManager.OpenPopup(this);
        }

        public void ClosePopup()
        {
            _UIManager.ClosePopup(this);
            OnClosedPopup?.Invoke();
        }

        public virtual void Show()
        {
            _view.Show();
        }

        public virtual void Hide()
        {
            _view.Hide();
        }

        public virtual void Release()
        {
            OnClosedPopup -= ClosePopup;
            _view.Release();
        }
    }

    public class PopupMediatorDependenices
    {
        public IUIManager UIManager { get; }

        public IAddressablePopupProvider PopupProvider {  get; }


        public PopupMediatorDependenices(IUIManager uIManager, IAddressablePopupProvider popupProvider)
        {
            UIManager = uIManager;
            PopupProvider = popupProvider;
        }
    }
}

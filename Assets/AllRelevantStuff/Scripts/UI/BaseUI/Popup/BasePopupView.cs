using UnityEngine;

namespace UI
{

    public interface IView
    {
        void Show();
        void Hide();
        void Release();
    }

    public interface IPopupView : IView
    {
        void Init(Camera camera, int sortingOrder);
        void SetCanvasOrder(int sortingOrder);
    }

    public abstract class BasePopupView : MonoBehaviour, IPopupView
    {
        [SerializeField] protected Canvas _canvas;

        public void Init(Camera camera, int sortingOrder)
        {
            _canvas.worldCamera  = camera;
            _canvas.sortingOrder = sortingOrder;
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void Release()
        {
            UnsubscribeButtons();
            Destroy(gameObject);
        }

        public void SetCanvasOrder(int sortingOrder)
        {
            _canvas.sortingOrder = sortingOrder;
        }

        protected virtual void SubscribeButtons() { }
        protected virtual void UnsubscribeButtons() { }
    }
}

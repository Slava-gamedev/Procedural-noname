using UnityEditor.Search;
using UnityEngine;

namespace UI
{
    public abstract class BasePopupView : MonoBehaviour
    {
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

        protected virtual void SubscribeButtons() { }
        protected virtual void UnsubscribeButtons() { }
    }
}

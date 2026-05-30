using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SettingsMenuView : MonoBehaviour
    {
        public event Action OnBackPressed;

        [SerializeField] private Button _backButton;
        [SerializeField] private TMP_Text _backText;
        [SerializeField] private TMP_Text _rebindOverlayText;
        [SerializeField] private GameObject _rebindOverlay;

        public void Init(string backText)
        {
            _backText.text = backText;
            UnsubscribeButtons();
            SubscribeButtons();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Release()
        {
            UnsubscribeButtons();
        }

        protected void SubscribeButtons()
        {
            _backButton.onClick.AddListener(OnBackButtonPressed);
        }

        protected void UnsubscribeButtons()
        {
            _backButton.onClick.RemoveListener(OnBackButtonPressed);
        }

        private void OnBackButtonPressed()
        {
            OnBackPressed?.Invoke();
        }
    }
}

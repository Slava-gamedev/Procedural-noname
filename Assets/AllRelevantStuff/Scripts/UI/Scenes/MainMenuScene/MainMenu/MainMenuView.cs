using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MainMenuView : MonoBehaviour
    {
        public event Action OnPlayPressed;
        public event Action OnSettingsPressed;
        public event Action OnExitPressed;

        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _exitButton;
        [SerializeField] private TMP_Text _playText;
        [SerializeField] private TMP_Text _settingsText;
        [SerializeField] private TMP_Text _exitText;

        public void Init(string playText, string settingsText, string exitText)
        {
            _playText.text = playText;
            _settingsText.text = settingsText;
            _exitText.text = exitText;

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
            _playButton.onClick.AddListener(OnPlayButtonPressed);
            _settingsButton.onClick.AddListener(OnSettingsButtonPressed);
            _exitButton.onClick.AddListener(OnExitButtonPressed);
        }

        protected void UnsubscribeButtons()
        {
            _playButton.onClick.RemoveListener(OnPlayButtonPressed);
            _settingsButton.onClick.RemoveListener(OnSettingsButtonPressed);
            _exitButton.onClick.RemoveListener(OnExitButtonPressed);
        }

        private void OnPlayButtonPressed()
        {
            OnPlayPressed?.Invoke();
        }

        private void OnSettingsButtonPressed()
        {
            OnSettingsPressed?.Invoke();
        }

        private void OnExitButtonPressed()
        {
            OnExitPressed?.Invoke();
        }
    }
}

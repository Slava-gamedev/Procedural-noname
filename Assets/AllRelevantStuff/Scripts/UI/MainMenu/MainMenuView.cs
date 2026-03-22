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

        public void Init(MainMenuModel model)
        {
            _playText.text = model.PlayText;
            _settingsText.text = model.SettingsText;
            _exitText.text = model.ExitText;

            UnsubscribeButtons();
            SubscribeButtons();
        }

        private void SubscribeButtons()
        {
            _playButton.onClick.AddListener(OnPlayButtonPressed);
            _settingsButton.onClick.AddListener(OnSettingsButtonPressed);
            _exitButton.onClick.AddListener(OnExitButtonPressed);
        }

        private void UnsubscribeButtons()
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

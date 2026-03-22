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

        public void Init(SettingsMenuModel model)
        {
            _backText.text = model.BackText;
            UnsubscribeButtons();
            SubscribeButtons();
        }

        private void SubscribeButtons()
        {
            _backButton.onClick.AddListener(OnBackButtonPressed);
        }

        private void UnsubscribeButtons()
        {
            _backButton.onClick.RemoveListener(OnBackButtonPressed);
        }

        private void OnBackButtonPressed()
        {
            OnBackPressed?.Invoke();
        }
    }
}

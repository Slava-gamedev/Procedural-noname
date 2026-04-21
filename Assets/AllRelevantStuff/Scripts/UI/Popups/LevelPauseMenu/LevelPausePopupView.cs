using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LevelPausePopupView : BasePopupView
    {
        public event Action OnCloseMenuPressed;
        public event Action OnMainMenuPressed;

        [SerializeField] private Button _closeMenuButton;
        [SerializeField] private Button _mainMenuButton;
        [SerializeField] private TMP_Text _closeMenuText;
        [SerializeField] private TMP_Text _mainMenuText;

        public void Init(LevelPausePopupModel model)
        {
            _closeMenuText.text = model.CloseMenuText;
            _mainMenuText.text = model.MainMenuText;

            UnsubscribeButtons();
            SubscribeButtons();
        }

        protected override void SubscribeButtons()
        {
            _closeMenuButton.onClick.AddListener(OnCloseMenuButtonPressed);
            _mainMenuButton.onClick.AddListener(OnMainMenuButtonPressed);
        }

        protected override void UnsubscribeButtons()
        {
            _closeMenuButton.onClick.RemoveListener(OnCloseMenuButtonPressed);
            _mainMenuButton.onClick.RemoveListener(OnMainMenuButtonPressed);
        }

        private void OnCloseMenuButtonPressed()
        {
            OnCloseMenuPressed?.Invoke();
        }

        private void OnMainMenuButtonPressed()
        {
            OnMainMenuPressed?.Invoke();
        }
    }
}

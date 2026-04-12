using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LevelCompletedPopupView : BasePopupView
    {
        public event Action OnNextLevelPressed;
        public event Action OnMainMenuPressed;

        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private Button _mainMenuButton;
        [SerializeField] private TMP_Text _nextLevelText;
        [SerializeField] private TMP_Text _mainMenuText;
        [SerializeField] private TMP_Text _congratulationsText;

        public void Init(LevelCompletedPopupModel model)
        {
            _nextLevelText.text = model.NextLevelText;
            _mainMenuText.text = model.MainMenuText;
            _congratulationsText.text = model.CongratulationsText;

            _congratulationsText.gameObject.SetActive(model.IsLastLevel);
            _nextLevelButton.gameObject.SetActive(!model.IsLastLevel);

            UnsubscribeButtons();
            SubscribeButtons();
        }

        protected override void SubscribeButtons()
        {
            _nextLevelButton.onClick.AddListener(OnNextLevelButtonPressed);
            _mainMenuButton.onClick.AddListener(OnMainMenuButtonPressed);
        }

        protected override void UnsubscribeButtons()
        {
            _nextLevelButton.onClick.RemoveListener(OnNextLevelButtonPressed);
            _mainMenuButton.onClick.RemoveListener(OnMainMenuButtonPressed);
        }

        private void OnNextLevelButtonPressed()
        {
            OnNextLevelPressed?.Invoke();
        }

        private void OnMainMenuButtonPressed()
        {
            OnMainMenuPressed?.Invoke();
        }
    }
}

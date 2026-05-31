using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Settings
{
    public class RebindActionView : MonoBehaviour
    {
        public Action OnRebindPressed;
        public Action OnResetPressed;
        public string LabelText => _actionLabel.text;

        [SerializeField] private TMP_Text _actionLabel;
        [SerializeField] private TMP_Text _rebindButtonText;
        [SerializeField] private Button _rebindButton;
        [SerializeField] private TMP_Text _resetButtonText;
        [SerializeField] private Button _resetToDefaultButton;



        public void SetLabelText(string labelText)
        {
            _actionLabel.text = labelText;
        }

        public void SetRebindText(string rebindButtonText)
        {
            _rebindButtonText.text = rebindButtonText;
        }

        public void SetResetText(string resetButtonText)
        {
            _resetButtonText.text = resetButtonText;
        }

        private void OnEnable()
        {
            SubscribeToButtons();
        }

        private void OnDisable()
        {
            UnsubscribeFromButtons();
        }

        private void SubscribeToButtons()
        {
            _rebindButton.onClick.AddListener(OnRebindClick);
            _resetToDefaultButton.onClick.AddListener(OnResetClick);
        }

        private void UnsubscribeFromButtons()
        {
            _rebindButton.onClick.RemoveListener(OnRebindClick);
            _resetToDefaultButton.onClick.RemoveListener(OnResetClick);
        }

        private void OnRebindClick()
        {
            OnRebindPressed?.Invoke();
        }

        private void OnResetClick()
        {
            OnResetPressed?.Invoke();
        }
    }
}

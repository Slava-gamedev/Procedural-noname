using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace UI.Settings
{
    public class KeyRebindingController
    {
        private const string kRebindingTable = "BindingLocalizations";
        private const string kResetKey = "Reset";

        private readonly ILocalizationManager _localizationManager;

        private List<RebindActionView> _rebindViews;
        private RebindOverlayMapper _overlayMapper;

        public KeyRebindingController(ILocalizationManager localizationManager)
        {
            _localizationManager = localizationManager;
        }


        public async UniTask Init(List<RebindActionView> rebindViews, RebindOverlayMapper overlayMapper)
        {
            _rebindViews = rebindViews;
            _overlayMapper = overlayMapper;
            await InitRebindViews();
            SubscribeToRebinds();
        }

        public void Release()
        {
            UnsubscribeFromRebinds();
        }

        private async UniTask InitRebindViews()
        {
            string resetText = await _localizationManager.GetLocalizedStringAsync(kRebindingTable, kResetKey);

            for (int i = 0; i < _rebindViews.Count; i++)
            {
                string rebindingKey = "someKey";
                string labelText = await _localizationManager.GetLocalizedStringAsync(kRebindingTable, rebindingKey);
                string rebindingButtonText = "someRebindingButtonText";

                RebindActionView rebindView = _rebindViews[i];
                rebindView.SetLabelText(labelText);
                rebindView.SetRebindText(rebindingButtonText);
                rebindView.SetResetText(resetText);
            }
        }

        private void SubscribeToRebinds()
        {

        }

        private void UnsubscribeFromRebinds()
        {

        }

        private void OnRebindClick()
        {

        }

        private void OnResetRebindClick()
        {

        }
    }
}

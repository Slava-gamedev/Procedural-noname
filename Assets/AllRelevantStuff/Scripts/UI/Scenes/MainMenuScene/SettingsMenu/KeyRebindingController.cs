using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using static UnityEngine.EventSystems.EventTrigger;

namespace UI.Settings
{

    public interface IKeyRebindingController
    {
        UniTask Init(InputActionMap actionMap, RebindOverlayMapper overlayMapper);
        void Release();
    }

    public class KeyRebindingController : IKeyRebindingController
    {
        private const string kRebindingTable = "BindingLocalizations";
        private const string kResetKey = "Reset";
        private const string kWaitingForInputKey = "WaitingForInput";

        private readonly ILocalizationManager _localizationManager;
        private readonly IUIPrefabProvider _prefabProvider;

        private List<RebindEntry> _rebindEntries;
        private RebindOverlayMapper _overlayMapper;
        private InputActionMap _inputActionMap;
        private string _waitingForInputText;


        public KeyRebindingController(ILocalizationManager localizationManager, IUIPrefabProvider prefabProvider)
        {
            _localizationManager = localizationManager;
            _prefabProvider = prefabProvider;
        }


        public async UniTask Init(InputActionMap actionMap, RebindOverlayMapper overlayMapper)
        {
            _inputActionMap = actionMap;
            _overlayMapper = overlayMapper;
            await SpawnRebindViews();
            await InitRebindViews();
            SubscribeToRebinds();
        }

        public void Release()
        {
            UnsubscribeFromRebinds();
            DestroyRebindViews();
        }

        private async UniTask SpawnRebindViews()
        {
            Transform spawnParent = _overlayMapper.SpawnParent;
            _rebindEntries = new List<RebindEntry>();
            ReadOnlyArray<InputBinding> bindings = _inputActionMap.bindings;

            for (int i = 0; i < bindings.Count; i++)
            {
                InputBinding binding = bindings[i];

                if (binding.isComposite)
                {
                    continue;
                }

                InputAction action = _inputActionMap.FindAction(binding.action);
                RebindActionView rebindView = await _prefabProvider.InstantiateAsync<RebindActionView>(spawnParent, UIPrefabType.RebindActionView);

                _rebindEntries.Add(new RebindEntry
                {
                    View = rebindView,
                    Action = action,
                    BindingIndex = i,
                    ActionBindingIndex = action.bindings.IndexOf(b => b == binding),
                    Binding = binding
                });
            }
        }

        private async UniTask InitRebindViews()
        {
            string resetText = await _localizationManager.GetLocalizedStringAsync(kRebindingTable, kResetKey);
            _waitingForInputText = await _localizationManager.GetLocalizedStringAsync(kRebindingTable, kWaitingForInputKey);

            string[] localizationKeys = _rebindEntries.Select(entry => entry.Binding.isPartOfComposite
                ? $"{entry.Action.name}_{entry.Binding.name}"
                : entry.Action.name).ToArray();

            IEnumerable<UniTask<string>> labelTasks = _rebindEntries.Select(entry =>
            {
                string localizationKey = entry.Binding.isPartOfComposite
                    ? $"{entry.Action.name}_{entry.Binding.name}"
                    : entry.Action.name;

                return _localizationManager.GetLocalizedStringAsync(kRebindingTable, localizationKey);
            });

            string[] labelTexts = await UniTask.WhenAll(labelTasks);


            for (int i = 0; i < _rebindEntries.Count; i++)
            {
                RebindEntry entry = _rebindEntries[i];
                InputAction action = entry.Action;
                RebindActionView rebindView = entry.View;

                rebindView.SetLabelText(labelTexts[i]);
                rebindView.SetRebindText(action.GetBindingDisplayString(entry.ActionBindingIndex));
                rebindView.SetResetText(resetText);
            }
        }

        private void SubscribeToRebinds()
        {
            for (int i = 0; i < _rebindEntries.Count; i++)
            {
                RebindEntry entry = _rebindEntries[i];
                RebindActionView rebindView = entry.View;

                rebindView.OnRebindPressed += () => OnRebindClick(entry);
                rebindView.OnResetPressed += () => OnResetRebindClick(entry);
            }
        }

        private void UnsubscribeFromRebinds()
        {
            for (int i = 0; i < _rebindEntries.Count; i++)
            {
                RebindEntry entry = _rebindEntries[i];
                RebindActionView rebindView = entry.View;
                rebindView.OnRebindPressed = null;
                rebindView.OnResetPressed = null;
            }
        }

        private void OnRebindClick(RebindEntry rebindEntry)
        {
            int actionBindingIndex = rebindEntry.ActionBindingIndex;
            RebindActionView rebindView = rebindEntry.View;
            InputAction action = rebindEntry.Action;

            string overlayText = string.Format(_waitingForInputText, rebindView.LabelText);
            _overlayMapper.RebindOverlayText.text = overlayText;
            _overlayMapper.RebindOverlay.SetActive(true);

            string previousBindingPath = action.bindings[actionBindingIndex].effectivePath;

            action.Disable();

            action.PerformInteractiveRebinding(actionBindingIndex)
                .WithControlsExcluding("<Mouse>/position")
                .WithControlsExcluding("<Mouse>/delta")
                .WithControlsExcluding("<Keyboard>/escape")
                .OnComplete(operation =>
                {
                    operation.Dispose();
                    action.Enable();
                    _overlayMapper.RebindOverlay.SetActive(false);

                    string newBindingPath = action.bindings[actionBindingIndex].effectivePath;

                    foreach (RebindEntry otherEntry in _rebindEntries)
                    {
                        if (otherEntry.Action == action && otherEntry.ActionBindingIndex == actionBindingIndex)
                            continue;

                        string otherBindingPath = otherEntry.Action.bindings[otherEntry.ActionBindingIndex].effectivePath;

                        if (otherBindingPath == newBindingPath)
                        {
                            otherEntry.Action.ApplyBindingOverride(otherEntry.ActionBindingIndex, previousBindingPath);
                            otherEntry.View.SetRebindText(otherEntry.Action.GetBindingDisplayString(otherEntry.ActionBindingIndex));
                            break;
                        }
                    }

                    rebindView.SetRebindText(action.GetBindingDisplayString(actionBindingIndex));
                })
                .OnCancel(operation =>
                {
                    operation.Dispose();
                    action.Enable();
                    _overlayMapper.RebindOverlay.SetActive(false);
                })
                .Start();
        }

        //TODO: add handling of identical values after reset
        private void OnResetRebindClick(RebindEntry rebindEntry)
        {
            int bindingIndex = rebindEntry.ActionBindingIndex;
            RebindActionView rebindView = rebindEntry.View;
            InputAction action = rebindEntry.Action;

            action.RemoveBindingOverride(bindingIndex);
            string rebindText = action.GetBindingDisplayString(bindingIndex);
            rebindView.SetRebindText(rebindText);
        }

        private void DestroyRebindViews()
        {
            Transform spawnParent = _overlayMapper.SpawnParent;

            foreach (Transform child in spawnParent)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        private class RebindEntry
        {
            public RebindActionView View;
            public InputAction Action;
            public int BindingIndex;
            public int ActionBindingIndex;
            public InputBinding Binding;
        }
    }
}

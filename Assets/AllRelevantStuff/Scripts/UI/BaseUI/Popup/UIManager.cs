using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public interface IUIManager
    {
        void OpenPopup(IPopupContext popup);
        void ClosePopup(IPopupContext popup);
    }


    public class UIManager : IUIManager
    {
        private const string kGameObjectName = "UIManager";
        private const int kSortingOrderStep = 100;
        private const int kDefaultSortingOrder = 0;
        private Camera Camera
        {
            get
            {
                if (_camera == null)
                {
                    _camera = Camera.main;
                }

                return _camera;
            }
        }

        private Transform PopupHolder
        {
            get
            {
                if (_popupHolder == null)
                {
                    var go = GameObject.Find(kGameObjectName);
                    _popupHolder = go?.transform;
                }

                if (_popupHolder == null)
                {
                    var go = new GameObject(kGameObjectName);
                    _popupHolder = go.transform;
                }

                return _popupHolder;
            }
        }

        private Transform _popupHolder;
        private Camera _camera;
        private int _sortingOrder = 0;
        private List<IPopupContext> _popups = new List<IPopupContext>();

        public void OpenPopup(IPopupContext popup)
        {
            _ = ProcessPopup(popup);
        }

        public void ClosePopup(IPopupContext popup)
        {
            popup.Hide();
            popup.Release();

            int popupIndex = _popups.IndexOf(popup);
            _popups.RemoveAt(popupIndex);
            UpdateViewOrder(popupIndex);
        }

        private async UniTask ProcessPopup(IPopupContext popup)
        {
            _popups.Add(popup);
            _sortingOrder += kSortingOrderStep;

            await popup.InitPopup(Camera, PopupHolder, _sortingOrder);
            popup.Show();
        }

        private void UpdateViewOrder(int startIndex)
        {
            var popupsCount = _popups.Count;
            var potentialStartIndex = startIndex < popupsCount ? 0 : startIndex;
            _sortingOrder = potentialStartIndex == 0 ? kDefaultSortingOrder : potentialStartIndex * kSortingOrderStep;
            for (int i = potentialStartIndex; i < popupsCount; i++)
            {
                IPopupContext popup = _popups[i];
                _sortingOrder += kSortingOrderStep;
                popup.SetCanvasOrder(_sortingOrder);
            }
        }
    }
}

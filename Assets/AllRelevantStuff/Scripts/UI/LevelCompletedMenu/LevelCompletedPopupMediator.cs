using Cysharp.Threading.Tasks;
using System;
using UI;
using UnityEngine;

public class LevelCompletedPopupMediator : BasePopupMediator<LevelCompletedPopupView>
{

    protected override PopupType _popupType => PopupType.LevelCompletedPopup;

    private LevelCompletedPopupController _controller;

    public LevelCompletedPopupMediator(PopupMediatorDependenices dependenices, LevelCompletedPopupController controller) : base(dependenices)
    {
        _controller = controller;
    }

    public override async UniTask InitPopup(Camera camera, Transform parent, int orderLayer = 0)
    {
        await base.InitPopup(camera, parent, orderLayer);
        await _controller.Init(_view, this);
    }
}

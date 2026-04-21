using Cysharp.Threading.Tasks;
using System;
using UI;
using UnityEngine;

public class LevelPausePopupMediator : BasePopupMediator<LevelPausePopupView>
{

    protected override PopupType _popupType => PopupType.LevelPausePopup;

    private LevelPausePopupController _controller;

    public LevelPausePopupMediator(PopupMediatorDependenices dependenices, LevelPausePopupController controller) : base(dependenices)
    {
        _controller = controller;
    }

    public override async UniTask InitPopup(Camera camera, Transform parent, int orderLayer = 0)
    {
        await base.InitPopup(camera, parent, orderLayer);
        await _controller.Init(_view, this);
    }
}

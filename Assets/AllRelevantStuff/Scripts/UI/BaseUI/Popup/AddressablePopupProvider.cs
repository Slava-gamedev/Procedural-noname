using UnityEngine;

public interface IPopupProvider : IAddressableGameObjectProvider<PopupType>
{

}

[CreateAssetMenu(fileName = "AddressablePopupProvider", menuName = "Scriptable Objects/AddressablePopupProvider")]
public class AddressablePopupProvider : BaseAddressableGameObjectProvider<PopupType>, IPopupProvider
{

}

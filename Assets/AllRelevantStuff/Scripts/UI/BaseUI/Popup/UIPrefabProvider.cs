using UnityEngine;

public interface IUIPrefabProvider : IAddressableGameObjectProvider<UIPrefabType>
{

}

[CreateAssetMenu(fileName = "UIPrefabProvider", menuName = "Scriptable Objects/UIPrefabProvider")]
public class UIPrefabProvider : BaseAddressableGameObjectProvider<UIPrefabType>, IUIPrefabProvider
{

}

public enum UIPrefabType
{
    RebindActionView = 0,
}

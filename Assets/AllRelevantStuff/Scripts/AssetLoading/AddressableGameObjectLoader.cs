using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class AddressableGameObjectLoader : BaseAddressableLoader<GameObject, AssetReferenceGameObject>
{
}

[Serializable]
public class GameObjectMapper<TType> where TType : Enum
{
    [field: SerializeField] public TType Type { get; private set; }
    [field: SerializeField] public AddressableGameObjectLoader Loader { get; private set; }
}

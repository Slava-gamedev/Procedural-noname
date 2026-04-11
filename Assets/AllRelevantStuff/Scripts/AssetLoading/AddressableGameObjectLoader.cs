using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

[Serializable]
public class AddressableGameObjectLoader : BaseAddressableLoader<GameObject, AssetReferenceGameObject>
{
}

[Serializable]
public class GameObjectMapper<T>
{
    [field: SerializeField] public T Type { get; private set; }
    [field: SerializeField] public AddressableGameObjectLoader Loader { get; private set; }
}

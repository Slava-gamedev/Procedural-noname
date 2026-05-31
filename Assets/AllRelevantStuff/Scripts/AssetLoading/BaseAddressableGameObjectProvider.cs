using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public interface IAddressableGameObjectProvider<TGameObjectType>
{
    UniTask<TGameObjectComponent> InstantiateAsync<TGameObjectComponent>(Transform parent, TGameObjectType type);
    void ClearSpecificGameObject(TGameObjectType type);
    void ClearAll();
}

public abstract class BaseAddressableGameObjectProvider<TGameObjectType> : ScriptableObject, IAddressableGameObjectProvider<TGameObjectType>
    where TGameObjectType : Enum
{
    [SerializeField] List<GameObjectMapper<TGameObjectType>> _gameObjects;

    public async UniTask<TGameObjectComponent> InstantiateAsync<TGameObjectComponent>(Transform parent, TGameObjectType type)
    {
        GameObjectMapper<TGameObjectType> mapper = _gameObjects.FirstOrDefault(popup => popup.Type.Equals(type));

        if (mapper == null)
        {
            throw new ArgumentException($"There was no gameObject with type '{type}' ");
        }

        AddressableGameObjectLoader loader = mapper.Loader;
        GameObject prefab = await loader.LoadAsync();
        GameObject instance = Instantiate(prefab, parent);
        TGameObjectComponent result = instance.GetComponent<TGameObjectComponent>();
        return result;
    }

    public void ClearSpecificGameObject(TGameObjectType type)
    {
        GameObjectMapper<TGameObjectType> mapper = _gameObjects.FirstOrDefault(popup => popup.Type.Equals(type));

        if (mapper == null)
        {
            throw new ArgumentException($"There was no gameObject with type '{type}' ");
        }

        AddressableGameObjectLoader loader = mapper.Loader;
        loader.Clear();
    }

    public void ClearAll()
    {
        for (int i = 0; i < _gameObjects.Count; i++)
        {
            GameObjectMapper<TGameObjectType> mapper = _gameObjects[i];
            mapper.Loader.Clear();
        }
    }
}

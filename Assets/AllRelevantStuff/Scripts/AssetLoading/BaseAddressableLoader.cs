using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[Serializable]
public abstract class BaseAddressableLoader<TObject, TRef>
       where TObject : class
       where TRef : AssetReference
{
    [SerializeField] protected TRef _reference;
    protected AsyncOperationHandle<TObject> _operationHandler;

    /// <summary>
    /// Loads asset asynchrounosly. Stores cached result until clearing.
    /// </summary>
    /// <returns>Asset of type, without instantiating</returns>
    public async UniTask<TObject> LoadAsync()
    {
        TObject loadedObject = null;

        if (_operationHandler.IsValid()
            && _operationHandler.Status == AsyncOperationStatus.Succeeded)
        {
            loadedObject = _operationHandler.Result;
        }
        else
        {
            if (!_reference.RuntimeKeyIsValid())
            {
                return null;
            }

            _operationHandler = Addressables.LoadAssetAsync<TObject>(_reference.RuntimeKey);
            await _operationHandler;
            loadedObject = _operationHandler.Result;
            int attempts = 0;

            while (_operationHandler.Status != AsyncOperationStatus.Succeeded
                   && attempts < 3)
            {
                _operationHandler = Addressables.LoadAssetAsync<TObject>(_reference.RuntimeKey);
                await _operationHandler;
                loadedObject = _operationHandler.Result;
                attempts++;
                Debug.LogFormat("Invoked attempt № {0} for {1}", attempts, _reference.AssetGUID);
            }
        }

        return loadedObject;
    }

    /// <summary>
    /// Releases cached resources 
    /// </summary>
    public void Clear()
    {
        if (_operationHandler.IsValid()
            && _operationHandler.Status == AsyncOperationStatus.Succeeded)
        {
            Addressables.Release(_operationHandler);
            _operationHandler = default;
        }
    }
}

using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public interface IAddressablePopupProvider
{
    UniTask<TPopupView> InstantiatePopupAsync<TPopupView>(Transform parent, PopupType type);
    void ClearSpecificPopup(PopupType type);
    void ClearAll();
}

[CreateAssetMenu(fileName = "AddressablePopupProvider", menuName = "Scriptable Objects/AddressablePopupProvider")]
public class AddressablePopupProvider : ScriptableObject, IAddressablePopupProvider
{
    [SerializeField] List<GameObjectMapper<PopupType>> _popups;

    public async UniTask<TPopupView> InstantiatePopupAsync<TPopupView>(Transform parent, PopupType type)
    {
        GameObjectMapper<PopupType> mapper = _popups.FirstOrDefault(popup => popup.Type == type);

        if(mapper == null)
        {
            throw new ArgumentException($"There was no popup with type '{type}' ");
        }

        AddressableGameObjectLoader loader = mapper.Loader;
        GameObject prefab = await loader.LoadAsync();
        GameObject instance = Instantiate(prefab, parent);
        TPopupView result = instance.GetComponent<TPopupView>();
        return result;
    }

    public void ClearSpecificPopup(PopupType type)
    {
        GameObjectMapper<PopupType> mapper = _popups.FirstOrDefault(popup => popup.Type == type);

        if (mapper == null)
        {
            throw new ArgumentException($"There was no popup with type '{type}' ");
        }

        AddressableGameObjectLoader loader = mapper.Loader;
        loader.Clear();
    }

    public void ClearAll()
    {
        for (int i = 0; i < _popups.Count; i++)
        {
            GameObjectMapper<PopupType> mapper = _popups[i];
            mapper.Loader.Clear();
        }
    }
}

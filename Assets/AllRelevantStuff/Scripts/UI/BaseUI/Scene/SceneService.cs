using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface ISceneService
{
    UniTask<ISceneView> LoadSceneAsync(string sceneKey, LoadSceneMode mode);
    UniTask UnloadSceneAsync(string sceneKey);
}

public class SceneService : ISceneService
{
    private readonly HashSet<string> _cache = new();

    public async UniTask<ISceneView> LoadSceneAsync(string sceneKey, LoadSceneMode mode)
    {
        ISceneView sceneView = null;

        if (!_cache.Contains(sceneKey))
        {

            await SceneManager.LoadSceneAsync(sceneKey, mode);


            UnityEngine.Debug.LogFormat("Scene {0} loaded", sceneKey);
            _cache.Add(sceneKey);
        }

        Scene instance = SceneManager.GetSceneByName(sceneKey);
        var mainGO = instance.GetRootGameObjects()[0];
        mainGO.TryGetComponent(out sceneView);

        return sceneView;
    }

    public async UniTask UnloadSceneAsync(string sceneKey)
    {
        if (_cache.TryGetValue(sceneKey, out string sceneName))
        {
            _cache.Remove(sceneKey);
            await SceneManager.UnloadSceneAsync(sceneKey);
        }
    }
}

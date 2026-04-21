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
    private readonly HashSet<string> _loadedScenes = new();

    public async UniTask<ISceneView> LoadSceneAsync(string sceneKey, LoadSceneMode mode)
    {
        ISceneView sceneView = null;

        if(mode == LoadSceneMode.Single)
        {
            _loadedScenes.Clear();
        }

        if (!_loadedScenes.Contains(sceneKey))
        {

            await SceneManager.LoadSceneAsync(sceneKey, mode);


            UnityEngine.Debug.LogFormat("Scene {0} loaded", sceneKey);
            _loadedScenes.Add(sceneKey);
        }

        Scene instance = SceneManager.GetSceneByName(sceneKey);
        var mainGO = instance.GetRootGameObjects()[0];
        mainGO.TryGetComponent(out sceneView);

        return sceneView;
    }

    public async UniTask UnloadSceneAsync(string sceneKey)
    {
        if (!_loadedScenes.TryGetValue(sceneKey, out string sceneName))
        {
            return;
        }

        Scene scene = SceneManager.GetSceneByName(sceneName);

        if (scene.isLoaded && _loadedScenes.Count > 1)
        {
            await SceneManager.UnloadSceneAsync(sceneName);
        }

        _loadedScenes.Remove(sceneKey);
    }
}

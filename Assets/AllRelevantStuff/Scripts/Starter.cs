using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class Starter : MonoBehaviour
{
    private MainMenuSceneMediator _mainMenuScene;
    private SendToSceneEventListener _sceneEventListener;

    [Inject]
    private void Construct(MainMenuSceneMediator mainMenuScene,
        SendToSceneEventListener sendToSceneEventListener)
    {
        _mainMenuScene = mainMenuScene;
        _sceneEventListener = sendToSceneEventListener;
    }

    private void Start()
    {
        _ = Init();
    }

    private async UniTask Init()
    {
        Application.targetFrameRate = 60;

        await _mainMenuScene.ShowSceneSingle();
        _sceneEventListener.Init();
    }
}

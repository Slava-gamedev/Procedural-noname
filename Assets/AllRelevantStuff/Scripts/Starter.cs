using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class Starter : MonoBehaviour
{
    private MainMenuSceneMediator _mainMenuScene;

    [Inject]
    private void Construct(MainMenuSceneMediator mainMenuScene)
    {
        _mainMenuScene = mainMenuScene;
    }

    private void Start()
    {
        _ = Init();
    }

    private async UniTask Init()
    {
        Application.targetFrameRate = 60;

        await _mainMenuScene.ShowSceneAdditive();
    }
}

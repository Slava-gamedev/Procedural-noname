using UI;
using UI.Settings;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{
    [SerializeField] AddressablePopupProvider _popupProvider;
    [SerializeField] UIPrefabProvider _uiPrefabProvider;

    public override void InstallBindings()
    {
        BindServicesAndManagers();
        BindListeners();
        BindProviders();
        BindUIDependencies();
        BindControllers();
        BindUI();
    }

    private void BindServicesAndManagers()
    {
        Container.Bind<IEventBus>().To<EventBus>().AsSingle();
        Container.Bind<ISceneUIManager>().To<SceneUIManager>().AsSingle();
        Container.Bind<ISceneService>().To<SceneService>().AsSingle();
        Container.Bind<IUIManager>().To<UIManager>().AsSingle();
        Container.Bind<ILocalizationManager>().To<LocalizationManager>().AsSingle();
        Container.Bind<ILevelDataService>().To<LevelDataService>().AsSingle();
    }

    private void BindListeners()
    {
        Container.Bind<SendToSceneEventListener>().To<SendToSceneEventListener>().AsSingle();

    }

    private void BindProviders()
    {
        Container.Bind<IPopupProvider>().FromInstance(_popupProvider).AsSingle();
        Container.Bind<IUIPrefabProvider>().FromInstance(_uiPrefabProvider).AsSingle();
    }

    private void BindUI()
    {
        Container.Bind<MainMenuSceneMediator>().To<MainMenuSceneMediator>().AsSingle();
        Container.Bind<LevelSceneMediator>().To<LevelSceneMediator>().AsTransient();
        Container.Bind<LevelSceneController>().To<LevelSceneController>().AsTransient();
        Container.Bind<LevelCompletedPopupMediator>().To<LevelCompletedPopupMediator>().AsTransient();
        Container.Bind<LevelCompletedPopupController>().To<LevelCompletedPopupController>().AsTransient();

        Container.Bind<LevelPausePopupMediator>().To<LevelPausePopupMediator>().AsTransient();
        Container.Bind<LevelPausePopupController>().To<LevelPausePopupController>().AsTransient();
    }

    private void BindControllers()
    {
        Container.Bind<IKeyRebindingController>().To<KeyRebindingController>().AsTransient();
    }

    private void BindUIDependencies()
    {
        Container.Bind<SceneMediatorDependenices>().To<SceneMediatorDependenices>().AsSingle();
        Container.Bind<PopupControllerDependenices>().To<PopupControllerDependenices>().AsSingle();
        Container.Bind<PopupMediatorDependenices>().To<PopupMediatorDependenices>().AsSingle();
    }
}
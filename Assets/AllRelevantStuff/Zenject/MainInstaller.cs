using UI;
using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{
    [SerializeField] AddressablePopupProvider _popupProvider;
    public override void InstallBindings()
    {
        BindServicesAndManagers();
        BindListeners();
        BindProviders();
        BindUIDependencies();
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
        Container.Bind<IAddressablePopupProvider>().FromInstance(_popupProvider).AsSingle();
    }

    private void BindUI()
    {
        Container.Bind<MainMenuSceneMediator>().To<MainMenuSceneMediator>().AsSingle();
        Container.Bind<LevelSceneMediator>().To<LevelSceneMediator>().AsTransient();
        Container.Bind<LevelSceneController>().To<LevelSceneController>().AsTransient();
        Container.Bind<LevelCompletedPopupMediator>().To<LevelCompletedPopupMediator>().AsTransient();
        Container.Bind<LevelCompletedPopupController>().To<LevelCompletedPopupController>().AsTransient();

    }

    private void BindUIDependencies()
    {
        Container.Bind<SceneMediatorDependenices>().To<SceneMediatorDependenices>().AsSingle();
        Container.Bind<PopupControllerDependenices>().To<PopupControllerDependenices>().AsSingle();
        Container.Bind<PopupMediatorDependenices>().To<PopupMediatorDependenices>().AsSingle();
    }
}
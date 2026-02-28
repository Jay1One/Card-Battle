using Core;
using Core.Signals;
using Gameplay.Ads;
using Gameplay.Progression;
using Gameplay.Units;
using UnityEngine;
using Zenject;

namespace Gameplay.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField] private LevelsCatalogSo _catalog;
        [SerializeField] private Player _playerPrefab;

        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            DeclareSignals();
            BindProgressionElements();
            BindAds();
        }

        private void DeclareSignals()
        {
            Container.DeclareSignal<SceneChangedSignal>();
        }

        private void BindAds()
        {
            Container.BindInterfacesAndSelfTo<AppodealAdsProvider>().AsSingle();
            Container.BindInterfacesAndSelfTo<AdvertizingManager>().FromNewComponentOnNewGameObject().
                WithGameObjectName("Ads").AsSingle().NonLazy();
        }

        private void BindProgressionElements()
        {
            Container.Bind<LevelsCatalogSo>().FromScriptableObject(_catalog).AsSingle();
            Container.Bind<LevelProgression>().AsSingle();
            Container.Bind<SceneLoader>().AsSingle();
            Container.Bind<RunState>().AsSingle().WithArguments(_playerPrefab.Health);
            Container.Bind<DeckBuilder>().AsTransient();
        }
    }
}
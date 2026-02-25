using System.Linq;
using Core.Card_Mechanics;
using Core.Signals;
using Gameplay.Progression;
using Gameplay.Systems;
using UnityEngine;
using Zenject;

namespace Gameplay.Installers
{
    public class BattleSceneInstaller : MonoInstaller
    { 
        [SerializeField] private UnitsSystem _unitsSystem;
        public override void InstallBindings()
        {
            DeclareSignals();
            BindBattleSystems();
            BindLevelDetails();
        }

        private void DeclareSignals()
        {
            Container.DeclareSignal<PlayerTurnStartedSignal>(); 
            Container.DeclareSignal<PlayerTurnFinishedSignal>();
            Container.DeclareSignal<EndTurnRequestedSignal>();
            
            Container.DeclareSignal<DrawCardStartedSignal>();
            Container.DeclareSignal<DrawCardFinishedSignal>();
        
            Container.DeclareSignal<PlayCardStartedSignal>();
            Container.DeclareSignal<PlayCardFinishedSignal>();
        
            Container.DeclareSignal<DiscardCardStartedSignal>();
            Container.DeclareSignal<DiscardCardFinishedSignal>();
        
            Container.DeclareSignal<StartingHandDealtSignal>();
        
            Container.DeclareSignal<CardLockChangeRequestedSignal>();
            Container.DeclareSignal<CardLockChangeProcessedSignal>();

            Container.DeclareSignal<RedrawStartedSignal>();
            Container.DeclareSignal<RedrawsChangedSignal>();
        
            Container.DeclareSignal<EnemyDiedSignal>();
            Container.DeclareSignal<EnemyDeathRegisteredSignal>();

            Container.DeclareSignal<PlayerDiedSignal>();
            Container.DeclareSignal<BattleEndedSignal>();
        }

        private void BindBattleSystems()
        {
            Container.BindInterfacesAndSelfTo<BattleController>().AsSingle();
            Container.BindInterfacesAndSelfTo<DeckController>().AsSingle();
            Container.BindInterfacesAndSelfTo<RedrawSystem>().AsSingle();
            Container.Bind<UnitsSystem>().FromInstance(_unitsSystem).AsSingle();
            Container.BindInterfacesAndSelfTo<ComboSystem>().AsSingle();
        }

        private void BindLevelDetails()
        {
            Container.Bind<LevelDataSo>().FromMethod(ctx =>
            {
                var catalog = ctx.Container.Resolve<LevelsCatalogSo>();
                var progress = ctx.Container.Resolve<LevelProgression>();
                
                return catalog.Levels[progress.CurrentLevelIndex];
            }).AsSingle();
            
            Container.Bind<Deck>().FromMethod(ctx =>
            {
                var run = ctx.Container.Resolve<RunState>();

                var cards = run.Deck
                    .Select(template => new Card(template))
                    .ToList();

                return new Deck(cards);
            }).AsSingle();
        }
    }
}
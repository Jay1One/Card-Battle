using UI.BattleScene.Views;
using UnityEngine;
using Zenject;

namespace UI.BattleScene
{
    public class BattleSceneUiInstaller : MonoInstaller
    {
        [SerializeField] private CardViewSettings _cardViewSettings;
        [SerializeField] private CardView _cardViewTemplate;
        
        public override void InstallBindings()
        {
            Container.Bind<CardViewSettings>().FromScriptableObject(_cardViewSettings).AsSingle();
            Container.Bind<CardViewFactory>().AsSingle();
            Container.Bind<CardView>().FromInstance(_cardViewTemplate).AsSingle();
        }
    }
}
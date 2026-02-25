using Gameplay.Systems;
using UnityEngine;
using Zenject;

namespace Gameplay
{
    public class BattleSceneBootstrapper : MonoBehaviour
    {
        private BattleController _battleController;
    
        [Inject]
        private void Construct(BattleController battleController)
        {
            _battleController = battleController;
        }
    
        private void Start()
        {
            _battleController.StartBattle();
        }
    }
}
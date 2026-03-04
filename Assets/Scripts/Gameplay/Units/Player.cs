using System;

namespace Gameplay.Units
{
    public class Player : Unit
    {
        public bool IsAlive { get; private set; } = true;
        public event Action PlayerDied;
        
        protected override void Awake()
        {
            base.Awake();
            BattleController.PlayerTurnStarted += OnPlayerTurnStarted;
        }

        private void OnDestroy()
        {
            BattleController.PlayerTurnStarted -= OnPlayerTurnStarted;
        }

        private void OnPlayerTurnStarted()
        {
            _armor.ResetValue();
        }
        
        protected override void Die()
        {
            base.Die();
            IsAlive = false;
            PlayerDied.Invoke();
        }
    }
}
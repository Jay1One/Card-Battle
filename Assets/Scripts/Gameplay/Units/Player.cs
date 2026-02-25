using Core.Signals;

namespace Gameplay.Units
{
    public class Player : Unit
    {
        public bool IsAlive { get; private set; } = true;
        
        protected override void Awake()
        {
            base.Awake();
            SignalBus.Subscribe<PlayerTurnStartedSignal>(OnPlayerTurnStarted);
        }

        private void OnDestroy()
        {
            SignalBus.Unsubscribe<PlayerTurnStartedSignal>(OnPlayerTurnStarted);
        }

        private void OnPlayerTurnStarted()
        {
            Armor.ResetValue();
        }
        
        protected override void Die()
        {
            base.Die();
            IsAlive = false;
            SignalBus.Fire<PlayerDiedSignal>();
        }
    }
}
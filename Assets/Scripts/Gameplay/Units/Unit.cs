using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Gameplay.Units
{
    public abstract class Unit : MonoBehaviour
    {
        [SerializeField] protected Health _health; 
        [SerializeField] protected Armor _armor;
        
        public Health Health => _health;
        public Armor Armor => _armor;
        
        protected SignalBus SignalBus;
        private UnitAnimation _animation;
        private TaskCompletionSource<bool> _attackDamageDealtCompletionSource;
        private TaskCompletionSource<bool> _attackFinishedCompletionSource;
        private TaskCompletionSource<bool> _blockFrameReachedCompletionSource;
        private TaskCompletionSource<bool> _blockFinishedCompletionSource;
        
        
        [Inject]
        public void Construct(SignalBus signalBus)
        {
            SignalBus = signalBus;
        }

        private void OnValidate()
        {
            _health.Initialize(_health.MaxHealth);
        }

        private void TakeDamage(int damage)
        {
            int remainingDamage = damage;
            _armor.BlockDamage(ref remainingDamage);
            
            if (remainingDamage>0)
            {
                _health.TakeDamage(remainingDamage);
            }

            if (_health.CurrentHealth==0)
            {
                Die();
            }
            else
            {
                _animation.AnimateDamaged();
            }
        }
        
        public async Task GainBlockAsync(int block, CancellationToken ct)
        {
            _blockFrameReachedCompletionSource = new TaskCompletionSource<bool>();
            _animation.AnimateBlock();
            
            await _blockFrameReachedCompletionSource.Task;
            
            _armor.AddArmor(block);
            
            _blockFinishedCompletionSource = new TaskCompletionSource<bool>();
            
            await _blockFinishedCompletionSource.Task;
        }

        public virtual async Task AttackAsync(Unit target, int damage,CancellationToken ct)
        {
            _attackDamageDealtCompletionSource = new TaskCompletionSource<bool>();
            _animation.AnimateAttack(target.transform);
            
            await _attackDamageDealtCompletionSource.Task;
            
            target.TakeDamage(damage);
            _attackFinishedCompletionSource = new TaskCompletionSource<bool>();
            
            await _attackFinishedCompletionSource.Task;
        }

        private void OnAttackFrameReached()
        {
            _attackDamageDealtCompletionSource.TrySetResult(true);
        }

        private void OnBlockFrameReached()
        {
            _blockFrameReachedCompletionSource.TrySetResult(true);
        }

        private void OnAttackAnimationFinished()
        {
            _attackFinishedCompletionSource.TrySetResult(true);
        }

        private void OnBlockFinished()
        {
            _blockFinishedCompletionSource.TrySetResult(true);
        }

        protected virtual void Die()
        {
            _animation.AnimateDeath();
        }
        
        protected virtual void Awake()
        {
            _health.Initialize(_health.MaxHealth);
            
            _animation = GetComponent<UnitAnimation>();
            
            _animation.AttackFrameReached += OnAttackFrameReached;
            _animation.AttackAnimationFinished += OnAttackAnimationFinished;
            _animation.BlockFrameReached += OnBlockFrameReached;
            _animation.BlockAnimationFinished += OnBlockFinished;
        }

        private void OnDestroy()
        {
            _animation.AttackFrameReached -= OnAttackFrameReached;
            _animation.AttackAnimationFinished -= OnAttackAnimationFinished;
            _animation.BlockFrameReached -= OnBlockFrameReached;
            _animation.BlockAnimationFinished -= OnBlockFinished;
        }
    }
}
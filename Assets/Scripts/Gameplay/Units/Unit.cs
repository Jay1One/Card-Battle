using System.Collections;
using Gameplay.Systems;
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
        
        protected BattleController BattleController;
        private UnitAnimation _animation;
        
        private UnitAction _currentAction;

        private bool _isAttackDamageDealt;
        private bool _isAttackFinished;
        private bool _isBlockFrameReached;
        private bool _isBlockFinished;
        
        
        [Inject]
        public void Construct(BattleController battleController)
        {
            BattleController = battleController;
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

        public IEnumerator AttackCoroutine (Unit target, int damage)
        {
            _isAttackFinished = false;
            _currentAction = new AttackAction(target,damage);
            _animation.AnimateAttack(target.transform);
            
            while (!_isAttackFinished)
            {
                yield return null;
            }
        }
        
        public IEnumerator GainBlockCoroutine(int blockAmount)
        {
            _isBlockFinished = false;
            _currentAction = new BlockAction(blockAmount);
            _animation.AnimateBlock();

            while (!_isBlockFinished)
            {
                yield return null;
            }
        }

        private void OnAttackFrameReached()
        {
            AttackAction attackAction = (AttackAction)_currentAction;
            attackAction.Target.TakeDamage(attackAction.Damage);
        }

        private void OnBlockFrameReached()
        {
            BlockAction blockAction = (BlockAction)_currentAction;
            _armor.AddArmor(blockAction.BlockAmount);
        }

        private void OnAttackAnimationFinished()
        {
            _isAttackFinished = true;
        }

        private void OnBlockFinished()
        {
            _isBlockFinished = true;
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
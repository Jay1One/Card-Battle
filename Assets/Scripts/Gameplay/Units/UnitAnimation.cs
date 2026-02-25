using System;
using System.Collections;
using UnityEngine;

namespace Gameplay.Units
{
    [RequireComponent(typeof(Animator))]
    public class UnitAnimation : MonoBehaviour
    {
        [SerializeField] private float _attackRange = 1f;
        [SerializeField] private float _approachTime = 0.2f;
        [SerializeField] private float _deathDisappearTime = 1f;
        [SerializeField] private float _timeStandingAfterAttack = 0.2f;
        
        private readonly string _attackTrigger = "Attack";
        private readonly string _deathTrigger = "Death";
        private readonly string _damagedTrigger = "Damaged";
        private readonly string _blockTrigger = "Block";
        
        private Animator _animator;
        private Vector3 _initialPosition;
        
        public event Action AttackFrameReached;
        public event Action AttackAnimationFinished;
        public event Action BlockFrameReached;
        public event Action BlockAnimationFinished;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _initialPosition = transform.position;
        }
        
        public void AnimateDamaged()
        {
            _animator.SetTrigger(_damagedTrigger);
        }

        public void AnimateDeath()
        {
            StartCoroutine(DeathCoroutine());
        }

        public void AnimateBlock()
        {
            _animator.SetTrigger(_blockTrigger);
        }

        public void ReachBlockFrame()
        {
            BlockFrameReached?.Invoke();
        }

        public void AnimateAttack(Transform target)
        {
            StartCoroutine(StartAttackCoroutine(target));
        }
        
        public void ReachAttackFrame()
        {
            AttackFrameReached?.Invoke();
            StartCoroutine(FinishAttackCoroutine());
        }

        public void ReachBlockLastFrame()
        {
            BlockAnimationFinished?.Invoke();
        }
        
        private IEnumerator StartAttackCoroutine(Transform target)
        {
            float approachDistance = Vector3.Distance(transform.position, target.position) - _attackRange;
            Vector3 endPosition = Vector3.MoveTowards(transform.position, target.position, approachDistance);
            
            yield return MoveCoroutine(transform.position, endPosition, _approachTime);
            
            _animator.SetTrigger(_attackTrigger);
        }

        private IEnumerator FinishAttackCoroutine()
        {
            yield return new WaitForSeconds(_timeStandingAfterAttack);
            yield return MoveCoroutine(transform.position, _initialPosition, _approachTime);
            AttackAnimationFinished?.Invoke();
        }

        private IEnumerator MoveCoroutine(Vector3 startPosition, Vector3 endPosition, float time)
        {
            float startTime = Time.time;
            float endTime = startTime + time;

            while (Time.time < endTime)
            {
                transform.position = Vector3.Lerp(startPosition, endPosition, (Time.time - startTime) / time);
                yield return null;
            }
            
            transform.position = endPosition;
        }

        private IEnumerator DeathCoroutine()
        {
            _animator.SetTrigger(_deathTrigger);
            yield return new WaitForSeconds(_deathDisappearTime);
            Destroy(gameObject);
        }
    }
}
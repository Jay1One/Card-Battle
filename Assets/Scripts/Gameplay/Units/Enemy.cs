using System;
using System.Collections;
using UnityEngine;

namespace Gameplay.Units
{
    public class Enemy : Unit
    {
        [SerializeField] private int _attackDamage = 10;
      
        public event Action<Enemy> EnemyDied;
        
        public IEnumerator MakeTurnCoroutine(Player player)
        {
            yield return AttackCoroutine(player, _attackDamage);
        }

        protected override void Die()
        {
            EnemyDied?.Invoke(this);
            base.Die();
        }
    }
}
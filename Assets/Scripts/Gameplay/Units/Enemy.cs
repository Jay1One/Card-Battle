using System.Threading;
using System.Threading.Tasks;
using Core.Signals;
using UnityEngine;

namespace Gameplay.Units
{
    public class Enemy : Unit
    {
        [SerializeField] private int _attackDamage = 10;
        private int _enemyId;

        public void SetId(int id)
        {
            _enemyId = id;
        }
        
        public async Task MakeTurnAsync(Player player, CancellationToken ct)
        {
            await AttackAsync(player, _attackDamage, ct);
        }

        public override async Task AttackAsync(Unit target, int damage, CancellationToken ct)
        {
            if (!((Player)target).IsAlive)
            {
                return;
            }
            
            await base.AttackAsync(target, damage, ct);
        }

        protected override void Die()
        {
            SignalBus.Fire(new EnemyDiedSignal(_enemyId));
            base.Die();
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay.Units;
using UnityEngine;
using Zenject;

namespace Gameplay.Systems
{
    public class EnemiesTurnSystem : MonoBehaviour
    {
        private UnitsSystem _unitsSystem;
        private List<Enemy> _enemies;
        private int _currentEnemyActingIndex;
        
        public event Action TurnFinished;

        [Inject]
        public void Construct (UnitsSystem unitsSystem)
        {
            _unitsSystem = unitsSystem;
        }
        
        public void MakeTurn()
        {
            StartCoroutine(TurnCoroutine());
        }

        private IEnumerator TurnCoroutine()
        {
            _enemies = _unitsSystem.Enemies.ToList();
            
            for (int i = 0; i < _enemies.Count; i++)
            {
                if (_unitsSystem.Player.IsAlive)
                {
                    yield return _enemies[i].MakeTurnCoroutine(_unitsSystem.Player);
                }
            }
            
            TurnFinished?.Invoke();
        }
    }
}
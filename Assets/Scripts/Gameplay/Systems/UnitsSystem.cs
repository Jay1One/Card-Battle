using System;
using System.Collections.Generic;
using Core;
using Core.Signals;
using Gameplay.Progression;
using Gameplay.Units;
using UnityEngine;
using Zenject;

namespace Gameplay.Systems
{
    public class UnitsSystem : MonoBehaviour
    {
        private const int MaxEnemies = 3;
        
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private Transform[] _enemySpawnPoints;
        [SerializeField] private Transform _playerSpawnPoint;
        
        private readonly Dictionary<int, Enemy> _enemies = new Dictionary<int, Enemy>();
        private int _nextEnemyId = 0;
        
        public  IReadOnlyDictionary<int,Enemy> Enemies => _enemies;
        public Player Player { get; private set; }
        
        private LevelDataSo _levelData;
        private SignalBus _signalBus;
        private DiContainer _diContainer;
        private RunState _runState;

        [Inject]
        public void Construct(LevelDataSo levelData, SignalBus signalBus, DiContainer diContainer, RunState runState)
        {
            _levelData = levelData;
            _signalBus = signalBus;
            _diContainer = diContainer;
            _runState = runState;
        }

        private void Awake()
        {
            _signalBus.Subscribe<EnemyDiedSignal>(OnEnemyDied);
            SpawnPlayer();
            SpawnStartingEnemies();
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<EnemyDiedSignal>(OnEnemyDied);
        }

        private void SpawnPlayer()
        {
            Player = _diContainer.InstantiatePrefab(_playerPrefab, _playerSpawnPoint.position,
                Quaternion.identity, transform).GetComponent<Player>();
            
            var health = Player.Health;
            health.Initialize(_runState.PlayerMaxHp, _runState.PlayerCurrentHp);
        }

        private void SpawnStartingEnemies()
        {
            for (int i = 0; i < _levelData.Enemies.Length; i++)
            {
                SpawnEnemy(_levelData.Enemies[i], i);
            }
        }

        private void SpawnEnemy(Enemy enemyPrefab, int spawnPointIndex)
        {
            Enemy enemy = _diContainer.InstantiatePrefab(enemyPrefab, _enemySpawnPoints[spawnPointIndex].position,
                Quaternion.identity, transform).GetComponent<Enemy>();
            _nextEnemyId++;
            enemy.SetId(_nextEnemyId);
            _enemies.Add(_nextEnemyId, enemy);
        }

        private void OnEnemyDied(EnemyDiedSignal signal)
        {
            _enemies.Remove(signal.EnemyID);
            _signalBus.Fire(new EnemyDeathRegisteredSignal());
        }
    }
}
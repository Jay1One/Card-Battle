using System;
using System.Collections.Generic;
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
        
        private readonly List<Enemy> _enemies = new List<Enemy>();
        private LevelDataSo _levelData;
        private DiContainer _diContainer;
        private RunState _runState;
        
        public  IReadOnlyList<Enemy> Enemies => _enemies;
        public Player Player { get; private set; }

        public event Action PlayerDied;
        public event Action EnemyDeathRegistered;

        [Inject]
        public void Construct(LevelDataSo levelData, DiContainer diContainer, RunState runState)
        {
            _levelData = levelData;
            _diContainer = diContainer;
            _runState = runState;
        }

        private void Awake()
        {
            SpawnPlayer();
            SpawnStartingEnemies();
        }

        private void OnDestroy()
        {
            Player.PlayerDied -= OnPlayerDied;
        }

        private void SpawnPlayer()
        {
            Player = _diContainer.InstantiatePrefab(_playerPrefab, _playerSpawnPoint.position,
                Quaternion.identity, transform).GetComponent<Player>();
            
            var health = Player.Health;
            health.Initialize(_runState.PlayerMaxHp, _runState.PlayerCurrentHp);
            Player.PlayerDied += OnPlayerDied;
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
            _enemies.Add(enemy);
            enemy.EnemyDied += OnEnemyDied;
        }

        private void OnEnemyDied(Enemy enemy)
        {
            enemy.EnemyDied -= OnEnemyDied;
            _enemies.Remove(enemy);
            EnemyDeathRegistered?.Invoke();
        }

        private void OnPlayerDied()
        {
            PlayerDied?.Invoke();
        }
    }
}
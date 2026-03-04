using System;
using Core.Signals;
using Gameplay.Progression;
using Zenject;

namespace Gameplay.Systems
{
    public class BattleController : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly DeckController _deckController;
        private readonly EnemiesTurnSystem _enemiesTurnSystem;
        private readonly UnitsSystem _unitsSystem;
        private readonly RunState _runState;
        private Turn _currentTurn;
        private bool _isBattleOver;
        
        public event Action PlayerTurnStarted;

        [Inject]
        public BattleController(SignalBus signalBus, RunState runState, DeckController deckController, 
            EnemiesTurnSystem enemiesTurnSystem, UnitsSystem unitsSystem)
        {
            _signalBus = signalBus;
            _runState = runState;
            _deckController = deckController;
            _enemiesTurnSystem = enemiesTurnSystem;
            _unitsSystem = unitsSystem;
        }

        public void StartBattle()
        {
            _isBattleOver = false;
            _currentTurn = Turn.PlayerTurn;
            PlayerTurnStarted?.Invoke();
            _deckController.StartPlayerTurn();
        }
    
        public void Initialize()
        {
            _deckController.PlayerTurnEnded += EndTurn;
            _enemiesTurnSystem.TurnFinished += EndTurn;

            _unitsSystem.EnemyDeathRegistered += OnEnemyDeathRegistered;
            _unitsSystem.PlayerDied += OnPlayerDied;
        }

        public void Dispose()
        {
            _deckController.PlayerTurnEnded -= EndTurn;
            _enemiesTurnSystem.TurnFinished -= EndTurn;
            
            _unitsSystem.EnemyDeathRegistered -= OnEnemyDeathRegistered;
            _unitsSystem.PlayerDied -= OnPlayerDied;
        }

        private void EndTurn()
        {
            if (!_isBattleOver)
            {
                if (_currentTurn == Turn.PlayerTurn)
                {
                    _currentTurn = Turn.EnemyTurn;
                    _enemiesTurnSystem.MakeTurn();
                }

                else if (_currentTurn == Turn.EnemyTurn)
                {
                    _currentTurn = Turn.PlayerTurn;
                    PlayerTurnStarted?.Invoke();
                    _deckController.StartPlayerTurn();
                }
            }
        }
        
        private void OnEnemyDeathRegistered()
        {
            if (_unitsSystem.Enemies.Count == 0)
                EndBattle(true);
        }

        private void OnPlayerDied()
        {
            EndBattle(false);
        }

        private void EndBattle(bool isWin)
        {
            if (_isBattleOver) return;

            _isBattleOver = true;
            _runState.ApplyBattleResult(_unitsSystem.Player.Health.CurrentHealth);
            _signalBus.Fire(new BattleEndedSignal(isWin));
        }
        
        private enum Turn
        {
            PlayerTurn,
            EnemyTurn
        }
    }
}
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core;
using Core.Signals;
using Gameplay.Progression;
using Gameplay.Units;
using Zenject;

namespace Gameplay.Systems
{
    public class BattleController : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly DeckController _deckController;
        private readonly UnitsSystem _unitsSystem;
        private readonly RunState _runState;
        private CancellationTokenSource _cts;
    
        private bool _isBattleOver = false;

        [Inject]
        public BattleController(SignalBus signalBus, RunState runState,DeckController deckController, UnitsSystem unitsSystem)
        {
            _signalBus = signalBus;
            _runState = runState;
            _deckController = deckController;
            _unitsSystem = unitsSystem;
            _cts = new CancellationTokenSource();
        }

        public void StartBattle()
        {
            _isBattleOver = false;
            _=RunBattleAsync(_cts.Token);
        }
    
        public void Initialize()
        {
            _signalBus.Subscribe<EnemyDeathRegisteredSignal>(OnEnemyDeathRegistered);
            _signalBus.Subscribe<PlayerDiedSignal>(OnPlayerDied);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<EnemyDeathRegisteredSignal>(OnEnemyDeathRegistered);
            _signalBus.Unsubscribe<PlayerDiedSignal>(OnPlayerDied);
        
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        private async Task RunBattleAsync(CancellationToken ct)
        {
            try
            {
                while (!_isBattleOver)
                {
                    ct.ThrowIfCancellationRequested();

                    await _deckController.MakeTurnAsync(ct);

                    if (_isBattleOver)
                        break;

                    await RunEnemyTurnAsync(ct);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }
    
        private async Task RunEnemyTurnAsync(CancellationToken ct)
        {
            foreach (var enemy in _unitsSystem.Enemies.Values.ToArray())
            {
                if (enemy==null)
                {
                    continue;
                }
                await enemy.MakeTurnAsync(_unitsSystem.Player, ct);
            }
        }
    
        private void OnEnemyDeathRegistered(EnemyDeathRegisteredSignal signal)
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
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Card_Mechanics;
using Core.Signals;
using Gameplay.Units;
using Zenject;

namespace Gameplay.Systems
{
    public class DeckController : IInitializable, IDisposable
    {
        private readonly Deck _deck;
        private readonly SignalBus _signalBus;
        private readonly UnitsSystem _unitsSystem;
        private readonly Dictionary<int,TaskCompletionSource<bool>> _tasks = new Dictionary<int,TaskCompletionSource<bool>>();
        private int _nextTaskID;
        private TaskCompletionSource<bool> _turnCompletion;
        private Task _endTurnTask = Task.CompletedTask;
        private CancellationToken _cancellationToken;
    
        [Inject]
        public DeckController(Deck deck, UnitsSystem unitsSystem, SignalBus signalBus)
        {
            _unitsSystem = unitsSystem;
            _deck = deck;
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<DrawCardFinishedSignal>(OnCardDrawn);
            _signalBus.Subscribe<DiscardCardFinishedSignal>(OnDiscardCardFinished);
            _signalBus.Subscribe<PlayCardFinishedSignal>(OnPlayCardFinished);
            _signalBus.Subscribe<EndTurnRequestedSignal>(OnEndTurnRequested);
        }
        
        public void Dispose()
        {
            _signalBus.Unsubscribe<DrawCardFinishedSignal>(OnCardDrawn);
            _signalBus.Unsubscribe<DiscardCardFinishedSignal>(OnDiscardCardFinished);
            _signalBus.Unsubscribe<PlayCardFinishedSignal>(OnPlayCardFinished);
            _signalBus.Unsubscribe<EndTurnRequestedSignal>(OnEndTurnRequested);

            foreach (var pair in _tasks)
            {
                pair.Value.TrySetCanceled();
            }
            _tasks.Clear();
        }
        
        public async Task MakeTurnAsync(CancellationToken ct)
        {
            _cancellationToken = ct;
            _turnCompletion = new TaskCompletionSource<bool>();
            
            _signalBus.Fire<PlayerTurnStartedSignal>();
            
            await DealStartingHandAsync(_cancellationToken);
        
            _signalBus.Fire(new StartingHandDealtSignal());
        
            _turnCompletion = new TaskCompletionSource<bool>();
            
            await _turnCompletion.Task;
        }
        
        private async Task DealStartingHandAsync(CancellationToken ct)
        {
            for (int i = 0; i < Deck.HandSize; i++)
            {
                await DrawCardAsync(i, ct);
            }
        }
        
        public async Task DrawCardAsync(int handIndex, CancellationToken ct)
        {
            Card card = _deck.DrawCard(handIndex);
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            _tasks.Add(++_nextTaskID, tcs);
        
            _signalBus.Fire(new DrawCardStartedSignal(handIndex, card, _nextTaskID));
        
            await tcs.Task;
        }
        
        private void OnCardDrawn(DrawCardFinishedSignal signal)
        {
            if (_tasks.Remove(signal.TaskID, out var tcs))
                tcs.TrySetResult(true);
        }

        public async Task DiscardCardAsync(int handIndex, CancellationToken ct)
        {
            Card card = _deck.Discard(handIndex);
        
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            _tasks.Add(++_nextTaskID, tcs);
        
            _signalBus.Fire(new DiscardCardStartedSignal(handIndex, card, _nextTaskID));
        
            await tcs.Task;
        }

        private void OnDiscardCardFinished(DiscardCardFinishedSignal signal)
        {
            if (_tasks.Remove(signal.TaskID, out var tcs))
                tcs.TrySetResult(true);
        }

        private async Task PlayHand(CancellationToken ct)
        {
            var hand = _deck.Hand;
            
            for (int i = 0; i < Deck.HandSize; i++)
            {
                if (_unitsSystem.Enemies.Count ==0)
                {
                    break;
                }
                
                Card card = hand[i];
                TaskCompletionSource<bool> playTask = new TaskCompletionSource<bool>();
                _tasks.Add(++_nextTaskID, playTask);
                _signalBus.Fire(new PlayCardStartedSignal(i, _nextTaskID));
                
                if (card.ActionType == CardActionType.Defense)
                {
                    await _unitsSystem.Player.GainBlockAsync(card.Value, ct);
                }

                if (card.ActionType == CardActionType.Attack)
                {
                    Enemy firstEnemy = _unitsSystem.Enemies.ToArray()[0].Value;
                    
                    await _unitsSystem.Player.AttackAsync(firstEnemy, card.Value, ct);
                }
                
                await playTask.Task;
                await DiscardCardAsync(i, ct);
            }
        }
        
        private void OnEndTurnRequested(EndTurnRequestedSignal signal)
        {
            if (_endTurnTask.IsCompleted)
                _endTurnTask = EndTurnAsync(_cancellationToken);
        }

        private async Task EndTurnAsync(CancellationToken ct)
        {
            if (_tasks.Count>0)
            {
                Task[] remainingTasks = _tasks.Values.Select(x => x.Task).ToArray();
                await Task.WhenAll(remainingTasks);
            }
            
            await PlayHand(ct);
            
            _turnCompletion.TrySetResult(true);
        }

        private void OnPlayCardFinished(PlayCardFinishedSignal signal)
        {
            _tasks[signal.TaskID].TrySetResult(true);
            _tasks.Remove(signal.TaskID);
        }
    }
}
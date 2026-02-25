using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Card_Mechanics;
using Core.Signals;
using Zenject;

namespace Gameplay.Systems
{
    public class RedrawSystem : IInitializable, IDisposable
    {
        private const int MaxRerolls = 3;
        
        private readonly SignalBus _signalBus;
        private readonly DeckController _deckController;
        private readonly bool [] _handLockedStates = new bool[Deck.HandSize];
        private int _currentRedraws;
        private bool _isPlayerTurn;
        private bool _redrawInProgress;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        [Inject]
        public RedrawSystem(SignalBus signalBus, DeckController deckController)
        {
            _signalBus = signalBus;
            _deckController = deckController;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<StartingHandDealtSignal>(OnStartingHandDealt);
            _signalBus.Subscribe<CardLockChangeRequestedSignal>(OnCardLockChangeRequested);
            _signalBus.Subscribe<RedrawStartedSignal>(OnRedrawStarted);
        }
        
        public void Dispose()
        {
            _signalBus.Unsubscribe<StartingHandDealtSignal>(OnStartingHandDealt);
            _signalBus.Unsubscribe<RedrawStartedSignal>(OnRedrawStarted);
            _signalBus.Unsubscribe<CardLockChangeRequestedSignal>(OnCardLockChangeRequested);
            
            _cts.Cancel();
            _cts.Dispose();
        }

        private void OnRedrawStarted()
        {
            if (_redrawInProgress || !_isPlayerTurn || _currentRedraws <= 0)
            {
                return;
            }
            
            _redrawInProgress = true;
            _ = RedrawAsync(_cts.Token);
        }

        private async Task RedrawAsync(CancellationToken ct)
        {
            try
            {
                _currentRedraws--;

                List<int> handIndexes = new List<int>();

                for (int i = 0; i < _handLockedStates.Length; i++)
                {
                    if (_handLockedStates[i] == false)
                    {
                        handIndexes.Add(i);
                    }
                }

                List<Task> discardTasks = new List<Task>();

                foreach (int handIndex in handIndexes)
                {
                    discardTasks.Add(_deckController.DiscardCardAsync(handIndex, ct));
                }

                await Task.WhenAll(discardTasks);

                List<Task> drawTasks = new List<Task>();

                foreach (int handIndex in handIndexes)
                {
                    drawTasks.Add(_deckController.DrawCardAsync(handIndex, ct));
                }

                await Task.WhenAll(drawTasks);

                if (_currentRedraws == 0)
                {
                    for (int i = 0; i < _handLockedStates.Length; i++)
                    {
                        if (_handLockedStates[i] == true)
                        {
                            _handLockedStates[i] = false;
                            _signalBus.Fire(new CardLockChangeProcessedSignal(i, true, 
                                false, _handLockedStates[i]));
                        }
                    }
                }

                _signalBus.Fire(new RedrawsChangedSignal(_currentRedraws, _currentRedraws <= 0));
            }

            catch (OperationCanceledException)
            {
            }
            finally
            {
                _redrawInProgress = false;
            }
        }

        private void OnStartingHandDealt()
        {
            _isPlayerTurn = true;
            _currentRedraws = MaxRerolls;

            for (int i = 0; i < _handLockedStates.Length; i++)
            {
                _handLockedStates[i] = false;
            }
            _signalBus.Fire(new RedrawsChangedSignal(_currentRedraws, _currentRedraws <= 0));
        }

        private bool AreAllCardsLocked()
        {
            bool result = true;

            foreach (bool handLockedState in _handLockedStates)
            {
                if (handLockedState == false)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        private void OnCardLockChangeRequested(CardLockChangeRequestedSignal signal)
        {
            bool isLockChangeAllowed = _isPlayerTurn && _currentRedraws > 0 && !_redrawInProgress;
            
            if (isLockChangeAllowed)
            {
                _handLockedStates[signal.HandIndex] = !_handLockedStates[signal.HandIndex];
            }
            
            _signalBus.Fire(new CardLockChangeProcessedSignal(signal.HandIndex, isLockChangeAllowed,
                !AreAllCardsLocked(), _handLockedStates[signal.HandIndex]));
        }
    }
}
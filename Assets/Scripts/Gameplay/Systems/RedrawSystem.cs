using System;
using System.Collections;
using System.Collections.Generic;
using Core.Card_Mechanics;
using UnityEngine;
using Zenject;

namespace Gameplay.Systems
{
    public class RedrawSystem : MonoBehaviour, IInitializable, IDisposable
    {
        private const int MaxRerolls = 3;
        
        private DeckController _deckController;
        private readonly bool [] _handLockedStates = new bool[Deck.HandSize];
        private int _currentRedraws;
        private bool _isPlayerTurn;
        private bool _redrawInProgress;
        int _discardActionsLeft;
        int _drawActionsLeft;

        
        public event Action<int> RedrawsChanged;
        public event Action<bool> CardLockRequestProcessed;
        public event Action<bool> RerollPossibilityChanged;
        
        [Inject]
        public void Construct(DeckController deckController)
        {
            _deckController = deckController;
        }

        public void Initialize()
        {
            _deckController.StartingHandDealt += OnStartingHandDealt;
        }
        
        public void Dispose()
        {
            _deckController.StartingHandDealt -= OnStartingHandDealt;
        }

        public void StartRedraw()
        {
            if (_redrawInProgress || !_isPlayerTurn || _currentRedraws <= 0)
            {
                return;
            }

            StartCoroutine(RedrawCoroutine());
        }

        private IEnumerator RedrawCoroutine()
        {
            _redrawInProgress = true;
            _currentRedraws--;
            _discardActionsLeft = 0;
            _drawActionsLeft = 0;
            
            List<int> handIndexes = new List<int>();
            
            for (int i = 0; i < _handLockedStates.Length; i++)
            {
                if (_handLockedStates[i] == false)
                {
                    handIndexes.Add(i);
                }
            }
      
            foreach (int index in handIndexes)
            {
                _discardActionsLeft++;
                StartCoroutine(_deckController.DiscardCardCoroutine(index, RegisterDiscardEnd));
            }

            while (_discardActionsLeft > 0)
            {
                yield return null;
            }
            
            foreach (int index in handIndexes)
            {
                _drawActionsLeft++;
                StartCoroutine(_deckController.DrawCardCoroutine(index, RegisterDrawEnd));
            }

            while (_drawActionsLeft > 0)
            {
                yield return null;
            }
            
            _redrawInProgress = false;
            RedrawsChanged?.Invoke(_currentRedraws);
        }
        
        private void RegisterDiscardEnd()
        {
            _discardActionsLeft--;
        }
        
        private void RegisterDrawEnd()
        {
            _drawActionsLeft--;
        }

        private void OnStartingHandDealt()
        {
            _isPlayerTurn = true;
            _currentRedraws = MaxRerolls;

            for (int i = 0; i < _handLockedStates.Length; i++)
            {
                _handLockedStates[i] = false;
            }
            
            RedrawsChanged?.Invoke(_currentRedraws);
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

        public void TryChangeCardLockState(int handIndex)
        {
            bool isLockChangeAllowed = _isPlayerTurn && _currentRedraws > 0 && !_redrawInProgress;
            
            if (isLockChangeAllowed)
            {
                _handLockedStates[handIndex] = !_handLockedStates[handIndex];
            }
            
            CardLockRequestProcessed?.Invoke(isLockChangeAllowed);
            RerollPossibilityChanged?.Invoke(!AreAllCardsLocked() && _currentRedraws > 0);
        }
    }
}
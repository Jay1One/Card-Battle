using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Card_Mechanics;
using UnityEngine;
using Zenject;

namespace Gameplay.Systems
{
    public class DeckController : MonoBehaviour
    {
        private Deck _deck;
        private UnitsSystem _unitsSystem;
        private int _nextActionID;
        private readonly List<int> _pendingActionIDs = new List<int>();
        
        public event Action StartingHandDealt;
        public event Action<int, Card, int> CardDrawStarted;
        public event Action CardDrawEnded;
        public event Action<int, int> CardDiscardStarted;
        public event Action CardDiscardEnded;
        public event Action<int> CardPlayStarted;
        public event Action EndTurnRequested; 
        public event Action PlayerTurnEnded;
        
        [Inject]
        public void Construct(Deck deck, UnitsSystem unitsSystem)
        {
            _unitsSystem = unitsSystem;
            _deck = deck;
        }

        public void StartPlayerTurn()
        {
            StartCoroutine(DealStartingHandCoroutine());
        }

        public void RegisterCardDraw(int actionID)
        {
            _pendingActionIDs.Remove(actionID);
        }

        public void RegisterCardDiscard(int actionID)
        {
            _pendingActionIDs.Remove(actionID);
        }
        
        public IEnumerator DrawCardCoroutine(int handIndex, Action drawFinishedCallBack = null)
        {
            _nextActionID++;
            _pendingActionIDs.Add(_nextActionID);
            Card card = _deck.DrawCard(handIndex);
            CardDrawStarted?.Invoke(handIndex, card, _nextActionID);
            
            while (_pendingActionIDs.Contains(_nextActionID))
            {
                yield return null;
            }
            
            CardDrawEnded?.Invoke();
            drawFinishedCallBack?.Invoke();
        }

        public IEnumerator DiscardCardCoroutine(int handIndex, Action discardFinishedCallBack = null)
        {
            _nextActionID++;
            _pendingActionIDs.Add(_nextActionID);
            _deck.Discard(handIndex);
            CardDiscardStarted?.Invoke(handIndex, _nextActionID);
            
            while (_pendingActionIDs.Contains(_nextActionID))
            {
                yield return null;
            }
            
            CardDiscardEnded?.Invoke();
            discardFinishedCallBack?.Invoke();
        }
        
        private IEnumerator PlayCardCoroutine(int handIndex)
        {
            if (_unitsSystem.Enemies.Count == 0) yield break;
            
            Card card = _deck.Hand[handIndex];
            CardPlayStarted?.Invoke(handIndex);

            if (card.ActionType == CardActionType.Attack)
            {
                var enemies = _unitsSystem.Enemies.ToArray();
                
                yield return _unitsSystem.Player.AttackCoroutine(enemies[0],card.Value);
            }

            if (card.ActionType == CardActionType.Defense)
            {
                yield return _unitsSystem.Player.GainBlockCoroutine(card.Value);
            }
        }
        
        private IEnumerator DealStartingHandCoroutine()
        {
            for (int i = 0; i < Deck.HandSize; i++)
            {
                yield return DrawCardCoroutine(i);
            }
            
            StartingHandDealt?.Invoke();
        }
        
        private IEnumerator PlayHandCoroutine()
        {
            for (int i = 0; i < _deck.Hand.Count; i++)
            {
                yield return PlayCardCoroutine(i);
                yield return DiscardCardCoroutine(i);
            }
      
            PlayerTurnEnded?.Invoke();
        }
        
        public void RequestTurnEnd()
        {
            EndTurnRequested?.Invoke();
            StartCoroutine(PlayHandCoroutine());
        }
    }
}
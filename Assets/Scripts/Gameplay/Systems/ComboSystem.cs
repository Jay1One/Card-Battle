using System;
using System.Linq;
using Core.Card_Mechanics;
using Core.Signals;
using UnityEngine;
using Zenject;

namespace Gameplay.Systems
{
    public class ComboSystem : IInitializable, IDisposable
    {
        private readonly Deck _deck;
        private readonly SignalBus _signalBus;
        
        [Inject]
        public ComboSystem(Deck deck, SignalBus signalBus)
        {
            _deck = deck;
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<DrawCardFinishedSignal>(OnDrawCardFinished);
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<DrawCardFinishedSignal>(OnDrawCardFinished);
        }

        private void OnDrawCardFinished()
        {
            foreach (var handCard in _deck.Hand)
            {
                if (handCard ==null)
                {
                    continue;
                }
                
                handCard.ClearComboBuff();
                
                int sameElements = 0;
                int sameSuits = 0;
                
                foreach (var otherHandCard in _deck.Hand)
                {

                    if (otherHandCard == null || handCard == otherHandCard)
                    {
                        continue;
                    }
                    
                    if (otherHandCard.Element == handCard.Element)
                    {
                        sameElements++;
                    }

                    if (otherHandCard.Suit == handCard.Suit)
                    {
                        sameSuits++;
                    }
                }
                
                handCard.ApplyComboBuff(sameElements + sameSuits);
            }
        }
    }
}
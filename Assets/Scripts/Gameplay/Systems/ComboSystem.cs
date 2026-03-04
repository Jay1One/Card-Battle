using System;
using Core.Card_Mechanics;
using Zenject;

namespace Gameplay.Systems
{
    public class ComboSystem : IInitializable, IDisposable
    {
        private readonly Deck _deck;
        private readonly DeckController _deckController;
        
        [Inject]
        public ComboSystem(Deck deck, DeckController deckController)
        {
            _deck = deck;
            _deckController = deckController;
        }

        public void Initialize()
        {
            _deckController.CardDrawEnded += OnDrawCardFinished;
        }

        public void Dispose()
        {
            _deckController.CardDrawEnded -= OnDrawCardFinished;
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
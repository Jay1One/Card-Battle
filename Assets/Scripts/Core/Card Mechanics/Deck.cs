using System.Collections.Generic;
using System.Linq;
using Random = System.Random;

namespace Core.Card_Mechanics
{
    public class Deck
    {
        public const int HandSize = 5;
    
        private List<Card> _drawPile;
        private readonly List<Card> _discardPile = new List<Card>();
        private readonly Card[] _hand = new Card[HandSize];
        private readonly Random _random = new Random();
    
        public IReadOnlyList<Card> Hand => _hand;

        public Deck(List<Card> cards)
        {
            _drawPile = cards;
        
            ShuffleDrawPile();
        }

        public IReadOnlyList<Card> DrawPile => _drawPile;
        public IReadOnlyList<Card> DiscardPile => _discardPile;

        public Card DrawCard(int handIndex)
        {
            if (_drawPile.Count == 0)
            {
                RefillDrawPileFromDiscardPile();
                ShuffleDrawPile();
            }
        
            int drawPileLastIndex = _drawPile.Count - 1;
            Card card = _drawPile[drawPileLastIndex];
            _drawPile.RemoveAt(drawPileLastIndex);
            _hand[handIndex] = card;
        
            return card;
        }

        public Card Discard(int index)
        {
            Card card = _hand[index];
            _discardPile.Add(card);
            _hand[index] = null;
            return card;
        }

        private void RefillDrawPileFromDiscardPile()
        {
            _drawPile.AddRange(_discardPile);
            _discardPile.Clear();
        }
    
        private void ShuffleDrawPile()
        {
            List<Card> shuffledDrawPile = _drawPile.OrderBy(_ => _random.Next()).ToList();
            _drawPile = shuffledDrawPile;
        }
    }
}

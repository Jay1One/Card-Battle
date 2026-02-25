using System;

namespace Core.Card_Mechanics
{
    public class Card
    {
        private readonly CardTemplate _template;
        public int Value { get; private set; }
        public Element Element { get; }
        public Suit Suit { get; }
        public CardActionType ActionType { get; }
    
        public event Action ComboBonusApplied; 
    
        public Card(CardTemplate template)
        {
            _template = template;
        
            Value = template.Value;
            Element = template.Element;
            Suit = template.Suit;
            ActionType = template.ActionType;
        }

        public void ApplyComboBuff(int bonus)
        {
            Value = _template.Value + bonus;
            ComboBonusApplied?.Invoke();
        }

        public void ClearComboBuff()
        {
            Value = _template.Value;
        }
    }
}

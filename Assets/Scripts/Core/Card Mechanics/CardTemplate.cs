namespace Core.Card_Mechanics
{
    public class CardTemplate
    {
        public CardActionType ActionType { get; }
        public Suit Suit { get; }
        public Element Element { get; }
        public int Value { get; }

        public CardTemplate(Suit suit, Element element, CardActionType actionType, int value)
        {
            Suit = suit;
            Element = element;
            ActionType = actionType;
            Value = value;
        }
    }
}

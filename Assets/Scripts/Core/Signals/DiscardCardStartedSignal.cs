using Core.Card_Mechanics;

namespace Core.Signals
{
    public class DiscardCardStartedSignal
    {
        public int HandIndex { get;}
        public Card Card { get; }
        public int TaskID { get; }

        public DiscardCardStartedSignal(int handIndex, Card card, int taskID)
        {
            HandIndex = handIndex;
            Card = card;
            TaskID = taskID;
        }
    }
}
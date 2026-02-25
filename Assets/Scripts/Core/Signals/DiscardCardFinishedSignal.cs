using Core.Card_Mechanics;

namespace Core.Signals
{
    public class DiscardCardFinishedSignal
    {
        public Card Card { get; }
        public int TaskID { get; }

        public DiscardCardFinishedSignal(Card card, int taskID)
        {
            Card = card;
            TaskID = taskID;
        }
    }
}
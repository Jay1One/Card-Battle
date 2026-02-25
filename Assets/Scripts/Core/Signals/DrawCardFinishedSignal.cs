using Core.Card_Mechanics;

namespace Core.Signals
{
    public class DrawCardFinishedSignal
    {
        public Card Card { get; }
        public int TaskID { get; }

        public DrawCardFinishedSignal(Card card, int taskID)
        {
            Card = card;
            TaskID = taskID;
        }
    }
}
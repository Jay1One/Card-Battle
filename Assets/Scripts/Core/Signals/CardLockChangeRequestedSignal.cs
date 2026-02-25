namespace Core.Signals
{
    public class CardLockChangeRequestedSignal
    {
        public int HandIndex { get; private set; }
        
        public CardLockChangeRequestedSignal(int handIndex)
        {
            HandIndex = handIndex;
        }
    }
}
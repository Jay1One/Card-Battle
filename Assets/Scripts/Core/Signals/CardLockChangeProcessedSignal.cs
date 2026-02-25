namespace Core.Signals
{
    public class CardLockChangeProcessedSignal
    {
        public int HandIndex { get; private set; }
        public bool IsLockChangeAllowed { get; private set; }
        public bool IsRerollPossible { get; private set; }
        public bool IsLocked { get; private set; }

        public CardLockChangeProcessedSignal(int handIndex, bool isLockChangeAllowed, bool isRerollPossible, bool isLocked)
        {
            HandIndex = handIndex;
            IsLockChangeAllowed = isLockChangeAllowed;
            IsRerollPossible = isRerollPossible;
            IsLocked = isLocked;
        }
    }
}
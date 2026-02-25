namespace Core.Signals
{
    public class RedrawsChangedSignal
    {
        public int RerollsLeft { get; private set; }
        public bool RerollsFinished { get; private set; }

        public RedrawsChangedSignal(int rerollsLeft, bool rerollsFinished)
        {
            RerollsLeft = rerollsLeft;
            RerollsFinished = rerollsFinished;
        }
    }
}
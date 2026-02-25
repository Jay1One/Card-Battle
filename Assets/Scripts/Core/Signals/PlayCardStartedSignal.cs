namespace Core.Signals
{
    public class PlayCardStartedSignal
    {
        public int HandIndex { get; }
        public int TaskID { get; private set; }

        public PlayCardStartedSignal(int handIndex, int taskID)
        {
            HandIndex = handIndex;
            TaskID = taskID;
        }
    }
}
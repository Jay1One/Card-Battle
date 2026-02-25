namespace Core.Signals
{
    public class PlayCardFinishedSignal
    {
        public int TaskID { get; private set; }

        public PlayCardFinishedSignal(int taskID)
        {
            TaskID = taskID;
        }
    }
}
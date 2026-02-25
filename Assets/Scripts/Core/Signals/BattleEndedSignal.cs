namespace Core.Signals
{
    public class BattleEndedSignal
    {
        public bool  IsWin { get; }

        public BattleEndedSignal(bool isWin)
        {
            IsWin = isWin;
        }
    }
}
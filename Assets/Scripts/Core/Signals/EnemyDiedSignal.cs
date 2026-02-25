namespace Core.Signals
{
    public class EnemyDiedSignal
    {
        public int EnemyID { get; private set; }

        public EnemyDiedSignal(int id)
        {
            EnemyID = id;
        }
    }
}
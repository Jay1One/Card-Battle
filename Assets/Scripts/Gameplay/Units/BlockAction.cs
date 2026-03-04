namespace Gameplay.Units
{
    public class BlockAction : UnitAction
    {
        public int BlockAmount { get; }

        public BlockAction(int blockAmount)
        {
            BlockAmount = blockAmount;
        }
    }
}
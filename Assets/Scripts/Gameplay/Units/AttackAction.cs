namespace Gameplay.Units
{
    public class AttackAction : UnitAction
    {
        public Unit Target { get;}
        public int Damage { get;}
        
        public AttackAction(Unit target, int damage)
        {
            Target = target;
            Damage = damage;
        }
    }
}
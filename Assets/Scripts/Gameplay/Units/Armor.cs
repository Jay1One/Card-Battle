using System;

namespace Gameplay.Units
{
    [Serializable]
    public class Armor
    {
        public int Value { get; private set; }
        
        public event Action ValueChanged;

        public void BlockDamage(ref int damage)
        {
            if (damage >= Value)
            {
                damage -= Value;
                Value = 0;
            }
            else
            {
                Value -= damage;
                damage = 0;
            }
            
            ValueChanged?.Invoke();
        }

        public void ResetValue()
        {
            Value = 0;
            ValueChanged?.Invoke();
        }

        public void AddArmor(int value)
        {
            Value += value;
            ValueChanged?.Invoke();
        }
    }
}
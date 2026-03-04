using System;

namespace Gameplay.Units
{
    [Serializable]
    public class Health
    {
        private int _maxHealth;
        private int _currentHealth;
        
        public int MaxHealth => _maxHealth;
        public int CurrentHealth => _currentHealth;
        
        public event Action<int> HealthChanged;

        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;

            if (_currentHealth < 0)
            {
                _currentHealth = 0;
            }

            HealthChanged?.Invoke(_currentHealth);
        }

        public void Initialize(int maxHp, int currentHp)
        {
            _maxHealth = maxHp;
            _currentHealth = currentHp;
        }

        public void Initialize(int maxHp)
        {
            Initialize(maxHp, maxHp);
        }
    }
}
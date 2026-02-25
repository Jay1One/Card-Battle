using System;
using UnityEngine;

namespace Gameplay.Units
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private int _maxHealth;
        public event Action<int> HealthChanged;
        private int _currentHealth;
        
        public int MaxHealth => _maxHealth;
        public int CurrentHealth => _currentHealth;

        private void Awake()
        {
            _currentHealth = _maxHealth;
        }

        private void Start()
        {
            HealthChanged?.Invoke(_currentHealth);
        }

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
            HealthChanged?.Invoke(_currentHealth);
        }
    }
}
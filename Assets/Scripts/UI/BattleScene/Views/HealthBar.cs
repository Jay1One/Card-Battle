using System.Collections;
using Gameplay.Units;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.BattleScene.Views
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Unit _unit;
        [SerializeField] private Image _fillImage;
        [SerializeField] private TMP_Text _healthText;
        [SerializeField] private float _updateTime = 0.5f;
        private Health _health;
        
        private Coroutine _updateCoroutine;

        private void Awake()
        {
            _health = _unit.Health;
            _health.HealthChanged += OnHealthChanged;
        }

        private void Start()
        {
            OnHealthChanged(_health.CurrentHealth);
        }

        private void OnHealthChanged(int health)
        {
            if (_updateCoroutine != null)
            {
                StopCoroutine(_updateCoroutine);
            }
            
            _healthText.text = health.ToString();
            StartCoroutine(UpdateHealthCoroutine());
        }

        private IEnumerator UpdateHealthCoroutine()
        {
            float startFill = _fillImage.fillAmount;
            float endFill = (float)_health.CurrentHealth/_health.MaxHealth;
            float startTime = Time.time;
            float endTime = startTime+ _updateTime;

            while (Time.time < endTime)
            {
               _fillImage.fillAmount = Mathf.Lerp(startFill, endFill, (Time.time - startTime) / _updateTime); 
               yield return null;
            }
            
            _fillImage.fillAmount = endFill;
        }
    }
}
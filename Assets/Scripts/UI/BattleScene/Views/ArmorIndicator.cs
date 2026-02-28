using Gameplay.Units;
using TMPro;
using UnityEngine;

namespace UI.BattleScene.Views
{
    public class ArmorIndicator : MonoBehaviour
    {
        [SerializeField] private Unit _unit;
        [SerializeField] private TMP_Text _armorText;
        [SerializeField] private CanvasGroup _canvasGroup;
        private Armor _armor;

        private void Awake()
        {
            _armor = _unit.Armor;
            _armor.ValueChanged += UpdateIndicator;
            
            UpdateIndicator();
        }

        private void OnDestroy()
        {
            _armor.ValueChanged -= UpdateIndicator;
        }

        private void UpdateIndicator()
        {
            if (_armor.Value > 0)
            {
                _canvasGroup.alpha = 1;
                _armorText.text = _armor.Value.ToString();
            }
            
            else
            {
                _canvasGroup.alpha = 0;
            }
        }
    }
}
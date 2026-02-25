using Gameplay.Units;
using TMPro;
using UnityEngine;

namespace UI.BattleScene.Views
{
    public class ArmorIndicator : MonoBehaviour
    {
        [SerializeField] private TMP_Text _armorText;
        [SerializeField] private Armor _armor;
        [SerializeField] private CanvasGroup _canvasGroup;

        private void Awake()
        {
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
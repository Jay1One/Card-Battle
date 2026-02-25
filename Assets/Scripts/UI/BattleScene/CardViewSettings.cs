using UnityEngine;

namespace UI.BattleScene
{
    [CreateAssetMenu(fileName = "CardView settings", menuName = "CardView settings", order = 1)]
    public class CardViewSettings : ScriptableObject
    {
        [SerializeField] private Sprite[] _elementSprites;
        [SerializeField] private Sprite[] _suitSprites;
        [SerializeField] private Sprite[] _actionSprites;
    
        public Sprite[] ElementSprites => _elementSprites;
        public Sprite[] SuitSprites => _suitSprites;
        public Sprite[] ActionSprites => _actionSprites;
    }
}
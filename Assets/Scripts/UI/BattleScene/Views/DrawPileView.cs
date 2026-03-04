using Core.Card_Mechanics;
using Gameplay.Systems;
using TMPro;
using UnityEngine;
using Zenject;

namespace UI.BattleScene.Views
{
    public class DrawPileView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _counterText;
        
        private DeckController _deckController;
        private Deck _deck;

        [Inject]
        public void Construct(Deck deck, DeckController deckController)
        {
            _deckController = deckController;
            _deck = deck;
        }

        private void Awake()
        {
            _deckController.CardDrawEnded += UpdateCounter;
        }

        private void OnDestroy()
        {
            _deckController.CardDrawEnded += UpdateCounter;
        }

        private void UpdateCounter()
        {
            _counterText.text = _deck.DrawPile.Count.ToString();
        }
    }
}
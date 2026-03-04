using Core.Card_Mechanics;
using Gameplay.Systems;
using TMPro;
using UnityEngine;
using Zenject;

namespace UI.BattleScene.Views
{
    public class DiscardPileView : MonoBehaviour
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
            _deckController.CardDiscardEnded += UpdateCounter;
        }

        private void OnDestroy()
        {
            _deckController.CardDrawEnded -= UpdateCounter;
            _deckController.CardDiscardEnded -= UpdateCounter;
        }

        private void UpdateCounter()
        {
            _counterText.text = _deck.DiscardPile.Count.ToString();
        }
    }
}
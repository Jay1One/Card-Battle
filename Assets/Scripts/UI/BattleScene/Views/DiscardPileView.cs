using Core.Card_Mechanics;
using Core.Signals;
using TMPro;
using UnityEngine;
using Zenject;

namespace UI.BattleScene.Views
{
    public class DiscardPileView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _counterText;
        
        private SignalBus _signalBus;
        private Deck _deck;

        [Inject]
        public void Construct(Deck deck, SignalBus signalBus)
        {
            _signalBus = signalBus;
            _deck = deck;
        }

        private void Awake()
        {
            _signalBus.Subscribe<DiscardCardFinishedSignal>(UpdateCounter);
            _signalBus.Subscribe<DrawCardStartedSignal>(UpdateCounter);
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<DiscardCardFinishedSignal>(UpdateCounter);
            _signalBus.Unsubscribe<DrawCardStartedSignal>(UpdateCounter);
        }

        private void UpdateCounter()
        {
            _counterText.text = _deck.DiscardPile.Count.ToString();
        }
    }
}
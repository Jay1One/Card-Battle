using System.Collections;
using Core.Card_Mechanics;
using DG.Tweening;
using Gameplay.Systems;
using UnityEngine;
using Zenject;

namespace UI.BattleScene.Views
{
    public sealed class HandView : MonoBehaviour
    { 
        [SerializeField] private Transform[] _handPoints;
        [SerializeField] private Transform _drawOriginPoint;
        [SerializeField] private Transform _discardEndPoint;
        [SerializeField] private float _drawTime = 0.5f;
        [SerializeField] private float _discardTime = 0.2f;
        [SerializeField] private float _playAnimationDistance = 1f;
        [SerializeField] private float _playCardTime = 0.2f;
        

        private readonly CardView[] _cardViews = new CardView[Deck.HandSize];
        private CardViewFactory _cardViewFactory;
        private DeckController _deckController;
    
        [Inject]
        public void Construct(CardViewFactory viewFactory, DeckController deckController)
        {
            _cardViewFactory = viewFactory;
            _deckController = deckController;
        }

        private void Awake()
        {
            _deckController.CardDrawStarted += OnDrawRequested;
            _deckController.CardDiscardStarted += OnDiscardRequested;
            _deckController.CardPlayStarted += OnPlayRequested;
        }

        private void OnDiscardRequested(int handIndex, int actionId)
        {
            StartCoroutine(AnimateDiscard(handIndex, actionId));
        }

        private void OnDrawRequested(int handIndex, Card card, int actionID)
        {
            StartCoroutine(AnimateDraw(handIndex, card, actionID));
        }

        private IEnumerator AnimateDraw(int handIndex, Card card, int taskID)
        {
            CardView cardView = _cardViewFactory.CreateCardView(card);
            cardView.transform.position = _drawOriginPoint.position;
            cardView.transform.DOMove(_handPoints[handIndex].position, _drawTime);
            _cardViews[handIndex] = cardView;
        
            yield return new WaitForSeconds(_drawTime);
        
            cardView.PlaceAtHand(handIndex);
            _deckController.RegisterCardDraw(taskID);
        }

        private IEnumerator AnimateDiscard(int handIndex, int actionID)
        {
            CardView cardView = _cardViews[handIndex];
            cardView.transform.DOMove(_discardEndPoint.position, _discardTime);
            cardView.transform.DOScale(Vector3.zero, _discardTime);
        
            yield return new WaitForSeconds(_discardTime);
        
            Destroy(cardView.gameObject);
            _deckController.RegisterCardDiscard(actionID);
        }

        private void OnPlayRequested(int handIndex)
        {
            StartCoroutine(AnimatePlay(handIndex));
        }

        private IEnumerator AnimatePlay(int handIndex)
        {           
            CardView cardView = _cardViews[handIndex];
            Vector3 endPos = _handPoints[handIndex].position + Vector3.up * _playAnimationDistance;
            cardView.transform.DOMove(endPos, _playCardTime);
            
            yield return new WaitForSeconds(_playCardTime);
        }

        public void OnDestroy()
        {
            if (_deckController != null)
            {
                _deckController.CardDrawStarted -= OnDrawRequested;
                _deckController.CardDiscardStarted -= OnDiscardRequested;
                _deckController.CardPlayStarted -= OnPlayRequested;
            }
        }
    }
}
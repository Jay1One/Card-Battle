using System.Collections;
using Core.Card_Mechanics;
using Core.Signals;
using DG.Tweening;
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
        
        private  SignalBus _bus;
        private readonly CardView[] _cardViews = new CardView[Deck.HandSize];
        private CardViewFactory _cardViewFactory;
    
        [Inject]
        public void Construct(SignalBus bus, CardViewFactory viewFactory)
        {
            _bus = bus;
            _cardViewFactory = viewFactory;
        }

        private void Awake()
        {
            _bus.Subscribe<DrawCardStartedSignal>(OnDrawRequested);
            _bus.Subscribe<PlayCardStartedSignal>(OnPlayRequested);
            _bus.Subscribe<DiscardCardStartedSignal>(OnDiscardRequested);
        }

        private void OnDiscardRequested(DiscardCardStartedSignal signal)
        {
            StartCoroutine(AnimateDiscard(signal.HandIndex, signal.Card, signal.TaskID));
        }

        private void OnDrawRequested(DrawCardStartedSignal signal)
        {
            StartCoroutine(AnimateDraw(signal.HandIndex, signal.Card, signal.TaskID));
        }

        private IEnumerator AnimateDraw(int handIndex, Card card, int taskID)
        {
            CardView cardView = _cardViewFactory.CreateCardView(card);
            cardView.transform.position = _drawOriginPoint.position;
            cardView.transform.DOMove(_handPoints[handIndex].position, _drawTime);
            _cardViews[handIndex] = cardView;
        
            yield return new WaitForSeconds(_drawTime);
        
            cardView.PlaceAtHand(handIndex);
            _bus.Fire(new DrawCardFinishedSignal(card, taskID));
        }

        private IEnumerator AnimateDiscard(int handIndex, Card card, int taskID)
        {
            CardView cardView = _cardViews[handIndex];
            cardView.transform.DOMove(_discardEndPoint.position, _discardTime);
            cardView.transform.DOScale(Vector3.zero, _discardTime);
        
            yield return new WaitForSeconds(_discardTime);
        
            Destroy(cardView.gameObject);
            _bus.Fire(new DiscardCardFinishedSignal(card, taskID));
        }

        private void OnPlayRequested(PlayCardStartedSignal signal)
        {
            StartCoroutine(AnimatePlay(signal.HandIndex, signal.TaskID));
        }

        private IEnumerator AnimatePlay(int handIndex, int taskID)
        {           
            CardView cardView = _cardViews[handIndex];
            Vector3 endPos = _handPoints[handIndex].position + Vector3.up * _playAnimationDistance;
            cardView.transform.DOMove(endPos, _playCardTime);
            yield return new WaitForSeconds(_playCardTime);
        
            _bus.Fire(new PlayCardFinishedSignal(taskID));
        }

        public void OnDestroy()
        {
            _bus.Unsubscribe<DrawCardStartedSignal>(OnDrawRequested);
            _bus.Unsubscribe<PlayCardStartedSignal>(OnPlayRequested);
            _bus.Unsubscribe<DiscardCardStartedSignal>(OnDiscardRequested);
        }
    }
}
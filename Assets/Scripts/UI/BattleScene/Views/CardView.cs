using Core.Card_Mechanics;
using Core.Signals;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace UI.BattleScene.Views
{
      public class CardView : MonoBehaviour , IPointerClickHandler
      {
            [SerializeField] private SpriteRenderer _suitSprite;
            [SerializeField] private SpriteRenderer _elementSprite; 
            [SerializeField] private SpriteRenderer _actionTypeSprite;
            [SerializeField] private SpriteRenderer _frame;
            [SerializeField] private TMP_Text _valueText;
            
            private SignalBus _signalBus;
            private Card _card;
            private int _handIndex;
            private bool _isAtHand;
            private float _scaleTime = 0.2f;
            private float _comboHighlightScale= 1.5f;
            

            [Inject]
            private void Construct(SignalBus signalBus)
            {
                  _signalBus = signalBus;
            }
            public void Initialize(Card card, Sprite suitSprite, Sprite elementSprite, Sprite actionTypeSprite)
            {
                  _suitSprite.sprite= suitSprite;
                  _elementSprite.sprite = elementSprite;
                  _actionTypeSprite.sprite = actionTypeSprite;
                  _valueText.text = card.Value.ToString();
                  _card = card;
                  
                  _card.ComboBonusApplied += OnComboBonusApplied;
                  _signalBus.Subscribe<CardLockChangeProcessedSignal>(OnCardViewLockChangeProcessed);
            }

            private void OnDestroy()
            {
                  _card.ComboBonusApplied -= OnComboBonusApplied;
                  _signalBus.Unsubscribe<CardLockChangeProcessedSignal>(OnCardViewLockChangeProcessed);
            }

            public void PlaceAtHand(int handIndex)
            {
                  _handIndex = handIndex;
                  _isAtHand = true;
            }

            public void OnPointerClick(PointerEventData eventData)
            {
                  if (_isAtHand)
                  {
                        _signalBus.Fire(new CardLockChangeRequestedSignal(_handIndex));   
                  }
            }

            private void OnCardViewLockChangeProcessed(CardLockChangeProcessedSignal signal)
            {
                  if (!_isAtHand || signal.HandIndex != _handIndex)
                  {
                        return;
                  }
                  
                  if (signal.IsLockChangeAllowed)
                  {
                        _frame.gameObject.SetActive(signal.IsLocked);
                  }
            }

            private void OnComboBonusApplied()
            {
                  if (_valueText.text == _card.Value.ToString())
                  {
                        return;
                  }
                  
                  _valueText.text = _card.Value.ToString();
                  _valueText.transform.DOKill();
                  _valueText.transform.DOScale(_comboHighlightScale, _scaleTime).
                        OnComplete(()=>_valueText.transform.DOScale(1f, _scaleTime));
            }
      }
}
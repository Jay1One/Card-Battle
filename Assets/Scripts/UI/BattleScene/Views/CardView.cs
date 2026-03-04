using Core.Card_Mechanics;
using DG.Tweening;
using Gameplay.Systems;
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
            
            private Card _card;
            private RedrawSystem _redrawSystem;
            private int _handIndex;
            private bool _isAtHand;
            private float _scaleTime = 0.2f;
            private float _comboHighlightScale= 1.5f;
            

            [Inject]
            private void Construct(RedrawSystem redrawSystem)
            {
                  _redrawSystem = redrawSystem;
            }
            
            public void Initialize(Card card, Sprite suitSprite, Sprite elementSprite, Sprite actionTypeSprite)
            {
                  _suitSprite.sprite= suitSprite;
                  _elementSprite.sprite = elementSprite;
                  _actionTypeSprite.sprite = actionTypeSprite;
                  _valueText.text = card.Value.ToString();
                  _card = card;
                  
                  _card.ComboBonusApplied += OnComboBonusApplied;
            }

            private void OnDestroy()
            {
                  transform.DOKill();
                  _card.ComboBonusApplied -= OnComboBonusApplied;
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
                        _redrawSystem.CardLockRequestProcessed += OnCardViewLockChangeProcessed;
                        _redrawSystem.TryChangeCardLockState(_handIndex);  
                  }
            }

            private void OnCardViewLockChangeProcessed(bool isAllowed)
            {
                  _redrawSystem.CardLockRequestProcessed -= OnCardViewLockChangeProcessed;
                  if (!_isAtHand)
                  {
                        return;
                  }
                  
                  if (isAllowed)
                  {
                        _frame.gameObject.SetActive(!_frame.gameObject.activeSelf);
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
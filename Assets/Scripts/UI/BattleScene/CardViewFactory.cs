using Core.Card_Mechanics;
using UI.BattleScene.Views;
using UnityEngine;
using Zenject;

namespace UI.BattleScene
{
    public class CardViewFactory
    {
        private readonly CardViewSettings _cardViewSettings;
        private readonly CardView _cardViewTemplate;
        private readonly DiContainer _container;

        [Inject]
        public CardViewFactory(CardViewSettings cardViewSettings, CardView cardViewTemplate, DiContainer container)
        {
            _cardViewSettings = cardViewSettings;
            _cardViewTemplate = cardViewTemplate; 
            _container = container;
        }

        public CardView CreateCardView(Card card)
        {
            CardView cardView = _container.InstantiatePrefab(_cardViewTemplate).GetComponent<CardView>();
          
            Sprite elementSprite = _cardViewSettings.ElementSprites[(int)card.Element];
            Sprite suitSprite = _cardViewSettings.SuitSprites[(int)card.Suit];
            Sprite actionSprite = _cardViewSettings.ActionSprites[(int)card.ActionType];
          
            cardView.Initialize(card, suitSprite, elementSprite, actionSprite);
          
            return cardView;
        }
    }
}
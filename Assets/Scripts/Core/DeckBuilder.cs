using System;
using System.Collections.Generic;
using Core.Card_Mechanics;

namespace Core
{
   public class DeckBuilder
   {
      private const int BaseActionValue = 1;
      
      public List<CardTemplate> CreateStandardCardTemplates()
      {
         int elementsCount = Enum.GetValues(typeof(Element)).Length;
         int suitsCount = Enum.GetValues(typeof(Suit)).Length;
         int cardActionsCount = Enum.GetValues(typeof(CardActionType)).Length;
         List<CardTemplate> cards = new List<CardTemplate>();

         for (int i = 0; i < suitsCount; i++)
         {
            for (int j = 0; j < elementsCount; j++)
            {
               for (int k = 0; k < cardActionsCount; k++)
               {
                  CardTemplate cardTemplate = new CardTemplate((Suit)i, (Element)j, (CardActionType) k, BaseActionValue);
                  cards.Add(cardTemplate);
               }
            }
         }
         
         return cards;
      }
   }
}
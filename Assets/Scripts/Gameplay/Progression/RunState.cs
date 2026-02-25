using System.Collections.Generic;
using Core;
using Core.Card_Mechanics;
using Gameplay.Units;
using Zenject;

namespace Gameplay.Progression
{
    public class RunState
    {
        public int PlayerMaxHp { get; private set; }
        public int PlayerCurrentHp { get; private set; }

        public List<CardTemplate> Deck { get; private set; } = new();
        private readonly DeckBuilder _deckBuilder;
        

        
        [Inject]
        public RunState(DeckBuilder deckBuilder, Health playerHealth)
        {
            _deckBuilder = deckBuilder;
            PlayerCurrentHp = playerHealth.CurrentHealth;
            PlayerMaxHp = playerHealth.MaxHealth;
        }

        public void StartNewRun()
        {
            Deck = _deckBuilder.CreateStandardCardTemplates();
            PlayerCurrentHp = PlayerMaxHp;
        }
        public void ApplyBattleResult(int hpAfterBattle)
        {
            PlayerCurrentHp = hpAfterBattle;
        }

        public void AddCard(CardTemplate template)
        {
            Deck.Add(template);
        }

        public void RemoveCard(int index)
        {
            Deck.RemoveAt(index);
        }
    }
}
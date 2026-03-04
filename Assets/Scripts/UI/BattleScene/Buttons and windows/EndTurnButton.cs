using Gameplay.Systems;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.BattleScene.Buttons_and_windows
{
     public class EndTurnButton : MonoBehaviour
     {
          [SerializeField] private Button _button;
          private DeckController _deckController;

          [Inject]
          public void Construct(SignalBus signalBus, DeckController deckController)
          {
               _deckController = deckController;
          }
          
          private void Awake()
          {
               _button.interactable = false;
               _button.onClick.AddListener(OnButtonClicked);
               _deckController.StartingHandDealt += OnStartingHandDealt;
          }

          private void OnButtonClicked()
          {
               _button.interactable = false;
               _deckController.RequestTurnEnd();
          }

          private void OnStartingHandDealt()
          {
               _button.interactable = true;
          }

          private void OnDestroy()
          {
               _deckController.StartingHandDealt -= OnStartingHandDealt;
          }
     }
}
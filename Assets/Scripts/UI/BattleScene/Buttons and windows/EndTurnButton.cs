using Core.Signals;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.BattleScene.Buttons_and_windows
{
     public class EndTurnButton : MonoBehaviour
     {
          [SerializeField] private Button _button;
          private SignalBus _signalBus;

          [Inject]
          public void Construct(SignalBus signalBus)
          {
               _signalBus = signalBus;
          }
          
          private void Awake()
          {
               _button.interactable = false;
               _button.onClick.AddListener(OnButtonClicked);
               _signalBus.Subscribe<StartingHandDealtSignal>(OnStartingHandDealt);
          }

          private void OnButtonClicked()
          {
               _button.interactable = false;
               _signalBus.Fire(new EndTurnRequestedSignal());
          }

          private void OnStartingHandDealt()
          {
               _button.interactable = true;
          }

          private void OnDestroy()
          {
               _signalBus.Unsubscribe<StartingHandDealtSignal>(OnStartingHandDealt);
          }
     }
}
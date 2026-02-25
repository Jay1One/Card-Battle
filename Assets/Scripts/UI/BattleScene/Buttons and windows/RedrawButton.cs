using Core.Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.BattleScene.Buttons
{
    public class RedrawButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _rerollText;
        private SignalBus _signalBus;
        
        [Inject]
        private void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        private void Awake()
        {
            _button.interactable = false;
            _button.onClick.AddListener(OnButtonClicked);
            
            _signalBus.Subscribe<CardLockChangeProcessedSignal>(OnCardLockChangeProcessed);
            _signalBus.Subscribe<StartingHandDealtSignal>(OnStartingHandDealt);
            _signalBus.Subscribe<RedrawsChangedSignal>(OnReDrawFinished);
            _signalBus.Subscribe<EndTurnRequestedSignal>(OnEndTurnRequested);
        }

        private void OnStartingHandDealt()
        {
            _button.interactable = true;
        }

        private void OnCardLockChangeProcessed(CardLockChangeProcessedSignal signal)
        {
            _button.interactable = signal.IsRerollPossible;
        }

        private void OnButtonClicked()
        {
            _button.interactable = false;
            _signalBus.Fire(new RedrawStartedSignal());
        }

        private void OnEndTurnRequested()
        {
            _button.interactable = false;
        }

        private void OnReDrawFinished(RedrawsChangedSignal signal)
        {
            _rerollText.text = signal.RerollsLeft.ToString();
            _button.interactable = !signal.RerollsFinished;
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<CardLockChangeProcessedSignal>(OnCardLockChangeProcessed);
            _signalBus.Unsubscribe<StartingHandDealtSignal>(OnStartingHandDealt);
            _signalBus.Unsubscribe<RedrawsChangedSignal>(OnReDrawFinished);
            _signalBus.Unsubscribe<EndTurnRequestedSignal>(OnEndTurnRequested);
        }
    }
}
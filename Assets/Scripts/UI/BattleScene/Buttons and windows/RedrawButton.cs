using Gameplay.Systems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.BattleScene.Buttons_and_windows
{
    public class RedrawButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _rerollText;
        private DeckController _deckController;
        private RedrawSystem _redrawSystem;
        
        [Inject]
        private void Construct(DeckController deckController, RedrawSystem redrawSystem)
        {
            _deckController = deckController;
            _redrawSystem = redrawSystem;
        }

        private void Awake()
        {
            _button.interactable = false;
            _button.onClick.AddListener(OnButtonClicked);
            
            _deckController.StartingHandDealt+= OnStartingHandDealt;
            _redrawSystem.RedrawsChanged += OnReDrawsChanged;

            _redrawSystem.RerollPossibilityChanged += OnRerollPossibilityChanged;
            _deckController.EndTurnRequested -= OnEndTurnRequested;
        }

        private void OnStartingHandDealt()
        {
            _button.interactable = true;
        }

        private void OnRerollPossibilityChanged(bool isPossible)
        {
            _button.interactable = isPossible;
        }

        private void OnButtonClicked()
        {
            _button.interactable = false;
            _redrawSystem.StartRedraw();
        }

        private void OnEndTurnRequested()
        {
            _button.interactable = false;
        }

        private void OnReDrawsChanged(int redrawsLeft)
        {
            _rerollText.text = redrawsLeft.ToString();
            _button.interactable = redrawsLeft > 0;
        }

        private void OnDestroy()
        {
            _deckController.StartingHandDealt-= OnStartingHandDealt;
            _redrawSystem.RerollPossibilityChanged += OnRerollPossibilityChanged;
            _redrawSystem.RedrawsChanged -= OnReDrawsChanged;
            _deckController.EndTurnRequested -= OnEndTurnRequested;
        }
    }
}
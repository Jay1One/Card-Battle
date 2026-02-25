using Core;
using Core.Signals;
using Gameplay.Progression;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.BattleScene.Buttons
{
    public class BattleEndWindow : MonoBehaviour
    { 
        [SerializeField] private GameObject _window; 
        [SerializeField] private TMP_Text _winText; 
        [SerializeField] private TMP_Text _loseText;
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private Button _menuButton;

        private SignalBus _signalBus;
        private SceneLoader _sceneLoader;
        private LevelProgression _levelProgression;
        private LevelsCatalogSo _levelsCatalogSo;

        [Inject]
        private void Construct(SignalBus bus, SceneLoader sceneLoader, LevelProgression progress, LevelsCatalogSo catalog)
        {
            _signalBus = bus;
            _sceneLoader = sceneLoader;
            _levelProgression = progress;
            _levelsCatalogSo = catalog;
        }

        private void Awake()
        {
            _window.SetActive(false);

            _signalBus.Subscribe<BattleEndedSignal>(OnBattleEnded);

            _nextLevelButton.onClick.AddListener(OnNextLevelButtonClicked);
            _menuButton.onClick.AddListener(OnMenuButtonClicked);
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<BattleEndedSignal>(OnBattleEnded);
        }

        private void OnBattleEnded(BattleEndedSignal signal)
        {
            _window.SetActive(true);

            bool isWin = signal.IsWin;

            _winText.gameObject.SetActive(isWin);
            _loseText.gameObject.SetActive(!isWin);

            if (isWin && _levelProgression.HasNext(_levelsCatalogSo.Levels))
            {
                _nextLevelButton.gameObject.SetActive(true);
                _menuButton.gameObject.SetActive(false);
            }
            
            else
            {
                _nextLevelButton.gameObject.SetActive(false);
                _menuButton.gameObject.SetActive(true);
            }
        }

        private void OnNextLevelButtonClicked()
        {
            _levelProgression.Advance();
            _sceneLoader.LoadBattleScene();
        }

        private void OnMenuButtonClicked()
        {
            _sceneLoader.LoadMainMenu();
        }
    }
}
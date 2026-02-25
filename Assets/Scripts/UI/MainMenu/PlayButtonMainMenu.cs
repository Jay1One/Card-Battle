using Gameplay.Progression;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.MainMenu
{
    public class PlayButtonMainMenu : MonoBehaviour
    {
        [SerializeField] private Button _playButton;
        
        private SceneLoader _sceneLoader;
        private LevelProgression _levelProgression;
        private RunState _runState;

        [Inject]
        private void Construct(SceneLoader sceneLoader, LevelProgression levelProgression, RunState runState)
        {
            _sceneLoader = sceneLoader;
            _levelProgression = levelProgression;
            _runState = runState;
        }

        private void Awake()
        {
            _playButton.onClick.AddListener(OnPlayButtonClicked);
        }

        private void OnPlayButtonClicked()
        {
            _levelProgression.StartNewRun();
            _runState.StartNewRun();
            _sceneLoader.LoadBattleScene();
        }
    }
}
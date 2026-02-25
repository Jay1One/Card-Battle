using Core.Signals;
using UnityEngine.SceneManagement;
using Zenject;

namespace Gameplay.Progression
{
      public class SceneLoader
      {
            private  const string MainMenu = "Main Menu";
            private const string BattleScene = "Battle Scene";
            
            private readonly SignalBus _signalBus;

            [Inject]
            public SceneLoader(SignalBus signalBus)
            {
                  _signalBus = signalBus;
            }
            
            public void LoadBattleScene()
            {
                  SceneManager.LoadSceneAsync(BattleScene, LoadSceneMode.Single);
                  _signalBus.Fire(new SceneChangedSignal());
            }

            public void LoadMainMenu()
            {
                  SceneManager.LoadSceneAsync(MainMenu, LoadSceneMode.Single);
                  _signalBus.Fire(new SceneChangedSignal());
            }
      }
}
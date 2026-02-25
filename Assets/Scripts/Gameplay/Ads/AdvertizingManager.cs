using System.Collections;
using Core.Signals;
using UnityEngine;
using Zenject;

namespace Gameplay.Ads
{
    
    public class AdvertizingManager : MonoBehaviour
    {
        private SignalBus _signalBus;
        private IAdvertizingProvider _advertizingProvider;
        private readonly float _interstitialCooldown = 180f;
        private bool _isInterstitialOnCooldown = false;
        private float _lastInterstitialTime;

        [Inject]
        public void Construct(SignalBus signalBus, IAdvertizingProvider advertizingProvider)
        {
            _signalBus = signalBus;
            _advertizingProvider = advertizingProvider;
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            _signalBus.Subscribe<SceneChangedSignal>(OnSceneChanged);

            StartCoroutine(TrackCooldownsCoroutine());
        }

        private void OnDestroy()
        {
            _signalBus.Unsubscribe<SceneChangedSignal>(OnSceneChanged);
        }

        private void OnSceneChanged()
        {
            if (!_isInterstitialOnCooldown && _advertizingProvider.IsInterstitialReady())
            {
                _advertizingProvider.ShowInterstitial();
                _lastInterstitialTime = Time.realtimeSinceStartup;
                _isInterstitialOnCooldown = true;
            }
        }

        private IEnumerator TrackCooldownsCoroutine()
        {
            while (true)
            {
                _isInterstitialOnCooldown= Time.realtimeSinceStartup -_lastInterstitialTime < _interstitialCooldown;
                yield return null;
            }
        }
    }
}

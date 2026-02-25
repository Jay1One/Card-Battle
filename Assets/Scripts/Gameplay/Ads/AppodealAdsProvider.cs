using AppodealStack.Monetization.Api;
using AppodealStack.Monetization.Common;
using Zenject;

namespace Gameplay.Ads
{
    public class AppodealAdsProvider : IAdvertizingProvider, IInitializable
    {
        
        public void ShowInterstitial()
        {
            Appodeal.Show(AppodealShowStyle.Interstitial);
        }

        public bool IsInterstitialReady()
        {
            return Appodeal.IsLoaded(AppodealAdType.Interstitial);
        }

        public void Initialize()
        {
            int adTypes = AppodealAdType.Interstitial;
            string appKey = "YOUR_APPODEAL_APP_KEY";
            AppodealCallbacks.Sdk.OnInitialized += OnInitializationFinished;
            Appodeal.Initialize(appKey, adTypes);
        }

        private void OnInitializationFinished(object sender, SdkInitializedEventArgs e)
        {
            
        }
    }
}
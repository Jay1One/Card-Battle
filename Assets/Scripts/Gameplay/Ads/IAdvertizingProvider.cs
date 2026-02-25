namespace Gameplay.Ads
{
    public interface IAdvertizingProvider
    {
        public void ShowInterstitial();
        public bool IsInterstitialReady();
    }
}
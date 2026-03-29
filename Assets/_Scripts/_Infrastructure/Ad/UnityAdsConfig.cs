using _Scripts._Infrastructure.Ad;
using UnityEngine;

[CreateAssetMenu(fileName = "UnityAdsConfig", menuName = "Configs/Unity Ads Config")]
public class UnityAdsConfig : ScriptableObject, IAdConfig
{
    public string AndroidGameId;
    public string IOSGameId;
    public string RewardedAndroidId;
    public string RewardedIOSId;
    public string InterstitialAndroidId;
    public string InterstitialIOSId;
    public bool TestMode;

    public AdProviderType ProviderType => AdProviderType.Unity;
}
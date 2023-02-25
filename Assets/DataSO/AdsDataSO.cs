using UnityEngine;

[CreateAssetMenu(menuName = "Data SO/Admob Data", fileName = "Admob Data")]
public class AdsDataSO : ScriptableObject
{
    [SerializeField] int _interstitialAdInterval;
    [Tooltip("the minimum delay between interstitial ad in seconds.")]
    [SerializeField] float _minDelayBetweenInterstitial = 30;

    [Space]
    [SerializeField] [Range(0f, 1f)] float _rewardedAdFrequency = .5f;

    // Test App Id = "ca-app-pub-3940256099942544~3347511713";
    string idBanner_DEFAULT = "ca-app-pub-3940256099942544/6300978111";
    string idInterstitial_DEFAULT = "ca-app-pub-3940256099942544/1033173712";
    string idReward_DEFAULT = "ca-app-pub-3940256099942544/5224354917";

    [Header("Admob Ad Units IOS:")]
    [SerializeField] [TextArea(1, 2)] string idBanner_IOS = "ca-app-pub-3940256099942544/6300978111";
    [SerializeField] [TextArea(1, 2)] string idInterstitial_IOS = "ca-app-pub-3940256099942544/1033173712";
    [SerializeField] [TextArea(1, 2)] string idReward_IOS = "ca-app-pub-3940256099942544/5224354917";

    [Header("Admob Ad Units ANDROID:")]
    [SerializeField] [TextArea(1, 2)] string idBanner_ANDROID = "ca-app-pub-3940256099942544/6300978111";
    [SerializeField] [TextArea(1, 2)] string idInterstitial_ANDROID = "ca-app-pub-3940256099942544/1033173712";
    [SerializeField] [TextArea(1, 2)] string idReward_ANDROID = "ca-app-pub-3940256099942544/5224354917";

    [Header("Enable Ads :")]
    [SerializeField] bool _enableBanner = true;
    [SerializeField] bool _enableInterstitial = true;
    [SerializeField] bool _enableRewarded = true;

    public int InterstitialAdInterval => _interstitialAdInterval;
    public float RewardedAdFrequency => _rewardedAdFrequency;

    public float MinDelayBetweenInterstitial => _minDelayBetweenInterstitial;

#if UNITY_IOS
    public string BannerID => idBanner_IOS;
    public string InterstitialID => idInterstitial_IOS;
    public string RewardedID => idReward_IOS;
#elif UNITY_ANDROID
    public string BannerID => idBanner_ANDROID;
    public string InterstitialID => idInterstitial_ANDROID;
    public string RewardedID => idReward_ANDROID;
#else
    public string BannerID => idBanner_DEFAULT;
    public string InterstitialID => idInterstitial_DEFAULT;
    public string RewardedID => idReward_DEFAULT;
#endif

    public bool BannerEnabled => _enableBanner;
    public bool InterstitialEnabled => _enableInterstitial;
    public bool RewardedEnabled => _enableRewarded;
}

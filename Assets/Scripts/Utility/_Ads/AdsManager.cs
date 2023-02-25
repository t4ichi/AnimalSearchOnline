using System;
using UnityEngine;
public class AdsManager : Singleton<AdsManager>
{
    [Header("Data")]
    [SerializeField] AdsDataSO _data;
    [SerializeField] AdmobAds _ads;

    [Header("Settings")]
    [SerializeField] bool _isInterstitialTimerPassed = false;
    [SerializeField] float _interstitialTimer;
    [SerializeField] int _interstitialCounter;



    public static event Action OnRewardedAdClosed;
    public static void HandleRewardAdClosed() => OnRewardedAdClosed?.Invoke();

    public static event Action OnRewardedAdWatchedComplete;
    public static void HandleRewardedAdWatchedComplete() => OnRewardedAdWatchedComplete?.Invoke();

    private void Start()
    {
        _interstitialTimer = _data.MinDelayBetweenInterstitial;
    }

    private void Update()
    {
        if (!_isInterstitialTimerPassed && Time.unscaledTime > _interstitialTimer)
        {
            _isInterstitialTimerPassed = true;
        }
    }

    /// <summary>
    /// バナーを表示
    /// </summary>
    public void ShowBanner()
    {
        _ads.RequestAndShowBanner();
    }

    /// <summary>
    /// バナーを非表示
    /// </summary>
    public void DestroyBanner()
    {
        _ads.DestroyBannerAd();
    }

    /// <summary>
    /// インタースティシャルを表示
    /// </summary>
    /// <param name="action"></param>
    public void ShowInterstitial(Action action = null)
    {
        if (_ads._isShowAd)
        {
            TextUIAnimation.InGameTextAnimation("AD", () =>
             {
                 _ads.ShowInterstitialAd();
                 action?.Invoke();
             });
        }
        else
        {
            action?.Invoke();
        }
    }

    /// <summary>
    /// 一定のタイミングごとにインタースティシャルを表示
    /// </summary>
    public bool CountInterstitial()
    {
        _interstitialCounter++;

        if (_isInterstitialTimerPassed && _interstitialCounter > _data.InterstitialAdInterval)
        {
            _isInterstitialTimerPassed = false;
            _interstitialTimer = Time.unscaledTime + _data.MinDelayBetweenInterstitial;
            _interstitialCounter = 0;
            return true;
        }
        return false;
    }

    /// <summary>
    /// リワード広告を表示
    /// </summary>
    public void ShowRewarded()
    {
        _ads.ShowRewardedAd();
    }
}
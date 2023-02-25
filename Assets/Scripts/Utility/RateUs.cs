using UnityEngine;

#if UNITY_IOS
using UnityEngine.iOS;
#endif

/// <summary>
/// アプリ内で表示するレビューポップアップ
/// </summary>
public class RateUs : Singleton<RateUs>
{
    [SerializeField] int _countToRate = 3;

    int _playCount;

    static bool _rateOff = false;

    public void SetRateOff(bool value)
    {
        _rateOff = value;
    }

    public void ClickPlay()
    {
        _playCount++;
        if (_playCount % _countToRate == 0 && !_rateOff)
        {
            Debug.Log("RateUs");
#if UNITY_IOS
            Device.RequestStoreReview();
#endif
        }
    }

    public void RateNow()
    {
#if UNITY_IOS
        Device.RequestStoreReview();
#else
        Application.OpenURL($"market://details?id={Application.identifier}");
#endif
    }
}

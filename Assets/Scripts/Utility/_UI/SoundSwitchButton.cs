using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// サウンドの設定をするSwitch
/// </summary>
public class SoundSwitchButton : MonoBehaviour
{
    public enum Sound
    {
        SE,BGM
    }

    public Sound sound;

    [SerializeField] Button button;

    [SerializeField] Image _bgImage;
    [SerializeField] RectTransform _handle;

    private const float SWITCH_DURATION = 0.36f;
    private bool _currentState = true;

    private float handlePosX;
    private Sequence sequence;

    private static readonly Color OFF_BG_COLOR = new Color(0.92f, 0.92f, 0.92f);
    private static readonly Color ON_BG_COLOR = new Color(0.05882353f, 0.5333334f, 0.9490196f);

    private void Start()
    {
        handlePosX = Mathf.Abs(_handle.anchoredPosition.x);

        Set();
        button.onClick.AddListener(() => ButtonListener());
    }

    private void Set()
    {
        switch (sound)
        {
            case Sound.SE:
                _currentState = SaveData.GetFxEnable();
                break;
            case Sound.BGM:
                _currentState = SaveData.GetMusicEnable();
                break;
        }
        SwitchSprite(0);
    }

    private void ButtonListener()
    {
        switch (sound)
        {
            case Sound.SE:
                SoundManager.Instance.ToggleSE(ref _currentState);
                break;
            case Sound.BGM:
                SoundManager.Instance.ToggleBGM(ref _currentState);
                break;
        }
        SwitchSprite(SWITCH_DURATION);
    }

    private void SwitchSprite(float duration)
    {
        var bgColor = _currentState ? ON_BG_COLOR : OFF_BG_COLOR;
        var handleDestX = _currentState ? handlePosX : -handlePosX;

        sequence?.Complete();
        sequence = DOTween.Sequence();
        sequence.Append(_bgImage.DOColor(bgColor, duration))
            .Join(_handle.DOAnchorPosX(handleDestX, duration / 2));
    }
}

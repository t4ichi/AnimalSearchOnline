using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using DG.Tweening;

/// <summary>
/// マッチメイキング中のメニュー
/// </summary>
public class MatchMakingMenu : Menu
{
    [Header("GameObject")]
    [SerializeField] GameObject _loadingObj1;
    [SerializeField] GameObject _loadingObj2;

    [Header("Button")]
    [SerializeField] Button _backButton;

    public static bool IsMatchMaking = false;

    private Tween _loadingTween1;
    private Tween _loadingTween2;

    private void Start()
    {
       ButtonExtentions.OnButtonPressed(_backButton, BackButtonListener);
    }

    public override void SetEnable()
    {
        base.SetEnable();
        IsMatchMaking = true;
        _backButton.interactable = true;
        _loadingTween1 = DotweenAnimations.LoadingCircleAnimation(_loadingObj1);
        _loadingTween2 = DotweenAnimations.LoadingCircleAnimation(_loadingObj2);
    }

    public override void SetDisable()
    {
        base.SetDisable();
        IsMatchMaking = false;
        _loadingTween1.Kill();
        _loadingTween2.Kill();
    }

    /// <summary>
    /// 戻るボタンを押したとき
    /// </summary>
    private void BackButtonListener()
    {
        Debug.Log("Back");
        SoundManager.Instance.PlayAudio(AudioType.CLICK);
        _backButton.interactable = false;
        PhotonNetwork.LeaveRoom();

        FadeManager.Instance.FadeWhileAction(() =>
        {
            MenuManager.Instance.SwitchMenu(MenuType.Main);
        });
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using NatSuite.Sharing;

/// <summary>
/// リザルト画面
/// </summary>
public class ResultWindow : Window
{
    [SerializeField] Button _restartButton;
    [SerializeField] Button _homeButton;
    [SerializeField] Button _shareOpenButton;
    [SerializeField] Button _shareCloseButton;
    [SerializeField] Button _shareButton;

    [Space]
    [SerializeField] UIPopup _sharePopup;
    [SerializeField] GameObject _shareButtonsObj;

    [Header("GameText")]
    [SerializeField] TextMeshProUGUI _gameText;
    [SerializeField] GameObject ClearText;
    [SerializeField] GameObject _buttonsObj;

    private string _win = "Win!";
    private string _lose = "Lose...";
    private string _draw = "Draw";

    private string _clear = "Clear!";
    private string _gameover = "GameOver...";

    ShareManager _shareManager;

    private void Start()
    {
        ButtonExtentions.OnButtonPressed(_restartButton, RestartButtonListener);
        ButtonExtentions.OnButtonPressed(_homeButton, HomeButtonListener);
        ButtonExtentions.OnButtonPressed(_shareOpenButton, ShareOpenButtonListener);
        ButtonExtentions.OnButtonPressed(_shareCloseButton, ShareCloseButtonListener);
        ButtonExtentions.OnButtonPressed(_shareButton, ShareButtonListener);

        _shareManager = GetComponent<ShareManager>();
    }

    public override void SetEnable()
    {
        base.SetEnable();

        _restartButton.interactable = false;
        _homeButton.interactable = false;
        _shareButton.interactable = false;

        _shareManager.Capture();

        switch (GameManager.CurrentGameEndState)
        {
            case GameEndState.Win:
                SoundManager.Instance.PlayAudio(AudioType.Clear);
                _gameText.text = _win;
                SaveData.AddWinCount();
                break;
            case GameEndState.Lose:
                SoundManager.Instance.PlayAudio(AudioType.GameOver);
                _gameText.text = _lose;
                SaveData.AddLoseCount();
                break;
            case GameEndState.Draw:
                SoundManager.Instance.PlayAudio(AudioType.Draw);
                _gameText.text = _draw;
                SaveData.AddDrawCount();
                break;
            case GameEndState.Clear:
                SoundManager.Instance.PlayAudio(AudioType.Clear);
                _gameText.text = _clear;
                break;
            case GameEndState.GameOver:
                SoundManager.Instance.PlayAudio(AudioType.GameOver);
                _gameText.text = _gameover;
                break;
        }

        DotweenAnimations.ClearAnimation(ClearText, () =>
        {
            var sq = DOTween.Sequence();
            sq.AppendInterval(1);
            sq.AppendCallback(() =>
            {
                AdSetting();
            });
        });
    }

    public override void SetDisable()
    {
        DotweenAnimations.MoveObject(_buttonsObj, new Vector2(0, -80), 0);
    }

    /// <summary>
    /// 広告を表示するか
    /// </summary>
    private void AdSetting()
    {
        if (AdsManager.Instance.CountInterstitial())
        {
            //広告が表示させるタイミング
            AdsManager.Instance.ShowInterstitial(() =>
            {
                DotweenAnimations.MoveObject(_buttonsObj, new Vector2(0, 80), 0.3f);
                _restartButton.interactable = true;
                _homeButton.interactable = true;
                _shareButton.interactable = true;
            });
        }
        else
        {
            DotweenAnimations.MoveObject(_buttonsObj, new Vector2(0, 80), 0.3f);
            _restartButton.interactable = true;
            _homeButton.interactable = true;
            _shareButton.interactable = true;
        }
    }

    //-------------------- ボタンリスナー ---------------------
    private void RestartButtonListener()
    {
        SoundManager.Instance.PlayAudio(AudioType.CLICK);
        GameManager.Window.CloseWindow("Result");
        switch (GameManager.CurrentGameMode)
        {
            case GameMode.Random:
                MatchMaking.Match.OnJoinRandomRoomButtonClick();
                break;
            case GameMode.Friend:
                MatchMaking.Match.OnJoinPrivateRoomButtonClick();
                break;
            case GameMode.Practice:
                StartCoroutine(GameManager.Game.InitGame(false));
                break;
        }
    }

    private void HomeButtonListener()
    {
        SoundManager.Instance.PlayAudio(AudioType.CLICK);
        FadeManager.Instance.FadeWhileAction(() =>
        {
            GameManager.Window.CloseWindow("Result");
            MenuManager.Instance.SwitchMenu(MenuType.Main);
        });
    }

    private void ShareOpenButtonListener()
    {
        SoundManager.Instance.PlayAudio(AudioType.CLICK);
        DotweenAnimations.MoveObject(_shareButtonsObj, new Vector2(0, 80), 0.3f);
        _shareManager.Load();
        _sharePopup.Open(0.7f,0.8f);
    }

    private void ShareCloseButtonListener()
    {
        SoundManager.Instance.PlayAudio(AudioType.CLICK);
        DotweenAnimations.MoveObject(_shareButtonsObj, new Vector2(0, -80), 0.3f);
        _sharePopup.Close(0.7f);
    }

    private void ShareButtonListener()
    {
        SoundManager.Instance.PlayAudio(AudioType.CLICK);
        Debug.Log("Share");
        var image = _shareManager.CapturedScreenshot;
        var text = "#どうぶつ推理オンライン";
        var payload = new SharePayload();
        payload.AddImage(image);
        payload.AddText(text);
        payload.Commit();
    }
}

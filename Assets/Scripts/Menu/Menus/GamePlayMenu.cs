using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲームプレイ中のメニュー
/// </summary>
public class GamePlayMenu : Menu
{
    [Header("Button")]
    [SerializeField] Button _pauseButton;

    [Header("GameObject")]
    [SerializeField] GameObject _topOnlineMode;
    [SerializeField] GameObject _topPracticeMode;

    [Header("GameUI")]
    [SerializeField] Canvas _top;
    [SerializeField] Canvas _middle;
    [SerializeField] Canvas _bottom;

    private void Start()
    {
        OnButtonPressed(_pauseButton, PauseButtonListener,true);
    }

    public override void SetEnable()
    {
        base.SetEnable();
        GameManager.IsGaming = true;

        _top.enabled = true;
        _middle.enabled = true;
        _bottom.enabled = true;

        if (GameManager.CurrentGameMode != GameMode.Practice)
        {
            _topOnlineMode.SetActive(true);
            _topPracticeMode.SetActive(false);
        }
        else
        {
            _topOnlineMode.SetActive(false);
            _topPracticeMode.SetActive(true);
        }
    }
    public override void SetDisable()
    {
        Debug.Log("GamePlayMenu SetDisable");
        base.SetDisable();
        GameManager.IsGaming = false;
        //GameManager.Game.Exit();

        _top.enabled = false;
        _middle.enabled = false;
        _bottom.enabled = false;
    }

    /// <summary>
    /// Pauseボタンを押したら
    /// </summary>
    private void PauseButtonListener()
    {
        if (GameManager.IsGaming == false) return;
        SoundManager.Instance.PlayAudio(AudioType.CLICK);
        GameManager.Window.OpenWindow("Pause");
    }
}

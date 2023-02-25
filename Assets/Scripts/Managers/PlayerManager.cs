using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum Players
{
    Master,
    Client
}

public class PlayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField] Button _pauseButton;

    [SerializeField] CanvasGroup _playerControllCg;
    [SerializeField] TextMeshProUGUI _controllText;

    [SerializeField] PlayerProfile _playerProfileYou;
    [SerializeField] PlayerProfile _playerProfileOther;

    private CanvasGroup _playerProfileYouImage;
    private CanvasGroup _playerProfileOtherImage;

    public static Players CurrentPlayer { get; set; }
    public static Players MyPlayer { get; set; }

    private int _randomPlayerNum;
    private Tween _playerControllTween;

    ScoreManager _scoreManager;

    //操作中プレイヤーのプロフィールの色
    float _playerActiveColor = 1;
    float _playerDisActiveColor = 0.6f;

    private void Start()
    {
        _scoreManager = GetComponent<ScoreManager>();
        _playerProfileYouImage = _playerProfileYou.gameObject.GetComponent<CanvasGroup>();
        _playerProfileOtherImage = _playerProfileOther.gameObject.GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public IEnumerator Init()
    {
        if (GameManager.CurrentGameMode != GameMode.Practice)
        {
            //プロフィールを更新
            yield return StartCoroutine(InitProfile());
            //先手後手を決める
            yield return StartCoroutine(InitFirstPlayer());
        }
        else
        {
            _pauseButton.interactable = false;
        }
        ActiveControllFilter();
    }

    /// <summary>
    /// ターンを終了
    /// </summary>
    public void TurnFinish()
    {
        if (GameManager.CurrentGameMode != GameMode.Practice)
        {
            SwitchControllText(false);
        }

        ActiveControllFilter(true);
    }

    /// <summary>
    /// 次のターンの準備
    /// </summary>
    public void Next()
    {
        if (GameManager.CurrentGameMode != GameMode.Practice)
        {
            SwitchPlayer();
        }
    }

    /// <summary>
    /// 次のターンを開始
    /// </summary>
    public void StartNext()
    {
        if (GameManager.CurrentGameMode != GameMode.Practice)
        {
            var ismyturn = IsMyTurn(CurrentPlayer);
            _scoreManager.AnswerButtonsInteractable(ismyturn);
            SwitchPlayerProfile(ismyturn);
            SwitchControllFilter(ismyturn);
        }
        else
        {
            _pauseButton.interactable = true;
            DisActiveControllFilter();
        }
    }

    /// <summary>
    /// 次ターンのプレイヤーを表示させるアニメーション
    /// </summary>
    public void NextAnimation(Action action = null)
    {
        var IsMyturn = IsMyTurn(CurrentPlayer);
        Debug.Log("IsMyturn: " + IsMyturn + " CurrentPlayer : " + CurrentPlayer.ToString());

        var text = IsMyturn ? "あなたのばん" : "あいてのばん";

        TextUIAnimation.InGameTextAnimation(text, () =>
         {
             action?.Invoke();
         });
    }

    /// <summary>
    /// タイムオーバー
    /// </summary>
    public void TimerOverAnimation()
    {
        var text = "タイムオーバー";
        SoundManager.Instance.PlayAudio(AudioType.TimeOver);

        TextUIAnimation.InGameTextAnimation(text, () =>
        {
            var winner = FlipPlayer(CurrentPlayer);
            var endstate = JadgeWinner(winner);
            GameManager.Game.EndGame(endstate);
        });
    }

    /// <summary>
    /// 自分のターンを判定
    /// </summary>
    public bool IsMyTurn(Players currentplayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (currentplayer == Players.Master)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (currentplayer == Players.Master)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    /// <summary>
    /// 勝敗を判定
    /// </summary>
    public GameEndState JadgeWinner(Players player)
    {
        if (player == MyPlayer)
        {
            return GameEndState.Win;
        }
        else
        {
            return GameEndState.Lose;
        }
    }

    //------------初期化------------
    /// <summary>
    /// プレイヤーデータを同期
    /// </summary>
    private IEnumerator InitProfile()
    {
        yield return new WaitUntil(() =>
        {
            return PhotonNetwork.LocalPlayer != null;
        });

        UpdatePlayerData(PhotonNetwork.LocalPlayer, _playerProfileYou);

        yield return new WaitUntil(() =>
        {
            return PhotonNetwork.PlayerListOthers[0] != null;
        });

        UpdatePlayerData(PhotonNetwork.PlayerListOthers[0], _playerProfileOther);

        _playerProfileYouImage.alpha = _playerActiveColor;
        _playerProfileOtherImage.alpha = _playerActiveColor;
    }

    /// <summary>
    /// 先手、後手を決める
    /// </summary>
    private IEnumerator InitFirstPlayer()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            MyPlayer = Players.Master;
            //先手プレイヤーを決める
            _randomPlayerNum = UnityEngine.Random.Range(0, 2);
            PhotonNetwork.CurrentRoom.SetFirstPlayer(_randomPlayerNum);
        }
        else
        {
            MyPlayer = Players.Client;
        }

        yield return new WaitUntil(() =>
        {
            return PhotonNetwork.CurrentRoom.GetFirstPlayer() != -1;
        });

        _randomPlayerNum = PhotonNetwork.CurrentRoom.GetFirstPlayer();

        if (_randomPlayerNum == 0)
        {
            CurrentPlayer = Players.Master;
        }
        //参加者が先攻
        else
        {
            CurrentPlayer = Players.Client;
        }
    }
    //------------ゲーム中------------
    /// <summary>
    /// 操作ボタンのフィルターをアクティブにする
    /// </summary>
    private void ActiveControllFilter(bool isAnimation = false)
    {
        if (isAnimation)
        {
            _playerControllCg.gameObject.SetActive(true);
            FadeIn(_playerControllCg);
        }
        else
        {
            _playerControllCg.gameObject.SetActive(true);
            _playerControllCg.alpha = 1;
        }
    }

    /// <summary>
    /// 操作ボタンのフィルターを非アクティブにする
    /// </summary>
    private void DisActiveControllFilter()
    {
        FadeOut(_playerControllCg)
            .OnComplete(() =>
            {
                _playerControllCg.gameObject.SetActive(false);
            });
    }

    /// <summary>
    /// 操作中テキストの状態を切り替える
    /// </summary>
    private void SwitchControllText(bool isActive)
    {
        if (isActive)
        {
            _controllText.gameObject.SetActive(true);
            _playerControllTween = FadeFlashText(_controllText);
        }
        else
        {
            if (_playerControllTween != null)
                _playerControllTween.Kill();
            _controllText.DOFade(0, 0.3f)
                .OnComplete(() => _controllText.gameObject.SetActive(false));
        }
    }

    /// <summary>
    /// 操作プレイヤーを切り替える
    /// </summary>
    private Players SwitchPlayer()
    {
        if (CurrentPlayer == Players.Master)
        {
            CurrentPlayer = Players.Client;
            return Players.Client;
        }
        else
        {
            CurrentPlayer = Players.Master;
            return Players.Master;
        }
    }

    /// <summary>
    /// プレイヤーを反転させる
    /// </summary>
    private Players FlipPlayer(Players player)
    {
        if (player == Players.Master)
        {
            return Players.Client;
        }
        else
        {
            return Players.Master;
        }
    }

    /// <summary>
    /// 次が自分のターンならFilterを削除、そうでないならTextをアニメーション
    /// </summary>
    private void SwitchControllFilter(bool isMyturn)
    {
        if (isMyturn)
        {
            DisActiveControllFilter();
        }
        else
        {
            SwitchControllText(true);
        }
    }

    /// <summary>
    /// プレイヤーデータのUIの色を切り替える
    /// </summary>
    private void SwitchPlayerProfile(bool isInteractable)
    {
        if (isInteractable)
        {
            _playerProfileYouImage.alpha = _playerActiveColor;
            _playerProfileOtherImage.alpha = _playerDisActiveColor;
        }
        else
        {
            _playerProfileYouImage.alpha = _playerDisActiveColor;
            _playerProfileOtherImage.alpha = _playerActiveColor;
        }
    }

    /// <summary>
    /// プレイヤーデータを更新
    /// </summary>
    private void UpdatePlayerData(Player player, PlayerProfile profile = null)
    {
        var name = player.NickName;
        int icon = player.GetIconID();
        int win = player.GetWinCount();
        var color = RankData.GetRank(win);
        profile.SetPlayerProfile(name, icon, win, color);
    }
    //------------アニメーション------------
    private Tween FadeIn(CanvasGroup canvasGroup,float time = 0.3f)
    {
        return canvasGroup.DOFade(1, time);
    }
    private Tween FadeOut(CanvasGroup canvasGroup, float time = 0.3f)
    {
        return canvasGroup.DOFade(0, time);
    }

    private Tween FadeFlashText(TextMeshProUGUI text)
    {
        text.gameObject.SetActive(true);
        text.DOFade(1f, 0);

        var tween = text.DOFade(0.8f, 1)
                        .SetLoops(-1, LoopType.Yoyo);
        return tween;
    }
}

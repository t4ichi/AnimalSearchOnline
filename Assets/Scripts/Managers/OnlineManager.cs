using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using DG.Tweening;

public class OnlineManager : MonoBehaviourPunCallbacks, IPunTurnManagerCallbacks
{
    PunTurnManager _turnManager;
    ScoreManager _scoreManager;

    public static bool IsTurnComplete = false;

    private void Awake()
    {
        _turnManager = GetComponent<PunTurnManager>();
        _scoreManager = GetComponent<ScoreManager>();
        _turnManager.TurnManagerListener = this;
    }

    /// <summary>
    /// 次のターンを開始
    /// </summary>
    public void StartTurn()
    {
        Debug.Log("StartTurn");
        if (PhotonNetwork.IsMasterClient)
        {
            _turnManager.BeginTurn();
        }
    }

    /// <summary>
    /// 新しいターンを開始
    /// </summary>
    /// <param name="turn">現在のターン</param>
    public void OnTurnBegins(int turn)
    {
        IsTurnComplete = false;

        if (turn == 1)
        {
            Debug.Log("OnTurnBegin " + turn + "Init");
            StartCoroutine(StartGame());
        }
        else
        {
            Debug.Log("OnTurnBegin " + turn + "Next");
            StartCoroutine(NextGame());
        }
    }

    /// <summary>
    /// プレイヤーのアクションを同期する
    /// </summary>
    public void OnPlayerMove(Player photonPlayer, int turn, object move)
    {
        Debug.Log("OnPlayerMove: " + photonPlayer + " turn: " + turn + " action: " + move);
        
        if((int)move >= 0)
        {
            _scoreManager.AnimalButton((int)move);
        }
        else
        {
            _scoreManager.DeleteButton();
        }
    }

    /// <summary>
    /// 現在のターンを終了
    /// </summary>
    public void OnTurnCompleted(int turn)
    {
        IsTurnComplete = true;
        Debug.Log("OnTurnCompleted: " + turn + "IsTurnCompleted :" + IsTurnComplete);
        _scoreManager.OkButton();
    }

    /// <summary>
    /// プレイヤーのアクションを終了
    /// </summary>
    public void OnPlayerFinished(Player photonPlayer)
    {
        Debug.Log("OnPlayerFinished");
        _turnManager.AddFinishPlayer(photonPlayer);
    }

    /// <summary>
    /// ゲームを終了
    /// </summary>
    /// <param name="state">ゲームの勝敗</param>
    public void GameFinish(GameEndState state)
    {
        Debug.Log("GameFinish:" + state);
        StartCoroutine(EndGame(state));
    }


    /// <summary>
    /// タイムオーバー
    /// </summary>
    public void OnTurnTimeEnds(int turn)
    {
        Debug.Log("OnTurnTimeEnds: " + turn + "IsTurnCompleted :" + IsTurnComplete);

        var sq = DOTween.Sequence();
        sq.AppendInterval(1);

        sq.AppendCallback(() =>
        {
            if (IsTurnComplete) return;
            GameManager.Game.TimeOver();
        });
    }


    #region private function
    /// <summary>
    /// ゲームをスタート
    /// </summary>
    private IEnumerator StartGame()
    {
        yield return StartCoroutine(GameManager.Game.InitGame());

        _turnManager.SetFinishedTurn();

        yield return new WaitUntil(() =>
        {
            return _turnManager.IsCompletedByAll;
        });

        FadeManager.Instance.FadeOut(() =>
        {
            GameManager.Game.StartGame();
        }, 0.24f);
    }

    /// <summary>
    /// 次のターンをスタート
    /// </summary>
    private IEnumerator NextGame()
    {
        _turnManager.SetFinishedTurn();
        yield return new WaitUntil(() =>
        {
            return _turnManager.IsCompletedByAll;
        });
        GameManager.Game.NextTurn();
    }

    /// <summary>
    /// ゲームを終了
    /// </summary>
    private IEnumerator EndGame(GameEndState state)
    {
        _turnManager.SetFinishedTurn();
        yield return new WaitUntil(() =>
        {
            return _turnManager.IsCompletedByAll;
        });
        GameManager.Game.EndGame(state);
    }
    #endregion
}

using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public enum GameEndState
{
    Clear,
    GameOver,
    Win,
    Lose,
    Draw
}

public enum GameMode
{
    Random,
    Friend,
    Practice,
}

public class GameManager : MonoBehaviour
{
    private static WindowManager _windowManager;
    public static WindowManager Window => _windowManager;

    private static GameManager _gameManager;
    public static GameManager Game => _gameManager;

    private static ScoreManager _scoreManager;
    public static ScoreManager Score => _scoreManager;

    public static GameEndState CurrentGameEndState { get; set; }
    public static GameMode CurrentGameMode { get; set; }
    public static bool IsGaming = false;
    
    PlayerManager _playerManager;
    TimeManager _timeManager;

    private void Awake()
    {
        _windowManager = GetComponent<WindowManager>();
        _gameManager = GetComponent<GameManager>();

        _scoreManager = GetComponent<ScoreManager>();
        _playerManager = GetComponent<PlayerManager>();
        _timeManager = GetComponent<TimeManager>();
    }

    private void OnEnable()
    {
        ScoreManager.OnGameNextTurn += NextTurn;
        ScoreManager.OnGameEnd += EndGame;
    }

    private void OnDisable()
    {
        ScoreManager.OnGameNextTurn -= NextTurn;
        ScoreManager.OnGameEnd -= EndGame;
    }

    /// <summary>
    /// ゲームの初期化
    /// </summary>
    /// <param name="isOnline">true = オンラインプレイ,false = オフラインプレイ</param>
    public IEnumerator InitGame(bool isOnline = true)
    {
        Debug.Log("InitGame");
        IsGaming = true;

        FadeManager.Instance.FadeIn(() =>
        {
            MenuManager.Instance.SwitchMenu(MenuType.GamePlay);
        }, 0.24f, true);

        yield return StartCoroutine(_scoreManager.Init());
        yield return StartCoroutine(_playerManager.Init());
        yield return StartCoroutine(_timeManager.Init());

        if (isOnline) yield break;

        //オフラインプレイならこのままスタート
        yield return new WaitForSeconds(0.5f);

        FadeManager.Instance.FadeOut(() =>
        {
            StartGame();
        });
    }

    /// <summary>
    /// ゲームを開始
    /// </summary>
    public void StartGame()
    {
        Debug.Log("StartGame");
        _scoreManager.StartAnimation(() =>
        {
            _timeManager.StartNext();
            _scoreManager.StartNext();
            _playerManager.StartNext();
        });
    }

    /// <summary>
    /// ターンを終了
    /// </summary>
    public void TurnFinish()
    {
        Debug.Log("TurnFinish");

        _timeManager.TurnFinish();
        _playerManager.TurnFinish();
        _scoreManager.TurnFinish();
        _scoreManager.Calculate();
    }
    /// <summary>
    /// 次のターンを開始
    /// </summary>
    public void NextTurn()
    {
        Debug.Log("NextTurn");
        _scoreManager.Next();
        _playerManager.Next();
        _timeManager.Next();

        if(CurrentGameMode != GameMode.Practice)
        {
            _playerManager.NextAnimation(() =>
            {
                Debug.Log("StartNext");
                _timeManager.StartNext();
                _scoreManager.StartNext();
                _playerManager.StartNext();
            });
        }
        else
        {
            _timeManager.StartNext();
            _scoreManager.StartNext();
            _playerManager.StartNext();
        }
    }

    /// <summary>
    /// タイムオーバー
    /// </summary>
    public void TimeOver()
    {
        Debug.Log("TimeOver");
        _scoreManager.TurnFinish();
        _timeManager.TurnFinish();
        _playerManager.TurnFinish();

        _playerManager.TimerOverAnimation();
    }

    /// <summary>
    /// ゲームを終了しリザルト画面を表示
    /// </summary>
    public void EndGame(GameEndState state)
    {
        Debug.Log("EndGame");
        CurrentGameEndState = state;
        Exit();

        if (CurrentGameMode != GameMode.Practice)
        {
            Debug.Log("LeaveRoom");
            PhotonNetwork.LeaveRoom();
        }

        _scoreManager.ShowAnswer(() =>
        {
            _windowManager.OpenWindow("Result");
        });
    }

    /// <summary>
    /// ゲームを終了
    /// </summary>
    public void Exit()
    {
        //DO
        Debug.Log("Exit");
        _scoreManager.TurnFinish();
        _timeManager.TurnFinish();
        _playerManager.TurnFinish();
        IsGaming = false;
    }

}

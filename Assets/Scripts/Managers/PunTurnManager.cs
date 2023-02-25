using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class PunTurnManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    Player sender;

    public int Turn
    {
        get { return PhotonNetwork.CurrentRoom.GetTurn(); }
        private set
        {
            PhotonNetwork.CurrentRoom.SetTurn(value);
        }
    }

    /// <summary>
    /// ターンの継続時間を秒単位で指定
    /// </summary>
    public float TurnDuration = 60f;

    /// <summary>
    /// 現在のターンの経過時間を秒で取得
    /// </summary>
    public float ElapsedTimeInTurn
    {
        get { return ((float)(PhotonNetwork.ServerTimestamp - PhotonNetwork.CurrentRoom.GetTurnStart())) / 1000.0f; }
    }

    /// <summary>
    /// 現在のターンの残り秒数
    /// </summary>
    public float RemainingSecondsInTurn
    {
        get { return Mathf.Max(0f, this.TurnDuration - this.ElapsedTimeInTurn); }
    }

    /// <summary>
    /// ターンが全員完了したかどうかを取得
    /// </summary>
    public bool IsCompletedByAll
    {
        get { return PhotonNetwork.CurrentRoom != null && Turn > -1 && this.finishedPlayers.Count == 2; }
    }

    /// <summary>
    /// 現在のターンが終了したかどうかを取得
    /// </summary>
    public bool IsOver
    {
        get { return this.RemainingSecondsInTurn <= 0f; }
    }

    /// <summary>
    /// コールバックをキャッチするためのスクリプトインスタンス
    /// </summary>
    public IPunTurnManagerCallbacks TurnManagerListener;


    /// <summary>
    /// 動作を終了したPlayerデータ
    /// </summary>
    public readonly HashSet<Player> finishedPlayers = new HashSet<Player>();

    /// <summary>
    /// ルームカスタムプロパティでデータを定義するために内部で使用
    /// </summary>
    public const byte TurnManagerEventOffset = 0;

    public const byte EvMove = 1 + TurnManagerEventOffset;
    public const byte EvFinalMove = 2 + TurnManagerEventOffset;
    public const byte EvFinish = 3 + TurnManagerEventOffset;


    public void InitTurn()
    {
        Turn = 0;
    }

    /// <summary>
    /// 新しいターンを開始
    /// </summary>
    public void BeginTurn()
    {
        Turn = this.Turn + 1;
    }

    public void OnTurnTimeEnd()
    {
        this.TurnManagerListener.OnTurnTimeEnds(this.Turn);
    }

    /// <summary>
    /// アクションを送信する
    /// </summary>
    public void SendMove(object move, bool finished)
    {
        Hashtable moveHt = new Hashtable();
        moveHt.Add("turn", Turn);
        moveHt.Add("move", move);

        byte evCode = (finished) ? EvFinalMove : EvMove;
        PhotonNetwork.RaiseEvent(evCode, moveHt, new RaiseEventOptions() { CachingOption = EventCaching.AddToRoomCache }, SendOptions.SendReliable);
        ProcessOnEvent(evCode, moveHt, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    /// <summary>
    /// プレイヤーが現在のターンを終了したかどうかを取得
    /// </summary>
    public bool GetPlayerFinishedTurn(Player player)
    {
        if (player != null && this.finishedPlayers != null && this.finishedPlayers.Contains(player))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// ターンを終了
    /// </summary>
    public void SetFinishedTurn()
    {
        PhotonNetwork.RaiseEvent(EvFinish, null, new RaiseEventOptions()
        { CachingOption = EventCaching.AddToRoomCache }, SendOptions.SendReliable);
        ProcessOnEvent(EvFinish, null, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    /// <summary>
    /// アクションが終了したプレイヤーを登録
    /// </summary>
    public void AddFinishPlayer(Player sender)
    {
        finishedPlayers.Add(sender);
    }

    #region Callbacks

    /// <summary>
    /// イベント時の処理
    /// </summary>
   　private void ProcessOnEvent(byte eventCode, object content, int senderId)
    {
        Debug.Log("ProcessOnEvent");
        if (senderId == -1)
        {
            return;
        }

        sender = PhotonNetwork.CurrentRoom.GetPlayer(senderId);

        switch (eventCode)
        {
            case EvMove:
                {
                    Hashtable evTable = content as Hashtable;
                    int turn = (int)evTable["turn"];
                    object move = evTable["move"];
                    this.TurnManagerListener.OnPlayerMove(sender, turn, move);
                    break;
                }
            case EvFinalMove:
                {
                    Hashtable evTable = content as Hashtable;
                    int turn = (int)evTable["turn"];

                    if (turn == this.Turn)
                    {
                        finishedPlayers.Add(sender);
                        TurnManagerListener.OnTurnCompleted(this.Turn);
                    }
                    break;
                }
            case EvFinish:
                {
                    sender = PhotonNetwork.CurrentRoom.GetPlayer(senderId);
                    TurnManagerListener.OnPlayerFinished(sender);
                    break;
                }
        }
    }

    /// <summary>
    /// コールバックの登録で呼び出される
    /// </summary>
    public void OnEvent(EventData photonEvent)
    {
        this.ProcessOnEvent(photonEvent.Code, photonEvent.CustomData, photonEvent.Sender);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("Turn"))
        {
            this.finishedPlayers.Clear();
            this.TurnManagerListener.OnTurnBegins(this.Turn);
        }
    }

    #endregion
}


public interface IPunTurnManagerCallbacks
{
    /// <summary>
    /// 新しいターンを開始
    /// </summary>
    /// <param name="turn">現在のターン</param>
    void OnTurnBegins(int turn);

    /// <summary>
    /// 現在のターンを終了
    /// </summary>
    void OnTurnCompleted(int turn);

    /// <summary>
    /// プレイヤーのアクションを同期する
    /// </summary>
    void OnPlayerMove(Player player, int turn, object move);

    /// <summary>
    /// プレイヤーのアクションを終了
    /// </summary>
    void OnPlayerFinished(Player player);

    /// <summary>
    /// タイムオーバー
    /// </summary>
    void OnTurnTimeEnds(int turn);
}


public static class TurnExtensions
{
    //Key
    public static readonly string TurnPropKey = "Turn";
    public static readonly string TurnStartPropKey = "TStart";
    public static readonly string FinishedTurnPropKey = "FToA";

    /// <summary>
    /// ターンをセット
    /// </summary>
    public static void SetTurn(this Room room, int turn)
    {
        if (room == null || room.CustomProperties == null)
        {
            return;
        }

        Hashtable turnProps = new Hashtable();
        turnProps[TurnPropKey] = turn;

        room.SetCustomProperties(turnProps);
    }

    /// <summary>
    /// タイマーをセット
    /// </summary>
    public static void SetStartTime(this Room room)
    {
        Hashtable turnProps = new Hashtable();
        turnProps[TurnStartPropKey] = PhotonNetwork.ServerTimestamp;
        room.SetCustomProperties(turnProps);
        turnProps.Clear();
    }

    /// <summary>
    /// 現在のターンを取得
    /// </summary>
    public static int GetTurn(this RoomInfo room)
    {
        if (room == null || room.CustomProperties == null || !room.CustomProperties.ContainsKey(TurnPropKey))
        {
            return 0;
        }

        return (int)room.CustomProperties[TurnPropKey];
    }


    /// <summary>
    /// ターンが開始された開始時刻を取得
    /// </summary>
    public static int GetTurnStart(this RoomInfo room)
    {
        if (room == null || room.CustomProperties == null || !room.CustomProperties.ContainsKey(TurnStartPropKey))
        {
            return 0;
        }

        return (int)room.CustomProperties[TurnStartPropKey];
    }
}
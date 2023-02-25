using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

/// <summary>
/// プレイヤーで共有するデータをカスタマイズ
/// </summary>
public static class PlayerPropertiesExtensions
{
    private static readonly Hashtable propsToSet = new Hashtable();

    //通信量削減のため、Keyはなるべく短く
    //PlayerData
    private const string IconIDKey = "i";
    private const string WinCountKey = "w";
    private const string LoseCountKey = "l";
    private const string DrawCountKey = "d";

    //InGame
    private const string FirstPlayerKey = "f";
    private const string AnswerListKey = "a";
    private const string AnswerButtonListKey = "b";

    //---------------Icon---------------
    public static int GetIconID(this Player player)
    {
        if (player.CustomProperties[IconIDKey] is int id)
        {
            return id;
        }
        else
        {
            return -1;
        }
    }
    public static void SetIconID(this Player player,int id)
    {
        propsToSet[IconIDKey] = id;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }

    //---------------Win---------------
    public static int GetWinCount(this Player player)
    {
        if (player.CustomProperties[WinCountKey] is int count)
        {
            return count;
        }
        else
        {
            return -1;
        }
    }
    public static void SetWinCount(this Player player, int count)
    {
        propsToSet[WinCountKey] = count;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }

    //---------------Lose---------------
    public static int GetLoseCount(this Player player)
    {
        if (player.CustomProperties[LoseCountKey] is int count)
        {
            return count;
        }
        else
        {
            return 0;
        }
    }
    public static void SetLoseCount(this Player player, int count)
    {
        propsToSet[LoseCountKey] = count;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }

    //---------------Draw---------------
    public static int GetDrawCount(this Player player)
    {
        if (player.CustomProperties[DrawCountKey] is int count)
        {
            return count;
        }
        else
        {
            return 0;
        }
    }
    public static void SetDrawCount(this Player player, int count)
    {
        propsToSet[DrawCountKey] = count;
        player.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }

    //---------------先攻後攻---------------
    public static int GetFirstPlayer(this Room room)
    {
        if (room.CustomProperties[FirstPlayerKey] is int i)
        {
            return i;
        }
        else
        {
            Debug.Log("先攻プレイヤーの取得に失敗");
            return -1;
        }
    }

    public static void SetFirstPlayer(this Room room, int i)
    {
        propsToSet[FirstPlayerKey] = i;
        room.SetCustomProperties(propsToSet);
        propsToSet.Clear();
    }

    //---------------Answer---------------
    public static int GetAnswer(this Room room, int index)
    {
        if (room.CustomProperties[AnswerListKey + index] is int value)
        {
            return value;
        }
        return -1;
    }

    public static void SetAnswer(this Room room, int[] answerArray)
    {
        for (int i = 0; i < answerArray.Length; i++)
        {
            propsToSet[AnswerListKey + i] = answerArray[i];
            room.SetCustomProperties(propsToSet);
            propsToSet.Clear();
        }
    }


    //---------------AnswerButton---------------
    public static int GetAnswerButton(this Room room, int index)
    {
        if (room.CustomProperties[AnswerButtonListKey + index] is int value)
        {
            return value;
        }
        return -1;
    }

    public static void SetAnswerButton(this Room room, int[] answerbuttonArray)
    {
        for (int i = 0; i < answerbuttonArray.Length; i++)
        {
            propsToSet[AnswerButtonListKey + i] = answerbuttonArray[i];
            room.SetCustomProperties(propsToSet);
            propsToSet.Clear();
        }
    }
}
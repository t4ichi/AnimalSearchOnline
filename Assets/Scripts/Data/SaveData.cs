using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// セーブデータ取得とセットを管理する
/// </summary>
public static class SaveData
{
    //NewGame
    public static bool IsNewGame()
    {
        return !PlayerPrefs.HasKey("IsNewGame");
    }

    public static void SetNewGame()
    {
        PlayerPrefs.SetInt("IsNewGame", 1);
    }

    //Sounds
    public static bool GetFxEnable()
    {
        return PlayerPrefs.GetInt("sfxState", 0) == 0;
    }

    public static void SetFx(bool fxEnable)
    {
        PlayerPrefs.SetInt("sfxState", fxEnable ? 0 : 1);
    }

    public static bool GetMusicEnable()
    {
        return PlayerPrefs.GetInt("musicState", 0) == 0;
    }

    public static void SetMusic(bool fxEnable)
    {
        PlayerPrefs.SetInt("musicState", fxEnable ? 0 : 1);
    }

    public static bool GetIsAdBlock()
    {
        return PlayerPrefs.GetInt("AdBlock", 0) == 1;
    }

    public static void SetAdBlock()
    {
        PlayerPrefs.SetInt("AdBlock", 1);
    }


    //Name
    public static void SetPlayerName(string name)
    {
        PlayerPrefs.SetString("PlayerName", name);
        LeaderBoardManager.SaveAction.Invoke();
    }

    public static string GetPlayerName()
    {
        return PlayerPrefs.GetString("PlayerName","NO NAME");
    }

    //Icon
    public static void SetIconID(int id)
    {
        PlayerPrefs.SetInt("IconID", id);
        LeaderBoardManager.SaveAction.Invoke();
    }

    public static int GetIconID()
    {
        return PlayerPrefs.GetInt("IconID",0);
    }

    public static void SetHaveIcon(int id)
    {
        PlayerPrefs.SetInt("IsHave" + id, 1);
    }

    public static bool IsHaveIcon(int id)
    {
        return PlayerPrefs.GetInt("IsHave" + id) == 1 ? true : false;
    }


    /// <param name="type">0=normal,1=reward</param>
    public static bool IsGetIconToday(int type)
    {
        switch (type)
        {
            case 0:
                return PlayerPrefs.GetInt("GetIconToday" + TodayKey()) == 1 ? true : false;
            case 1:
                return PlayerPrefs.GetInt("RewardAdGetIconToday" + TodayKey()) == 1 ? true : false;
        }
        return true;
    }

    /// <param name="type">0=normal,1=reward</param>
    public static void SetGetIconToday(int type)
    {
        switch (type)
        {
            case 0:
                PlayerPrefs.SetInt("GetIconToday" + TodayKey(), 1);
                break;
            case 1:
                PlayerPrefs.SetInt("RewardAdGetIconToday" + TodayKey(), 1);
                break;
        }
    }

    //Win
    public static void AddWinCount(int value = 1)
    {
        var count = GetWinCount();
        PlayerPrefs.SetInt("WinCount", count + value);

        var monthcount = GetMonthWinCount();
        PlayerPrefs.SetInt("WinCount" + MonthKey(), monthcount + value);

        LeaderBoardManager.SaveAction.Invoke();
    }
    public static int GetWinCount()
    {
        return PlayerPrefs.GetInt("WinCount", 0);
    }

    public static int GetMonthWinCount()
    {
        return PlayerPrefs.GetInt("WinCount" + MonthKey(), 0);
    }

    //Lose
    public static void AddLoseCount(int value = 1)
    {
        var count = GetLoseCount();
        PlayerPrefs.SetInt("LoseCount",count + value);

        var monthcount = GetMonthLoseCount();
        PlayerPrefs.SetInt("LoseCount" + MonthKey(), monthcount + value);
    }
    public static int GetLoseCount()
    {
        return PlayerPrefs.GetInt("LoseCount");
    }

    public static int GetMonthLoseCount()
    {
        return PlayerPrefs.GetInt("LoseCount" + MonthKey());
    }

    //Draw
    public static void AddDrawCount(int value = 1)
    {
        var count = GetDrawCount();
        PlayerPrefs.SetInt("DrawCount", count + value);

        var monthcount = GetMonthDrawCount();
        PlayerPrefs.SetInt("DrawCount" + MonthKey(), monthcount + value);
    }

    public static int GetDrawCount()
    {
        return PlayerPrefs.GetInt("DrawCount");
    }

    public static int GetMonthDrawCount()
    {
        return PlayerPrefs.GetInt("DrawCount" + MonthKey());
    }

    //Total
    public static int GetEverBattleCount()
    {
        return GetWinCount() + GetLoseCount() + GetDrawCount();
    }

    public static int GetMonthBattleCount()
    {
        return GetMonthWinCount() + GetMonthLoseCount() + GetMonthDrawCount();
    }

    //LeaderBoard

    public static string GetObjectIDEver()
    {
        return PlayerPrefs.GetString(string.Format("{0}_{1}_{2}", "board", "Win_Ever", "objectId"), null);
    }

    public static void SetObjectIDEver(string objid)
    {
        PlayerPrefs.SetString(string.Format("{0}_{1}_{2}", "board", "Win_Ever", "objectId"), objid);
        Debug.Log("Set :"+GetObjectIDEver());
    }


    public static string GetObjectIDMonth()
    {
        return PlayerPrefs.GetString(string.Format("{0}_{1}_{2}", "board", "Win_Month" + MonthKey(), "objectId"), null);
    }

    public static void SetObjectIDMonth(string objid)
    {
        PlayerPrefs.SetString(string.Format("{0}_{1}_{2}", "board", "Win_Month" + MonthKey(), "objectId"), objid);
    }


    public static int TodayKey()
    {
        DateTime now = DateTime.Now;
        return now.Year * 1000 + now.Month * 100 + now.Day;
    }

    public static int MonthKey()
    {
        DateTime now = DateTime.Now;
        return now.Year * 100 + now.Month;
    }
}

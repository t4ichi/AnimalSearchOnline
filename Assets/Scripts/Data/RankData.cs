using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RankData
{
    //灰
    public static Color _rank1 = new Color(0.5450981f, 0.5960785f, 0.6509804f);
    //緑
    private static Color _rank2 = new Color(0.5450981f, 0.7882353f, 0.3019608f);
    //青
    private static Color _rank3 = new Color(0.3686275f, 0.5803922f, 1f);
    //紫
    private static Color _rank4 = new Color(0.627451f, 0.4431373f, 1f);
    //赤
    private static Color _rank5 = new Color(0.9333333f, 0.3019608f, 0.3019608f);

    public static Color GetMyRank()
    {
        var win = SaveData.GetWinCount();

        return GetRank(win);
    }

    public static int GetRankNum(int wincount)
    {
        if (wincount < 10) return 1;
        else if (wincount < 50) return 2;
        else if (wincount < 300) return 3;
        else if (wincount < 1000) return 4;
        else return 5;
    }

    /// <summary>
    /// 今までの勝利数から色を返す
    /// </summary>
    public static Color GetRank(int win)
    {
        if (win < 10) return _rank1;
        else if (win < 50) return _rank2;
        else if (win < 300) return _rank3;
        else if (win < 1000) return _rank4;
        else return _rank5;
    }

    /// <summary>
    /// ランクから色を返す
    /// </summary>
    public static Color GetRankColor(int rank)
    {
        if (rank == 1) return _rank1;
        else if (rank == 2) return _rank2;
        else if (rank == 3) return _rank3;
        else if (rank == 4) return _rank4;
        else return _rank5;
    }
}

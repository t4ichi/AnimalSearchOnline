using System;
using System.Collections;
using NCMB;
using NCMB.Extensions;
using UnityEngine;
using TMPro;

public enum ScoreData
{
    WINEVER,
    WINTHISMONTH,
    WINMONTH,
}

public enum RankerData
{
    RANKER,
    NEIGHBOR,
}

public class LeaderBoardManager : MonoBehaviour
{
    [SerializeField] RankingNode _myrankingNode;
    [SerializeField] GameObject rankingNodePrefab;
    [SerializeField] GameObject notFoundNodePrefab;
    [SerializeField] GameObject unavailableNodePrefab;
    [Header("Contents")]
    [SerializeField]
    RectTransform scrollViewContent;

    [Space]
    [SerializeField] TextMeshProUGUI _playerCountText;

    public static Action SaveAction;

    private const string CLASS_NAME_EVER = "Win_Ever";
    private string CLASS_NAME_MONTH = "Win_Month" + SaveData.MonthKey();

    private const string COLUMN_NAME = "name";
    private const string COLUMN_ICON = "icon";
    private const string COLUMN_RANK = "rank";
    private const string COLUMN_SCORE = "score";

    private int _currentRank;

    private Color _myBgColor = new Color(0.85f, 0.85f, 0.85f);

    private NCMBObject _ncmbRecord_ever;
    private NCMBObject _ncmbRecord_month;

    private static LeaderBoardManager _leaderBoardManager;
    public static LeaderBoardManager Leader => _leaderBoardManager;

    private void Awake()
    {
        _leaderBoardManager = GetComponent<LeaderBoardManager>();
    }

    private void OnEnable()
    {
        SaveAction += Save;
    }

    private void OnDisable()
    {
        SaveAction -= Save;
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public IEnumerator Init(ScoreData data, RankerData rankerData)
    {
        LoadMyData(data);

        yield return StartCoroutine(SaveAsync_EverWinScore());
        yield return StartCoroutine(SaveAsync_WinThisMonth());
        yield return StartCoroutine(LoadScoreAsync(data,rankerData));
    }

    /// <summary>
    /// データを保存
    /// </summary>
    public void Save()
    {
        StartCoroutine(SaveAsync_EverWinScore());
        StartCoroutine(SaveAsync_WinThisMonth());
    }

    /// <summary>
    /// プレイヤーの人数を表示
    /// </summary>
    public void LoadPlayerCount(ScoreData data)
    {
        string key = "";
        switch (data)
        {
            case ScoreData.WINEVER:
                key = CLASS_NAME_EVER;
                break;
            case ScoreData.WINTHISMONTH:
                key = CLASS_NAME_MONTH;
                break;
            case ScoreData.WINMONTH:

                break;
        }
        var query = new NCMBQuery<NCMBObject>(key);
        query.CountAsync((count, e) =>
        {
            if (e != null)
            {
                //検索失敗時の処理
                Debug.LogWarning("LoadPlayerCount : 取得失敗");
                return;
            }

            // 成功時の処理
            _playerCountText.text = count.ToString();

        });
    }

    /// <summary>
    /// 指定したスコアデータをロード
    /// </summary>
    public IEnumerator LoadScoreAsync(ScoreData data, RankerData rankerData)
    {
        string classkey = "";
        int currentscore = 0;
        switch (data)
        {
            case ScoreData.WINEVER:
                classkey = CLASS_NAME_EVER;
                currentscore = SaveData.GetWinCount();
                break;
            case ScoreData.WINTHISMONTH:
                classkey = CLASS_NAME_MONTH;
                currentscore = SaveData.GetMonthWinCount();
                break;
            case ScoreData.WINMONTH:
                break;
        }

        switch (rankerData)
        {
            case RankerData.RANKER:
                yield return StartCoroutine(LoadTopRankers(classkey));
                break;
            case RankerData.NEIGHBOR:
                yield return StartCoroutine(LoadNeighbors(classkey, currentscore));
                break;
        }
        LoadPlayerCount(data);
    }


    #region private function

    /// <summary>
    /// 今までの勝利数を保存
    /// </summary>
    private IEnumerator SaveAsync_EverWinScore()
    {
        if (_ncmbRecord_ever == null)
        {
            _ncmbRecord_ever = new NCMBObject(CLASS_NAME_EVER);
            _ncmbRecord_ever.ObjectId = SaveData.GetObjectIDEver();
        }

        _ncmbRecord_ever[COLUMN_SCORE] = SaveData.GetWinCount();
        _ncmbRecord_ever[COLUMN_NAME] = SaveData.GetPlayerName();
        _ncmbRecord_ever[COLUMN_ICON] = SaveData.GetIconID();
        _ncmbRecord_ever[COLUMN_RANK] = RankData.GetRankNum(SaveData.GetWinCount());

        NCMBException errorResult = null;
        yield return _ncmbRecord_ever.YieldableSaveAsync(error => errorResult = error);

        if (errorResult != null)
        {

            Debug.LogWarning(errorResult.ErrorCode + errorResult.ErrorMessage);

            switch (errorResult.ErrorCode)
            {
                case "E404001":
                    //NCMBのコンソールから直接削除した場合に、該当のobjectIdが無いので発生
                    _ncmbRecord_ever.ObjectId = null;
                    yield return _ncmbRecord_ever.YieldableSaveAsync(error => errorResult = error);
                    break;
                case "E405001":
                    //NCMBのコンソールから直接削除した場合に、該当のobjectIdが無いので発生
                    _ncmbRecord_ever.ObjectId = null;
                    yield return _ncmbRecord_ever.YieldableSaveAsync(error => errorResult = error);
                    break;
            }
        }

        //ObjectIDを保存して次に備える
        SaveData.SetObjectIDEver(_ncmbRecord_ever.ObjectId);
    }

    /// <summary>
    /// 今月の勝利数を保存
    /// </summary>
    private IEnumerator SaveAsync_WinThisMonth()
    {
        if (_ncmbRecord_month == null)
        {
            _ncmbRecord_month = new NCMBObject(CLASS_NAME_MONTH);
            _ncmbRecord_month.ObjectId = SaveData.GetObjectIDMonth();
        }

        _ncmbRecord_month[COLUMN_SCORE] = SaveData.GetMonthWinCount();
        _ncmbRecord_month[COLUMN_NAME] = SaveData.GetPlayerName();
        _ncmbRecord_month[COLUMN_ICON] = SaveData.GetIconID();
        _ncmbRecord_month[COLUMN_RANK] = RankData.GetRankNum(SaveData.GetWinCount());

        NCMBException errorResult = null;
        yield return _ncmbRecord_month.YieldableSaveAsync(error => errorResult = error);

        if (errorResult != null)
        {

            Debug.LogWarning(errorResult.ErrorCode + errorResult.ErrorMessage);

            switch (errorResult.ErrorCode)
            {
                case "E404001":
                    //NCMBのコンソールから直接削除した場合に、該当のobjectIdが無いので発生
                    _ncmbRecord_month.ObjectId = null;
                    yield return _ncmbRecord_month.YieldableSaveAsync(error => errorResult = error);
                    break;
                case "E405001":
                    //NCMBのコンソールから直接削除した場合に、該当のobjectIdが無いので発生
                    _ncmbRecord_month.ObjectId = null;
                    yield return _ncmbRecord_month.YieldableSaveAsync(error => errorResult = error);
                    break;
            }
        }

        //ObjectIDを保存して次に備える
        SaveData.SetObjectIDMonth(_ncmbRecord_month.ObjectId);
    }

    /// <summary>
    /// 自分のデータを_myrankingNodeに反映
    /// </summary>
    private void LoadMyData(ScoreData data)
    {
        int score = 0;
        switch (data)
        {
            case ScoreData.WINEVER:
                score = SaveData.GetWinCount();
                break;
            case ScoreData.WINMONTH:
                score = SaveData.GetMonthWinCount();
                break;
        }

        _myrankingNode.Icon.sprite = ResoucesData.GetAnimalDataList().list[SaveData.GetIconID()].Texture;
        _myrankingNode.ScoreText.text = score.ToString();
        _myrankingNode.IconStroke.color = RankData.GetRank(SaveData.GetWinCount());
        _myrankingNode.NameText.text = SaveData.GetPlayerName();
    }

    /// <summary>
    /// スコアが高い人のデータをlimit分ロード
    /// </summary>
    private IEnumerator LoadTopRankers(string classkey,int limit = 15)
    {
        var query = new YieldableNcmbQuery<NCMBObject>(classkey);

        query.Limit = limit;
        query.OrderByDescending(COLUMN_SCORE);

        yield return query.FindAsync();

        Debug.Log("データ取得 : " + query.Count.ToString() + "件");

        DestroyContents(scrollViewContent);


        if (query.Error != null)
        {
            Debug.LogError(query.Error.ErrorMessage);
            Instantiate(unavailableNodePrefab, scrollViewContent);
        }
        else if (query.Count > 0)
        {
            int rank = 0;
            foreach (var r in query.Result)
            {
                var n = Instantiate(rankingNodePrefab, scrollViewContent);
                var rankNode = n.GetComponent<RankingNode>();

                rankNode.NoText.text = (++rank).ToString();

                if(r[COLUMN_NAME] != null)
                    rankNode.NameText.text = r[COLUMN_NAME].ToString();

                if(r[COLUMN_ICON] != null)
                {
                    int iconID = Convert.ToInt32(r[COLUMN_ICON]);
                    rankNode.Icon.sprite = ResoucesData.GetAnimalDataList().list[iconID].Texture;
                }

                if(r[COLUMN_SCORE] != null)
                {
                    int score = Convert.ToInt32(r[COLUMN_SCORE]);
                    rankNode.ScoreText.text = score.ToString();
                }

                if(r[COLUMN_RANK] != null)
                {
                    var myrank = Convert.ToInt32(r[COLUMN_RANK]);
                    Color iconStroke = RankData.GetRankColor(myrank);
                    rankNode.IconStroke.color = iconStroke;
                }

                if (r.ObjectId == SaveData.GetObjectIDEver() || r.ObjectId == SaveData.GetObjectIDMonth())
                {
                    rankNode.BgImage.color = _myBgColor;
                }
            }
        }
        else
        {
            Instantiate(notFoundNodePrefab, scrollViewContent);
        }
    }

    /// <summary>
    /// スコアが近い人のデータをロード
    /// </summary>
    private IEnumerator LoadNeighbors(string classkey,int currentscore,int limit = 3)
    {
        //ライバル前後の数
        int harf = limit;
        var query = new YieldableNcmbQuery<NCMBObject>(classkey);

        yield return StartCoroutine(LoadCurrentRank(classkey,currentscore));
        // スキップする数を決める
        int numskip = _currentRank - (harf + 1);
        if (numskip < 0) numskip = 0;

        query.OrderByDescending(COLUMN_SCORE);
        query.Skip = numskip;
        query.Limit = (harf*2 + 1);
        yield return query.FindAsync();

        DestroyContents(scrollViewContent);


        if (query.Error != null)
        {
            Debug.LogError(query.Error.ErrorMessage);
            Instantiate(unavailableNodePrefab, scrollViewContent);
        }
        else if (query.Count > 0)
        {
            int rank = numskip;
            foreach (var r in query.Result)
            {
                var n = Instantiate(rankingNodePrefab, scrollViewContent);
                var rankNode = n.GetComponent<RankingNode>();
                rankNode.NoText.text = (++rank).ToString();

                if(r[COLUMN_NAME] != null)
                {
                    rankNode.NameText.text = r[COLUMN_NAME].ToString();
                }

                if(r[COLUMN_ICON] != null)
                {
                    int iconID = Convert.ToInt32(r[COLUMN_ICON]);
                    rankNode.Icon.sprite = ResoucesData.GetAnimalDataList().list[iconID].Texture;
                }

                if(r[COLUMN_SCORE] != null)
                {
                    int score = Convert.ToInt32(r[COLUMN_SCORE]);
                    rankNode.ScoreText.text = score.ToString();
                }

                if(r[COLUMN_RANK] != null)
                {
                    var myrank = Convert.ToInt32(r[COLUMN_RANK]);
                    Color iconStroke = RankData.GetRankColor(myrank);
                    rankNode.IconStroke.color = iconStroke;
                    if (r.ObjectId == SaveData.GetObjectIDEver() || r.ObjectId == SaveData.GetObjectIDMonth())
                    {
                        rankNode.BgImage.color = _myBgColor;
                    }
                }
            }
        }
        else
        {
            Instantiate(notFoundNodePrefab, scrollViewContent);
        }
    }

    /// <summary>
    /// 現在の順位をロード
    /// </summary>
    private IEnumerator LoadCurrentRank(string classkey, int currentScore)
    {
        _currentRank = 0;

        NCMBQuery<NCMBObject> rankQuery = new NCMBQuery<NCMBObject>(classkey);
        
        rankQuery.OrderByDescending(COLUMN_SCORE);
        NCMBObject myobj = new NCMBObject(classkey);
 
        if(classkey == CLASS_NAME_EVER)
        {
            myobj.ObjectId = SaveData.GetObjectIDEver();
        }
        else if(classkey == CLASS_NAME_MONTH)
        {
            myobj.ObjectId = SaveData.GetObjectIDMonth();
        }

        int isfetch = 0;
        myobj.FetchAsync((NCMBException e) => {
            if (e != null)
            {
                //エラー処理
                isfetch = -1;
            }
            else
            {
                //成功時の処理
                isfetch = 1;

            }
        });
        yield return new WaitUntil(() => (isfetch != 0));

        bool isCompleteRank = false;
        rankQuery.WhereGreaterThan(COLUMN_SCORE, currentScore);
        rankQuery.CountAsync((int count, NCMBException e) =>
        {
            if (e != null)
            {
                //件数取得失敗
                Debug.Log("取得失敗");
                _currentRank = -1;
            }
            else
            {
                //件数取得成功
                _currentRank += count;
                Debug.Log("AddRank:" + _currentRank);
                isCompleteRank = true;
            }
        });

        if (_currentRank == -1) yield break;

        yield return new WaitUntil(() => isCompleteRank);

        bool isCompleteDate = false;
        NCMBQuery<NCMBObject> createDateQuery = new NCMBQuery<NCMBObject>(classkey);
        createDateQuery.WhereEqualTo(COLUMN_SCORE, currentScore);
        createDateQuery.WhereLessThan("createDate", myobj.CreateDate);

        createDateQuery.CountAsync((int count, NCMBException e) =>
        {
            if (e != null)
            {
                //件数取得失敗
                Debug.Log("取得失敗");
                _currentRank = -1;
            }
            else
            {
                //件数取得成功
                _currentRank += count + 1;
                isCompleteDate = true;
                Debug.Log("Rank:" + _currentRank);
            }
        });


        Debug.Log("createDate:" + myobj.CreateDate);

        if (_currentRank == -1) yield break;
        yield return new WaitUntil(() => isCompleteDate);
    }

    /// <summary>
    /// rect内のオブジェクトを削除する
    /// </summary>
    /// <param name="rect"></param>
    private void DestroyContents(RectTransform rect)
    {
        int nodeCount = rect.childCount;
        for (int i = nodeCount - 1; i >= 0; i--)
        {
            Destroy(rect.GetChild(i).gameObject);
        }
    }
    #endregion
}
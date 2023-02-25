using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using NCMB;
using NCMB.Extensions;
using TMPro;

public class RankingSceneManager : MonoBehaviour
{
    private const string OBJECT_ID = "objectId";
    private const string COLUMN_SCORE = "score";
    private const string COLUMN_NAME = "name";
    private const string COLUMN_ICON = "icon";

    [SerializeField] RectTransform scrollViewContent;
    [SerializeField] GameObject rankingNodePrefab;
    [SerializeField] GameObject readingNodePrefab;
    [SerializeField] GameObject notFoundNodePrefab;
    [SerializeField] GameObject unavailableNodePrefab;

    [Header("MyRank")]

    [SerializeField] RankingNode _myrankingNode;

    private string _objectid = null;

    private string ObjectID
    {
        get { return _objectid ?? (_objectid = PlayerPrefs.GetString(BoardIdPlayerPrefsKey, null)); }
        set
        {
            if (_objectid == value)
                return;
            PlayerPrefs.SetString(BoardIdPlayerPrefsKey, _objectid = value);
        }
    }

    private string BoardIdPlayerPrefsKey
    {
        get { return string.Format("{0}_{1}_{2}", "board", _board.ClassName, OBJECT_ID); }
    }

    private RankingInfo _board;
    private IScore _lastScore;

    private NCMBObject _ncmbRecord;

    /// <summary>
    /// 入力した名前
    /// </summary>
    /// <value>The name of the inputted.</value>
    private string InputtedNameForSave
    {
        get
        {
            return SaveData.GetPlayerName();
        }
    }

    private static RankingSceneManager _rankingSceneManager;
    public static RankingSceneManager Ranking => _rankingSceneManager;

    private void Awake()
    {
        _rankingSceneManager = GetComponent<RankingSceneManager>();
    }

    public IEnumerator Init(int boardID = 0)
    {
        _board = RankingLoader.Instance.CurrentRanking;
        _lastScore = RankingLoader.Instance.LastScore;

        Debug.Log(BoardIdPlayerPrefsKey + "=" + PlayerPrefs.GetString(BoardIdPlayerPrefsKey, null));

        yield return SendScore();

        SetMyData(boardID);
    }

    IEnumerator SendScore()
    {
        yield return StartCoroutine(SendScoreEnumerator(0));
        yield return StartCoroutine(SendScoreEnumerator(1));
    }


    private void SetMyData(int boardID)
    {
        _myrankingNode.Icon.sprite = ResoucesData.GetAnimalDataList().list[SaveData.GetIconID()].Texture;

        //int score = SaveData.GetWinCount();
        int score = 0;
        if (boardID == 0)
            score = SaveData.GetWinCount();
        //else if (boardID == 1)
        //    score = SaveData.GetTodayWinCount();

        _myrankingNode.ScoreText.text = score.ToString();
        _myrankingNode.IconStroke.color = RankData.GetRank(score);
        _myrankingNode.NameText.text = SaveData.GetPlayerName();

        int currentRank = 0;
        // データスコアから検索
        NCMBQuery<NCMBObject> rankQuery = new NCMBQuery<NCMBObject>(_board.ClassName);
        rankQuery.WhereGreaterThan(COLUMN_SCORE, score);
        rankQuery.CountAsync((int count, NCMBException e) => {

            if (e != null)
            {
                //件数取得失敗
                //_rank.text = "取得失敗";
            }
            else
            {
                //件数取得成功
                currentRank = count + 1; // 自分よりスコアが上の人がn人いたら自分はn+1位
                Debug.Log("自分の順位:" + currentRank);
                _myrankingNode.NoText.text = currentRank.ToString();
            }
        });
        //return currentRank;

        //int no = GetMyNo(score);
        Debug.Log("順位:" + currentRank);
        //_myrankingNode.NoText.text = currentRank.ToString();

        //_icon.sprite = ResoucesData.GetAnimalDataList().list[SaveData.GetIconID()].Texture;
        //int score = SaveData.GetWinCount();
        //_score.text = score.ToString();
        //_stroke.color = RankData.GetRank(score);
        //_name.text = SaveData.GetPlayerName();
        //_rank.text = GetMyRank(score).ToString();
    }

    private IEnumerator SendScoreEnumerator(int boardID)
    {
        //ハイスコア送信
        if (_ncmbRecord == null)
        {
            string name = RankingLoader.Instance.RankingBoards.GetRankingInfo(boardID).ClassName;

            _ncmbRecord = new NCMBObject(name);
            _ncmbRecord.ObjectId = ObjectID;
        }

        //string name = RankingLoader.Instance.RankingBoards.GetRankingInfo(boardID).ClassName;
        //_ncmbRecord = new NCMBObject(name);
        //_ncmbRecord.ObjectId = ObjectID;

        Debug.Log("ClassName: " + name);

        _ncmbRecord[COLUMN_NAME] = InputtedNameForSave;

        if (boardID == 0)
            _ncmbRecord[COLUMN_SCORE] = SaveData.GetWinCount();
        //else if (boardID == 1)
        //    _ncmbRecord[COLUMN_SCORE] = SaveData.GetTodayWinCount();

        _ncmbRecord[COLUMN_ICON] = SaveData.GetIconID();

        NCMBException errorResult = null;

        yield return _ncmbRecord.YieldableSaveAsync(error => errorResult = error);

        if (errorResult != null)
        {
            //NCMBのコンソールから直接削除した場合に、該当のobjectIdが無いので発生する（らしい）
            _ncmbRecord.ObjectId = null;
            yield return _ncmbRecord.YieldableSaveAsync(error => errorResult = error); //新規として送信
        }

        //ObjectIDを保存して次に備える
        ObjectID = _ncmbRecord.ObjectId;
    }

    /// <summary>
    /// ランキング取得＆表示
    /// </summary>
    /// <returns>The ranking board.</returns>
    private IEnumerator LoadRankingBoard()
    {
        int nodeCount = scrollViewContent.childCount;
        for (int i = nodeCount - 1; i >= 0; i--)
        {
            Destroy(scrollViewContent.GetChild(i).gameObject);
        }

        var msg = Instantiate(readingNodePrefab, scrollViewContent);

        //2017.2.0b3の描画されないバグ暫定対応
        //MaskOffOn();

        var so = new YieldableNcmbQuery<NCMBObject>(_board.ClassName);
        so.Limit = 30;
        if (_board.Order == ScoreOrder.OrderByAscending)
        {
            so.OrderByAscending(COLUMN_SCORE);
        }
        else
        {
            so.OrderByDescending(COLUMN_SCORE);
        }

        yield return so.FindAsync();

        Debug.Log("データ取得 : " + so.Count.ToString() + "件");
        Destroy(msg);

        if (so.Error != null)
        {
            Instantiate(unavailableNodePrefab, scrollViewContent);
        }
        else if (so.Count > 0)
        {
            int rank = 0;
            foreach (var r in so.Result)
            {
                var n = Instantiate(rankingNodePrefab, scrollViewContent);
                var rankNode = n.GetComponent<RankingNode>();
                rankNode.NoText.text = (++rank).ToString();
                rankNode.NameText.text = r[COLUMN_NAME].ToString();

                int iconID = System.Convert.ToInt32(r[COLUMN_ICON]);
                rankNode.Icon.sprite = ResoucesData.GetAnimalDataList().list[iconID].Texture;

                var s = _board.BuildScore(r[COLUMN_SCORE].ToString());
                rankNode.ScoreText.text = s != null ? s.TextForDisplay : "エラー";

                int wincountEver = System.Convert.ToInt32(r[COLUMN_SCORE]);
                Color iconStroke = RankData.GetRank(wincountEver);

                rankNode.IconStroke.color = iconStroke;

                //Debug.Log(r[COLUMN_SCORE].ToString());
            }
        }
        else
        {
            Instantiate(notFoundNodePrefab, scrollViewContent);
        }
    }

    public IEnumerator LoadRankingBoard(RectTransform content)
    {
        int nodeCount = content.childCount;
        for (int i = nodeCount - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        var msg = Instantiate(readingNodePrefab, content);

        //2017.2.0b3の描画されないバグ暫定対応
        //MaskOffOn();

        var so = new YieldableNcmbQuery<NCMBObject>(_board.ClassName);
        so.Limit = 30;
        if (_board.Order == ScoreOrder.OrderByAscending)
        {
            so.OrderByAscending(COLUMN_SCORE);
        }
        else
        {
            so.OrderByDescending(COLUMN_SCORE);
        }

        yield return so.FindAsync();

        Debug.Log("データ取得 : " + so.Count.ToString() + "件");
        Destroy(msg);

        if (so.Error != null)
        {
            Instantiate(unavailableNodePrefab, content);
        }
        else if (so.Count > 0)
        {
            int rank = 0;
            foreach (var r in so.Result)
            {
                var n = Instantiate(rankingNodePrefab, content);
                var rankNode = n.GetComponent<RankingNode>();
                rankNode.NoText.text = (++rank).ToString();
                rankNode.NameText.text = r[COLUMN_NAME].ToString();

                int iconID = System.Convert.ToInt32(r[COLUMN_ICON]);
                rankNode.Icon.sprite = ResoucesData.GetAnimalDataList().list[iconID].Texture;

                var s = _board.BuildScore(r[COLUMN_SCORE].ToString());
                rankNode.ScoreText.text = s != null ? s.TextForDisplay : "エラー";

                int wincountEver = System.Convert.ToInt32(r[COLUMN_SCORE]);
                Color iconStroke = RankData.GetRank(wincountEver);

                rankNode.IconStroke.color = iconStroke;

                //Debug.Log(r[COLUMN_SCORE].ToString());
            }
        }
        else
        {
            Instantiate(notFoundNodePrefab, content);
        }
    }


    //public void OnCloseButtonClick()
    //{
    //    UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("Ranking");
    //}

    private void MaskOffOn()
    {
        //2017.2.0b3でなぜかScrollViewContentを追加しても描画されない場合がある。
        //親maskをOFF/ONすると直るので無理やり・・・
        var m = scrollViewContent.parent.GetComponent<Mask>();
        m.enabled = false;
        m.enabled = true;
    }
}
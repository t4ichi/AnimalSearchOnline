using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// リーダーボードの表示を管理
/// </summary>
public class LeaderBoardUIManager : MonoBehaviour
{
    public static Color openColor = new Color(1, 1, 1);
    public static Color closeColor = new Color(0.6509804f, 0.6509804f, 0.6509804f);

    public static Color textopenColor = new Color(0.3490196f, 0.3490196f, 0.3490196f);
    public static Color textcloseColor = new Color(1f, 1f, 1f);

    public int currentIndex = 0;

    [Header("Priod")]
    [SerializeField] Button _priodEverButton;
    [SerializeField] Button _priodMonthButton;
    [Header("Ranker")]
    [SerializeField] Button _rankerTopButton;
    [SerializeField] Button _rankerNeighborButton;


    [SerializeField] TextMeshProUGUI _everBattleCount;
    [SerializeField] TextMeshProUGUI _everWinCount;

    [SerializeField] TextMeshProUGUI _monthBattleCount;
    [SerializeField] TextMeshProUGUI _monthWinCount;

    private static LeaderBoardUIManager _selectorSwitchManager;
    public static LeaderBoardUIManager Selector => _selectorSwitchManager;


    private ScoreData _currentScoreData = ScoreData.WINEVER;
    private RankerData _currentRankerData = RankerData.RANKER;

    void Awake()
    {
        _selectorSwitchManager = GetComponent<LeaderBoardUIManager>();
    }

    void Start()
    {
        _priodEverButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayAudio(AudioType.CLICK);
            _currentScoreData = ScoreData.WINEVER;
            StartCoroutine(LeaderBoardManager.Leader.LoadScoreAsync(_currentScoreData, _currentRankerData));
            SwitchButton(_priodEverButton,_priodMonthButton);
        });

        _priodMonthButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayAudio(AudioType.CLICK);
            _currentScoreData = ScoreData.WINTHISMONTH;
            StartCoroutine(LeaderBoardManager.Leader.LoadScoreAsync(_currentScoreData, _currentRankerData));
            SwitchButton(_priodMonthButton, _priodEverButton);
        });

         _rankerTopButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayAudio(AudioType.CLICK);
            _currentRankerData = RankerData.RANKER;
            StartCoroutine(LeaderBoardManager.Leader.LoadScoreAsync(_currentScoreData, _currentRankerData));
            SwitchButton(_rankerTopButton, _rankerNeighborButton);
        });

        _rankerNeighborButton.onClick.AddListener(() =>
        {
            SoundManager.Instance.PlayAudio(AudioType.CLICK);
            _currentRankerData = RankerData.NEIGHBOR;
            StartCoroutine(LeaderBoardManager.Leader.LoadScoreAsync(_currentScoreData, _currentRankerData));
            SwitchButton(_rankerNeighborButton, _rankerTopButton);
        });

        SwitchButton(_priodEverButton, _priodMonthButton);
        SwitchButton(_rankerTopButton, _rankerNeighborButton);
    }

    public void SetEnable()
    {
        StartCoroutine(LeaderBoardManager.Leader.Init(_currentScoreData,_currentRankerData));
        SetMyData();
    }

    public void SetDisable() { }

    private void SwitchButton(Button pressedButton,Button otherButton)
    {
        pressedButton.interactable = false;
        otherButton.interactable = true;

        pressedButton.gameObject.GetComponent<Image>().color = openColor;
        pressedButton.transform.Find("text").gameObject.GetComponent<TextMeshProUGUI>().color = textopenColor;


        otherButton.gameObject.GetComponent<Image>().color = closeColor;
        otherButton.transform.Find("text").gameObject.GetComponent<TextMeshProUGUI>().color = textcloseColor;
    }

    private void SetMyData()
    {
        _everBattleCount.text = SaveData.GetEverBattleCount().ToString();
        _everWinCount.text = SaveData.GetWinCount().ToString();

        _monthBattleCount.text = SaveData.GetMonthBattleCount().ToString();
        _monthWinCount.text = SaveData.GetMonthWinCount().ToString();
    }
}
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun.UtilityScripts;

public class TimeManager : MonoBehaviour
{
    [SerializeField] GameObject _timerObj;
    [SerializeField] Image _timerFillAmount;
    [SerializeField] TextMeshProUGUI _timerText;

    public float TurnDuration = 60f;

    PunTurnManager _turnManager;
    IEnumerator _timerIE;

    private int _remainTime;
    private int _nowCount;

    private void Start()
    {
        _turnManager = GetComponent<PunTurnManager>();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public IEnumerator Init()
    {
        if (GameManager.CurrentGameMode != GameMode.Practice)
        {
            _timerObj.SetActive(true);
            _timerText.text = ((int)_turnManager.TurnDuration).ToString();
            _timerFillAmount.fillAmount = 1;
        }
        else
        {
            _timerObj.SetActive(false);
        }
        yield break;
    }

    /// <summary>
    /// タイマーを止める
    /// </summary>
    public void Stop()
    {
        if(_timerIE != null)
        {
            StopCoroutine(_timerIE);
            _timerIE = null;
        }
    }

    /// <summary>
    /// ターンを終了
    /// </summary>
    public void TurnFinish()
    {
        if (GameManager.CurrentGameMode != GameMode.Practice)
        {
            Stop();
        }
    }

    /// <summary>
    /// 次のターンの準備
    /// </summary>
    public void Next()
    {
        _timerText.text = ((int)_turnManager.TurnDuration).ToString();
        _timerFillAmount.fillAmount = 1;
    }

    /// <summary>
    /// 次のターンを開始
    /// </summary>
    public void StartNext()
    {
        if (GameManager.CurrentGameMode != GameMode.Practice)
        {
            _timerIE = UpdateTimer();
            StartCoroutine(_timerIE);

            _timerText.text = ((int)_turnManager.TurnDuration).ToString();
            _timerFillAmount.fillAmount = 1;
        }
    }

    /// <summary>
    /// タイマーを更新
    /// </summary>
    private IEnumerator UpdateTimer()
    {
        PhotonNetwork.CurrentRoom.SetStartTime();

        yield return new WaitForSeconds(0.5f);

        while (((int)_turnManager.RemainingSecondsInTurn) >= 0.5f)
        {
            _remainTime = Mathf.FloorToInt(_turnManager.RemainingSecondsInTurn);
            _timerText.text = _remainTime.ToString();
            _timerFillAmount.fillAmount = _turnManager.RemainingSecondsInTurn / _turnManager.TurnDuration;
            CountDown(_remainTime);
            yield return null;
        }

        _timerText.text = "0";
        _timerFillAmount.fillAmount = 0;
        yield return new WaitForSeconds(0.5f);

        Debug.Log("TimeOver IsTurnComplete: " + OnlineManager.IsTurnComplete);
        if (OnlineManager.IsTurnComplete) yield break;

        _turnManager.OnTurnTimeEnd();
    }

    /// <summary>
    /// カウントダウンする。残りの時間がtime以下なら音で警告する
    /// </summary>
    private void CountDown(int remain,int time = 5)
    {
        if(remain <= time)
        {
            if(_nowCount != remain)
            {
                _nowCount = remain;
                SoundManager.Instance.PlayAudio(AudioType.TimeCount);
            }
        }
    }
}

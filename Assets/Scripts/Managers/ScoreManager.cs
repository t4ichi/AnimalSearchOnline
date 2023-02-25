using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviourPunCallbacks
{
    [Header("Buttons")]
    [SerializeField] List<AnswerButton> _answerbuttonList;
    [SerializeField] Button _deleteButton;
    [SerializeField] Button _okButton;

    [SerializeField] List<GameObject> _bottomButtons;

    [Space]
    [SerializeField] List<Log> _loglist = new List<Log>();
    [SerializeField] Log _settingAnswer;

    [SerializeField] List<Image> _boardPanels = new List<Image>();

    [Header("UI")]
    [SerializeField] Image[] _answerImages = new Image[3];
    [SerializeField] List<GameObject> _answerObjects = new List<GameObject>();
    [SerializeField] Sprite _questionImage;
    [SerializeField] GameObject _hitblowTextObj;
    [SerializeField] TextMeshProUGUI _nextText;

    [Header("Data")]
    [SerializeField] List<int> _answerData = new List<int>();
    [SerializeField] List<int> _answerbuttonData = new List<int>();
    [SerializeField] List<int> _setList;

    public static int _turnCounter;

    private int _selectedIndex;

    private int hit;
    private int blow;

    public bool IsButtonPress { get; set; }

    public static readonly int _answerCount = 3;
    private static readonly int _answerButtonCount = 6;

    public static event Action<GameEndState> OnGameEnd;
    public static event Action OnGameNextTurn;

    AnimalDataList _animaldataList;

    PlayerManager _playerManager;
    PunTurnManager _turnManager;
    OnlineManager _gameCore;

    private void Start()
    {
        _setList = new List<int>();
        _playerManager = GetComponent<PlayerManager>();
        _turnManager = GetComponent<PunTurnManager>();
        _gameCore = GetComponent<OnlineManager>();

        _animaldataList = ResoucesData.GetAnimalDataList();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public IEnumerator Init()
    {
        //データの初期化
        yield return StartCoroutine(InitData());

        //データ作成,ボタン初期化
        yield return StartCoroutine(CreateData());

        //削除、決定ボタンに処理を入れる
        InitDeleteAndOKButton();

        //画像を初期化
        InitLogList();
        InitAnswerImages();

        //最初はボタンが押せないように
        IsButtonPress = false;
        AnswerButtonsInteractable(false);
        ResetInsteractable();

        //アニメーションの初期化
        InitAnimation();
    }

    /// <summary>
    /// ゲームを開始
    /// </summary>
    public void StartGame()
    {
        if (GameManager.CurrentGameMode != GameMode.Practice)
        {

        }
        else
        {
            AnswerButtonsInteractable(true);
            IsButtonPress = true;
        }
    }

    /// <summary>
    /// ターンを終了
    /// </summary>
    public void TurnFinish()
    {
        IsButtonPress = false;
        AnswerButtonsInteractable(false);
        ResetInsteractable();
    }

    /// <summary>
    /// スコアを算出する
    /// </summary>
    public void Calculate()
    {
        var score = JadgeScore(_setList, _answerData);
        hit = score[0];
        blow = score[1];

        //Logにセット
        _loglist[_turnCounter].InitSetScore(hit, blow);
        for (int i = 0; i < _answerCount; i++)
        {
            _loglist[_turnCounter].SetAnimalImage(i, _setList[i]);
        }

        string hb;
        if (hit == _answerCount)
        {
            hb = _answerCount + "ヒット!";
        }
        else
        {
            hb = hit + "ヒット " + blow + "ブロー";
        }

        _nextText.text = hb;

        if (hit >= _answerCount)
        {
            GameManager.IsGaming = false;
        }

        var seq = DOTween.Sequence();

        seq.Append(_loglist[_turnCounter].SetScoreAnimation());
        seq.AppendInterval(0.75f);
        seq.Append(HitBlowAnimation());
        seq.OnComplete(() =>
        {
            JadgeNext(hit);
        });
    }

    /// <summary>
    /// 次のターンの準備
    /// </summary>
    public void Next()
    {
        _setList.Clear();
        _selectedIndex = 0;
        _settingAnswer.ResetAnimalImage();
        _turnCounter++;
    }

    /// <summary>
    /// 次のターンを開始
    /// </summary>
    public void StartNext()
    {
        if (GameManager.CurrentGameMode != GameMode.Practice)
        {
            var ismyturn = _playerManager.IsMyTurn(PlayerManager.CurrentPlayer);
            AnswerButtonsInteractable(ismyturn);

            IsButtonPress = ismyturn;
        }
        else
        {
            AnswerButtonsInteractable(true);
            IsButtonPress = true;
        }
    }

    /// <summary>
    /// スコアをジャッジ
    /// </summary>
    public int[] JadgeScore(List<int> _setList, List<int> _answerData)
    {
        int[] array = new int[2];
        int hit = 0;
        int blow = 0;

        for (int i = 0; i < _setList.Count; i++)
        {
            if (_setList.Contains(_answerData[i]))
            {
                if (_setList[i] == _answerData[i])
                {
                    hit++;
                }
                else
                {
                    blow++;
                }
            }
        }
        array[0] = hit;
        array[1] = blow;

        return array;
    }

    /// <summary>
    /// 答えを表示
    /// </summary>
    public void ShowAnswer(Action action)
    {
        var seq = DOTween.Sequence();

        seq.AppendCallback(() =>
        {
            SoundManager.Instance.PlayAudio(AudioType.ShowAnswer);

            var al = ResoucesData.GetAnimalDataList().list;

            for (int i = 0; i < _answerCount; i++)
            {
                _answerImages[i].sprite = al[_answerData[i]].Texture;
            }
        });

        seq.AppendInterval(1f);
        seq.AppendCallback(() =>
        {
            action?.Invoke();
        });
    }

    /// <summary>
    /// 回答ボタンを押せる状態を切り替える
    /// </summary>
    public void AnswerButtonsInteractable(bool isInteractable)
    {
        foreach (var b in _answerbuttonList)
        {
            b.Button.interactable = isInteractable;
        }
    }

    //-------------ゲーム内ボタン---------------
    public void AnimalButton(int animalid)
    {
        SoundManager.Instance.PlayAudio(AudioType.AnimalClick);
        _setList.Add(animalid);
        _settingAnswer.SetAnimalImage(_selectedIndex, animalid);
        _selectedIndex++;
        OnSetlistChanged();

        StartCoroutine(ButtonPressInterval());
    }

    public void DeleteButton()
    {
        SoundManager.Instance.PlayAudio(AudioType.AnimalClick);
        _setList.RemoveAt(_setList.Count - 1);
        _settingAnswer.RemoveAnimalImage(_selectedIndex - 1);
        _selectedIndex--;
        OnSetlistChanged();

        StartCoroutine(DeleteButtonPressInterval());
    }

    public void OkButton()
    {
        _okButton.interactable = false;
        GameManager.Game.TurnFinish();
    }

    //---------------初期化---------------------
    /// <summary>
    /// データの初期化
    /// </summary>
    private IEnumerator InitData()
    {
        _turnCounter = 0;
        _selectedIndex = 0;
        _setList.Clear();
        _answerData.Clear();
        _answerbuttonData.Clear();
        _settingAnswer.ResetAnimalImage();

        yield break;
    }

    /// <summary>
    /// //データ作成
    /// </summary>
    private IEnumerator CreateData()
    {
        if (GameManager.CurrentGameMode != GameMode.Practice)
        {
            yield return StartCoroutine(SyncData());
        }
        else
        {
            _answerData = CreateAnswerData();
            _answerbuttonData = CreateAnswerButtonData();
        }
        InitAnimalButton();
    }

    /// <summary>
    /// ログを初期化
    /// </summary>
    private void InitLogList()
    {
        for (int i = 0; i < _loglist.Count; i++)
        {
            int n = i;
            _loglist[n].Init();
        }
    }

    /// <summary>
    /// 答えのパネルを初期化
    /// </summary>
    private void InitAnswerImages()
    {
        for (int i = 0; i < _answerImages.Length; i++)
        {
            _answerImages[i].sprite = null;
        }
    }

    /// <summary>
    /// 削除、決定ボタンを初期化
    /// </summary>
    private void InitDeleteAndOKButton()
    {
        //オンラインのとき
        if (GameManager.CurrentGameMode != GameMode.Practice)
        {
            ButtonExtentions.OnButtonPressed(_deleteButton, () =>
            {
                if (_setList.Count == 0) return;
                _turnManager.SendMove(-1, false);
            });

            ButtonExtentions.OnButtonPressed(_okButton, () =>
            {
                if (_setList.Count < _answerCount) return;
                _turnManager.SendMove(null, true);
            });
        }
        //オフラインのとき
        else
        {
            ButtonExtentions.OnButtonPressed(_deleteButton, () =>
            {
                if (_setList.Count == 0) return;
                DeleteButton();
            });

            ButtonExtentions.OnButtonPressed(_okButton, () =>
            {
                if (_setList.Count < _answerCount) return;
                OkButton();
            });
        }
    }

    /// <summary>
    /// どうぶつボタンを初期化
    /// </summary>
    private void InitAnimalButton()
    {
        for (int i = 0; i < _answerbuttonList.Count; i++)
        {
            int n = i;
            var animalData = _answerbuttonData[n];
            _answerbuttonList[n].Init(animalData);

            if (GameManager.CurrentGameMode != GameMode.Practice)
            {
                _answerbuttonList[n].Button.onClick.AddListener(() =>
                {
                    if (_setList.Count < _answerData.Count)
                    {
                        _turnManager.SendMove(animalData, false);
                    }
                });
            }
            else
            {
                _answerbuttonList[n].Button.onClick.AddListener(() =>
                {
                    if (_setList.Count < _answerData.Count)
                    {
                        AnimalButton(animalData);
                    }
                });
            }
        }
    }

    /// <summary>
    /// 決定、削除ボタンを押せないようする
    /// </summary>
    private void ResetInsteractable()
    {
        _okButton.interactable = false;
        _deleteButton.interactable = false;
    }

    /// <summary>
    /// 答えのデータを生成
    /// </summary>
    private List<int> CreateAnswerData()
    {
        var indexlist = new List<int>();
        for (int i = 0; i < _animaldataList.list.Count; i++)
        {
            int n = i;
            indexlist.Add(n);
        }

        var copy = new List<int>(indexlist);
        var answerlist = new List<int>();

        for (int i = 0; i < _answerCount; i++)
        {
            int ram = UnityEngine.Random.Range(0, copy.Count);
            answerlist.Add(copy[ram]);
            copy.Remove(copy[ram]);
        }
        return answerlist;
    }

    /// <summary>
    /// ボタンのデータを生成
    /// </summary>
    private List<int> CreateAnswerButtonData()
    {
        //答えのどうぶつは必ず入れる
        var _answerbuttonlist = new List<int>();
        for (int i = 0; i < _answerData.Count; i++)
        {
            _answerbuttonlist.Add(_answerData[i]);
        }

        //重複なしの乱数を生成
        List<int> array = new List<int>();
        for (int i = 0; i < _animaldataList.list.Count; i++)
        {
            array.Add(i);
        }

        //ハズレ用のどうぶつを入れる
        while (_answerbuttonlist.Count < _answerbuttonList.Count)
        {
            int ran = array[UnityEngine.Random.Range(0, array.Count)];
            array.Remove(ran);

            if (!_answerbuttonlist.Contains(ran))
            {
                _answerbuttonlist.Add(ran);
            }
        }
        _answerbuttonlist.Sort();
        return _answerbuttonlist;
    }

    /// <summary>
    /// 生成した答えとボタンのデータを同期する
    /// </summary>
    private IEnumerator SyncData()
    {
        //ホストなら生成したデータを送信する
        if (PhotonNetwork.IsMasterClient)
        {
            _answerData = CreateAnswerData();
            _answerbuttonData = CreateAnswerButtonData();

            PhotonNetwork.CurrentRoom.SetAnswer(_answerData.ToArray());
            PhotonNetwork.CurrentRoom.SetAnswerButton(_answerbuttonData.ToArray());
        }
        //ホストではないなら同期する必要がある
        else
        {
            var answerArray = new int[_answerCount];
            for (int i = 0; i < _answerCount; i++)
            {
                yield return new WaitUntil(() =>
                {
                    return PhotonNetwork.CurrentRoom.GetAnswer(i) != -1;
                });

                answerArray[i] = PhotonNetwork.CurrentRoom.GetAnswer(i);
            }
            _answerData.AddRange(answerArray);

            //ボタンを同期
            var answerButtonArray = new int[_answerButtonCount];
            for (int i = 0; i < _answerButtonCount; i++)
            {
                yield return new WaitUntil(() =>
                {
                    return PhotonNetwork.CurrentRoom.GetAnswerButton(i) != -1;
                });

                answerButtonArray[i] = PhotonNetwork.CurrentRoom.GetAnswerButton(i);
            }
            _answerbuttonData.AddRange(answerButtonArray);
        }
    }

    //---------------ゲーム中--------------------
    /// <summary>
    /// プレイヤーの回答に変更があった場合
    /// </summary>
    private void OnSetlistChanged()
    {
        if (GameManager.CurrentGameMode != GameMode.Practice)
        {
            if (PlayerManager.CurrentPlayer == PlayerManager.MyPlayer)
            {
                _okButton.interactable = (_setList.Count == _answerCount);
                _deleteButton.interactable = (_setList.Count != 0);
            }
            else
            {
                ResetInsteractable();
            }
        }
        else
        {
            _okButton.interactable = (_setList.Count == _answerCount);
            _deleteButton.interactable = (_setList.Count != 0);
        }
    }

    /// <summary>
    /// hitとターン数から次の行動を決める
    /// </summary>
    /// <param name="hit"></param>
    private void JadgeNext(int hit)
    {
        //クリアした場合
        if (hit >= _answerCount)
        {
            if (GameManager.CurrentGameMode != GameMode.Practice)
            {
                var endState = _playerManager.JadgeWinner(PlayerManager.CurrentPlayer);
                _gameCore.GameFinish(endState);
            }
            else
            {
                OnGameEnd(GameEndState.Clear);
            }
        }
        //Logの数を超えた場合はゲームオーバー
        else if (_turnCounter >= _loglist.Count)
        {
            if (GameManager.CurrentGameMode != GameMode.Practice)
            {
                _gameCore.GameFinish(GameEndState.Draw);
            }
            else
            {
                OnGameEnd(GameEndState.GameOver);
            }

            return;
        }
        //続ける場合
        else
        {
            if (GameManager.CurrentGameMode != GameMode.Practice)
            {
                _gameCore.StartTurn();
            }
            else
            {
                OnGameNextTurn();
            }
        }
    }

    //---------------ボタンの挙動---------------------
    private IEnumerator ButtonPressInterval()
    {
        AnswerButtonsInteractable(false);
        yield return new WaitForSeconds(0.2f);

        if (!IsButtonPress) yield break;
        AnswerButtonsInteractable(true);
    }

    private IEnumerator DeleteButtonPressInterval()
    {
        _deleteButton.interactable = false;
        yield return new WaitForSeconds(0.2f);

        if (!IsButtonPress || (_setList.Count == 0)) yield break;
        _deleteButton.interactable = true;
    }

    //---------------アニメーション--------------
    /// <summary>
    /// UIの配置を初期化
    /// </summary>
    private void InitAnimation()
    {
        DotweenAnimations.InitObjectPosition(_answerObjects);
        DotweenAnimations.InitObjectPosition(_bottomButtons);
        DotweenAnimations.InitImageRotate(_boardPanels);
    }

    /// <summary>
    /// スタート時のアニメーション
    /// </summary>
    public void StartAnimation(Action action = null)
    {
        var sequence = DOTween.Sequence();

        var boardRotate = DotweenAnimations.RotateImageAnimation(_boardPanels);
        var buttonApper = DotweenAnimations.ButtonAppearAnimation(_bottomButtons);

        var answerimages = DotweenAnimations.ButtonAppearAnimation(_answerObjects);
        var startAnimation = AnswerAnimation();

        sequence.Append(answerimages);
        sequence.Join(boardRotate);
        sequence.Join(buttonApper);

        sequence.Append(startAnimation);
        sequence.AppendInterval(1);
        sequence.AppendCallback(() =>
        {
            SoundManager.Instance.PlayAudio(AudioType.Start);
            if (GameManager.CurrentGameMode != GameMode.Practice)
            {
                _playerManager.NextAnimation(() =>
                {
                    action?.Invoke();
                });
            }
            else
            {
                TextUIAnimation.InGameTextAnimation("Start!", () =>
                {
                    action?.Invoke();
                });
            }
        });
    }

    /// <summary>
    /// ゲーム開始前にランダムでどうぶつを切り替えるアニメーション
    /// </summary>
    private Tween AnswerAnimation()
    {
        var sequence = DOTween.Sequence();

        for (int i = 0; i < _answerImages.Length; i++)
        {
            var spritechage = DOTween.Sequence();
            int n = i;

            spritechage.AppendCallback(() =>
            {
                int ran = UnityEngine.Random.Range(0, _animaldataList.list.Count - 1);
                _answerImages[n].sprite = _animaldataList.list[ran].Texture;
            })
            .SetDelay(0.05f)
            .SetLoops(8, LoopType.Restart)
            .OnComplete(() =>
            {
                _answerImages[n].sprite = _questionImage;
                SoundManager.Instance.PlayAudio(AudioType.Stop);
            });

            sequence.Append(spritechage);
        }
        return sequence;
    }

    private Tween HitBlowAnimation()
    {
        _hitblowTextObj.SetActive(true);

        var cg = _hitblowTextObj.GetComponent<CanvasGroup>();
        var t = _hitblowTextObj.transform;
        cg.DOFade(0, 0);
        t.DOScale(new Vector3(5, 5, 1), 0);

        var seq = DOTween.Sequence();
        seq.AppendCallback(() =>
        {
            if (hit == _answerCount)
            {
                SoundManager.Instance.PlayAudio(AudioType.ClearNext);
            }
            else
            {
                SoundManager.Instance.PlayAudio(AudioType.Next);
            }
        });
        seq.Append(cg.DOFade(1, 0.25f));
        seq.Join(t.DOScale(new Vector3(1, 1, 1), 0.25f).SetEase(Ease.OutSine));

        seq.AppendInterval(1.25f);
        seq.Append(cg.DOFade(0, 0.1f));

        return seq;
    }
}
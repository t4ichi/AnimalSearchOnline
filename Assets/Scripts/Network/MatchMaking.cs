using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections;

public class MatchMaking : MonoBehaviourPunCallbacks
{
    private static MatchMaking _match;
    public static MatchMaking Match => _match;

    OnlineManager _gameCore;
    PunTurnManager _turnManager;

    //マスターサーバーに接続しようとしたか
    public static bool IsMatchingServer = false;
    public static bool IsConnectMasterServer { get; set; }

    [Header("Wating")]
    [SerializeField] TextMeshProUGUI _passwordText;
    [SerializeField] TextMeshProUGUI _watingText;
    [SerializeField] PlayerProfile _myprofile;
    [SerializeField] PlayerProfile _otherprofile;

    [Header("Friend")]
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] TMP_InputField passwordInputField = default;

    [SerializeField] Button joinRoomButton = default;
    [SerializeField] Button _backButton;

    private event Action OnMaxPlayers;
    private string _currentPassward;

    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
        _gameCore = GetComponent<OnlineManager>();
        _turnManager = GetComponent<PunTurnManager>();
    }

    private void Start()
    {
        _match = GetComponent<MatchMaking>();

        // マスターサーバーに接続するまでは、入力できないようにする
        canvasGroup.interactable = false;
        // パスワードを入力する前は、ルーム参加ボタンを押せないようにする
        joinRoomButton.interactable = false;

        passwordInputField.onValueChanged.AddListener(OnPasswordInputFieldValueChanged);

        OnMaxPlayers += OnMaxPlayersRoom;
    }


    public void OnJoinRandomRoomButtonClick()
    {
        JoinRoom(GameMode.Random);
    }

    public void OnJoinPrivateRoomButtonClick()
    {
        canvasGroup.interactable = false;
        JoinRoom(GameMode.Friend);
    }

    private void CloseBackButton()
    {
        _backButton.interactable = false;
        _backButton.gameObject.SetActive(false);
    }

    //------------------Callbacks---------------------

    /// <summary>
    /// 自分が部屋に参加した場合
    /// </summary>
    public override void OnJoinedRoom()
    {
        StartCoroutine(Init());
        // ルームが満員になったら、以降そのルームへの参加を不許可にする
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            if (_turnManager.Turn == 0)
            {
                OnMaxPlayers?.Invoke();
            }
        }
    }
    /// <summary>
    /// 他のプレイヤーが部屋に入ってきた場合
    /// </summary>
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            if (_turnManager.Turn == 0)
            {
                OnMaxPlayers?.Invoke();
            }
        }
    }

    /// <summary>
    /// マスターサーバーに接続した場合
    /// </summary>
    public override void OnConnectedToMaster()
    {
        IsConnectMasterServer = true;
        IsMatchingServer = true;
        canvasGroup.interactable = true;
        StopCoroutine(TryConnect());
    }

    /// <summary>
    /// サーバーとの接続が切断された場合
    /// </summary>
    public override void OnDisconnected(DisconnectCause cause)
    {
        IsConnectMasterServer = false;
        IsMatchingServer = true;
        StartCoroutine(TryConnect());
    }

    /// <summary>
    /// 自分が切断した場合
    /// </summary>
    public override void OnLeftRoom()
    {
        //バックボタンで戻った場合は何もしない
        if (_backButton.interactable == false) return;

        if (MenuManager.Instance.GetCurrentMenu == MenuType.MatchMaking || GameManager.IsGaming)
        {
            ErrorPopup.Error.Open(ErrorType.ConnectionBad, () =>
            {
                GameManager.Game.Exit();
                FadeManager.Instance.FadeWhileAction(() =>
                {
                    if (GameManager.CurrentGameMode == GameMode.Random)
                    {
                        if (ScoreManager._turnCounter > 0)
                        {
                            SaveData.AddLoseCount();
                        }
                    }
                    FadeManager.Instance.FadeWhileAction(() =>
                    {
                        MenuManager.Instance.SwitchMenu(MenuType.Main);
                    });
                });
            });
        }
    }

    /// <summary>
    /// 相手に切断された場合
    /// </summary>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (MenuManager.Instance.GetCurrentMenu == MenuType.MatchMaking || GameManager.IsGaming)
        {
            ErrorPopup.Error.Open(ErrorType.Disconnection, () =>
            {
                if (GameManager.IsGaming)
                {
                    GameManager.Game.Exit();
                    if (GameManager.CurrentGameMode == GameMode.Random)
                    {
                        if (ScoreManager._turnCounter > 0)
                        {
                            GameManager.Game.EndGame(GameEndState.Win);
                        }
                        else
                        {
                            FadeManager.Instance.FadeWhileAction(() =>
                            {
                                MenuManager.Instance.SwitchMenu(MenuType.Main);
                            });
                        }
                    }
                    else
                    {
                        FadeManager.Instance.FadeWhileAction(() =>
                        {
                            MenuManager.Instance.SwitchMenu(MenuType.Main);
                        });
                    }
                }
                else
                {
                    FadeManager.Instance.FadeWhileAction(() =>
                    {
                        MenuManager.Instance.SwitchMenu(MenuType.Main);
                    });
                }
            });
        }
    }

    /// <summary>
    /// ランダムで参加できるルームが存在しないなら、新規でルームを作成する
    /// </summary>
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        switch (GameManager.CurrentGameMode)
        {
            case GameMode.Random:
                // ルームの参加人数を2人に設定する
                var roomOptions = new RoomOptions();
                roomOptions.MaxPlayers = 2;

                PhotonNetwork.CreateRoom(null, roomOptions);
                break;
            case GameMode.Friend:
                // ルームへの参加が失敗したら、パスワードを再び入力できるようにする
                passwordInputField.text = string.Empty;
                canvasGroup.interactable = true;
                break;
        }
    }

    #region private function
    /// <summary>
    /// 部屋のプレイヤーの人数が揃った場合
    /// </summary>
    private void OnMaxPlayersRoom()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        CloseBackButton();
        StartCoroutine(StartToGame());
    }

    /// <summary>
    /// パスワードを入力するフィールドの値が変化した場合
    /// </summary>
    private void OnPasswordInputFieldValueChanged(string value)
    {
        // パスワードを6桁入力した時のみ、ルーム参加ボタンを押せるようにする
        joinRoomButton.interactable = (value.Length == 6);
    }

    /// <summary>
    /// 指定した部屋に参加する
    /// </summary>
    private void JoinRoom(GameMode gameMode)
    {
        if (IsConnectMasterServer)
        {
            GameManager.CurrentGameMode = gameMode;

            if (gameMode == GameMode.Random)
            {
                PhotonNetwork.JoinRandomRoom();
            }
            else if (gameMode == GameMode.Friend)
            {
                JoinPrivateRoom();
            }

            FadeManager.Instance.FadeIn();
        }
        else
        {
            ErrorPopup.Error.Open(ErrorType.ConnectionBad);
            Debug.Log("接続失敗");
        }
    }

    /// <summary>
    /// パスワードありの部屋に参加する
    /// </summary>
    private void JoinPrivateRoom()
    {
        // ルームを非公開に設定する（新規でルームを作成する場合）
        var roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        roomOptions.IsVisible = false;
        // パスワードと同じ名前のルームに参加する（ルームが存在しなければ作成してから参加する）
        _currentPassward = passwordInputField.text;
        PhotonNetwork.JoinOrCreateRoom(_currentPassward, roomOptions, TypedLobby.Default);
    }

    /// <summary>
    /// マッチメイキングの初期化
    /// </summary>
    private IEnumerator Init()
    {
        MenuManager.Instance.SwitchMenu(MenuType.MatchMaking);

        if(GameManager.CurrentGameMode == GameMode.Friend)
        {
            _passwordText.gameObject.SetActive(true);
            _passwordText.text = "パスワード: "+ _currentPassward;
        }
        else
        {
            _passwordText.gameObject.SetActive(false);
        }

        _watingText.text = "対戦相手を待っています";
        _watingText.color = new Color(1, 1, 1);

        _myprofile.gameObject.SetActive(false);
        _otherprofile.gameObject.SetActive(false);

        _backButton.interactable = true;
        _backButton.gameObject.SetActive(true);

        yield return StartCoroutine(SetMyProfile());

        FadeManager.Instance.FadeOut(() =>
        {
            DotweenAnimations.MatchMakingProfile(_myprofile.gameObject);
        });

    }

    /// <summary>
    /// 自分のプロフィール情報をセット
    /// </summary>
    private IEnumerator SetMyProfile()
    {
        //自分の情報を送信
        var name = SaveData.GetPlayerName();
        PhotonNetwork.NickName = name;
        int icon = SaveData.GetIconID();
        PhotonNetwork.LocalPlayer.SetIconID(icon);
        int win = SaveData.GetWinCount();
        PhotonNetwork.LocalPlayer.SetWinCount(win);

        //送信されるまで待機
        yield return new WaitUntil(() =>
        {
            return
            PhotonNetwork.NickName != null &&
            PhotonNetwork.LocalPlayer.GetIconID() != -1 &&
            PhotonNetwork.LocalPlayer.GetWinCount() != -1;
        });

        //情報を反映
        _myprofile.SetPlayerProfile(name, icon, win, RankData.GetMyRank());
    }

    /// <summary>
    /// 他プレイヤーの情報をセット
    /// </summary>
    private IEnumerator UpdateOtherPlayerData()
    {
        var player = PhotonNetwork.PlayerListOthers[0];
        var profile = _otherprofile;

        //情報を受け取るまで待機
        yield return new WaitUntil(() =>
        {
            return
            player.NickName != null &&
            player.GetIconID() != -1 &&
            player.GetWinCount() != -1;
        });

        var name = player.NickName;
        int icon = player.GetIconID();
        int win = player.GetWinCount();

        //反映
        profile.SetPlayerProfile(name, icon, win, RankData.GetRank(win));
    }


    /// <summary>
    /// ゲームを開始する
    /// </summary>
    private IEnumerator StartToGame()
    {
        yield return new WaitForSeconds(1);
        yield return StartCoroutine(UpdateOtherPlayerData());
        yield return StartCoroutine(MatchingCompleteAnimation());
        yield return new WaitForSeconds(2);
        _gameCore.StartTurn();
    }

    /// <summary>
    /// 一秒ごとに接続できるかチェックする
    /// </summary>
    private IEnumerator TryConnect()
    {
        yield return new WaitForSeconds(1);
        PhotonNetwork.ConnectUsingSettings();
    }

    //-----------------Animation-------------------
    private IEnumerator MatchingCompleteAnimation()
    {
        bool isAnimation = false;

        DotweenAnimations.MatchMakingProfile(_otherprofile.gameObject, () =>
        {
            isAnimation = true;
        });

        yield return new WaitUntil(() =>
        {
            return isAnimation;
        });

        SoundManager.Instance.PlayAudio(AudioType.MatchComplete);
        _watingText.text = "開始します!";

        _watingText.color = new Color(0.93f, 0.8f, 0.4f);
        DotweenAnimations.MatchCompleteText(_watingText);
    }

    #endregion
}
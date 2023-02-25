using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

/// <summary>
/// メインメニュー
/// </summary>
public class MainMenu : Menu
{
    [Header("Main")]
    [SerializeField] GameObject _gameLogoObj;
    [SerializeField] GameObject _mainWindow;
    [SerializeField] Button _playButtonRandomMatch;
    [SerializeField] Button _playButtonPractice;
    [SerializeField] Button _playButtonFriendMatch;
    [SerializeField] Button _friendMatchPopupClose;
    [SerializeField] Button _playButtonFriendMatchOk;
    [SerializeField] UIPopup _frinedMatchPopup;

    private Tween _logotween;

    [Header("Setting")]
    [SerializeField] GameObject _settingWindow;
    [SerializeField] GameObject _creditWindow;
    [SerializeField] Button _settingButton;
    [SerializeField] Button _settingCloseButton;

    [SerializeField] CreditDataSO _creditData;

    [Space]
    //[SerializeField] AdBlockPopup _adblockPopup;
    [SerializeField] UIPopup _adblockPopup;
    [SerializeField] Button _adblockOpenButton;
    [SerializeField] Button _adblockButton;

    [SerializeField] GameObject _isPurchedPanel;

    [SerializeField] Button _restoreButton;
    [SerializeField] Button _reviewButton;
    [SerializeField] Button _feedbackButton;

    [Header("Credit")]
    [SerializeField] Button _creditButton;
    [SerializeField] Button _creditCloseButton;
    [SerializeField] Button _developButton;

    [Header("Record")]
    [SerializeField] GameObject _recordWindow;
    [SerializeField] Button _recordOpenButton;
    [SerializeField] Button _recordCloseButton;
    [SerializeField] Button _rankingOpenButton;

    [Space]
    [SerializeField] TextMeshProUGUI _everBattleCountText;
    [SerializeField] TextMeshProUGUI _everWinCountText;

    [Space]
    [SerializeField] TextMeshProUGUI _todayBattleCountText;
    [SerializeField] TextMeshProUGUI _todayWinCountText;

    [Header("HowToPlay")]
    [SerializeField] GameObject _howtoplayWindow;
    [SerializeField] Button _howtoButton;
    [SerializeField] Button _howtoplayClose;

    [Header("PlayerSettings")]
    [SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] Image _playerIconImage;
    [SerializeField] IconSetting _mainmenuIcon;
    [SerializeField] Image _popupPlayerIconImage;

    [SerializeField] UIPopup _namePopup;
    [Space]
    [SerializeField] Button _nameOpenButton;
    [SerializeField] Button _nameOKButton;
    [SerializeField] TMP_InputField _nameInputField;

    [Space]
    [SerializeField] GameObject _playerIcon;
    [SerializeField] RectTransform _playerIconField;

    [Header("Icon")]
    [SerializeField] GameObject _iconWindow;
    [SerializeField] Button _iconOpenButton;
    [SerializeField] Button _iconCloseButton;

    [SerializeField] TextMeshProUGUI _getIconPanelText;
    [SerializeField] Button _getIconButton;
    [SerializeField] Button _getIconCloseButton;
    [SerializeField] Button _RewardAdgetIconButton;

    [SerializeField] UIPopup _getIconPopup;

    private List<int> NotHaveIconList = new List<int>();
    private List<GameObject> IconList = new List<GameObject>();

    private string ADBLOCK_KEY;

    private void Start()
    {
        //PlayButtons
        OnButtonPressed(_playButtonRandomMatch, PlayRandomMatch, true);

        OnButtonPressed(_playButtonFriendMatch, PlayFriendMatch, true);
        OnButtonPressed(_friendMatchPopupClose, FriendMatchPopupClose, true);
        OnButtonPressed(_playButtonFriendMatchOk, JoinPrivateRoom, false);

        OnButtonPressed(_playButtonPractice, PlayPractice,true);

        //UnderButtons
        OnButtonPressed(_settingButton, SettingButtonListener, true);
        OnButtonPressed( _settingCloseButton, SettingClose, true);

        OnButtonPressed(_howtoButton, HowToPlayButtonListener, true);
        OnButtonPressed(_howtoplayClose, HowToPlayCloseButtonListener, true);

        //PlayerSetting
        OnButtonPressed(_nameOpenButton, NameOpenButton, true);
        OnButtonPressed(_nameOKButton, NameOKButton, true);

        //Icon
        OnButtonPressed(_iconOpenButton, IconOpenButton, true);
        OnButtonPressed(_iconCloseButton, IconCloseButton, true);
        OnButtonPressed(_getIconButton, GetIconButtonListener, false);
        OnButtonPressed(_getIconCloseButton, GetIconCloseButtonListener, true);
        OnButtonPressed(_RewardAdgetIconButton, RewardAdIconButtonListener, false);

        //Setting
        OnButtonPressed(_restoreButton, RestoreButtonListener, true);
        OnButtonPressed(_reviewButton, ReviewButtonListener, true);
        OnButtonPressed(_creditButton, CreditButtonListener, true);
        OnButtonPressed(_creditCloseButton, CreditCloseButtonListener, true);
        OnButtonPressed(_developButton, DevelopButtonListener, true);
        OnButtonPressed(_feedbackButton, FeedBackButtonListener, true);

        //Record
        OnButtonPressed(_recordOpenButton, RecordOpenListener, true);
        OnButtonPressed(_recordCloseButton, RecordCloseListener, true);

        //AdBlock
        if (SaveData.GetIsAdBlock())
        {
            _isPurchedPanel.SetActive(true);
        }
        else
        {
            _isPurchedPanel.SetActive(false);
            OnButtonPressed(_adblockOpenButton, AdBlockOpenButtonListener, true);
            OnButtonPressed(_adblockButton, AdBlockButtonListener, true);
        }

        //Init
        SetName(SaveData.GetPlayerName());
        _nameInputField.text = SaveData.GetPlayerName();
        IconSettingInit();

        //Fade
        StartCoroutine(FadeOpen());
    }

    public override void SetEnable()
    {
        base.SetEnable();
        RateUs.Instance.ClickPlay();
        PurchasingManager.PurchaseCompleted += PurchaseCompleteHandler;
        AdsManager.OnRewardedAdWatchedComplete += GetIconRewardAdCompleted;
        AdsManager.Instance.ShowBanner();
        _mainmenuIcon.ChengeIconColor();
        _playButtonRandomMatch.interactable = true;
        _logotween = DotweenAnimations.LogoAnimation(_gameLogoObj.GetComponent<RectTransform>(),0.1f);
    }
    public override void SetDisable()
    {
        base.SetDisable();
        _frinedMatchPopup.Disable();
        PurchasingManager.PurchaseCompleted -= PurchaseCompleteHandler;
        AdsManager.Instance.DestroyBanner();
        _logotween.Kill();

    }

    //--------------------Main--------------------------
    private void PlayRandomMatch()
    {
        Debug.Log("RandomMatch");
        SoundManager.Instance.PlayAudio(AudioType.CLICK);
        MatchMaking.Match.OnJoinRandomRoomButtonClick();
    }

    private void PlayFriendMatch()
    {
        Debug.Log("FriendMatch");
        SoundManager.Instance.PlayAudio(AudioType.CLICK);
        _frinedMatchPopup.Open();
    }

    private void FriendMatchPopupClose()
    {
        SoundManager.Instance.PlayAudio(AudioType.CLICK);
        _frinedMatchPopup.Close();
    }

    private void JoinPrivateRoom()
    {
        SoundManager.Instance.PlayAudio(AudioType.CLICK);
        MatchMaking.Match.OnJoinPrivateRoomButtonClick();
    }

    private void PlayPractice()
    {
        Debug.Log("Mode : Practice");
        SoundManager.Instance.PlayAudio(AudioType.CLICK);
        GameManager.CurrentGameMode = GameMode.Practice;

        StartCoroutine(GameManager.Game.InitGame(false));
    }

    //--------------------Setting--------------------------

    private void SettingButtonListener()
    {
        SoundManager.Instance.PlayAudio(AudioType.CLICK);
        DotweenAnimations.WindowOpenAnimation(_mainWindow, _settingWindow);
    }

    public void SettingClose()
    {
        SoundManager.Instance.PlayAudio(AudioType.CLICK);
        DotweenAnimations.WindowCloseAnimation(_mainWindow, _settingWindow);
    }

    //--------------------HowToPlay--------------------------
    private void HowToPlayButtonListener()
    {
        SoundManager.Instance.PlayAudio(AudioType.CLICK);
        DotweenAnimations.WindowOpenAnimation(_mainWindow,_howtoplayWindow);
    }

    public void HowToPlayCloseButtonListener()
    {
        SoundManager.Instance.PlayAudio(AudioType.CLICK);
        DotweenAnimations.WindowCloseAnimation(_mainWindow, _howtoplayWindow);
    }

    //---------------------Name---------------------------
    private void NameOpenButton()
    {
        SoundManager.Instance.PlayAudio(AudioType.CLICK);
        _namePopup.Open();
    }

    private void NameOKButton()
    {
        SoundManager.Instance.PlayAudio(AudioType.CLICK);
        var name = _nameInputField.text;
        var ngwords = ResoucesData.GetNGWordData();

        if (name.IncludeAny(ngwords.NGWordList.ToArray()))
        {
            //NGワードに引っかかった場合
            ErrorPopup.Error.Open(ErrorType.NGWordCaution,()=>
            {
                _nameInputField.text = "";
            });
        }
        else
        {
            var newName = name;

            if (newName == string.Empty)
            {
                newName = SaveData.GetPlayerName();
            }

            SaveData.SetPlayerName(newName);
            SetName(newName);

            _namePopup.Close();
        }
    }

    private void SetName(string name)
    {
        _nameText.text = name;
    }

    //--------------------Icon--------------------------

    private void IconSettingInit()
    {
        var al = ResoucesData.GetAnimalDataList();

        if (SaveData.IsNewGame())
        {
            int getid = Random.Range(0, al.list.Count);
            SaveData.SetHaveIcon(getid);
            SaveData.SetIconID(getid);
            SaveData.SetNewGame();
        }

        for (int i = 0; i < al.list.Count; i++)
        {
            int n = i;
            var a = Instantiate(_playerIcon, _playerIconField);
            IconList.Add(a);

            var image = a.transform.Find("icon").GetComponent<Image>();
            var b = a.GetComponent<Button>();

            image.sprite = al.list[i].Texture;
            b.onClick.AddListener(() =>
            {
                _playerIconImage.sprite = al.list[n].Texture;
                _popupPlayerIconImage.sprite = al.list[n].Texture;
                SaveData.SetIconID(n);
                IconCloseButton();
            });

            if (!SaveData.IsHaveIcon(n))
            {
                image.color = new Color(0.349f, 0.349f, 0.349f);
                b.interactable = false;
                NotHaveIconList.Add(n);
            }
        }

        if (SaveData.IsGetIconToday(0)) _getIconButton.interactable = false;
        if (SaveData.IsGetIconToday(1)) _RewardAdgetIconButton.interactable = false;
        CheckGetAllIcon();

        //Icon
        _playerIconImage.sprite = al.list[SaveData.GetIconID()].Texture;
        _popupPlayerIconImage.sprite = al.list[SaveData.GetIconID()].Texture;
    }

    private void IconOpenButton()
    {
        SoundManager.Instance.PlayAudio(AudioType.CLICK);
        DotweenAnimations.WindowOpenAnimation(_mainWindow, _iconWindow);
    }

    private void IconCloseButton()
    {
        SoundManager.Instance.PlayAudio(AudioType.CLICK);
        DotweenAnimations.WindowCloseAnimation(_mainWindow, _iconWindow);
    }

    private void GetIconButtonListener()
    {
        SoundManager.Instance.PlayAudio(AudioType.CLICK);
        _getIconButton.interactable = false;
        GetIconButton();
        SaveData.SetGetIconToday(0);
    }

    private void GetIconCloseButtonListener()
    {
        SoundManager.Instance.PlayAudio(AudioType.CLICK);
        _getIconPopup.Close();
    }

    private void RewardAdIconButtonListener()
    {
        SoundManager.Instance.PlayAudio(AudioType.CLICK);
        AdsManager.Instance.ShowRewarded();
    }
    private void GetIconRewardAdCompleted()
    {
        _RewardAdgetIconButton.interactable = false;
        GetIconButton();
        SaveData.SetGetIconToday(1);
    }

    private void GetIconButton()
    {
        SoundManager.Instance.PlayAudio(AudioType.Get);
        var pickID = NotHaveIconList[Random.Range(0, NotHaveIconList.Count)];
        SaveData.SetHaveIcon(pickID);
        NotHaveIconList.Remove(pickID);
        CheckGetAllIcon();

        var icon =IconList[pickID].transform.Find("icon").GetComponent<Image>();
        var button = IconList[pickID].GetComponent<Button>();

        icon.color = new Color(1, 1, 1);
        button.interactable = true;

        var al = ResoucesData.GetAnimalDataList();

        var popupicon = _getIconPopup.gameObject.transform.Find("Container/icon").GetComponent<Image>();
        popupicon.sprite = al.list[pickID].Texture;
        _getIconPopup.Open();
    }

    

    private void CheckGetAllIcon()
    {
        if (NotHaveIconList.Count == 0)
        {
            _getIconPanelText.text = "コンプリート!";
            _getIconButton.interactable = false;
            _RewardAdgetIconButton.interactable = false;
        }
        else
        {
            _getIconPanelText.text = "一日一回限定!";
        }
    }

    //--------------------Setting--------------------------
    private void AdBlockOpenButtonListener()
    {
        SoundManager.Instance.PlayAudio(AudioType.CLICK);
        _adblockPopup.Open();
    }

    private void AdBlockButtonListener()
    {
        SoundManager.Instance.PlayAudio(AudioType.CLICK);

#if UNITY_IOS
        ADBLOCK_KEY = ResoucesData.GetIAPProductData().Products[0].ID_IOS;
#elif UNITY_ANDROID
        ADBLOCK_KEY = ResoucesData.GetIAPProductData().Products[0].ID_ANDROID;
#endif
        PurchasingManager.Purchase(ADBLOCK_KEY);
    }

    private void PurchaseCompleteHandler(string productID)
    {
        if (productID == ADBLOCK_KEY)
        {
            SaveData.SetAdBlock();
            _isPurchedPanel.SetActive(true);
            _adblockPopup.Close();
        }
    }
    private void RestoreButtonListener()
    {
        SoundManager.Instance.PlayAudio(AudioType.CLICK);
        PurchasingManager.RestorePurchases();
    }

    private void ReviewButtonListener()
    {
        SoundManager.Instance.PlayAudio(AudioType.CLICK);
        Application.OpenURL(_creditData.AppURL);
    }


    private void FeedBackButtonListener()
    {
        SoundManager.Instance.PlayAudio(AudioType.CLICK);
        Application.OpenURL(_creditData.FeedbackURL);
    }

    private void CreditButtonListener()
    {
        SoundManager.Instance.PlayAudio(AudioType.CLICK);
        DotweenAnimations.WindowOpenAnimation(_settingWindow, _creditWindow);
    }

    private void CreditCloseButtonListener()
    {
        SoundManager.Instance.PlayAudio(AudioType.CLICK);
        DotweenAnimations.WindowCloseAnimation(_settingWindow, _creditWindow);
    }
    private void DevelopButtonListener()
    {
        SoundManager.Instance.PlayAudio(AudioType.CLICK);
        Application.OpenURL(_creditData.TwitterURL);
    }

    private void RecordOpenListener()
    {
        SoundManager.Instance.PlayAudio(AudioType.CLICK);
        DotweenAnimations.WindowOpenAnimation(_mainWindow, _recordWindow);

        LeaderBoardUIManager.Selector.SetEnable();
    }

    private void RecordCloseListener()
    {
        SoundManager.Instance.PlayAudio(AudioType.CLICK);
        DotweenAnimations.WindowCloseAnimation(_mainWindow, _recordWindow);

        LeaderBoardUIManager.Selector.SetDisable();
    }

    //--------------------アニメーション--------------------------
    private IEnumerator FadeOpen()
    {
        FadeManager.Instance.LoadingAnimationPlay();
        yield return new WaitUntil(() =>
        {
            return MatchMaking.IsMatchingServer;
        });
        FadeManager.Instance.FadeOut(() =>
        {
            FadeManager.Instance.LoadingAnimationEnd();
        });
    }

}

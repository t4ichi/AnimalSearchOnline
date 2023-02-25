using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] AudioSource _bgMusicSource;

    [Space(10)]
    [Range(0, 1)] [SerializeField] float _musicVolume = 1f;
    [Range(0, 1)] [SerializeField] float _fxVolume = 1f;

    [Header("Audio Data")]
    [SerializeField] AudioDataSO _audioData;

    bool _musicEnable = true;
    bool _fxEnable = true;

    GameObject oneShotGameObject;
    AudioSource oneShotAudioSource;

    private void Start()
    {
        _fxEnable = SaveData.GetFxEnable();
        _musicEnable = SaveData.GetMusicEnable();

        if (_musicEnable) PlayBackgroundMusic();
    }

    /// <summary>
    /// 指定した音を再生する
    /// </summary>
    /// <param name="type">再生する音</param>
    public void PlayAudio(AudioType type)
    {
        if (!_fxEnable) return;

        if (oneShotGameObject == null)
        {
            oneShotGameObject = new GameObject("Sound");
            oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
        }

        AudioClip clip = GetClip(type);
        oneShotAudioSource.volume = _fxVolume;
        oneShotAudioSource.PlayOneShot(clip);
    }

    /// <summary>
    /// トグルでBGMのON,OFFを切り替える
    /// </summary>
    public void ToggleBGM(ref bool state)
    {
        _musicEnable = !_musicEnable;
        UpdateMusic();
        state = _musicEnable;
        SaveData.SetMusic(_musicEnable);
    }

    /// <summary>
    /// トグルでSEのON,OFFを切り替える
    /// </summary>
    public void ToggleSE(ref bool state)
    {
        _fxEnable = !_fxEnable;
        state = _fxEnable;
        SaveData.SetFx(_fxEnable);
    }

    #region private function

    /// <summary>
    /// BGMを再生する
    /// </summary>
    private void PlayBackgroundMusic()
    {
        _bgMusicSource.Stop();
        _bgMusicSource.clip = _audioData.BackgroundMusic;
        _bgMusicSource.volume = _musicVolume;
        _bgMusicSource.Play();
    }

    /// <summary>
    /// BGMを更新
    /// </summary>
    private void UpdateMusic()
    {
        if (!_musicEnable)
            _bgMusicSource.Stop();
        else
            PlayBackgroundMusic();
    }

    /// <summary>
    /// 指定したクリップを取得する
    /// </summary>
    private AudioClip GetClip(AudioType type)
    {
        switch (type)
        {
            case AudioType.CLICK:
                return _audioData.ClickClip;
            //InGame
            case AudioType.AnimalClick:
                return _audioData.AnimalClickClip;
            case AudioType.OkClick:
                return _audioData.OkClickClip;
            case AudioType.Clear:
                return _audioData.ClearClip;
            case AudioType.GameOver:
                return _audioData.GameOverClip;
            case AudioType.Stop:
                return _audioData.StopClip;
            case AudioType.Start:
                return _audioData.StartClip;
            case AudioType.Next:
                return _audioData.NextClip;
            case AudioType.ClearNext:
                return _audioData.ClearNextClip;
            case AudioType.ShowAnswer:
                return _audioData.ShowAnswerClip;
            case AudioType.Draw:
                return _audioData.DrawClip;
            case AudioType.MatchPlayer:
                return _audioData.MatchPlayerClip;
            case AudioType.MatchComplete:
                return _audioData.MatchCompleteClip;
            case AudioType.Get:
                return _audioData.GetClip;
            case AudioType.TimeOver:
                return _audioData.TimeOverClip;
            case AudioType.TimeCount:
                return _audioData.TimeCountClip;
            case AudioType.ButtonAppear:
                return _audioData.ButtonAppearClip;
            case AudioType.Hit:
                return _audioData.HitClip;
            case AudioType.Success:
                return _audioData.SuccessClip;
            case AudioType.Wrong:
                return _audioData.WrongClip;
            default:
                return _audioData.FailClip;
        }
    }
    #endregion
}

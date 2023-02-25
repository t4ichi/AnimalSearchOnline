using UnityEngine;

[CreateAssetMenu()]
public class AudioDataSO : ScriptableObject
{
    [field: SerializeField, Space]
    public AudioClip BackgroundMusic { get; private set; }

    [field: SerializeField, Space]
    public AudioClip ClickClip { get; private set; }

    [field: SerializeField]
    public AudioClip FailClip { get; private set; }

    //InGame
    [field: SerializeField]
    public AudioClip ButtonAppearClip { get; private set; }

    [field: SerializeField]
    public AudioClip AnimalClickClip { get; private set; }

    [field: SerializeField]
    public AudioClip OkClickClip { get; private set; }

    [field: SerializeField]
    public AudioClip ClearClip { get; private set; }

    [field: SerializeField]
    public AudioClip GameOverClip { get; private set; }

    [field: SerializeField]
    public AudioClip StopClip { get; private set; }

    [field: SerializeField]
    public AudioClip StartClip { get; private set; }

    [field: SerializeField]
    public AudioClip NextClip { get; private set; }

    [field: SerializeField]
    public AudioClip ClearNextClip { get; private set; }

    [field: SerializeField]
    public AudioClip ShowAnswerClip { get; private set; }

    [field: SerializeField]
    public AudioClip DrawClip { get; private set; }

    [field: SerializeField]
    public AudioClip TimeCountClip { get; private set; }

    [field: SerializeField]
    public AudioClip TimeOverClip { get; private set; }

    [field: SerializeField]
    public AudioClip HitClip { get; private set; }

    //MatchMaking
    [field: SerializeField]
    public AudioClip MatchPlayerClip { get; private set; }

    [field: SerializeField]
    public AudioClip MatchCompleteClip { get; private set; }

    //Celebrate
    [field: SerializeField]
    public AudioClip GetClip { get; private set; }

    [field: SerializeField]
    public AudioClip SuccessClip { get; private set; }

    [field: SerializeField]
    public AudioClip WrongClip { get; private set; }
}

public enum AudioType
{
    FAIL, CLICK,Clear,GameOver,

    //InGame
    ButtonAppear,Hit,
    Stop,Start,Next,ClearNext,ShowAnswer,Draw,AnimalClick,OkClick,TimeOver,TimeCount,

    //MatchMaking
    MatchPlayer,MatchComplete,

    //Celebrate
    Get,

    //HowToPlay
    Success,Wrong
}

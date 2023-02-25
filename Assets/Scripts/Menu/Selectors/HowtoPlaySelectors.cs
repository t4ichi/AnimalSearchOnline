using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 遊び方を説明するSelectorの管理
/// </summary>
public class HowtoPlaySelectors : SelectorManager
{
    public override void NextButton()
    {
        base.NextButton();
        SoundManager.Instance.PlayAudio(AudioType.CLICK);
    }

    public override void BackButton()
    {
        base.BackButton();
        SoundManager.Instance.PlayAudio(AudioType.CLICK);
    }
}

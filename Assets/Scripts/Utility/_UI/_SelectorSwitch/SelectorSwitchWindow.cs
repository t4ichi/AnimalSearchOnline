using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class SelectorSwitchWindow : MonoBehaviour
{
    //このWindowを開くボタン
    public Button openButton;
    //ボタンの色を変える
    public Image _buttonImage;

    public virtual void SetEnable()
    {
        gameObject.SetActive(true);
        _buttonImage.color = LeaderBoardUIManager.openColor;
    }

    public virtual void SetDisable()
    {
        gameObject.SetActive(false);
        _buttonImage.color = LeaderBoardUIManager.closeColor;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class ButtonExtentions
{
    /// <summary>
    /// ボタンにアクションを追加
    /// </summary>
    public static void OnButtonPressed(Button button, UnityAction buttonListener)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(buttonListener);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AnswerButton : MonoBehaviour
    , IPointerClickHandler
    , IPointerDownHandler
    , IPointerEnterHandler
    , IPointerExitHandler
    , IPointerUpHandler
{
    public Image sprite;
    public Button Button;

    public bool IsButtonDown { get; private set; }

    /// <summary>
    /// どうぶつのアイコンをセットし、初期化する
    /// </summary>
    /// <param name="id"></param>
    public void Init(int id)
    {
        Button.onClick.RemoveAllListeners();
        sprite.sprite = ResoucesData.GetAnimalDataList().list[id].Texture;
        IsButtonDown = false;
    }

    //----------イベントリスナー-------------

    public void OnPointerClick(PointerEventData data)
    {
        IsButtonDown = false;
    }

    public void OnPointerDown(PointerEventData data)
    {
        IsButtonDown = true;
    }

    public void OnPointerEnter(PointerEventData data)
    {
        IsButtonDown = false;
    }

    public void OnPointerExit(PointerEventData data)
    {
        IsButtonDown = false;
    }
    public void OnPointerUp(PointerEventData data)
    {
        IsButtonDown = false;
    }
}
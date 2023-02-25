using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

/// <summary>
/// ボタンを押すとimageが少し下に動くアニメーション
/// </summary>
public class ButtonAnimation : MonoBehaviour,
IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] RectTransform image;
    [SerializeField] float _moveValue = 4;

    private Vector3 normal = new Vector3(0, 0, 0);
    private Vector3 pressed;

    Button _button;

    private void Start()
    {
        pressed = new Vector3(0, -_moveValue, 0);
        _button = GetComponent<Button>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!_button.interactable) return;
        image.DOLocalMove(pressed, 0.1f);
    }

    public void OnPointerUp(PointerEventData data)
    {
        image.DOLocalMove(normal, 0.1f);
    }
}

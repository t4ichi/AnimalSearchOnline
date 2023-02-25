using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class UIPopup : MonoBehaviour
{
    [SerializeField] GameObject PopupObj;
    [SerializeField] CanvasGroup BG;
    [SerializeField] GameObject PopupCanvas;

    [Header("もし閉じるボタンが必要なら設定")]
    [SerializeField] bool _isUseClose = false;
    [SerializeField] Button _closeButton;


    protected virtual void Awake()
    {
        if (_isUseClose)
        {
            _closeButton.onClick.AddListener(() => Close());
        }
    }

    /// <summary>
    /// ポップアップをアニメーションして開く
    /// </summary>
    public void Open(float firstScale = 0.9f, float openedScale = 1f)
    {
        var CanvasAlpha = PopupCanvas.GetComponent<CanvasGroup>();
        var CanvasTransform = PopupCanvas.GetComponent<RectTransform>();

        PopupObj.SetActive(true);
        BG.alpha = 0;
        CanvasAlpha.alpha = 0;
        CanvasTransform.DOScale(new Vector3(firstScale, firstScale, 1), 0f);

        var sequence = DOTween.Sequence();
        sequence.SetUpdate(true);
        sequence.Append(BG.DOFade(0.8f, 0.2f));
        sequence.Append(CanvasAlpha.DOFade(1, 0.2f));
        sequence.Append(CanvasTransform.DOScale(new Vector3(openedScale, openedScale, 1), 0.2f));
    }

    /// <summary>
    /// ポップアップをアニメーションして閉じる
    /// </summary>
    public void Close(float openedScale = 0.9f,Action action = null)
    {
        var CanvasAlpha = PopupCanvas.GetComponent<CanvasGroup>();
        var CanvasTransform = PopupCanvas.GetComponent<RectTransform>();

        var sequence = DOTween.Sequence();
        sequence.SetUpdate(true);
        sequence.Append(CanvasAlpha.DOFade(0, 0.1f));
        sequence.Append(CanvasTransform.DOScale(new Vector3(openedScale, openedScale, 1), 0.1f));
        sequence.Append(BG.DOFade(0, 0.2f));
        sequence.AppendCallback(() =>
        {
            PopupObj.SetActive(false);
            action?.Invoke();
        });
    }
    /// <summary>
    /// ポップアップを閉じる
    /// </summary>
    public void Disable()
    {
        PopupObj.SetActive(false);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using DG.Tweening;

public enum ErrorType
{
    None,
    Disconnection,
    ConnectionBad,
    NeedUpdate,
    NGWordCaution,
}

/// <summary>
/// エラーを表示するためのポップアップ
/// </summary>
public class ErrorPopup : MonoBehaviour
{
    [SerializeField] GameObject _gameObject;
    [SerializeField] CanvasGroup _bgCanvasGroup;
    [SerializeField] GameObject _container;
    [SerializeField] CanvasGroup _containerCg;

    [Space]
    [SerializeField] TextMeshProUGUI _titleText;
    [SerializeField] Button _okButton;

    private static ErrorPopup _errorPopup;
    public static ErrorPopup Error => _errorPopup;

    private void Start()
    {
        _errorPopup = GetComponent<ErrorPopup>();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Popupを開く。引数にエラーの種類を設定
    /// </summary>
    public void Open(ErrorType error, Action onClick = null)
    {
        SetErrorType(error, onClick);
        OpenPopupAnimation();
    }

    /// <summary>
    /// Popupを閉じる
    /// </summary>
    public void Close(Action action = null)
    {
        var sequence = DOTween.Sequence();
        sequence.Append(_containerCg.DOFade(0, 0.1f));
        sequence.Append(_container.transform.DOScale(new Vector3(0.9f, 0.9f, 1), 0.1f));
        sequence.Append(_bgCanvasGroup.DOFade(0, 0.2f));
        sequence.AppendCallback(() =>
        {
            _gameObject.SetActive(false);
            action?.Invoke();
        });
    }

    /// <summary>
    /// エラーの種類を設定
    /// </summary>
    private void SetErrorType(ErrorType error,Action onClick = null)
    {
        switch (error)
        {
            case ErrorType.None:
                _titleText.text = "NONE";
                ButtonExtentions.OnButtonPressed(_okButton,() => Close());
                break;
            case ErrorType.Disconnection:
                _titleText.text = "通信が切断されました";
                ButtonExtentions.OnButtonPressed(_okButton, () =>
                {
                    onClick?.Invoke();
                    Close();
                });
                break;
            case ErrorType.ConnectionBad:
                _titleText.text = "通信環境を確認してください";
                ButtonExtentions.OnButtonPressed(_okButton, () =>
                {
                    Close();
                });
                break;
            case ErrorType.NeedUpdate:
                _titleText.text = "アップデートが必要です";
                ButtonExtentions.OnButtonPressed(_okButton, () =>
                {
                    var url = ResoucesData.GetCreditData().AppURL;
                    Application.OpenURL(url);
                });
                break;
            case ErrorType.NGWordCaution:
                _titleText.text = "他の人が不快に思う言葉が　使われている可能性があります";
                ButtonExtentions.OnButtonPressed(_okButton, () =>
                {
                    onClick?.Invoke();
                    Close();
                });
                break;
        }
    }


    //--------------アニメーション------------
    private void OpenPopupAnimation(Action action = null)
    {
        _gameObject.SetActive(true);
        _bgCanvasGroup.alpha = 0;
        _containerCg.alpha = 0;
        _container.transform.DOScale(new Vector3(0.9f, 0.9f), 0);

        var sequence = DOTween.Sequence();
        sequence.Append(_bgCanvasGroup.DOFade(0.8f, 0.2f));
        sequence.Append(_containerCg.DOFade(1, 0.2f));
        sequence.Append(_container.transform.DOScale(new Vector3(1, 1, 1), 0.2f));
        sequence.AppendCallback(() =>
        {
            action?.Invoke();
        });
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class TextUIAnimation : MonoBehaviour
{
    [SerializeField] RectTransform gameUITextRect;
    [SerializeField] RectTransform bgRect;
    [SerializeField] RectTransform textRect;

    [SerializeField] TextMeshProUGUI text;

    [SerializeField] Canvas _canvas;

    private static RectTransform _rect;
    private static RectTransform _bgrect;
    private static RectTransform _textrect;
    private static TextMeshProUGUI _text;

    private static GameObject _gameObject;


    private void Awake()
    {
        _rect = gameUITextRect;
        _bgrect = bgRect;
        _textrect = textRect;
        _text = text;
        _gameObject = gameObject;

        var width = _rect.rect.width;
        var height = 100;

        var textpos = (width / 2) + (_textrect.sizeDelta.x / 2);

        //Init
        _bgrect.sizeDelta = new Vector2(width, height);
        _bgrect.DOScale(new Vector3(1, 0), 0);
        _textrect.DOAnchorPos(new Vector3(-textpos, 0), 0);

        _gameObject.SetActive(false);

        _canvas.enabled = true;
    }

    [SerializeField] float bgtime = 0.25f;
    [SerializeField] float time = 0.5f;

    /// <summary>
    /// 左からテキストを表示させる
    /// </summary>
    public static void InGameTextAnimation(string text, Action action = null)
    {
        float bgtime = 0.25f;
        float time = 0.5f;

        //Set
        var width = _rect.rect.width;
        var height = 100;

        var textpos = (width / 2) + (_textrect.sizeDelta.x / 2);
        _text.text = text;

        //Init
        _bgrect.sizeDelta = new Vector2(width, height);
        _bgrect.DOScale(new Vector3(1, 0), 0);
        _textrect.DOAnchorPos(new Vector3(-textpos, 0), 0);

        _gameObject.SetActive(true);

        //Play
        var sq = DOTween.Sequence();

        sq.Append(_bgrect.DOScale(new Vector3(1, 1), bgtime));
        sq.Append(_textrect.DOAnchorPos(new Vector3(0, 0), time).SetEase(Ease.OutCubic));

        sq.AppendInterval(1f);

        sq.Append(_textrect.DOAnchorPos(new Vector3(textpos, 0), time).SetEase(Ease.OutCubic));
        sq.Append(_bgrect.DOScale(new Vector3(1, 0), bgtime));

        sq.AppendCallback(() =>
        {
            _gameObject.SetActive(false);
            action?.Invoke();
        });
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : Singleton<FadeManager>
{
    public bool IsStartFade = true;

    [Header("Loading")]

    [SerializeField] GameObject _LoadingObj;
    [SerializeField] Image _loadingImage;
    [SerializeField] Image _cogInSide;
    [SerializeField] GameObject _loadingCircleObj;

    private CanvasGroup _canvasGroup;
    private Sequence _loadingSequence;
    
    private void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        if (IsStartFade)
        {
            _canvasGroup.alpha = 1;
            gameObject.SetActive(true);
        }
        else
        {
            _canvasGroup.alpha = 0;
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// ローディングのアニメーションを実行する
    /// </summary>
    public void LoadingAnimationPlay()
    {
        _LoadingObj.SetActive(true);

        _loadingSequence = DOTween.Sequence();
        _loadingSequence.Append(_loadingImage.transform.DOLocalRotate(new Vector3(0, 0, -360), 1,
            RotateMode.FastBeyond360))
            .SetEase(Ease.Linear)
            .SetLoops(-1,LoopType.Restart);

        _loadingSequence.Join(_cogInSide.transform.DOLocalRotate(new Vector3(0, 0, 360), 1,
            RotateMode.FastBeyond360))
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);

        _loadingSequence.Join(DotweenAnimations.LoadingCircleAnimation(_loadingCircleObj));
    }

    /// <summary>
    /// ローディングのアニメーションを終了
    /// </summary>
    public void LoadingAnimationEnd()
    {
        _loadingSequence.Kill();
        _LoadingObj.SetActive(false);
    }

    /// <summary>
    /// フェード終了時実行
    /// </summary>
    public void FadeEndAction(Action action, float animatetime = 0.24f)
    {
        _canvasGroup.alpha = 0;
        gameObject.SetActive(true);

        var sequence = DOTween.Sequence();

        sequence.Append(_canvasGroup.DOFade(endValue: 1, duration: animatetime));

        sequence.Append(_canvasGroup.DOFade(0, animatetime));
        sequence.AppendCallback(() =>
        {
            gameObject.SetActive(false);
            action?.Invoke();
        });
    }
    /// <summary>
    /// フェード中に実行
    /// </summary>
    public void FadeWhileAction(Action action, float animatetime = 0.25f)
    {
        _canvasGroup.alpha = 0;
        gameObject.SetActive(true);

        var sequence = DOTween.Sequence();
        sequence.Append(_canvasGroup.DOFade(endValue: 1, duration: animatetime));

        sequence.AppendCallback(() =>
        {
            action?.Invoke();
        });

        sequence.Append(_canvasGroup.DOFade(0, animatetime));
        sequence.AppendCallback(() => gameObject.SetActive(false));
    }

    /// <summary>
    /// Fade開始、終了に実行
    /// </summary>
    public void FadeWhileEndAction(Action whileAction, Action endAction, float animatetime = 0.24f)
    {
        _canvasGroup.alpha = 0;
        gameObject.SetActive(true);

        var sequence = DOTween.Sequence();

        sequence.Append(_canvasGroup.DOFade(endValue: 1, duration: animatetime));
        sequence.AppendCallback(() =>
        {
            whileAction?.Invoke();
        });
        sequence.AppendInterval(0.5f);
        sequence.Append(_canvasGroup.DOFade(0, animatetime));
        sequence.AppendCallback(() =>
        {
            gameObject.SetActive(false);
            endAction?.Invoke();
        });
    }

    /// <summary>
    /// フェードイン
    /// </summary>
    public void FadeIn(Action action = null, float animatetime = 0.24f, bool isloading = false)
    {
        _canvasGroup.alpha = 0;
        gameObject.SetActive(true);
        if (isloading)
        {
            LoadingAnimationPlay();
        }

        _canvasGroup.DOFade(endValue: 1, duration: animatetime)
            .OnComplete(() =>
            {
                action?.Invoke();
            });
    }

    /// <summary>
    /// フェードアウト
    /// </summary>
    public void FadeOut(Action action = null, float animatetime = 0.24f)
    {
        gameObject.SetActive(true);
        _canvasGroup.alpha = 1;

        _canvasGroup.DOFade(endValue: 0, duration: animatetime)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                if(_loadingSequence != null)
                {
                    LoadingAnimationEnd();
                }

                action?.Invoke();
            });
    }
}
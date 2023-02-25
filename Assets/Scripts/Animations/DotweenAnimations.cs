using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// UIアニメーション
/// </summary>
public class DotweenAnimations : MonoBehaviour
{
    /// <summary>
    /// ロードアニメーション
    /// </summary>
    public static Tween LoadingCircleAnimation(GameObject imagesObj)
    {
        Image[] circles = imagesObj.GetComponentsInChildren<Image>();
        float DURATION = 0.75f;
        Sequence sequence = DOTween.Sequence();

        for (var i = 0; i < circles.Length; i++)
        {
            circles[i].rectTransform.anchoredPosition = new Vector2((i - circles.Length / 2) * 8f, 0);
            sequence
                .SetLoops(-1, LoopType.Restart)
                .SetDelay((DURATION / 2) * ((float)i / circles.Length))
                .Append(circles[i].rectTransform.DOAnchorPosY(5f, DURATION / 4))
                .Append(circles[i].rectTransform.DOAnchorPosY(0f, DURATION / 4))
                .AppendInterval((DURATION / 2) * ((float)(1 - i) / circles.Length));

            sequence.Play();
        }
        return sequence;
    }

    //------------メイン画面--------------

    /// <summary>
    /// タイトルのアニメーション
    /// </summary>
    public static Tween LogoAnimation(RectTransform rect, float scale = 0.1f)
    {
        var tween = rect.DOScale(new Vector3(scale, scale), 2)
                .SetRelative()
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.Linear)
                .OnKill(() =>
                {
                    rect.DOScale(new Vector3(1, 1, 1), 0);
                });

        return tween;
    }

    /// <summary>
    /// windowを表示
    /// </summary>
    public static void WindowOpenAnimation(GameObject close, GameObject open, float time = 0.3f)
    {
        open.SetActive(true);

        var closet = close.GetComponent<RectTransform>();
        var opent = open.GetComponent<RectTransform>();

        var width = closet.rect.width;

        closet.DOAnchorPos(new Vector2(0, 0), 0);
        opent.DOAnchorPos(new Vector2(width, 0), 0);

        var sq = DOTween.Sequence();

        sq.Append(opent.DOAnchorPos(new Vector2(0, 0), time))
            .SetEase(Ease.OutQuad);
        sq.Join(closet.DOAnchorPos(new Vector2(-width / 2, 0), time));
    }

    /// <summary>
    /// windowを非表示
    /// </summary>
    public static void WindowCloseAnimation(GameObject closed, GameObject opend, float time = 0.3f)
    {
        var closedt = closed.GetComponent<RectTransform>();
        var opendt = opend.GetComponent<RectTransform>();

        var width = closedt.rect.width;

        closedt.DOAnchorPos(new Vector2(-width / 2, 0), 0);
        opendt.DOAnchorPos(new Vector2(0, 0), 0);

        var sq = DOTween.Sequence();

        sq.Append(opendt.DOAnchorPos(new Vector2(width, 0), time))
            .SetEase(Ease.OutQuad);
        sq.Join(closedt.DOAnchorPos(new Vector2(0, 0), time));
        sq.AppendCallback(() => opend.SetActive(false));
    }

    //------------ゲーム画面-------------

    /// <summary>
    /// Imageの回転を初期化
    /// </summary>
    public static void InitImageRotate(List<Image> images)
    {
        for (int i = 0; i < images.Count; i++)
        {
            images[i].transform.DOScale(new Vector3(1, 0, 1), 0f);
            images[i].gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// オブジェクトの位置を初期化
    /// </summary>
    public static void InitObjectPosition(List<GameObject> gameObjects)
    {
        for (int i = 0; i < gameObjects.Count; i++)
        {
            int n = i;
            gameObjects[n].transform.DOScale(new Vector3(0, 0), 0);
            gameObjects[n].transform.DORotate(new Vector3(0, 0, -60), 0);
            gameObjects[n].SetActive(true);
        }
    }

    /// <summary>
    /// 順番にImageを縦方向に回転させる
    /// </summary>
    public static Tween RotateImageAnimation(List<Image> images, float rotatetime = 0.02f, float delay = 0.04f)
    {
        Sequence sequence = DOTween.Sequence();
        for (int i = 0; i < images.Count; i++)
        {
            sequence
                .Append(images[i].transform.DOScale(new Vector3(1, 1, 1), rotatetime))
                .Append(images[i].transform.DOScale(new Vector3(1, 0, 1), rotatetime))
                .Append(images[i].transform.DOScale(new Vector3(1, 1, 1), rotatetime));
        }
        return sequence;
    }

    /// <summary>
    /// どうぶつボタンを出現させる
    /// </summary>
    public static Tween ButtonAppearAnimation(List<GameObject> gameObjects, float time = 0.2f, float delay = 0.1f)
    {
        var sequence = DOTween.Sequence();
        for (int i = 0; i < gameObjects.Count; i++)
        {
            var s = DOTween.Sequence();
            int n = i;

            s.AppendInterval(delay * i);

            s.Append(gameObjects[n].transform.DOScale(new Vector3(1, 1), time));
            s.Join(gameObjects[n].transform.DORotate(new Vector3(0, 0, 0), time, RotateMode.FastBeyond360));
            s.AppendInterval(delay);

            s.OnComplete(() => SoundManager.Instance.PlayAudio(AudioType.ButtonAppear));
            sequence.Join(s);
        }

        return sequence;
    }

    //----------------Result画面-------------------

    /// <summary>
    /// clear の表示
    /// </summary>
    public static void ClearAnimation(GameObject obj, Action action = null)
    {
        var line = obj.transform.Find("Lines").transform;
        var texts = obj.transform.Find("Texts").transform;
        var cg = obj.transform.Find("Texts").GetComponent<CanvasGroup>();

        cg.DOFade(0.2f, 0);
        line.DOScale(new Vector3(0, 1, 1), 0);
        texts.DOScale(new Vector3(2, 2, 1), 0);

        var sq = DOTween.Sequence();
        sq.Append(cg.DOFade(1, 0.35f));
        sq.Join(texts.DOScale(new Vector3(1, 1, 1), 0.35f));
        sq.Insert(0.25f, line.DOScale(new Vector3(1, 1, 1), 0.15f));

        sq.AppendCallback(() =>
        {
            action?.Invoke();
        });
    }

    /// <summary>
    /// オブジェクトをanchorPosに移動させる
    /// </summary>
    public static void MoveObject(GameObject obj, Vector2 anchorPos, float time = 0.3f)
    {
        obj.GetComponent<RectTransform>().DOAnchorPos(anchorPos, time);
    }

    //------------------Pause---------------------

    /// <summary>
    /// ポーズ画面を表示
    /// </summary>
    public static void PauseOpenAnimation(GameObject obj1, GameObject obj2)
    {
        var cg1 = obj1.GetComponent<CanvasGroup>();
        var cg2 = obj2.GetComponent<CanvasGroup>();

        var rect1 = obj1.GetComponent<RectTransform>();
        var rect2 = obj2.GetComponent<RectTransform>();

        var text1 = obj1.transform.Find("text").GetComponent<TextMeshProUGUI>();
        var text2 = obj2.transform.Find("text").GetComponent<TextMeshProUGUI>();

        var textrect1 = text1.GetComponent<RectTransform>();
        var textrect2= text2.GetComponent<RectTransform>();

        obj1.SetActive(true);
        cg1.alpha = 0;
        rect1.DOAnchorPos(new Vector2(40, -40), 0);
        text1.color = new Color(1, 1, 1, 0);
        text1.gameObject.SetActive(false);
        textrect1.DOAnchorPos(new Vector2(100, 0),0);

        obj2.SetActive(true);
        cg2.alpha = 0;
        rect2.DOAnchorPos(new Vector2(40, -40), 0);
        text2.color = new Color(1, 1, 1, 0);
        text2.gameObject.SetActive(false);
        textrect2.DOAnchorPos(new Vector2(100, 0), 0);

        var sq = DOTween.Sequence();

        var time1 = 0.4f;

        sq.Append(cg1.DOFade(1, time1));
        sq.Join(rect1.DOAnchorPos(new Vector2(40, -110), time1));

        sq.Join(cg2.DOFade(1, time1));
        sq.Join(rect2.DOAnchorPos(new Vector2(40, -170), time1));

        sq.AppendCallback(() =>
        {
            text1.gameObject.SetActive(true);
            text2.gameObject.SetActive(true);
        });

        var time2 = 0.4f;

        sq.Append(text1.DOColor(new Color(1, 1, 1, 1), time2));
        sq.Join(textrect1.DOAnchorPos(new Vector2(130, 0), time2));

        sq.Join(text2.DOColor(new Color(1, 1, 1, 1), time2));
        sq.Join(textrect2.DOAnchorPos(new Vector2(130, 0), time2));
    }

    /// <summary>
    /// ポーズ画面を非表示
    /// </summary>
    public static void PauseCloseAnimation(GameObject obj1, GameObject obj2,Action action = null)
    {
        var cg1 = obj1.GetComponent<CanvasGroup>();
        var cg2 = obj2.GetComponent<CanvasGroup>();

        var rect1 = obj1.GetComponent<RectTransform>();
        var rect2 = obj2.GetComponent<RectTransform>();

        var text1 = obj1.transform.Find("text").GetComponent<TextMeshProUGUI>();
        var text2 = obj2.transform.Find("text").GetComponent<TextMeshProUGUI>();

        var textrect1 = text1.GetComponent<RectTransform>();
        var textrect2 = text2.GetComponent<RectTransform>();

        var sq = DOTween.Sequence();

        var time1 = 0.3f;

        sq.Append(text1.DOColor(new Color(1, 1, 1, 0), time1));
        sq.Join(textrect1.DOAnchorPos(new Vector2(100, 0), time1));

        sq.Join(text2.DOColor(new Color(1, 1, 1, 0), time1));
        sq.Join(textrect2.DOAnchorPos(new Vector2(100, 0), time1));


        var time2 = 0.15f;

        sq.Append(cg1.DOFade(0, time2));
        sq.Join(rect1.DOAnchorPos(new Vector2(40, -40), time2));

        sq.Join(cg2.DOFade(0, time2));
        sq.Join(rect2.DOAnchorPos(new Vector2(40, -40), time2));

        sq.AppendCallback(() =>
        {
            text1.gameObject.SetActive(false);
            text2.gameObject.SetActive(false);
            action?.Invoke();
        });
    }

    //--------------Match画面--------------

    /// <summary>
    /// プロフィールを出現させる
    /// </summary>
    public static void MatchMakingProfile(GameObject obj, Action action = null)
    {
        obj.SetActive(true);

        var cg = obj.GetComponent<CanvasGroup>();
        var tr = obj.GetComponent<RectTransform>();

        cg.alpha = 0;
        var width = 400;

        var y = tr.anchoredPosition.y;
        tr.anchoredPosition = new Vector2(width, y);

        var sq = DOTween.Sequence();
        sq.Append(tr.DOAnchorPos(new Vector2(0, y), 0.3f));
        sq.Join(cg.DOFade(1, 0.3f));
        SoundManager.Instance.PlayAudio(AudioType.MatchPlayer);
        sq.AppendCallback(() =>
        {
            action?.Invoke();
        });
    }

    /// <summary>
    /// マッチング完了時のテキストアニメーション
    /// </summary>
    public static void MatchCompleteText(TextMeshProUGUI text, float time = 0.5f)
    {
        text.DOFade(0, 0);
        text.transform.DOScale(new Vector3(1.5f, 1.5f), 0);

        var sq = DOTween.Sequence();
        sq.Append(text.transform.DOScale(new Vector3(1, 1), time));
        sq.Join(text.DOFade(1, time));
    }
}

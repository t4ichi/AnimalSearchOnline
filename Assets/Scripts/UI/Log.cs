using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Log : MonoBehaviour
{
    [SerializeField] Image AnswerImage;
    [SerializeField] Image CounterImage;

    [SerializeField] Image[] _animalImages = new Image[3];
    [SerializeField] Image[] _scoreImages = new Image[3];

    [SerializeField] Sprite _hitImage;
    [SerializeField] Sprite _blowImage;
    [SerializeField] Sprite _noneImage;

    private AnimalDataList AnimaldataList => ResoucesData.GetAnimalDataList();
    private int _count;

    /// <summary>
    /// 初期化
    /// </summary>
    public void Init()
    {
        foreach (var a in _animalImages)
        {
            a.sprite = null;
        }

        foreach (var s in _scoreImages)
        {
            s.DOFade(0, 0);
        }
    }

    /// <summary>
    /// スコア表示を初期化
    /// </summary>
    public void InitSetScore(int hit, int blow)
    {
        _count = hit + blow;
        for (int i = 0; i < hit+blow; i++)
        {
            if (i < hit)
            {
                _scoreImages[i].sprite = _hitImage;
            }
            else
            {
                _scoreImages[i].sprite = _blowImage;
            }
            InitSetAnimation(_scoreImages[i]);
        }
    }
    /// <summary>
    /// どうぶつをセット
    /// </summary>
    public void SetAnimalImage(int index ,int animalid)
    {
        _animalImages[index].sprite = AnimaldataList.list[animalid].Texture;
    }

    /// <summary>
    /// 最後に追加したどうぶつを削除
    /// </summary>
    public void RemoveAnimalImage(int index)
    {
        _animalImages[index].sprite = _noneImage;
    }

    /// <summary>
    /// すべてのどうぶつを削除
    /// </summary>
    public void ResetAnimalImage()
    {
        foreach (var a in _animalImages)
        {
            a.sprite = _noneImage;
        }
    }

    //---------------アニメーション--------------

    public Tween SetScoreAnimation()
    {
        var sequence = DOTween.Sequence();

        for (int i = 0; i < _count; i++)
        {
            sequence.AppendCallback(() => SoundManager.Instance.PlayAudio(AudioType.Hit));
            sequence.Append(SetAnimation(_scoreImages[i]));
        }

        return sequence;
    }

    private void InitSetAnimation(Image image, float scale = 5f)
    {
        image.DOFade(0, 0);
        image.transform.DOScale(scale, 0);
    }


    private Tween SetAnimation(Image image,float time = 0.2f)
    {
        var sequence = DOTween.Sequence();

        sequence.Append(image.transform.DOScale(1, time));
        sequence.Join(image.DOFade(1, time));

        return sequence;
    }
}

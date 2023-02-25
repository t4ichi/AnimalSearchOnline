using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class SelectorManager : MonoBehaviour
{
    public List<Selector> selectors = new List<Selector>();
    public int currentIndex = 0;

    [Header("Buttons")]
    [SerializeField] Button _nextButton;
    [SerializeField] Button _backButton;

    [Header("Navigate")]
    [SerializeField] Image _navigateImage;
    [SerializeField] RectTransform _navigateField;
    [Space]
    [SerializeField] Color _ONColor = new Color(1,1,1);
    [SerializeField] Color _OFFColor = new Color(0.5f,0.5f,0.5f);

    List<Image> _navigateImages = new List<Image>();

    private void Start()
    {
        _nextButton.onClick.AddListener(() =>
        {
            if (currentIndex + 1 < selectors.Count)
            {
                NextButton();
            }
        });
        _backButton.onClick.AddListener(() =>
        {
            if(currentIndex - 1 >= 0)
            {
                BackButton();
            }
        });

        for(int i = 0;i < selectors.Count; i++)
        {
            var n = Instantiate(_navigateImage, _navigateField);
            _navigateImages.Add(n);
            n.color = _OFFColor;
        }

        Set(currentIndex);
    }

    /// <summary>
    /// 次のSelectorに遷移する
    /// </summary>
    public virtual void NextButton()
    {
        OffSet(currentIndex);
        currentIndex++;
        Set(currentIndex);
    }

    /// <summary>
    /// 前のSelectorに戻る
    /// </summary>
    public virtual void BackButton()
    {
        OffSet(currentIndex);
        currentIndex--;
        Set(currentIndex);
    }

    private void Set(int i)
    {
        selectors[i].Open();
        _navigateImages[i].color = _ONColor;
    }

    private void OffSet(int i)
    {
        selectors[i].Close();
        _navigateImages[i].color = _OFFColor;
    }
}

using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Selector4 : Selector
{
    [SerializeField] List<AnswerButton> _answerbuttonList;
    [SerializeField] Button _deleteButton;
    [SerializeField] Button _okButton;

    [SerializeField] Log _settingAnswer;

    public List<int> _answerData = new List<int>() {2,5,1};
    public List<int> _answerbuttonData = new List<int>() {0,1,2,3,4,5};
    public List<int> _setList;

    int _selectedIndex;

    public static readonly int _answerCount = 3;

    [SerializeField] Sprite _successImage;
    [SerializeField] Sprite _wrongImage;

    [SerializeField] Image _jedgeImage;

    private void Start()
    {
        InitAnimalButton();
        InitDeleteAndOKButton();
    }

    public void AnimalButton(int animalid)
    {
        SoundManager.Instance.PlayAudio(AudioType.AnimalClick);
        _setList.Add(animalid);
        _settingAnswer.SetAnimalImage(_selectedIndex, animalid);
        _selectedIndex++;
        OnSetlistChanged();

        StartCoroutine(ButtonPressInterval());
    }

    public void DeleteButton()
    {
        SoundManager.Instance.PlayAudio(AudioType.AnimalClick);
        _setList.RemoveAt(_setList.Count - 1);
        _settingAnswer.RemoveAnimalImage(_selectedIndex - 1);
        _selectedIndex--;
        OnSetlistChanged();

        StartCoroutine(DeleteButtonPressInterval());
    }

    public void OkButton()
    {
        _okButton.interactable = false;
        _deleteButton.interactable = false;

        AnswerButtonsInteractable(false);

        var sq = DOTween.Sequence();

        sq.AppendCallback(() =>
        {
            var score = GameManager.Score.JadgeScore(_setList, _answerData);
            int hit = score[0];
            int blow = score[1];
            Debug.Log("hit:" + hit + "blow:" + blow);

            if (hit == _answerCount)
            {
                Debug.Log("正解");
                SoundManager.Instance.PlayAudio(AudioType.Success);
                _jedgeImage.sprite = _successImage;
            }
            else
            {
                Debug.Log("不正解");
                SoundManager.Instance.PlayAudio(AudioType.Wrong);
                _jedgeImage.sprite = _wrongImage;
            }

            _jedgeImage.DOFade(0, 0);
            _jedgeImage.gameObject.SetActive(true);
        });
        sq.Append(_jedgeImage.DOFade(1, 0.3f));
        sq.AppendInterval(0.5f);
        sq.Append(_jedgeImage.DOFade(0, 0.3f))
            .OnComplete(() => _jedgeImage.gameObject.SetActive(false));

        sq.AppendCallback(() =>
        {
            _setList.Clear();
            _settingAnswer.ResetAnimalImage();
            AnswerButtonsInteractable(true);
            _selectedIndex = 0;
        });
    }

    private void InitAnimalButton()
    {
        for (int i = 0; i < _answerbuttonList.Count; i++)
        {
            int n = i;
            var animalData = _answerbuttonData[n];
            _answerbuttonList[n].Init(animalData);

            _answerbuttonList[n].Button.onClick.AddListener(() =>
            {
                if (_setList.Count < _answerData.Count)
                {
                    AnimalButton(animalData);
                }
            });
        }
    }
    private void InitDeleteAndOKButton()
    {
        ButtonExtentions.OnButtonPressed(_deleteButton, () =>
        {
            if (_setList.Count == 0) return;
            DeleteButton();
        });

        ButtonExtentions.OnButtonPressed(_okButton, () =>
        {
            if (_setList.Count < _answerCount) return;
            OkButton();
        });
    }

    private void OnSetlistChanged()
    {
        _okButton.interactable = (_setList.Count == _answerCount);
        _deleteButton.interactable = (_setList.Count != 0);
    }

    public void AnswerButtonsInteractable(bool isInteractable)
    {
        foreach (var b in _answerbuttonList)
        {
            b.Button.interactable = isInteractable;
        }
    }

    IEnumerator ButtonPressInterval()
    {
        AnswerButtonsInteractable(false);
        yield return new WaitForSeconds(0.2f);
        AnswerButtonsInteractable(true);
    }

    IEnumerator DeleteButtonPressInterval()
    {
        _deleteButton.interactable = false;
        yield return new WaitForSeconds(0.2f);

        if ((_setList.Count == 0)) yield break;
        _deleteButton.interactable = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerProfile : MonoBehaviour
{
    [SerializeField] Image _icon;
    [SerializeField] Image _stroke;
    [SerializeField] TextMeshProUGUI _name;
    [SerializeField] TextMeshProUGUI _recordText;

    private AnimalDataList animalDataList => ResoucesData.GetAnimalDataList();

    /// <summary>
    /// プレイヤーのプロフィールをセット
    /// </summary>
    public void SetPlayerProfile(string name, int icon, int win, Color color)
    {
        try
        {
            _name.text = name;
            _icon.sprite = animalDataList.list[icon].Texture;
            _recordText.text = $"{win}";
            _stroke.color = color;
        }
        catch
        {
            _name.text = "NO NAME";
            _icon.sprite = animalDataList.list[0].Texture;
            _recordText.text = $"{0}";
            _stroke.color = RankData._rank1;
        }
    }
}
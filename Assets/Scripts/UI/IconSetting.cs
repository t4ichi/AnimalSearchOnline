using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconSetting : MonoBehaviour
{
    //アイコンの枠の色
    [SerializeField] Image _stroke;

    //自分のランクに合わせて枠の色を変える
    private Color _iconColor => RankData.GetMyRank();

    private void OnEnable()
    {
        _stroke.color = _iconColor;
    }

    public void ChengeIconColor()
    {
        _stroke.color = _iconColor;
    }
}

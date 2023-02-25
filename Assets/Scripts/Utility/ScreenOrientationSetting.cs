using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenOrientationSetting : MonoBehaviour
{
    private void Start()
    {
        //iPad なら回転をオンにする
        bool isipad = IsIPad;
        Screen.autorotateToLandscapeLeft = isipad;
        Screen.autorotateToLandscapeRight = isipad;
    }

    /// <summary>
    /// 現在の機種がiPadか判定
    /// </summary>
    public static bool IsIPad
    {
        get
        {
            Debug.Log("IsiPad:" + SystemInfo.deviceModel.Contains("iPad"));
            return SystemInfo.deviceModel.Contains("iPad");
        }
    }
}

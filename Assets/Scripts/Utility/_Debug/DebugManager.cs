using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugManager : MonoBehaviour
{
    [SerializeField] Button _debugButton;
    [SerializeField] GameObject _inspectorCanvas;

    private void Start()
    {
        if(_debugButton != null)
            _debugButton.onClick.AddListener(() => DebugButton());
    }

    /// <summary>
    /// ゲーム内でインスペクターを表示
    /// </summary>
    private void DebugButton()
    {
        if(_inspectorCanvas != null)
            _inspectorCanvas.SetActive(!_inspectorCanvas.activeSelf);
    }
}

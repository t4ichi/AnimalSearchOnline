using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// windowNameから呼び出しが可能なUIの抽象クラス
/// </summary>
public abstract class Window : MonoBehaviour
{
    public string windowName = "My Window";
    [SerializeField] bool _useAnimation = false;

    private Canvas _canvas;

    protected virtual void Awake()
    {
        _canvas = GetComponent<Canvas>();
    }
    public virtual void SetEnable()
    {
        _canvas.enabled = true;
        if (!_useAnimation) return;
    }

    public virtual void SetDisable()
    {
        DisableCanvas();
    }

    public void DisableCanvas()
    {
        _canvas.enabled = false;
    }
}

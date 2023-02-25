using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class WindowManager : MonoBehaviour
{
    public List<Window> windows = new List<Window>();

    private Hashtable _windowTable = new Hashtable();
    private Stack<Window> _windowStack = new Stack<Window>();

    public int currentWindowIndex = 0;
    public string GetCurrentWindow => _currentWindow;

    private string _currentWindow;

    private void Start()
    {
        RegistartAllWindows();
    }

    /// <summary>
    /// ウインドウを切り替える
    /// </summary>
    public void SwitchWindow(string name)
    {
        CloseWindow();
        OpenWindow(name);
    }

    /// <summary>
    /// 指定したウインドウを開く
    /// </summary>
    public void OpenWindow(string name)
    {
        if (!WindowExist(name))
        {
            Debug.LogWarning($"登録されていないウインドウ : {name}");
            return;
        }

        var window = GetWindow(name);
        window.SetEnable();
        _windowStack.Push(window);
        _currentWindow = window.windowName;
    }

    /// <summary>
    /// 現在のウインドウを閉じる
    /// </summary>
    public void CloseWindow()
    {
        if (_windowStack.Count <= 0) return;

        var lastStack =  _windowStack.Pop();
        lastStack.SetDisable();

        if (_windowStack.Count > 0)
            _currentWindow = _windowStack.Peek().windowName;

        if (_windowStack.Count <= 0)
            _currentWindow = "";
    }

    /// <summary>
    /// 指定したウインドウを閉じる
    /// </summary>
    public void CloseWindow(string name)
    {
        var window = GetWindow(name);
        window.DisableCanvas();
        window.SetDisable();
    }

    #region private function
    private void RegistartAllWindows()
    {
        foreach(var w in windows)
        {
            RegisterWindow(w);
            w.DisableCanvas();
        }
    }

    private void RegisterWindow(Window window)
    {
        if (WindowExist(window.windowName)) return;
        _windowTable.Add(window.windowName, window);
    }

    private Window GetWindow(string name)
    {
        if (!WindowExist(name)) return null;
        return (Window)_windowTable[name];
    }

    private bool WindowExist(string name)
    {
        return _windowTable.ContainsKey(name);
    }
    #endregion
}
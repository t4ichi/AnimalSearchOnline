using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
/// <summary>
/// MenuTypeから呼び出しができるUIの抽象クラス
/// </summary>
public abstract class Menu : MonoBehaviour
{
    [SerializeField] MenuType _type;

    public MenuType Type => _type;
    private Canvas _canvas;

    protected virtual void Awake()
    {
        _canvas = GetComponent<Canvas>();
    }

    public virtual void SetEnable()
    {
        EnableCanvas();
    }

    public virtual void SetDisable()
    {
        DisableCanvas();
    }

    public void EnableCanvas()
    {
        _canvas.enabled = true;
    }

    public void DisableCanvas()
    {
        _canvas.enabled = false;
    }

    /// <summary>
    /// checkBarrage : ボタンの連打を防止する
    /// </summary>
    protected void OnButtonPressed(Button button, UnityAction buttonListener,bool checkBarrage = false)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            if (checkBarrage)
            {
                button.interactable = false;
                StartCoroutine(EnableButton(button));
            }
            buttonListener();
        });
    }

    private IEnumerator EnableButton(Button button)
    {
        // 1秒後に解除         
        yield return new WaitForSeconds(1);
        button.interactable = true;
    }
}
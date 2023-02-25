using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ポーズ画面
/// </summary>
public class PauseWindow : Window
{
    [SerializeField] Button _hintbutton;
    [SerializeField] Button _exitbutton;

    [SerializeField] GameObject _hintObj;
    [SerializeField] GameObject _exitObj;

    private void Start()
    {
        ButtonExtentions.OnButtonPressed(_hintbutton, HintButtonListener);
        ButtonExtentions.OnButtonPressed(_exitbutton, ExitButtonListener);
    }

    public override void SetEnable()
    {
        base.SetEnable();
        DotweenAnimations.PauseOpenAnimation(_hintObj, _exitObj);
    }

    /// <summary>
    /// ポーズからゲームに戻る
    /// </summary>
    public void BackToGame()
    {
        DotweenAnimations.PauseCloseAnimation(_hintObj, _exitObj, () =>
        {
            GameManager.Window.CloseWindow("Pause");
        });
    }

    //------------------- ボタンリスナー ------------------------
    private void HintButtonListener()
    {
        DotweenAnimations.PauseCloseAnimation(_hintObj, _exitObj, () =>
        {
            GameManager.Window.CloseWindow("Pause");
            GameManager.Game.EndGame(GameEndState.GameOver);
        });
    }

    private void ExitButtonListener()
    {
        FadeManager.Instance.FadeWhileAction(() =>
        {
            GameManager.Window.CloseWindow("Pause");
            MenuManager.Instance.SwitchMenu(MenuType.Main);
        });
    }
}

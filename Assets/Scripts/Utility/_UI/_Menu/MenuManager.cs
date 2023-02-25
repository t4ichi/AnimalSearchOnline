using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : Singleton<MenuManager>
{
    [SerializeField]
    private List<Menu> _menus = new List<Menu>();
    private Hashtable _menuTable = new Hashtable();
    [SerializeField]private Stack<Menu> _menuStack = new Stack<Menu>();

    [SerializeField]private MenuType _currentMenu;
    
    public MenuType GetCurrentMenu => _currentMenu;

    private void Start()
    {
        RegisterAllMenus();
        OpenMenu(MenuType.Main);
    }

    /// <summary>
    /// メニューを切り替える
    /// </summary>
    public void SwitchMenu(MenuType type)
    {
        Debug.Log($"SwitchMenu : {type}");
        CloseMenu();
        Debug.Log("OpenMenu");
        OpenMenu(type);
    }

    /// <summary>
    /// 指定したメニューを開く
    /// </summary>
    public void OpenMenu(MenuType type)
    {
        //Debug.Log("OpenMenu");
        if (type == MenuType.None) return;
        if (!MenuExist(type))
        {
            Debug.LogWarning($"登録されていないメニュー : {type}");
            return;
        }

        Menu menu = GetMenu(type);
        menu.SetEnable();
        _menuStack.Push(menu);

        _currentMenu = menu.Type;
    }

    /// <summary>
    /// 現在のメニューを閉じる
    /// </summary>
    public void CloseMenu()
    {
        if (_menuStack.Count <= 0) return;
        Menu lastMenuStack = _menuStack.Pop();

        lastMenuStack.SetDisable();

        if (_menuStack.Count > 0)
            _currentMenu = _menuStack.Peek().Type;
    }

    #region private function
    private void RegisterAllMenus()
    {
        foreach (Menu menu in _menus)
        {
            RegisterMenu(menu);
            menu.DisableCanvas();
        }
    }

    private void RegisterMenu(Menu menu)
    {
        if (menu.Type == MenuType.None) return;
        if (MenuExist(menu.Type)) return;

        _menuTable.Add(menu.Type, menu);
    }

    public Menu GetMenu(MenuType type)
    {
        if (!MenuExist(type)) return null;

        return (Menu)_menuTable[type];
    }

    private bool MenuExist(MenuType type)
    {
        return _menuTable.ContainsKey(type);
    }
    #endregion
}
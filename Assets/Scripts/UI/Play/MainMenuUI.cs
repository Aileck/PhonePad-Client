using UnityEngine;
using UnityEngine.UI;
using static MenuOptions;

public class MainMenuUI : UIPanel
{
    public Button subMenu1Button;
    public Button subMenu2Button;
    public Button subMenu3Button;
    public Button backToGameButton;
    public UIStackManager stackManager;

    void Start()
    {
        if (subMenu1Button != null)
            subMenu1Button.onClick.AddListener(() => OpenSubMenu(SubMenuOption.Disconnect));
        if (subMenu2Button != null)
            subMenu2Button.onClick.AddListener(() => OpenSubMenu(SubMenuOption.EditLayout));
        if (subMenu3Button != null)
            subMenu3Button.onClick.AddListener(() => OpenSubMenu(SubMenuOption.EditButtons));
        if (backToGameButton != null)
            backToGameButton.onClick.AddListener(BackToGame);
    }

    public void OpenSubMenu(SubMenuOption option)
    {
        stackManager.PushSubMenu(option);
    }

    public void BackToGame()
    {
        stackManager.PopToPanel("Game");
    }

    public override void OnPanelOpen()
    {
    }

    public override void OnPanelClose()
    {
    }

    public override void OnPanelFocus()
    {
    }

    public override void OnPanelLoseFocus()
    {
    }
}
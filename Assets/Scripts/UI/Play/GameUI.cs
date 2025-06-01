using UnityEngine;
using UnityEngine.UI;

public class GameUI : UIPanel
{
    public Button openMainMenuButton;
    public UIStackManager stackManager;

    public GameObject gamepad;

    void Start()
    {
        if (openMainMenuButton != null)
        {
            openMainMenuButton.onClick.AddListener(OpenMainMenu);
        }
    }

    public void OpenMainMenu()
    {
        stackManager.PushPanel("MainMenu");
        gamepad.SetActive(false);
    }

    public override void OnPanelOpen()
    {
        gamepad.SetActive(true);
    }

    public override void OnPanelClose()
    {
    }

    public override void OnPanelFocus()
    {
        gamepad.SetActive(true);

    }

    public override void OnPanelLoseFocus()
    {

    }
}

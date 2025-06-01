using UnityEngine;
using UnityEngine.UI;
using static MenuOptions;

public class SubMenuUI : UIPanel
{
    public Button saveButton;
    public Button cancelButton;
    public UIStackManager stackManager;

    private SubMenuOption currentOption;

    void Start()
    {
        if (saveButton != null)
            saveButton.onClick.AddListener(SaveAndBack);
        if (cancelButton != null)
            cancelButton.onClick.AddListener(CancelAndBack);
    }

    public void SetMenuType(SubMenuOption option)
    {
        currentOption = option;
    }

    public SubMenuOption GetMenuType()
    {
        return currentOption;
    }

    public void SaveAndBack()
    {
        stackManager.PopPanel();
    }

    public void CancelAndBack()
    {
        stackManager.PopPanel();
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
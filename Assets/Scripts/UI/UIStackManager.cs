using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static MenuOptions;
public class UIPanel : MonoBehaviour
{
    public virtual void OnPanelOpen() { }
    public virtual void OnPanelClose() { }
    public virtual void OnPanelFocus() { }
    public virtual void OnPanelLoseFocus() { }
}

public class UIStackManager : MonoBehaviour
{
    public GameUI gamePanel;
    public MainMenuUI mainMenuPanel;
    public List<SubMenuUI> subMenuPanels;

    private Stack<UIPanel> uiStack = new Stack<UIPanel>();
    [SerializeField] private Dictionary<string, UIPanel> panelDict = new Dictionary<string, UIPanel>();

    void Awake()
    {
        InitializePanels();
    }

    void Start()
    {
        PushPanel("Game");
    }

    void InitializePanels()
    {
        if (gamePanel != null)
            panelDict["Game"] = gamePanel.GetComponent<UIPanel>();
        if (mainMenuPanel != null)
            panelDict["MainMenu"] = mainMenuPanel.GetComponent<UIPanel>();

        for (int i = 0; i < subMenuPanels.Count; i++)
        {
            if (subMenuPanels[i] != null)
            {
                string panelKey = "SubMenu" + i;
                panelDict[panelKey] = subMenuPanels[i].GetComponent<UIPanel>();
            }
        }

        foreach (var panel in panelDict.Values)
        {
            if (panel != null)
                panel.gameObject.SetActive(false);
        }
    }

    public void PushPanel(string panelName)
    {
        if (!panelDict.ContainsKey(panelName))
        {
            Debug.LogError("Panel " + panelName + " not found!");
            return;
        }

        if (uiStack.Count > 0)
        {
            var currentPanel = uiStack.Peek();
            currentPanel.OnPanelLoseFocus();
            currentPanel.gameObject.SetActive(false);
        }

        var newPanel = panelDict[panelName];
        uiStack.Push(newPanel);

        newPanel.gameObject.SetActive(true);
        newPanel.OnPanelOpen();
        newPanel.OnPanelFocus();
    }

    public void PushSubMenu(SubMenuOption option)
    {
        int index = (int)option;
        string panelName = "SubMenu" + index;

        if (panelDict.ContainsKey(panelName))
        {
            var subMenuPanel = panelDict[panelName].GetComponent<SubMenuUI>();
            if (subMenuPanel != null)
            {
                subMenuPanel.SetMenuType(option);
            }
            PushPanel(panelName);
        }
        else
        {
            Debug.LogError("SubMenu for option " + option + " not found!");
        }
    }

    public void PopPanel()
    {
        if (uiStack.Count <= 1)
        {
            return;
        }

        var currentPanel = uiStack.Pop();
        currentPanel.OnPanelLoseFocus();
        currentPanel.OnPanelClose();
        currentPanel.gameObject.SetActive(false);

        if (uiStack.Count > 0)
        {
            var previousPanel = uiStack.Peek();
            previousPanel.gameObject.SetActive(true);
            previousPanel.OnPanelFocus();
        }
    }

    public void PopToPanel(string panelName)
    {
        if (!panelDict.ContainsKey(panelName))
        {
            Debug.LogError("Panel " + panelName + " not found!");
            return;
        }

        var targetPanel = panelDict[panelName];

        while (uiStack.Count > 0 && uiStack.Peek() != targetPanel)
        {
            var panelToPop = uiStack.Pop();
            panelToPop.OnPanelLoseFocus();
            panelToPop.OnPanelClose();
            panelToPop.gameObject.SetActive(false);
        }

        if (uiStack.Count > 0)
        {
            var currentPanel = uiStack.Peek();
            currentPanel.gameObject.SetActive(true);
            currentPanel.OnPanelFocus();
        }
    }

    public UIPanel GetCurrentPanel()
    {
        return uiStack.Count > 0 ? uiStack.Peek() : null;
    }

    public int GetStackCount()
    {
        return uiStack.Count;
    }

    public Dictionary<string, UIPanel> GetPanelDict()
    {
        return panelDict;
    }
}
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static GamepadConfig;

public class ShowProfileButtons : MonoBehaviour
{
    [SerializeField] GameObject dualShockPanel;
    [SerializeField] GameObject xboxPanel;
    [SerializeField] private GamepadConfig gamepadConfig;

    private GameObject xboxButtonHolder;
    private GameObject dualShockButtonHolder;

    [SerializeField] private int xboxProfileIndex = 0;
    [SerializeField] private int playstationProfileIndex = 0;

    void Start()
    {
        InitializeHolders();
        DrawButtons();
        ProfileSelector.OnSelectionChanged += HandleSelectionChanged;
    }

    void OnDestroy()
    {
        ProfileSelector.OnSelectionChanged -= HandleSelectionChanged;
    }

    void InitializeHolders()
    {
        xboxButtonHolder = new GameObject("XboxButtonHolder");
        dualShockButtonHolder = new GameObject("DualShockButtonHolder");
        xboxButtonHolder.transform.SetParent(xboxPanel.transform, false);
        dualShockButtonHolder.transform.SetParent(dualShockPanel.transform, false);
    }

    void DrawButtons()
    {
        ClearExistingButtons();
        Debug.Log("Drawing buttons for profiles: " + xboxProfileIndex + ", " + playstationProfileIndex);
        DrawXboxButtons();
        DrawDualShockButtons();
    }

    void ClearExistingButtons()
    {
        if (xboxButtonHolder != null)
        {
            DestroyImmediate(xboxButtonHolder);
        }
        if (dualShockButtonHolder != null)
        {
            DestroyImmediate(dualShockButtonHolder);
        }
        InitializeHolders();
    }

    void DrawXboxButtons()
    {
        RectTransform xboxPanelRect = xboxPanel.GetComponent<RectTransform>();
        XboxProfile profile = gamepadConfig.GetXboxProfile(xboxProfileIndex);

        var buttonFields = typeof(XboxProfile).GetFields()
            .Where(field => field.FieldType == typeof(ButtonProfile))
            .ToArray();

        foreach (var field in buttonFields)
        {
            ButtonProfile button = (ButtonProfile)field.GetValue(profile);

            float posX = button.position.x * (xboxPanelRect.rect.width / 2);
            float posY = button.position.y * (xboxPanelRect.rect.height / 2);

            GameObject imageGO = new GameObject($"Button_{field.Name}", typeof(Image));
            imageGO.transform.SetParent(xboxButtonHolder.transform);

            Image image = imageGO.GetComponent<Image>();
            image.sprite = button.iconImage;
            image.rectTransform.anchoredPosition = new Vector2(posX, posY);
            image.rectTransform.localScale = new Vector3(0.4f, 0.4f, 1f);
        }
    }

    void DrawDualShockButtons()
    {
        RectTransform dualShockPanelRect = dualShockPanel.GetComponent<RectTransform>();
        DualShockProfile dualShockProfile = gamepadConfig.GetDualShockProfile(playstationProfileIndex);

        var dualShockButtonFields = typeof(DualShockProfile).GetFields()
            .Where(field => field.FieldType == typeof(ButtonProfile))
            .ToArray();

        foreach (var field in dualShockButtonFields)
        {
            ButtonProfile button = (ButtonProfile)field.GetValue(dualShockProfile);

            float posX = button.position.x * (dualShockPanelRect.rect.width / 2);
            float posY = button.position.y * (dualShockPanelRect.rect.height / 2);

            GameObject imageGO = new GameObject($"Button_{field.Name}", typeof(Image));
            imageGO.transform.SetParent(dualShockButtonHolder.transform);

            Image image = imageGO.GetComponent<Image>();
            image.sprite = button.iconImage;
            image.rectTransform.anchoredPosition = new Vector2(posX, posY);
            image.rectTransform.localScale = new Vector3(0.4f, 0.4f, 1f);
        }
    }

    void HandleSelectionChanged(int newXboxIndex, int newPlaystationIndex)
    {
        xboxProfileIndex = newXboxIndex;
        playstationProfileIndex = newPlaystationIndex;
        DrawButtons();
    }
}
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using static GamepadConfig;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

public class EditButtonManager : MonoBehaviour
{
    [System.Serializable]
    public class TableData
    {
        public Sprite image2D;        // Button icon image
        public bool toggle1;          // Is visible
        public bool toggle2;          // Swipe to activate
        public bool toggle3;          // Is toggle
    }

    private RectTransform scrollViewRect;
    private RectTransform tablePanel;
    [SerializeField] private float cellWidth = 300;
    [SerializeField] private float cellHeight = 75;
    [SerializeField] private float spacing = 5f;
    [SerializeField] private float viewportHeight = 1000f;
    [SerializeField] private int fontSize = 40;
    [SerializeField] private GamepadConfig gamepadConfig;

    private List<TableData> dataList = new List<TableData>();

    void OnEnable()
    {
        InitTable();
        InitializeTableData();

        // Create table
        CreateTable();
    }

    private void InitializeTableData()
    {
        dataList.Clear();

        // Add table header
        dataList.Add(new TableData
        {
            image2D = null,
            toggle1 = false,
            toggle2 = false,
            toggle3 = false
        });

        // Get current gamepad type
        GamepadType gamepadType = AppLifeTimeManager.Instance.GetSessionGamepad();
        int profileIndex = AppLifeTimeManager.Instance.GetSessionConfigProfileIndex();

        if (gamepadType == GamepadType.GAMEPAD_XBOX360)
        {
            XboxProfile profile = gamepadConfig.xboxProfiles[profileIndex];
            CreateTableFromProfile(profile);
        }
        else if (gamepadType == GamepadType.GAMEPAD_DUALSHOCK)
        {
            DualShockProfile profile = gamepadConfig.dualShockProfiles[profileIndex];
            CreateTableFromProfile(profile);
        }
    }

    private void CreateTableFromProfile<T>(T profile) where T : GamepadConfig.Profile
    {
        var buttonFields = profile.GetType().GetFields()
            .Where(field => field.FieldType == typeof(ButtonProfile))
            .ToArray();

        foreach (var field in buttonFields)
        {
            ButtonProfile buttonProfile = (ButtonProfile)field.GetValue(profile);
            if (buttonProfile != null)
            {
                dataList.Add(new TableData
                {
                    image2D = buttonProfile.iconImage,
                    toggle1 = buttonProfile.isVisible,
                    toggle2 = !buttonProfile.pressToActivate,
                    toggle3 = buttonProfile.toggle
                });
            }
        }
    }

    void Start()
    {
        CreateTable();
    }

    private void InitTable()
    {
        GameObject scrollViewObj = new GameObject("ScrollView");
        scrollViewObj.transform.SetParent(this.transform, false);
        scrollViewRect = scrollViewObj.AddComponent<RectTransform>();
        ScrollRect scrollRect = scrollViewObj.AddComponent<ScrollRect>();
        Image scrollViewImage = scrollViewObj.AddComponent<Image>();
        scrollViewImage.color = new Color(1, 1, 1, 0.1f);

        scrollViewRect.anchorMin = new Vector2(0.5f, 0.5f);
        scrollViewRect.anchorMax = new Vector2(0.5f, 0.5f);
        scrollViewRect.pivot = new Vector2(0.5f, 0.5f);
        scrollViewRect.anchoredPosition = Vector2.zero;

        GameObject viewportObj = new GameObject("Viewport");
        viewportObj.transform.SetParent(scrollViewRect, false);
        RectTransform viewportRect = viewportObj.AddComponent<RectTransform>();
        Image viewportImage = viewportObj.AddComponent<Image>();
        viewportImage.color = new Color(1, 1, 1, 0.1f);
        Mask viewportMask = viewportObj.AddComponent<Mask>();
        viewportMask.showMaskGraphic = true;

        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.sizeDelta = Vector2.zero;
        viewportRect.pivot = new Vector2(0.5f, 0.5f);
        viewportRect.offsetMin = Vector2.zero;
        viewportRect.offsetMax = Vector2.zero;

        GameObject contentObj = new GameObject("Content");
        contentObj.transform.SetParent(viewportRect, false);
        tablePanel = contentObj.AddComponent<RectTransform>();
        Image contentImage = contentObj.AddComponent<Image>();
        contentImage.color = new Color(1, 1, 1, 0.05f);
        contentImage.sprite = Resources.Load<Sprite>("Dark UI/Free/Button_Large_A");

        tablePanel.anchorMin = new Vector2(0, 1);
        tablePanel.anchorMax = new Vector2(1, 1);
        tablePanel.pivot = new Vector2(0.5f, 1);
        tablePanel.anchoredPosition = Vector2.zero;

        GameObject scrollbarObj = new GameObject("Scrollbar");
        scrollbarObj.transform.SetParent(scrollViewRect, false);
        Scrollbar scrollbar = scrollbarObj.AddComponent<Scrollbar>();
        Image scrollbarImage = scrollbarObj.AddComponent<Image>();

        GameObject slidingArea = new GameObject("Sliding Area");
        slidingArea.transform.SetParent(scrollbarObj.transform, false);
        RectTransform slidingAreaRect = slidingArea.AddComponent<RectTransform>();

        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(slidingArea.transform, false);
        Image handleImage = handle.AddComponent<Image>();
        RectTransform handleRect = handle.GetComponent<RectTransform>();

        RectTransform scrollbarRect = scrollbarObj.GetComponent<RectTransform>();
        scrollbarRect.anchorMin = new Vector2(1, 0);
        scrollbarRect.anchorMax = new Vector2(1, 1);
        scrollbarRect.pivot = new Vector2(1, 1);
        scrollbarRect.sizeDelta = new Vector2(20, 0);
        scrollbarRect.anchoredPosition = new Vector2(20, 0);

        slidingAreaRect.anchorMin = Vector2.zero;
        slidingAreaRect.anchorMax = Vector2.one;
        slidingAreaRect.sizeDelta = Vector2.zero;

        handleRect.anchorMin = Vector2.zero;
        handleRect.anchorMax = Vector2.one;
        handleRect.sizeDelta = Vector2.zero;

        scrollbar.handleRect = handleRect;
        scrollbar.targetGraphic = handleImage;

        scrollRect.content = tablePanel;
        scrollRect.viewport = viewportRect;
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.scrollSensitivity = 10;
        scrollRect.movementType = ScrollRect.MovementType.Elastic;
        scrollRect.elasticity = 0.1f;
        scrollRect.inertia = true;
        scrollRect.decelerationRate = 0.135f;
        scrollRect.verticalScrollbar = scrollbar;

        float totalWidth = (cellWidth * 4) + (spacing * 3);
        scrollViewRect.sizeDelta = new Vector2(totalWidth + 20, viewportHeight); 
    }
    private void CreateTable()
    {
        if (tablePanel == null) return;

        foreach (Transform child in tablePanel)
        {
            Destroy(child.gameObject);
        }

        float totalWidth = (cellWidth * 4) + (spacing * 3);
        float totalHeight = (dataList.Count * cellHeight) + ((dataList.Count - 1) * spacing);
        
        tablePanel.sizeDelta = new Vector2(0, totalHeight); 

        // Create header row
        CreateRow(0, dataList[0]);

        // Get current profile for button updates
        GamepadType gamepadType = AppLifeTimeManager.Instance.GetSessionGamepad();
        int profileIndex = AppLifeTimeManager.Instance.GetSessionConfigProfileIndex();
        var buttonFields = gamepadType == GamepadType.GAMEPAD_XBOX360 
            ? gamepadConfig.xboxProfiles[profileIndex].GetType().GetFields()
                .Where(field => field.FieldType == typeof(ButtonProfile))
                .ToArray()
            : gamepadConfig.dualShockProfiles[profileIndex].GetType().GetFields()
                .Where(field => field.FieldType == typeof(ButtonProfile))
                .ToArray();

        // Create data rows
        for (int i = 1; i < dataList.Count; i++)
        {
            var buttonProfile = (ButtonProfile)buttonFields[i - 1].GetValue(
                gamepadType == GamepadType.GAMEPAD_XBOX360 
                    ? gamepadConfig.xboxProfiles[profileIndex] 
                    : (object)gamepadConfig.dualShockProfiles[profileIndex]
            );
            CreateRow(i, dataList[i], buttonProfile);
        }

        tablePanel.anchoredPosition = new Vector2(0, 0);
    }

    private void CreateRow(int rowIndex, TableData data, ButtonProfile buttonProfile = null)
    {
        string keyText = i18nManager.Instance.Translate("play_button");
        GameObject imageCell = CreateCell(rowIndex, 0, keyText);
        Image imageComponent = imageCell.GetComponent<Image>();
        imageComponent.sprite = Resources.Load<Sprite>("Dark UI/Free/Button_Large_A");
        imageComponent.preserveAspect = true;

        if (data.image2D != null)
        {
            GameObject imageContainer = new GameObject("ImageContainer");
            imageContainer.transform.SetParent(imageCell.transform, false);
            Image centeredImage = imageContainer.AddComponent<Image>();
            centeredImage.sprite = data.image2D;
            centeredImage.preserveAspect = true;

            RectTransform containerRect = imageContainer.GetComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0.1f, 0.1f);
            containerRect.anchorMax = new Vector2(0.9f, 0.9f);
            containerRect.sizeDelta = Vector2.zero;
            containerRect.anchoredPosition = Vector2.zero;

            imageComponent.sprite = Resources.Load<Sprite>("Dark UI/Free/Button_Large_A");
        }

        string isVisibleText = i18nManager.Instance.Translate("play_is_visible");
        string swipeToTriggerText = i18nManager.Instance.Translate("play_swipe_to_trigger");
        string toggleTriggerText = i18nManager.Instance.Translate("play_toggle_to_trigger");

        if (buttonProfile != null)
        {
            CreateToggleCell(rowIndex, 1, isVisibleText, data.toggle1, buttonProfile, (value) => buttonProfile.isVisible = value);
            CreateToggleCell(rowIndex, 2, swipeToTriggerText, data.toggle2, buttonProfile, (value) => buttonProfile.pressToActivate = !value);
            CreateToggleCell(rowIndex, 3, toggleTriggerText, data.toggle3, buttonProfile, (value) => buttonProfile.toggle = value);
        }
        else
        {
            // Create header toggles without functionality
            CreateToggleCell(rowIndex, 1, isVisibleText, false);
            CreateToggleCell(rowIndex, 2, swipeToTriggerText, false);
            CreateToggleCell(rowIndex, 3, toggleTriggerText, false);
        }
    }

    private GameObject CreateCell(int rowIndex, int colIndex, string label)
    {
        GameObject cellObj = new GameObject($"Cell_{rowIndex}_{colIndex}");
        cellObj.transform.SetParent(tablePanel, false);

        Image cellImage = cellObj.AddComponent<Image>();
        cellImage.sprite = Resources.Load<Sprite>("Dark UI/Free/Button_Large_A");
        cellImage.color = rowIndex == 0 ? new Color(0.8f, 0.8f, 0.8f, 1) : new Color(1, 1, 1, 0.5f);

        if (rowIndex == 0)
        {
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(cellObj.transform, false);
            TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
            textComponent.fontSize = fontSize;
            textComponent.alignment = TextAlignmentOptions.Center;
            textComponent.color = Color.black;
            textComponent.text = label;

            RectTransform textRectTransform = textObj.GetComponent<RectTransform>();
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.sizeDelta = Vector2.zero;
            textRectTransform.anchoredPosition = Vector2.zero;
        }

        RectTransform rectTransform = cellObj.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.pivot = new Vector2(0, 1);
        
        float xPos = colIndex * (cellWidth + spacing);
        float yPos = -rowIndex * (cellHeight + spacing);
        rectTransform.anchoredPosition = new Vector2(xPos, yPos);
        rectTransform.sizeDelta = new Vector2(cellWidth, cellHeight);

        return cellObj;
    }

    private void CreateToggleCell(int rowIndex, int colIndex, string label, bool isOn, ButtonProfile buttonProfile = null, System.Action<bool> onValueChanged = null)
    {
        GameObject cellObj = CreateCell(rowIndex, colIndex, label);

        if (rowIndex > 0)
        {
            GameObject toggleObj = new GameObject("Toggle");
            toggleObj.transform.SetParent(cellObj.transform, false);
            Toggle toggle = toggleObj.AddComponent<Toggle>();

            GameObject background = new GameObject("Background");
            background.transform.SetParent(toggleObj.transform, false);
            Image backgroundImage = background.AddComponent<Image>();
            backgroundImage.sprite = Resources.Load<Sprite>("Dark UI/Free/32");
            backgroundImage.color = Color.white;

            GameObject checkmark = new GameObject("Checkmark");
            checkmark.transform.SetParent(background.transform, false);
            Image checkmarkImage = checkmark.AddComponent<Image>();
            checkmarkImage.color = Color.black;

            toggle.targetGraphic = backgroundImage;
            toggle.graphic = checkmarkImage;
            toggle.isOn = isOn;

            if (buttonProfile != null && onValueChanged != null)
            {
                toggle.onValueChanged.AddListener((bool value) => {
                    onValueChanged(value);
                    if (gamepadConfig != null)
                    {
                        #if UNITY_EDITOR
                        UnityEditor.EditorUtility.SetDirty(gamepadConfig);
                        #endif
                    }
                });
            }

            RectTransform toggleRect = toggleObj.GetComponent<RectTransform>();
            toggleRect.anchorMin = new Vector2(0.5f, 0.5f);
            toggleRect.anchorMax = new Vector2(0.5f, 0.5f);
            toggleRect.sizeDelta = new Vector2(40, 40);
            toggleRect.anchoredPosition = Vector2.zero;

            RectTransform backgroundRect = background.GetComponent<RectTransform>();
            backgroundRect.anchorMin = Vector2.zero;
            backgroundRect.anchorMax = Vector2.one;
            backgroundRect.sizeDelta = Vector2.zero;
            backgroundRect.anchoredPosition = Vector2.zero;

            RectTransform checkmarkRect = checkmark.GetComponent<RectTransform>();
            checkmarkRect.anchorMin = new Vector2(0.1f, 0.1f);
            checkmarkRect.anchorMax = new Vector2(0.9f, 0.9f);
            checkmarkRect.sizeDelta = Vector2.zero;
            checkmarkRect.anchoredPosition = Vector2.zero;
        }
    }

    void OnDisable()
    {
        if (tablePanel != null)
        {
            foreach (Transform child in tablePanel)
            {
                Destroy(child.gameObject);
            }
        }

        if (scrollViewRect != null)
        {
            Destroy(scrollViewRect.gameObject);
        }

        tablePanel = null;
        scrollViewRect = null;
    }

    public void Button_Save()
    {
        AppLifeTimeManager.Instance.SaveGamepadConfig();
    }
}
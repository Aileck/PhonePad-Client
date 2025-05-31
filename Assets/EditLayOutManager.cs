using System.Linq;
using UnityEngine;
using UnityEngine.UI;

using static GamepadConfig;

public class EditLayOutManager : MonoBehaviour
{
    [SerializeField] private GameObject layoutPanel;
    [SerializeField] private GamepadConfig gamepadConfig;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        // Set the layout panel to be active when this script is enabled
        if (layoutPanel != null)
        {
            layoutPanel.SetActive(true);

            PrintLayout();
        }
    }

    void OnDisable()
    {
        // Clean up and deactivate the layout panel when this script is disabled
        if (layoutPanel != null)
        {
            // Clear all dynamically created button images
            ClearLayout();

            // Set the layout panel to be inactive
            layoutPanel.SetActive(false);
        }
    }
    void ClearLayout()
    {
        if (layoutPanel == null) return;

        RectTransform panelRect = layoutPanel.GetComponent<RectTransform>();

        // Find and destroy all dynamically created button GameObjects
        for (int i = panelRect.childCount - 1; i >= 0; i--)
        {
            Transform child = panelRect.GetChild(i);

            // Check if this is a dynamically created button (starts with "Button_")
            if (child.name.StartsWith("Button_"))
            {
                // Use DestroyImmediate in edit mode, Destroy in play mode
                if (Application.isPlaying)
                {
                    Destroy(child.gameObject);
                }
                else
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }
    }

    void PrintLayout()
    {
        GamepadType gamepadType = AppLifeTimeManager.Instance.GetSessionGamepad();
        int profileIndex = AppLifeTimeManager.Instance.GetSessionConfigProfileIndex();

        RectTransform panelRect = layoutPanel.GetComponent<RectTransform>();

        if (gamepadType == GamepadType.GAMEPAD_XBOX360)
        {
            XboxProfile profile = gamepadConfig.xboxProfiles[profileIndex];

            var buttonFields = typeof(XboxProfile).GetFields()
                .Where(field => field.FieldType == typeof(ButtonProfile))
                .ToArray();

            foreach (var field in buttonFields)
            {
                ButtonProfile button = (ButtonProfile)field.GetValue(profile);

                Vector2 normalizedPos = new Vector2 (
                    Mathf.Clamp(button.position.x, -1f, 1f),
                    Mathf.Clamp(button.position.y, -1f, 1f)
                 );

                float posX = normalizedPos.x * (panelRect.rect.width / 2);
                float posY = normalizedPos.y * (panelRect.rect.height / 2);

                GameObject imageGO = new GameObject($"Button_{field.Name}", typeof(Image));
                imageGO.transform.SetParent(panelRect.transform);

                Image image = imageGO.GetComponent<Image>();
                image.sprite = button.iconImage;

                image.rectTransform.anchoredPosition = new Vector2(posX, posY);
                image.rectTransform.localScale = new Vector3(button.scale.x, button.scale.y, 1f);
            }
        }
        else if (gamepadType == GamepadType.GAMEPAD_DUALSHOCK)
        {
            DualShockProfile profile = gamepadConfig.dualShockProfiles[profileIndex];

            var dualShockButtonFields = typeof(DualShockProfile).GetFields()
                .Where(field => field.FieldType == typeof(ButtonProfile))
                .ToArray();

            foreach (var field in dualShockButtonFields)
            {
                ButtonProfile button = (ButtonProfile)field.GetValue(profile);

                Vector2 normalizedPos = new Vector2(
                    Mathf.Clamp(button.position.x, -1f, 1f),
                    Mathf.Clamp(button.position.y, -1f, 1f)
                 );

                float posX = normalizedPos.x * (panelRect.rect.width / 2);
                float posY = normalizedPos.y * (panelRect.rect.height / 2);

                GameObject imageGO = new GameObject($"Button_{field.Name}", typeof(Image));
                imageGO.transform.SetParent(panelRect.transform);

                Image image = imageGO.GetComponent<Image>();
                image.sprite = button.iconImage;

                EditableButton editableProperty = imageGO.AddComponent<EditableButton>();
                editableProperty.Initialice(button.name, layoutPanel.GetComponent<RectTransform>(), image);
                editableProperty.SetNormalizedPosition(normalizedPos);
                editableProperty.SetScale(button.scale);
            }
        }

    }

    public void Button_SaveLayout()
    {
        RectTransform panelRect = layoutPanel.GetComponent<RectTransform>();

        GamepadType gamepadType = AppLifeTimeManager.Instance.GetSessionGamepad();
        int profileIndex = 1;

        AppLifeTimeManager.Instance.SetSessionConfigProfileIndex(profileIndex);

        if (gamepadType == GamepadType.GAMEPAD_XBOX360)
        {
            XboxProfile profile = gamepadConfig.GetXboxProfile(profileIndex);

            for (int i = panelRect.childCount - 1; i >= 0; i--)
            {
                EditableButton editableButton = panelRect.GetChild(i).GetComponent<EditableButton>();

                profile.UpdateButtonPosition(
                    editableButton.GetButtonName(),
                    editableButton.GetNormalizedPosition()
                );

                profile.UpdateButtonScale(
                    editableButton.GetButtonName(),
                    editableButton.GetScale()
                );
            }

            this.gameObject.SetActive(false);
        }
        else if (gamepadType == GamepadType.GAMEPAD_DUALSHOCK)
        {
            DualShockProfile profile = gamepadConfig.GetDualShockProfile(profileIndex);

            for (int i = panelRect.childCount - 1; i >= 0; i--)
            {
                EditableButton editableButton = panelRect.GetChild(i).GetComponent<EditableButton>();

                if (editableButton == null)
                {
                    continue; // Skip if the button is not editable
                }
                profile.UpdateButtonPosition(
                    editableButton.GetButtonName(),
                    editableButton.GetNormalizedPosition()
                );

                profile.UpdateButtonScale(
                    editableButton.GetButtonName(),
                    editableButton.GetScale()
                );
            }

            this.gameObject.SetActive(false);
        }
        // Optionally, you can provide feedback to the user that the layout has been saved
    }

    public void Button_CancelEdit()
    {
        this.gameObject.SetActive(false);
    }
}

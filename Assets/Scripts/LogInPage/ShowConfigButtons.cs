using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static GamepadConfig;

public class ShowConfigButtons : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject dualShockPanel;
    [SerializeField] GameObject xboxPanel;
    [SerializeField] private GamepadConfig gamepadConfig;

    public Sprite sticker;
    void Start()
    {
        RectTransform dualShockPanelRect = dualShockPanel.GetComponent<RectTransform>();
        RectTransform xboxPanelRect = xboxPanel.GetComponent<RectTransform>();

        // Set the position of the xbox to the left side of the screen
        XboxProfile profile = gamepadConfig.xboxProfiles[0];

        var buttonFields = typeof(XboxProfile).GetFields()
            .Where(field => field.FieldType == typeof(ButtonProfile)) 
            .ToArray();

        foreach (var field in buttonFields)
        {
            ButtonProfile button = (ButtonProfile)field.GetValue(profile);

            float posX = button.position.x * (xboxPanelRect.rect.width / 2);
            float posY = button.position.y * (xboxPanelRect.rect.height / 2);

            GameObject imageGO = new GameObject($"Button_{field.Name}", typeof(Image));
            imageGO.transform.SetParent(xboxPanel.transform);

            Image image = imageGO.GetComponent<Image>();
            image.sprite = button.iconImage;

            image.rectTransform.anchoredPosition = new Vector2(posX, posY);
            image.rectTransform.localScale = new Vector3(0.4f, 0.4f, 1f);
        }

        // Set the position of the dualshock to the right side of the screen
        DualShockProfile dualShockProfile = gamepadConfig.dualShockProfiles[0];

        var dualShockButtonFields = typeof(DualShockProfile).GetFields()
            .Where(field => field.FieldType == typeof(ButtonProfile))
            .ToArray();

        foreach (var field in dualShockButtonFields)
        {
            ButtonProfile button = (ButtonProfile)field.GetValue(dualShockProfile);

            float posX = button.position.x * (dualShockPanelRect.rect.width / 2);
            float posY = button.position.y * (dualShockPanelRect.rect.height / 2);

            GameObject imageGO = new GameObject($"Button_{field.Name}", typeof(Image));

            imageGO.transform.SetParent(dualShockPanel.transform);

            Image image = imageGO.GetComponent<Image>();

            image.sprite = button.iconImage;
            image.rectTransform.anchoredPosition = new Vector2(posX, posY);
            image.rectTransform.localScale = new Vector3(0.4f, 0.4f, 1f);
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }


}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DPadComponent : MonoBehaviour
{
    [SerializeField] private RectTransform dPadBackground;

    [SerializeField] private RectTransform UpArrowSprite;
    [SerializeField] private RectTransform DownArrowSprite;
    [SerializeField] private RectTransform LeftArrowSprite;
    [SerializeField] private RectTransform RightArrowSprite;

    [SerializeField] private Button northButton;
    [SerializeField] private Button southButton;
    [SerializeField] private Button eastButton;
    [SerializeField] private Button westButton;

    [SerializeField] private Button northWestButton;
    [SerializeField] private Button northEastButton;
    [SerializeField] private Button southWestButton;
    [SerializeField] private Button southEastButton;

    private InputAction physicalDPad;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AddButtonEvents(northButton, Vector2.up);
        AddButtonEvents(southButton, Vector2.down);
        AddButtonEvents(eastButton, Vector2.right);
        AddButtonEvents(westButton, Vector2.left);

        AddButtonEvents(northEastButton, new Vector2(1, 1));
        AddButtonEvents(northWestButton, new Vector2(-1, 1));
        AddButtonEvents(southEastButton, new Vector2(1, -1));
        AddButtonEvents(southWestButton, new Vector2(-1, -1));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AddButtonEvents(Button button, Vector2 direction)
    {
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        var pointerDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        pointerDown.callback.AddListener((_) => OnButtonPress(direction));
        trigger.triggers.Add(pointerDown);

        var pointerUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        pointerUp.callback.AddListener((_) => ResetDPad());
        trigger.triggers.Add(pointerUp);
    }

    private void OnButtonPress(Vector2 direction)
    {
        // Simulate the D-Pad press animation
        float skewAmount = 5f; // Angle of skew in degrees
        float scaleAmount = 0.95f;

        dPadBackground.localEulerAngles = new Vector3(0, 0, -direction.x * skewAmount);

        dPadBackground.localScale = new Vector3(
            1 - Mathf.Abs(direction.x) * 0.05f,
            1 - Mathf.Abs(direction.y) * 0.05f,
            1);
    }

    private void ResetDPad()
    {
        dPadBackground.localEulerAngles = Vector3.zero;
        dPadBackground.localScale = Vector3.one;
    }
}

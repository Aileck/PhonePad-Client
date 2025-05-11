using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static GamepadConfig;

public class ButtonComponent : MonoBehaviour, IGamepadComponent
{
    [SerializeField] private GamepadAction action;
    [SerializeField] private Button button;

    private InputAction physicalButton;
    [SerializeField] private bool virtualButtonPressed;

    private InputType lastInputType = InputType.VIRTUAL;

    // Congfiguration
    private Profile gamepadConfig;

    private RectTransform referenceParent;

    void Awake()
    {
        referenceParent = GameObject.FindGameObjectWithTag("Reference").GetComponent<RectTransform>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize physical button action
        physicalButton = InputActionManager.Instance.GetAction(action);

        // Add button events
        AddButtonEvents();
    }

    void Update()
    {
        if (gamepadConfig.ignorePhysicalGamepad)
        {
            return;
        }

        if (physicalButton != null && physicalButton.IsPressed())
        {
            lastInputType = InputType.PYSHICAL;
        }
    }

    void FixedUpdate()
    {
        if (gamepadConfig.ignorePhysicalGamepad)
        {
            return;
        }

        // Only update the virtual button if the gamepad is connected and no virtual input is being used
        if (gamepadConfig.syncVirtualInputWithGamepad &&
            lastInputType == InputType.PYSHICAL)
        {
            bool isPressed = physicalButton != null && physicalButton.IsPressed();

            if (isPressed)
            {
                PressButton();
                if (action == GamepadAction.LeftStickButton)
                {
                    Debug.Log("Left Stick Button Pressed");
                }

                if (action == GamepadAction.RightStickButton)
                {
                    Debug.Log("Right Stick Button Pressed");
                }
            }
            else
            {
                ReleaseButton();
            }
        }
    }

    private void AddButtonEvents()
    {
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        var pointerDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        pointerDown.callback.AddListener((_) => OnButtonPress());
        trigger.triggers.Add(pointerDown);

        var pointerUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        pointerUp.callback.AddListener((_) => OnButtonRelease());
        trigger.triggers.Add(pointerUp);
    }

    private void OnButtonPress()
    {
        virtualButtonPressed = true;
        PressButton();

        SetLastInputType(InputType.VIRTUAL);
    }

    private void OnButtonRelease()
    {
        virtualButtonPressed = false;
        ReleaseButton();
    }

    private void PressButton()
    {
        // Apply only visual scale change
        float scaleAmount = gamepadConfig.buttonPressTransformScale;
        button.transform.localScale = new Vector3(scaleAmount, scaleAmount, 1);

        // Manuel set of the button color
        ColorBlock colors = button.colors;
        button.targetGraphic.color = colors.pressedColor;
    }

    private void ReleaseButton()
    {
        button.transform.localScale = Vector3.one;

        // Manuel reset of the button color
        ColorBlock colors = button.colors;
        button.targetGraphic.color = colors.normalColor;
    }

    public bool GetButtonInput()
    {
        if (gamepadConfig.ignorePhysicalGamepad)
        {
            return virtualButtonPressed;
        }
        // If no gamepad is connected, return the virtual button state
        if (Gamepad.current == null)
        {
            return virtualButtonPressed;
        }

        // Return depending on the last input type
        if (lastInputType == InputType.PYSHICAL)
        {
            bool physicalInput = physicalButton != null && physicalButton.IsPressed();
            return physicalInput;
        }
        else
        {
            return virtualButtonPressed;
        }
    }

    // Implementing IGamepadComponent interface
    public Vector2 GetNormalizedPosition()
    {
        Vector2 inputOffset = button.GetComponent<RectTransform>().anchoredPosition;

        float maxHorizontal = referenceParent.rect.width / 2;
        float maxVertical = referenceParent.rect.height / 2;

        // Normalize to range -1 to 1
        Vector2 normalized = new Vector2(
            Mathf.Clamp(inputOffset.x / maxHorizontal, -1f, 1f),
            Mathf.Clamp(inputOffset.y / maxVertical, -1f, 1f)
        );

        return normalized;
    }

    public void SetNormalizedPosition(Vector2 normalizedPos)
    {
        normalizedPos.x = Mathf.Clamp(normalizedPos.x, -1f, 1f);
        normalizedPos.y = Mathf.Clamp(normalizedPos.y, -1f, 1f);

        // Calculate the actual position based on the normalized values
        float posX = normalizedPos.x * (referenceParent.rect.width / 2);
        float posY = normalizedPos.y * (referenceParent.rect.height / 2);

        // Set the anchored position
        button.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, posY);
    }

    public void SetProfile(Profile config)
    {
        gamepadConfig = config;

    }

    public void SetLastInputType(InputType type)
    {
        lastInputType = type;
    }

    public void SetIcon(Sprite sprite)
    {
        button.gameObject.GetComponent<Image>().sprite = sprite;
    }

    public Vector2 GetScale()
    {
        return button.transform.localScale;
    }

    public void SetScale(Vector2 scale)
    {
        button.transform.localScale = scale;
    }

}
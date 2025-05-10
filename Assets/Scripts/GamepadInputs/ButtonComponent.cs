using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ButtonComponent : MonoBehaviour, IGamepadComponent
{
    [SerializeField] private GamepadAction action;
    [SerializeField] private Button button;

    private InputAction physicalButton;
    private bool virtualButtonPressed;

    private InputType lastInputType = InputType.VIRTUAL;

    // Congfiguration
    private GamepadConfig gamepadConfig;

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
        if (physicalButton != null && physicalButton.IsPressed())
        {
            lastInputType = InputType.PYSHICAL;
        }
    }

    void FixedUpdate()
    {
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

    public bool GetButtonState()
    {
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
    public Vector2 GetPosition()
    {
        return ((RectTransform)button.transform).anchoredPosition;
    }

    public void SetPosition(Vector2 position)
    {
        ((RectTransform)button.transform).anchoredPosition = position;
    }

    public void SetConfig(GamepadConfig config)
    {
        gamepadConfig = config;
    }

    public void SetLastInputType(InputType type)
    {
        lastInputType = type;
    }
}
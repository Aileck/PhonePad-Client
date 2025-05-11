using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DPadComponent : MonoBehaviour, IGamepadComponent
{
    [SerializeField] private RectTransform dPadBackground;

    [SerializeField] private RectTransform UpArrowSprite;
    [SerializeField] private RectTransform DownArrowSprite;
    [SerializeField] private RectTransform LeftArrowSprite;
    [SerializeField] private RectTransform RightArrowSprite;

    [SerializeField] private Button upButton;
    [SerializeField] private Button downButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button leftButton;

    [SerializeField] private Button upLeftButton;
    [SerializeField] private Button upRightButton;
    [SerializeField] private Button downLeftButton;
    [SerializeField] private Button downRightButton;

    private InputAction physicalUp;
    private InputAction physicalDown;
    private InputAction physicalLeft;
    private InputAction physicalRight;

    private RectTransform referenceParent;

    // Up, Down, Right, Left = Vector2.up, Vector2.down, Vector2.right, Vector2.left
    // U_L, U_R, D_L, D_R = Vector2(1, 1), Vector2(-1, 1), Vector2(1, -1), Vector2(-1, -1)
    [SerializeField] private Vector2 virtualDPad;

    private InputType lastInputType = InputType.VIRTUAL;
    // Congfiguration
    private GamepadConfig gamepadConfig;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize all arrow sprites to be inactive
        DeactivateAllArrows();

        AddButtonEvents(upButton, Vector2.up);
        AddButtonEvents(downButton, Vector2.down);
        AddButtonEvents(rightButton, Vector2.right);
        AddButtonEvents(leftButton, Vector2.left);

        AddButtonEvents(upRightButton, new Vector2(1, 1));
        AddButtonEvents(upLeftButton, new Vector2(-1, 1));
        AddButtonEvents(downRightButton, new Vector2(1, -1));
        AddButtonEvents(downLeftButton, new Vector2(-1, -1));

        // Initialize the physical DPad actions
        physicalUp = InputActionManager.Instance.GetAction(GamepadAction.Up);
        physicalDown = InputActionManager.Instance.GetAction(GamepadAction.Down);
        physicalLeft = InputActionManager.Instance.GetAction(GamepadAction.Left);
        physicalRight = InputActionManager.Instance.GetAction(GamepadAction.Right);

        referenceParent = GameObject.FindGameObjectWithTag("Reference").GetComponent<RectTransform>();
    }

    void Update()
    {
        if (gamepadConfig.ignorePhysicalGamepad)
        {
            return;
        }

        // Check if any physical input is active
        if (isPhysicalInputActive())
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

        // Only update the animation  if the gamepad is connected and no virtual input is being used
        if (gamepadConfig.syncVirtualInputWithGamepad &&
            lastInputType == InputType.PYSHICAL)
        {
            Vector2 input = GetPhysicalPadInput();

            if (input != Vector2.zero)
            {
                ActivateArrows(input);
            }
            else
            {
                ResetDPad();
            }
        }

    }

    public Vector2 GetDpadInput()
    {
        if (gamepadConfig.ignorePhysicalGamepad)
        {
            return virtualDPad;
        }

        // If no gamepad is connected, return the virtual input
        if (Gamepad.current == null)
        {
            return virtualDPad;
        }

        // Return depending on the last input type
        if (lastInputType == InputType.PYSHICAL)
        {
            Vector2 physicalInput = GetPhysicalPadInput();
            return physicalInput;
        }
        else
        {
            return virtualDPad;
        }

    }

    public bool GetUpInput()
    {
        Vector2 input = GetDpadInput();
        return input.y > 0;
    }

    public bool GetDownInput()
    {
        Vector2 input = GetDpadInput();
        return input.y < 0;
    }

    public bool GetLeftInput()
    {
        Vector2 input = GetDpadInput();
        return input.x < 0;
    }

    public bool GetRightInput()
    {
        Vector2 input = GetDpadInput();
        return input.x > 0;
    }

    private void AddButtonEvents(Button button, Vector2 direction)
    {
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        var pointerDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        pointerDown.callback.AddListener((_) => OnButtonPress(direction));
        trigger.triggers.Add(pointerDown);

        var pointerUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        pointerUp.callback.AddListener((_) => ResetDPad());
        pointerUp.callback.AddListener((_) => ResetDPadValue());
        trigger.triggers.Add(pointerUp);
    }

    private void OnButtonPress(Vector2 direction)
    {
        virtualDPad = direction;
        ActivateArrows(direction);

        SetLastInputType(InputType.VIRTUAL);
    }

    private void ResetDPad()
    {
        dPadBackground.localEulerAngles = Vector3.zero;
        dPadBackground.localScale = Vector3.one;

        // Deactivate all arrow sprites when released
        DeactivateAllArrows();
    }

    private void ResetDPadValue()
    {
        virtualDPad = Vector2.zero;
    }

    private void ActivateArrows(Vector2 direction)
    {
        // Deactivate all arrows first
        DeactivateAllArrows();

        // Apply only visual scale change
        float scaleAmount = gamepadConfig.buttonPressTransformScale;
        dPadBackground.localScale = new Vector3(scaleAmount, scaleAmount, 1);

        // Activate arrows based on the direction
        // Check horizontal direction
        if (direction.x > 0)
        {
            RightArrowSprite.gameObject.SetActive(true);
        }
        else if (direction.x < 0)
        {
            LeftArrowSprite.gameObject.SetActive(true);
        }

        // Check vertical direction
        if (direction.y > 0)
        {
            UpArrowSprite.gameObject.SetActive(true);
        }
        else if (direction.y < 0)
        {
            DownArrowSprite.gameObject.SetActive(true);
        }
    }

    private void DeactivateAllArrows()
    {
        // Deactivate all arrow sprites
        if (UpArrowSprite != null) UpArrowSprite.gameObject.SetActive(false);
        if (DownArrowSprite != null) DownArrowSprite.gameObject.SetActive(false);
        if (LeftArrowSprite != null) LeftArrowSprite.gameObject.SetActive(false);
        if (RightArrowSprite != null) RightArrowSprite.gameObject.SetActive(false);
    }

    private bool isPhysicalInputActive()
    {
        return physicalUp != null && physicalDown != null && physicalLeft != null && physicalRight != null &&
               (physicalUp.IsPressed() || physicalDown.IsPressed() || physicalLeft.IsPressed() || physicalRight.IsPressed());
    }

    private Vector2 GetPhysicalPadInput()
    {
        float horizontalInput = -physicalLeft.ReadValue<float>() + physicalRight.ReadValue<float>();

        float verticalInput = physicalUp.ReadValue<float>() - physicalDown.ReadValue<float>();

        Vector2 physicalInput = new Vector2(horizontalInput, verticalInput);

        if (physicalInput.magnitude > 1)
        {
            physicalInput.Normalize();
        }

        return physicalInput;
    }

    // Implementing IGamepadComponent interface
    public Vector2 GetNormalizedPosition()
    {
        Vector2 normalized = new Vector2(
            dPadBackground.anchoredPosition.x / referenceParent.rect.width,
            dPadBackground.anchoredPosition.y / referenceParent.rect.height
        );
        return normalized;
    }

    public void SetNormalizedPosition(Vector2 position)
    {
        Vector2 anchored = new Vector2(
            position.x * referenceParent.rect.width,
            position.y * referenceParent.rect.height
        );

        dPadBackground.anchoredPosition = anchored;
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
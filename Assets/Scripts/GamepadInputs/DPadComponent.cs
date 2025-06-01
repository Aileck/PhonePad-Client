using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static GamepadConfig;

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

    [SerializeField] private Vector2 virtualDPad;

    private InputType lastInputType = InputType.VIRTUAL;

    private Profile gamepadConfig;

    private bool pressToActivate = true;
    private bool toggleActive = false;
    private bool isToggled = false;
    private Vector2 toggledValue = Vector2.zero;

    void Awake()
    {
        referenceParent = GameObject.FindGameObjectWithTag("Reference").GetComponent<RectTransform>();
    }

    void Start()
    {
        DeactivateAllArrows();

        AddButtonEvents(upButton, Vector2.up);
        AddButtonEvents(downButton, Vector2.down);
        AddButtonEvents(rightButton, Vector2.right);
        AddButtonEvents(leftButton, Vector2.left);

        AddButtonEvents(upRightButton, new Vector2(1, 1));
        AddButtonEvents(upLeftButton, new Vector2(-1, 1));
        AddButtonEvents(downRightButton, new Vector2(1, -1));
        AddButtonEvents(downLeftButton, new Vector2(-1, -1));

        physicalUp = InputActionManager.Instance.GetAction(GamepadAction.Up);
        physicalDown = InputActionManager.Instance.GetAction(GamepadAction.Down);
        physicalLeft = InputActionManager.Instance.GetAction(GamepadAction.Left);
        physicalRight = InputActionManager.Instance.GetAction(GamepadAction.Right);
    }

    void Update()
    {
        if (gamepadConfig.ignorePhysicalGamepad)
        {
            return;
        }

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
            return GetVirtualDPadInput();
        }

        if (Gamepad.current == null)
        {
            return GetVirtualDPadInput();
        }

        if (lastInputType == InputType.PYSHICAL)
        {
            Vector2 physicalInput = GetPhysicalPadInput();
            return physicalInput;
        }
        else
        {
            return GetVirtualDPadInput();
        }
    }

    private Vector2 GetVirtualDPadInput()
    {
        if (toggleActive && isToggled)
        {
            return toggledValue;
        }
        return virtualDPad;
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
        pointerDown.callback.AddListener((data) => OnButtonPointerDown((PointerEventData)data, direction));
        trigger.triggers.Add(pointerDown);

        var pointerUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        pointerUp.callback.AddListener((data) => OnButtonPointerUp((PointerEventData)data, direction));
        trigger.triggers.Add(pointerUp);

        var pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        pointerEnter.callback.AddListener((data) => OnButtonPointerEnter((PointerEventData)data, direction));
        trigger.triggers.Add(pointerEnter);

        var pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        pointerExit.callback.AddListener((data) => OnButtonPointerExit((PointerEventData)data, direction));
        trigger.triggers.Add(pointerExit);
    }

    private void OnButtonPointerDown(PointerEventData eventData, Vector2 direction)
    {
        if (pressToActivate)
        {
            if (toggleActive)
            {
                if (isToggled && toggledValue == direction)
                {
                    isToggled = false;
                    toggledValue = Vector2.zero;
                    ResetDPad();
                    ResetDPadValue();
                }
                else
                {
                    isToggled = true;
                    toggledValue = direction;
                    OnButtonPress(direction);
                }
            }
            else
            {
                OnButtonPress(direction);
            }
        }
    }

    private void OnButtonPointerUp(PointerEventData eventData, Vector2 direction)
    {
        if (pressToActivate && !toggleActive)
        {
            ResetDPad();
            ResetDPadValue();
        }
    }

    private void OnButtonPointerEnter(PointerEventData eventData, Vector2 direction)
    {
        if (!pressToActivate)
        {
            if (toggleActive)
            {
                if (isToggled && toggledValue == direction)
                {
                    isToggled = false;
                    toggledValue = Vector2.zero;
                    ResetDPad();
                    ResetDPadValue();
                }
                else
                {
                    isToggled = true;
                    toggledValue = direction;
                    OnButtonPress(direction);
                }
            }
            else
            {
                OnButtonPress(direction);
            }
        }
    }

    private void OnButtonPointerExit(PointerEventData eventData, Vector2 direction)
    {
        if (!pressToActivate && !toggleActive)
        {
            ResetDPad();
            ResetDPadValue();
        }
    }

    private void OnButtonPress(Vector2 direction)
    {
        if (!toggleActive)
        {
            virtualDPad = direction;
        }
        ActivateArrows(direction);
        SetLastInputType(InputType.VIRTUAL);
    }

    private void ResetDPad()
    {
        dPadBackground.localEulerAngles = Vector3.zero;
        dPadBackground.localScale = Vector3.one;
        DeactivateAllArrows();
    }

    private void ResetDPadValue()
    {
        if (!toggleActive)
        {
            virtualDPad = Vector2.zero;
        }
    }

    private void ActivateArrows(Vector2 direction)
    {
        DeactivateAllArrows();

        float scaleAmount = gamepadConfig.buttonPressTransformScale;
        dPadBackground.localScale = new Vector3(scaleAmount, scaleAmount, 1);

        if (direction.x > 0)
        {
            RightArrowSprite.gameObject.SetActive(true);
        }
        else if (direction.x < 0)
        {
            LeftArrowSprite.gameObject.SetActive(true);
        }

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

    public Vector2 GetNormalizedPosition()
    {
        Vector2 inputOffset = dPadBackground.anchoredPosition;

        float maxHorizontal = referenceParent.rect.width / 2;
        float maxVertical = referenceParent.rect.height / 2;

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

        float posX = normalizedPos.x * (referenceParent.rect.width / 2);
        float posY = normalizedPos.y * (referenceParent.rect.height / 2);

        dPadBackground.anchoredPosition = new Vector2(posX, posY);
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
    }

    public Vector2 GetScale()
    {
        return dPadBackground.localScale;
    }

    public void SetScale(Vector2 scale)
    {
        dPadBackground.localScale = scale;
    }

    public void SetVisibility(bool isVisible)
    {
        Debug.Log($"Setting DPad visibility to {isVisible}");

        dPadBackground.gameObject.SetActive(isVisible);
        UpArrowSprite.gameObject.SetActive(isVisible);
        DownArrowSprite.gameObject.SetActive(isVisible);
        LeftArrowSprite.gameObject.SetActive(isVisible);
        RightArrowSprite.gameObject.SetActive(isVisible);
    }

    public void SetPressToActivate(bool isPressed)
    {
        pressToActivate = isPressed;
    }

    public void SetToggleActive(bool isActive)
    {
        toggleActive = isActive;
        if (!isActive)
        {
            isToggled = false;
            toggledValue = Vector2.zero;
            ResetDPad();
            ResetDPadValue();
        }
    }
}
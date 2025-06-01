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

    private Profile gamepadConfig;
    private RectTransform referenceParent;

    private bool pressToActivate = true;
    private bool toggleActive = false;
    private bool isToggled = false;
    private bool isHovering = false;

    void Awake()
    {
        referenceParent = GameObject.FindGameObjectWithTag("Reference").GetComponent<RectTransform>();
    }

    void Start()
    {
        physicalButton = InputActionManager.Instance.GetAction(action);
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
        pointerDown.callback.AddListener((data) => OnButtonPointerDown((PointerEventData)data));
        trigger.triggers.Add(pointerDown);

        var pointerUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        pointerUp.callback.AddListener((data) => OnButtonPointerUp((PointerEventData)data));
        trigger.triggers.Add(pointerUp);

        var pointerEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        pointerEnter.callback.AddListener((data) => OnButtonPointerEnter((PointerEventData)data));
        trigger.triggers.Add(pointerEnter);

        var pointerExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        pointerExit.callback.AddListener((data) => OnButtonPointerExit((PointerEventData)data));
        trigger.triggers.Add(pointerExit);
    }

    private void OnButtonPointerDown(PointerEventData eventData)
    {
        if (pressToActivate)
        {
            if (toggleActive)
            {
                isToggled = !isToggled;
                if (isToggled)
                {
                    OnButtonPress();
                }
                else
                {
                    OnButtonRelease();
                }
            }
            else
            {
                OnButtonPress();
            }
        }
    }

    private void OnButtonPointerUp(PointerEventData eventData)
    {
        if (pressToActivate && !toggleActive)
        {
            OnButtonRelease();
        }
    }

    private void OnButtonPointerEnter(PointerEventData eventData)
    {
        isHovering = true;

        if (!pressToActivate)
        {
            if (toggleActive)
            {
                isToggled = !isToggled;
                if (isToggled)
                {
                    OnButtonPress();
                }
                else
                {
                    OnButtonRelease();
                }
            }
            else
            {
                OnButtonPress();
            }
        }
    }

    private void OnButtonPointerExit(PointerEventData eventData)
    {
        isHovering = false;

        if (!pressToActivate && !toggleActive)
        {
            OnButtonRelease();
        }
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
        float scaleAmount = gamepadConfig.buttonPressTransformScale;
        button.transform.localScale = new Vector3(scaleAmount, scaleAmount, 1);

        ColorBlock colors = button.colors;
        button.targetGraphic.color = colors.pressedColor;
    }

    private void ReleaseButton()
    {
        button.transform.localScale = Vector3.one;

        ColorBlock colors = button.colors;
        button.targetGraphic.color = colors.normalColor;
    }

    public bool GetButtonInput()
    {
        if (gamepadConfig.ignorePhysicalGamepad)
        {
            return GetVirtualButtonState();
        }

        if (Gamepad.current == null)
        {
            return GetVirtualButtonState();
        }

        if (lastInputType == InputType.PYSHICAL)
        {
            bool physicalInput = physicalButton != null && physicalButton.IsPressed();
            return physicalInput;
        }
        else
        {
            return GetVirtualButtonState();
        }
    }

    private bool GetVirtualButtonState()
    {
        if (toggleActive)
        {
            return isToggled;
        }
        return virtualButtonPressed;
    }

    public Vector2 GetNormalizedPosition()
    {
        Vector2 inputOffset = button.GetComponent<RectTransform>().anchoredPosition;

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

    public void SetVisibility(bool isVisible)
    {
        button.gameObject.SetActive(isVisible);
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
            if (virtualButtonPressed)
            {
                OnButtonRelease();
            }
        }
    }
}
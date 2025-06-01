using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static GamepadConfig;

public class StickComponent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler, IGamepadComponent
{
    [SerializeField] private GamepadAction action;

    [SerializeField] private RectTransform stickBackground;
    [SerializeField] private RectTransform stickKnob;

    private float virtualStickRadius;

    private InputType lastInputType = InputType.VIRTUAL;

    [SerializeField] private Vector2 virtualStick;
    private InputAction physicalStick;

    private RectTransform referenceParent;

    private bool isDisabled = false;
    private Profile gamepadConfig;

    private bool pressToActivate = true;
    private bool toggleActive = false;
    private bool isToggled = false;
    private Vector2 toggledValue = Vector2.zero;
    private bool isHovering = false;

    void Awake()
    {
        referenceParent = GameObject.FindGameObjectWithTag("Reference").GetComponent<RectTransform>();
    }

    void Start()
    {
        physicalStick = InputActionManager.Instance.GetAction(action);
        virtualStickRadius = stickBackground.rect.width * 0.5f;
        ResetKnob();
    }

    void Update()
    {
        if (gamepadConfig.ignorePhysicalGamepad || isDisabled)
        {
            return;
        }

        if (physicalStick != null && physicalStick.ReadValue<Vector2>() != Vector2.zero)
        {
            lastInputType = InputType.PYSHICAL;
        }

        if (!pressToActivate && isHovering)
        {
            Vector2 pointerPosition = Pointer.current.position.ReadValue();

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                stickBackground,
                pointerPosition,
                null,
                out Vector2 localPoint))
            {
                Vector2 clampedInput = GetClampedStickInput(localPoint);
                SetKnobPosition(clampedInput);

                if (!toggleActive)
                {
                    SetVirtulInput(clampedInput);
                }

                SetLastInputType(InputType.VIRTUAL);
            }
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
            Vector2 input = physicalStick.ReadValue<Vector2>() * virtualStickRadius;
            Vector2 clampedInput = GetClampedStickInput(input);
            SetKnobPosition(clampedInput);
        }
    }

    public Vector2 GetJoystickInput()
    {
        if (gamepadConfig.ignorePhysicalGamepad)
        {
            return GetVirtualInput();
        }

        if (Gamepad.current == null)
        {
            return GetVirtualInput();
        }

        if (lastInputType == InputType.PYSHICAL)
        {
            Vector2 physicalInput = physicalStick.ReadValue<Vector2>();
            return physicalInput;
        }
        else
        {
            return GetVirtualInput();
        }
    }

    private Vector2 GetVirtualInput()
    {
        if (toggleActive && isToggled)
        {
            return toggledValue;
        }
        return virtualStick;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (pressToActivate)
        {
            OnDrag(eventData);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;

        if (!pressToActivate && !toggleActive)
        {
            virtualStick = Vector2.zero;
            ResetKnob();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (pressToActivate && !isHovering)
        {
            return;
        }

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            stickBackground,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint))
        {
            Vector2 clampedInput = GetClampedStickInput(localPoint);
            SetKnobPosition(clampedInput);

            if (toggleActive)
            {
                toggledValue = clampedInput / virtualStickRadius;
                if (!isToggled)
                {
                    isToggled = true;
                }
            }
            else
            {
                SetVirtulInput(clampedInput);
            }

            SetLastInputType(InputType.VIRTUAL);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (toggleActive)
        {
            if (isToggled && virtualStick.magnitude < 0.1f)
            {
                isToggled = false;
                toggledValue = Vector2.zero;
                virtualStick = Vector2.zero;
                ResetKnob();
            }
        }
        else
        {
            if (pressToActivate || !isHovering)
            {
                virtualStick = Vector2.zero;
                ResetKnob();
            }
        }
    }

    private void ResetKnob()
    {
        stickKnob.localPosition = Vector2.zero;
    }

    private Vector2 GetClampedStickInput(Vector2 input)
    {
        float magnitude = input.magnitude;
        Vector2 clampedInput = input;

        if (magnitude > virtualStickRadius)
        {
            clampedInput = input.normalized * virtualStickRadius;
        }

        return clampedInput;
    }

    private void SetKnobPosition(Vector2 position)
    {
        stickKnob.localPosition = position;
    }

    private void SetVirtulInput(Vector2 position)
    {
        virtualStick = position / virtualStickRadius;
    }

    public Vector2 GetNormalizedPosition()
    {
        Vector2 inputOffset = stickBackground.anchoredPosition;

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

        stickBackground.anchoredPosition = new Vector2(posX, posY);
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
        return stickBackground.localScale;
    }

    public void SetScale(Vector2 scale)
    {
        stickBackground.localScale = scale;
    }

    public void SetDisabled(bool disabled)
    {
        isDisabled = disabled;
    }

    public void SetVisibility(bool isVisible)
    {
        stickBackground.gameObject.SetActive(isVisible);
        stickKnob.gameObject.SetActive(isVisible);
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
        }
    }
}
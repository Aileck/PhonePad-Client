using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static GamepadConfig;

public class StickComponent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IGamepadComponent
{
    [SerializeField] private GamepadAction action;

    [SerializeField] private RectTransform stickBackground;
    [SerializeField] private RectTransform stickKnob;

    private float virtualStickRadius;
    
    private InputType lastInputType = InputType.VIRTUAL;

    [SerializeField] private Vector2 virtualStick;
    private InputAction physicalStick;

    private RectTransform referenceParent;

    // Congfiguration
    private Profile gamepadConfig;

    void Awake()
    {
        referenceParent = GameObject.FindGameObjectWithTag("Reference").GetComponent<RectTransform>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        physicalStick = InputActionManager.Instance.GetAction(action);

        virtualStickRadius = stickBackground.rect.width * 0.5f;


        ResetKnob();
    }

    void Update()
    {
        if (gamepadConfig.ignorePhysicalGamepad)
        {
            return;
        }

        if (physicalStick != null && physicalStick.ReadValue<Vector2>() != Vector2.zero)
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

        // Only update the virtual joystick if the gamepad is connected and no virtual input is being used
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
            return virtualStick;
        }

        // If no gamepad is connected, return the virtual joystick input
        if (Gamepad.current == null)
        {
            return virtualStick;
        }

        // Return depending on the last input type
        if (lastInputType == InputType.PYSHICAL)
        {
            Vector2 physicalInput = physicalStick.ReadValue<Vector2>();

            return physicalInput;
        }
        else
        {
            return virtualStick;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            stickBackground,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint))
        {
            Vector2 clampedInput = GetClampedStickInput(localPoint);
            SetKnobPosition(clampedInput);
            SetVirtulInput(clampedInput);

            SetLastInputType(InputType.VIRTUAL);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        virtualStick = Vector2.zero;
        ResetKnob();
    }

    private void ResetKnob()
    {
        stickKnob.localPosition = Vector2.zero;
    }

    private Vector2 GetClampedStickInput(Vector2 input)
    {
        // Clamp input to joystick radius
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

    // Implements IGamepadComponent interface

    public Vector2 GetNormalizedPosition()
    {
        Vector2 inputOffset = stickBackground.anchoredPosition;

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
        // Donot set icon for stick
    }

    public Vector2 GetScale()
    {
        return stickBackground.localScale;
    }

    public void SetScale(Vector2 scale)
    {
        stickBackground.localScale = scale;
    }

}

using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class StickComponent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IGamepadComponent
{
    [SerializeField] private GamepadAction action;

    [SerializeField] private RectTransform stickBackground;
    [SerializeField] private RectTransform stickKnob;

    private float virtualStickRadius;
    
    private InputType lastInputType = InputType.VIRTUAL;

    private Vector2 virtualStick;
    private InputAction physicalStick;

    // Congfiguration
    private GamepadConfig gamepadConfig;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        physicalStick = InputActionManager.Instance.GetAction(action);

        virtualStickRadius = stickBackground.rect.width * 0.5f;

        ResetKnob();
    }

    void Update()
    {
        if (physicalStick != null && physicalStick.ReadValue<Vector2>() != Vector2.zero)
        {
            lastInputType = InputType.PYSHICAL;
        }
    }

    void FixedUpdate()
    {
        // Only update the virtual joystick if the gamepad is connected and no virtual input is being used
        if (gamepadConfig.syncVirtualInputWithGamepad &&
            lastInputType == InputType.PYSHICAL)
        {
            Vector2 input = physicalStick.ReadValue<Vector2>() * virtualStickRadius;
            Vector2 clampedInput = GetClampedStickInput(input);
            SetKnobPosition(clampedInput);

            Debug.Log("PS " + physicalStick.ReadValue<Vector2>());
            Debug.Log("CI " + clampedInput);

        }
    }

    public Vector2 GetJoystickInput()
    {
        // If no gamepad is connected, return the virtual joystick input
        if (physicalStick == null)
        {
            return virtualStick;
        }

        Vector2 physicalInput = physicalStick.ReadValue<Vector2>();

        // If gamepad input is not (0,0), use it instead of virtual joystick input
        if (physicalInput != Vector2.zero)
        {
            return physicalInput;
        }

        // If gamepad input is (0,0), fall back to virtual joystick input
        return virtualStick;
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

    public Vector2 GetPosition()
    {
        return stickBackground.anchoredPosition;
    }

    public void SetPosition(Vector2 position)
    {
        stickBackground.anchoredPosition = position;
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

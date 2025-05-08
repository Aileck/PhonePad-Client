using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;


public class VirtualJoyStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private GamepadAction action;
    [SerializeField] private InputAction joyStickAction;

    [SerializeField] private RectTransform joystickBackground;
    [SerializeField] private RectTransform joystickKnob;

    private Vector2 joystickInput;
    private float joystickRadius;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        joystickRadius = joystickBackground.rect.width * 0.5f;

        ResetKnob();
    }

    public Vector2 GetJoystickInput()
    {
        Gamepad gamepad = Gamepad.current;

        // If no gamepad is connected, return the virtual joystick input
        if (gamepad == null)
        {
            return joystickInput;
        }

        Vector2 gamepadInput = gamepad.leftStick.ReadValue();

        // If gamepad input is not (0,0), use it instead of virtual joystick input
        if (gamepadInput != Vector2.zero)
        {
            return gamepadInput;
        }

        // If gamepad input is (0,0), fall back to virtual joystick input
        return joystickInput;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            joystickBackground,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint))
        {
            Vector2 direction = localPoint;
            float magnitude = direction.magnitude;

            if (magnitude > joystickRadius)
            {
                direction = direction.normalized * joystickRadius;
            }

            joystickKnob.localPosition = direction;

            joystickInput = direction / joystickRadius;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        joystickInput = Vector2.zero;
        ResetKnob();
    }

    private void ResetKnob()
    {
        joystickKnob.localPosition = Vector2.zero;
    }
}

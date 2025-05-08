using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class StickComponent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IGamepadComponent
{
    [SerializeField] private GamepadAction action;

    [SerializeField] private RectTransform stickBackground;
    [SerializeField] private RectTransform stickKnob;

    private float joystickRadius;

    private Vector2 virtualStick;
    private InputAction physicalStick;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        physicalStick = InputActionManager.Instance.GetAction(action);

        joystickRadius = stickBackground.rect.width * 0.5f;

        ResetKnob();
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
            Vector2 direction = localPoint;
            float magnitude = direction.magnitude;

            if (magnitude > joystickRadius)
            {
                direction = direction.normalized * joystickRadius;
            }

            stickKnob.localPosition = direction;

            virtualStick = direction / joystickRadius;
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

    public Vector2 GetPosition()
    {
        return stickBackground.anchoredPosition;
    }

    public void SetPosition(Vector2 position)
    {
        stickBackground.anchoredPosition = position;
    }
}

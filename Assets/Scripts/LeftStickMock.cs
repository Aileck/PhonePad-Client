using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class LeftStickMock : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private InputActionAsset gameplayActions;
    [SerializeField] private InputAction leftStickAction;

    [SerializeField] private RectTransform virtualStickBackground;
    [SerializeField] private RectTransform virtualStickKnob;

    private Vector2 joystickInput;
    private float joystickRadius;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        joystickRadius = virtualStickBackground.rect.width * 0.5f;

        leftStickAction = gameplayActions.FindAction("LeftStick");

        ResetKnob();
    }

    public Vector2 GetJoystickInput()
    {
        Gamepad gamepad = Gamepad.current;

        // If no gamepad is connected, return the virtual joystick input
        if (gamepad == null)
        {
            Debug.Log("Gamepad not connected — using virtual joystick input." + joystickInput);
            return joystickInput;
        }

        Vector2 gamepadInput = gamepad.leftStick.ReadValue();

        // If gamepad input is not (0,0), use it instead of virtual joystick input
        if (gamepadInput != Vector2.zero)
        {
            Debug.Log("Gamepad connected — using non-zero gamepad input: " + gamepadInput);
            return gamepadInput;
        }

        // If gamepad input is (0,0), fall back to virtual joystick input
        Debug.Log("Gamepad connected but input is zero — using virtual joystick input: " + joystickInput);
        return joystickInput;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown called");

        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag called");
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            virtualStickBackground,
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

            virtualStickKnob.localPosition = direction;

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
        virtualStickKnob.localPosition = Vector2.zero;
    }
}

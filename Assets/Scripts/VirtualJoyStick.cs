using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;


public class VirtualJoyStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private RectTransform joystickBackground;
    [SerializeField] private RectTransform joystickKnob;

    private Vector2 joystickInput;
    private Vector2 joystickCenter;
    private float joystickRadius;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        joystickCenter = joystickBackground.position;
        joystickRadius = joystickBackground.rect.width * 0.5f;

        ResetKnob();
    }

    public Vector2 GetJoystickInput()
    {
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

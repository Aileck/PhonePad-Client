using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EditableButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    const int DPAD_SIZE_MULTIPLIER = 3;

    [SerializeField] private ButtonName buttonName;
    private RectTransform referencePanel;
    private Image buttonImage;
    private RectTransform rectTransform;

    private bool isDragging = false;
    private Vector2 dragStartPos;
    private Vector2 initialAnchoredPos;

    private bool isPinching = false;
    private float initialDistance;
    private Vector3 initialScale;
    private Vector2[] lastTouchPositions = new Vector2[2];

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        HandleTouchInput();
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            if (!isPinching)
            {
                isPinching = true;
                isDragging = false;
                initialDistance = Vector2.Distance(touch1.position, touch2.position);
                initialScale = buttonImage.transform.localScale;
                lastTouchPositions[0] = touch1.position;
                lastTouchPositions[1] = touch2.position;
            }
            else
            {
                float currentDistance = Vector2.Distance(touch1.position, touch2.position);
                float scaleMultiplier = currentDistance / initialDistance;
                Vector3 newScale = initialScale * scaleMultiplier;
                newScale = Vector3.Max(newScale, Vector3.one * 0.1f);
                newScale = Vector3.Min(newScale, Vector3.one * 5f);
                buttonImage.transform.localScale = newScale;
            }
        }
        else
        {
            if (isPinching)
            {
                isPinching = false;
                SaveCurrentState();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Input.touchCount <= 1)
        {
            isDragging = true;
            dragStartPos = eventData.position;
            initialAnchoredPos = rectTransform.anchoredPosition;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDragging)
        {
            isDragging = false;
            SaveCurrentState();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging && Input.touchCount <= 1)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                referencePanel,
                eventData.position,
                eventData.pressEventCamera,
                out localPoint
            );

            rectTransform.anchoredPosition = localPoint;
        }
    }

    private void SaveCurrentState()
    {
        Vector2 currentPos = GetNormalizedPosition();
        Vector2 currentScale = GetScale();

        SetNormalizedPosition(currentPos);
        SetScale(currentScale);
    }

    public void Initialice(ButtonName name, RectTransform panel, Image image)
    {
        buttonName = name;
        referencePanel = panel;
        buttonImage = image;
        rectTransform = GetComponent<RectTransform>();
    }

    public Vector2 GetNormalizedPosition()
    {
        Vector2 inputOffset = buttonImage.GetComponent<RectTransform>().anchoredPosition;
        float maxHorizontal = referencePanel.rect.width / 2;
        float maxVertical = referencePanel.rect.height / 2;

        Vector2 normalized = new Vector2(
            Mathf.Clamp(inputOffset.x / maxHorizontal, -1f, 1f),
            Mathf.Clamp(inputOffset.y / maxVertical, -1f, 1f)
        );
        return normalized;
    }

    public Vector2 GetScale()
    {
        if (buttonName == ButtonName.DPad)
        {
            return buttonImage.transform.localScale / DPAD_SIZE_MULTIPLIER;
        }
        return buttonImage.transform.localScale;
    }

    public void SetScale(Vector2 scale)
    {
        if (buttonName == ButtonName.DPad)
        {
            scale *= DPAD_SIZE_MULTIPLIER;
        }
        buttonImage.transform.localScale = scale;
    }

    public void SetNormalizedPosition(Vector2 normalizedPos)
    {
        normalizedPos.x = Mathf.Clamp(normalizedPos.x, -1f, 1f);
        normalizedPos.y = Mathf.Clamp(normalizedPos.y, -1f, 1f);

        float posX = normalizedPos.x * (referencePanel.rect.width / 2);
        float posY = normalizedPos.y * (referencePanel.rect.height / 2);
        buttonImage.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, posY);
    }

    public ButtonName GetButtonName()
    {
        return buttonName;
    }
}
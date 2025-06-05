using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LogInHandler : MonoBehaviour
{
    // UI element to change based on app state
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject selectGamepadPanel;
    [SerializeField] private TMP_Text helpText;
    [SerializeField] private TMP_Text errorText;
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private TMP_InputField portInputField;
    [SerializeField] private ProfileSelector selector;

    // Keyboard
    [SerializeField] private RectTransform uiRoot;
    [SerializeField] private float keyboardOffsetMultiplier = 1.2f;

    private const string ipPrefKey = "server_ip";
    private const string portPrefKey = "server_port";

    private bool keyboardVisible = false;
    private Vector2 originalUIPosition;
    private int lastScreenHeight;

    void Start()
    {
        string savedIP = PlayerPrefs.GetString(ipPrefKey, "192.168.1.100");
        string savedPort = PlayerPrefs.GetString(portPrefKey, "60001");
        ipInputField.text = savedIP;
        portInputField.text = savedPort;

        InitializeKeyboardAdaptation();
    }

    void Update()
    {
        CheckKeyboardStatus();
    }

    private void InitializeKeyboardAdaptation()
    {
        if (uiRoot == null)
        {
            Transform currentTransform = transform;
            while (currentTransform.parent != null)
            {
                currentTransform = currentTransform.parent;
                Canvas canvas = currentTransform.GetComponent<Canvas>();
                if (canvas != null)
                {
                    uiRoot = canvas.GetComponent<RectTransform>();
                    break;
                }
            }

            if (uiRoot == null)
            {
                uiRoot = loginPanel.GetComponent<RectTransform>();
            }
        }

        if (uiRoot != null)
        {
            originalUIPosition = uiRoot.anchoredPosition;
        }

        lastScreenHeight = Screen.height;

        ipInputField.onSelect.AddListener(OnInputFieldSelected);
        portInputField.onSelect.AddListener(OnInputFieldSelected);
        ipInputField.onDeselect.AddListener(OnInputFieldDeselected);
        portInputField.onDeselect.AddListener(OnInputFieldDeselected);
    }

    private void CheckKeyboardStatus()
    {
        if (TouchScreenKeyboard.visible && !keyboardVisible)
        {
            OnKeyboardShow();
        }
        else if (!TouchScreenKeyboard.visible && keyboardVisible)
        {
            OnKeyboardHide();
        }
    }

    private void OnInputFieldSelected(string value)
    {
        StartCoroutine(DelayedKeyboardCheck());
    }

    private void OnInputFieldDeselected(string value)
    {
        if (!ipInputField.isFocused && !portInputField.isFocused)
        {
            StartCoroutine(DelayedKeyboardHide());
        }
    }

    private IEnumerator DelayedKeyboardCheck()
    {
        yield return new WaitForSeconds(0.1f);
        if (TouchScreenKeyboard.visible && !keyboardVisible)
        {
            OnKeyboardShow();
        }
    }

    private IEnumerator DelayedKeyboardHide()
    {
        yield return new WaitForSeconds(0.1f);
        if (!TouchScreenKeyboard.visible && keyboardVisible)
        {
            OnKeyboardHide();
        }
    }

    private void OnKeyboardShow()
    {
        if (keyboardVisible || uiRoot == null) return;

        keyboardVisible = true;

        float keyboardHeight = GetKeyboardHeight();
        Vector2 newPosition = originalUIPosition;

        if (uiRoot.anchorMin.y == 0 && uiRoot.anchorMax.y == 0)
        {
            newPosition.y += keyboardHeight * keyboardOffsetMultiplier;
        }
        else if (uiRoot.anchorMin.y == 0.5f && uiRoot.anchorMax.y == 0.5f)
        {
            newPosition.y += keyboardHeight * keyboardOffsetMultiplier;
        }
        else
        {
            uiRoot.offsetMin = new Vector2(uiRoot.offsetMin.x, uiRoot.offsetMin.y + keyboardHeight * keyboardOffsetMultiplier);
            uiRoot.offsetMax = new Vector2(uiRoot.offsetMax.x, uiRoot.offsetMax.y + keyboardHeight * keyboardOffsetMultiplier);
            return;
        }

        StartCoroutine(MoveUISmooth(uiRoot.anchoredPosition, newPosition, 0.3f));

    }

    private void OnKeyboardHide()
    {
        if (!keyboardVisible || uiRoot == null) return;

        keyboardVisible = false;

        if (uiRoot.anchorMin.y != 0 || uiRoot.anchorMax.y != 1)
        {
            uiRoot.offsetMin = new Vector2(uiRoot.offsetMin.x, uiRoot.offsetMin.y - GetKeyboardHeight() * keyboardOffsetMultiplier);
            uiRoot.offsetMax = new Vector2(uiRoot.offsetMax.x, uiRoot.offsetMax.y - GetKeyboardHeight() * keyboardOffsetMultiplier);
            return;
        }

        StartCoroutine(MoveUISmooth(uiRoot.anchoredPosition, originalUIPosition, 0.3f));
    }

    private float GetKeyboardHeight()
    {
        if (TouchScreenKeyboard.area.height > 0)
        {
            return TouchScreenKeyboard.area.height;
        }

        float screenHeight = Screen.height;

        if (Application.platform == RuntimePlatform.Android)
        {
            return screenHeight * 0.4f;
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return screenHeight * 0.35f;
        }

        return screenHeight * 0.4f;
    }

    private IEnumerator MoveUISmooth(Vector2 from, Vector2 to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            t = Mathf.SmoothStep(0f, 1f, t);

            Vector2 currentPos = Vector2.Lerp(from, to, t);
            uiRoot.anchoredPosition = currentPos;

            yield return null;
        }

        uiRoot.anchoredPosition = to;
    }

    public async void Button_StartConnection()
    {
        string ip = ipInputField.text;
        string port = portInputField.text;
        PlayerPrefs.SetString(ipPrefKey, ip);
        PlayerPrefs.SetString(portPrefKey, port);

        errorText.gameObject.SetActive(true);
        errorText.text = i18nManager.Instance.Translate("menu_login_connecting");

        PetitionStatus isConnected = await AppLifeTimeManager.Instance.GetWebSocket().Send_ConnectionPetition(ip, port);

        if (isConnected.success)
        {
            AppLifeTimeManager.Instance.ToSelectGamepad();
        }
        else
        {
            errorText.gameObject.SetActive(true);

            if (isConnected.message == PetitionStatusMessagge.TIME_OUT)
            {
                errorText.text = i18nManager.Instance.Translate("menu_login_timeout");
            }
            else if (isConnected.message == PetitionStatusMessagge.MAX_CONNECTION)
            {
                errorText.text = i18nManager.Instance.Translate("menu_login_max_connections");
            }
            else
            {
                errorText.text = i18nManager.Instance.Translate("menu_login_error");
            }
        }
    }

    public void Button_StartPlayDualShock4()
    {
        AppLifeTimeManager.Instance.SetSessionGamepad(GamepadType.GAMEPAD_DUALSHOCK);
        AppLifeTimeManager.Instance.GetWebSocket().Send_RegisterGamepad(GamepadType.GAMEPAD_DUALSHOCK);
        AppLifeTimeManager.Instance.SetSessionConfigProfileIndex(selector.GetCurrentDualShockProfileIndex());
        AppLifeTimeManager.Instance.ToPlaying();
    }

    public void Button_StartPlayXbox()
    {
        AppLifeTimeManager.Instance.SetSessionGamepad(GamepadType.GAMEPAD_XBOX360);
        AppLifeTimeManager.Instance.GetWebSocket().Send_RegisterGamepad(GamepadType.GAMEPAD_XBOX360);
        AppLifeTimeManager.Instance.SetSessionConfigProfileIndex(selector.GetCurrentXboxProfileIndex());
        AppLifeTimeManager.Instance.ToPlaying();
    }

    void OnEnable()
    {
        AppLifeTimeManager.OnStateChanged += HandleStateChange;
    }

    void OnDisable()
    {
        AppLifeTimeManager.OnStateChanged -= HandleStateChange;

        if (ipInputField != null)
        {
            ipInputField.onSelect.RemoveListener(OnInputFieldSelected);
            ipInputField.onDeselect.RemoveListener(OnInputFieldDeselected);
        }
        if (portInputField != null)
        {
            portInputField.onSelect.RemoveListener(OnInputFieldSelected);
            portInputField.onDeselect.RemoveListener(OnInputFieldDeselected);
        }
    }

    private void HandleStateChange(AppLifeTimeManager.AppState state)
    {
        switch (state)
        {
            case AppLifeTimeManager.AppState.RequestingLogin:
                loginPanel.SetActive(true);
                selectGamepadPanel.SetActive(false);
                helpText.text = i18nManager.Instance.Translate("menu_login_help");
                if (uiRoot != null && !keyboardVisible)
                {
                    uiRoot.anchoredPosition = originalUIPosition;
                }
                break;

            case AppLifeTimeManager.AppState.SelectingGamepad:
                loginPanel.SetActive(false);
                selectGamepadPanel.SetActive(true);
                helpText.text = i18nManager.Instance.Translate("menu_select_gamepad_help");
                errorText.gameObject.SetActive(false);

                if (keyboardVisible)
                {
                    OnKeyboardHide();
                }

                break;
        }
    }
}
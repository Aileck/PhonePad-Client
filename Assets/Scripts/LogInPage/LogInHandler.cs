using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LogInHandler : MonoBehaviour
{
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject selectGamepadPanel;
    [SerializeField] private TMP_Text helpText;
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private TMP_InputField portInputField;
    [SerializeField] private RectTransform mainUIContainer;
    [SerializeField] private float keyboardOffsetY = 100f;

    private const string ipPrefKey = "server_ip";
    private const string portPrefKey = "server_port";

    private TouchScreenKeyboard activeKeyboard;
    private TMP_InputField currentActiveInputField;
    private Vector3 originalUIPosition;
    private bool isKeyboardActive = false;

    void Start()
    {
        string savedIP = PlayerPrefs.GetString(ipPrefKey, "192.168.1.100");
        string savedPort = PlayerPrefs.GetString(portPrefKey, "8080");
        ipInputField.text = savedIP;
        portInputField.text = savedPort;

        if (mainUIContainer != null)
        {
            originalUIPosition = mainUIContainer.anchoredPosition;
        }

        SetupInputFieldEvents();
    }

    void Update()
    {
        CheckKeyboardStatus();
    }

    private void SetupInputFieldEvents()
    {
        ipInputField.onSelect.AddListener((string value) => {
            OnInputFieldSelected(ipInputField);
        });

        ipInputField.onDeselect.AddListener((string value) => {
            OnInputFieldDeselected();
        });

        portInputField.onSelect.AddListener((string value) => {
            OnInputFieldSelected(portInputField);
        });

        portInputField.onDeselect.AddListener((string value) => {
            OnInputFieldDeselected();
        });
    }

    private void OnInputFieldSelected(TMP_InputField inputField)
    {
        currentActiveInputField = inputField;

        if (Application.isMobilePlatform)
        {
            OpenVirtualKeyboard(inputField);
        }
        else
        {
            AdjustUIForKeyboard(true);
        }
    }

    private void OnInputFieldDeselected()
    {
        CloseVirtualKeyboard();
        AdjustUIForKeyboard(false);
        currentActiveInputField = null;
    }

    private void OpenVirtualKeyboard(TMP_InputField inputField)
    {
        if (Application.isMobilePlatform)
        {
            TouchScreenKeyboardType keyboardType = TouchScreenKeyboardType.Default;

            if (inputField == ipInputField)
            {
                keyboardType = TouchScreenKeyboardType.NumbersAndPunctuation;
            }
            else if (inputField == portInputField)
            {
                keyboardType = TouchScreenKeyboardType.NumberPad;
            }

            activeKeyboard = TouchScreenKeyboard.Open(
                inputField.text,
                keyboardType,
                false,
                false,
                false,
                false,
                "",
                0
            );

            isKeyboardActive = true;
            AdjustUIForKeyboard(true);
        }
    }

    private void CloseVirtualKeyboard()
    {
        if (activeKeyboard != null)
        {
            activeKeyboard.active = false;
            activeKeyboard = null;
        }
        isKeyboardActive = false;
    }

    private void CheckKeyboardStatus()
    {
        if (activeKeyboard != null && currentActiveInputField != null)
        {
            if (activeKeyboard.text != currentActiveInputField.text)
            {
                currentActiveInputField.text = activeKeyboard.text;
            }

            if (activeKeyboard.status == TouchScreenKeyboard.Status.Done ||
                activeKeyboard.status == TouchScreenKeyboard.Status.Canceled)
            {
                OnInputFieldDeselected();
            }
        }

        if (Application.isMobilePlatform)
        {
            CheckSystemKeyboardHeight();
        }
    }

    private void CheckSystemKeyboardHeight()
    {
        Rect safeArea = Screen.safeArea;
        float screenHeight = Screen.height;

        bool keyboardVisible = (safeArea.height < screenHeight * 0.8f);

        if (keyboardVisible && !isKeyboardActive)
        {
            AdjustUIForKeyboard(true);
            isKeyboardActive = true;
        }
        else if (!keyboardVisible && isKeyboardActive && activeKeyboard == null)
        {
            AdjustUIForKeyboard(false);
            isKeyboardActive = false;
        }
    }

    private void AdjustUIForKeyboard(bool keyboardShown)
    {
        if (mainUIContainer == null) return;

        if (keyboardShown)
        {
            Vector3 newPosition = originalUIPosition;
            newPosition.y += keyboardOffsetY;
            mainUIContainer.anchoredPosition = newPosition;
        }
        else
        {
            mainUIContainer.anchoredPosition = originalUIPosition;
        }
    }

    public async void Button_StartConnection()
    {
        CloseVirtualKeyboard();
        AdjustUIForKeyboard(false);

        string ip = ipInputField.text;
        string port = portInputField.text;

        PlayerPrefs.SetString(ipPrefKey, ip);
        PlayerPrefs.SetString(portPrefKey, port);

        bool isConnected = await AppLifeTimeManager.Instance.GetWebSocket().Send_ConnectionPetition(ip, port);

        if (isConnected)
        {
            Debug.Log("Connection successful!");
            AppLifeTimeManager.Instance.ToSelectGamepad();
        }
        else
        {
            helpText.text = "Failed to connect. Please check your IP and port.";
        }

        Debug.Log("End of function!");
    }

    public void Button_StartPlayDualShock4()
    {
        AppLifeTimeManager.Instance.SetSessionGamepad(GamepadType.GAMEPAD_DUALSHOCK);
        AppLifeTimeManager.Instance.GetWebSocket().Send_RegisterGamepad(GamepadType.GAMEPAD_DUALSHOCK);
        AppLifeTimeManager.Instance.ToPlaying();
    }

    public void Button_StartPlayXbox()
    {
        AppLifeTimeManager.Instance.SetSessionGamepad(GamepadType.GAMEPAD_XBOX360);
        AppLifeTimeManager.Instance.GetWebSocket().Send_RegisterGamepad(GamepadType.GAMEPAD_XBOX360);
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
            ipInputField.onSelect.RemoveAllListeners();
            ipInputField.onDeselect.RemoveAllListeners();
        }

        if (portInputField != null)
        {
            portInputField.onSelect.RemoveAllListeners();
            portInputField.onDeselect.RemoveAllListeners();
        }

        CloseVirtualKeyboard();
    }

    private void HandleStateChange(AppLifeTimeManager.AppState state)
    {
        switch (state)
        {
            case AppLifeTimeManager.AppState.RequestingLogin:
                loginPanel.SetActive(true);
                selectGamepadPanel.SetActive(false);
                break;
            case AppLifeTimeManager.AppState.SelectingGamepad:
                loginPanel.SetActive(false);
                selectGamepadPanel.SetActive(true);
                CloseVirtualKeyboard();
                AdjustUIForKeyboard(false);
                break;
        }
    }
}
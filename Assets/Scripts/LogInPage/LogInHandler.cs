using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogInHandler : MonoBehaviour
{
    // UI element to change based on app state
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject selectGamepadPanel;
    [SerializeField] private TMP_Text helpText;

    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private TMP_InputField portInputField;

    private const string ipPrefKey = "server_ip";
    private const string portPrefKey = "server_port";
    void Start()
    {
        string savedIP = PlayerPrefs.GetString(ipPrefKey, "192.168.1.100");
        string savedPort = PlayerPrefs.GetString(portPrefKey, "8080");

        ipInputField.text = savedIP;
        portInputField.text = savedPort;
    }

    public async void Button_StartConnection()
    {
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
    }

    private void HandleStateChange(AppLifeTimeManager.AppState state)
    {
        switch (state)
        {
            case AppLifeTimeManager.AppState.RequestingLogin:
                loginPanel.SetActive(true);
                selectGamepadPanel.SetActive(false);
                //helpText.text = "Please log in to continue.";
                break;

            case AppLifeTimeManager.AppState.SelectingGamepad:
                loginPanel.SetActive(false);
                selectGamepadPanel.SetActive(true);
                //helpText.text = "Please select a gamepad to continue.";
                break;
        }
    }
}

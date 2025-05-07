using UnityEngine;
using NativeWebSocket;
using TMPro;
using MessagePack;
using UnityEngine.InputSystem;

public class WebSocketConnector : MonoBehaviour
{
    public bool connectionTesting = false; 
    // For connection
    public TMP_InputField inputField;
    public TMP_Text statusText;
    private WebSocket websocket;

    private const string defaultPort = "8080";
    private const string ipPrefKey = "server_ip";

    // To send message
    public TMP_InputField messageInputField;

    // For testing
    [SerializeField] private GamepadMocker gamepad;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the saved IP address from PlayerPrefs
        string savedIP = PlayerPrefs.GetString(ipPrefKey, "192.168.1.100");

        inputField.text = savedIP;
    }

    // Update is called once per frame
    void Update()
    {
#if !UNITY_WEBGL
//#if !UNITY_WEBGL || UNITY_EDITOR
        websocket?.DispatchMessageQueue();
#endif

        if (gamepad.IsConnected())
        {
            // Send gamepad state to server
            SendMessageToServer();
        }
    }

    private async void OnApplicationQuit()
    {
        if (websocket != null)
            await websocket.Close();
    }

    public async void TryConnect()
    {
        string ip = (connectionTesting)? "echo.websocket.org" : inputField.text;
        string port = (connectionTesting) ? "8080": defaultPort;

        string wsUrl = "ws://" + ip + ":" + port;


        PlayerPrefs.SetString(ipPrefKey, ip);

        string testing = (connectionTesting) ? "Testing" : "Connecting";
        statusText.text = testing + " to " + wsUrl;

        websocket = new WebSocket(wsUrl);
        //websocket = new WebSocket("wss://ws.postman-echo.com/raw");

        websocket.OnOpen += () =>
        {
            WebSockerPayload payload = new WebSockerPayload
            {
                action = WebSocketAction.handshake.ToString(),
                id = gamepad.GetUUID().ToString(),
                gamepadType = gamepad.GetGamepadType().ToString(),
                gamepadData = null
            };

            byte[] msgpackBytes = MessagePackSerializer.Serialize(payload);
            Debug.Log("Connection open!");
        };

        websocket.OnError += (e) =>
        {
            statusText.text = "Error! " + e;
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            statusText.text = "Connection closed!";
            Debug.Log("Connection closed!");
        };

        websocket.OnMessage += (bytes) =>
        {
            // May send with https://zh.wikipedia.org/wiki/%C3%98MQ
            WebSocketResponse response = MessagePackSerializer.Deserialize<WebSocketResponse>(bytes);

            if (response.action.Equals("handshake_ack"))
            {
                Debug.Log("Conencted! ");
            }
            else if (response.action.Equals("register_ack"))
            {
                gamepad.SetConnected(true);
                gamepad.SetGamepadID(int.Parse(response.payload));
                statusText.text = "Registered! " + response.payload;
                Debug.Log("Registered! " + response.payload);
            }
        };

        await websocket.Connect();
    }

    public void SendMessageToServer()
    {
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            GamepadData data = gamepad.GetGamepadStateAsJson();

            WebSockerPayload payload = new WebSockerPayload
            {
                action = WebSocketAction.input.ToString(),
                id = gamepad.GetUUID().ToString(),
                gamepadType = gamepad.GetGamepadType().ToString(),
                gamepadData = data
            };

            byte[] msgpackBytes = MessagePackSerializer.Serialize(payload);

            websocket.Send(msgpackBytes);
        }
        else
        {
            statusText.text = "Not connected!";
            Debug.Log("Send failed: not connected.");
        }
    }

    public void RegisterMyGamepad()
    {
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            WebSockerPayload payload = new WebSockerPayload
            {
                action = WebSocketAction.register.ToString(),
                id = null,
                gamepadType = gamepad.GetGamepadType().ToString(),
                gamepadData = null
            };

            byte[] msgpackBytes = MessagePackSerializer.Serialize(payload);

            websocket.Send(msgpackBytes);
        }
        else
        {
            statusText.text = "Not connected!";
            Debug.Log("Send failed: not connected.");
        }
    }
}

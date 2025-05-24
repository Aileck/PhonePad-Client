using UnityEngine;
using NativeWebSocket;
using TMPro;
using MessagePack;
using System.Collections;
using System;

public class WebSocketConnector : MonoBehaviour
{
    public bool connectionTesting = false; 
    // For connection
    public TMP_InputField inputField;
    public TMP_Text statusText;
    private WebSocket websocket;

    // Connection testing
    private bool isDelayTesting = false;

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

        // The ping coroutine is not neccesary for now
        //StartPingCoroutine();
    }

    // Update is called once per frame
    void Update()
    {
#if !UNITY_WEBGL  || UNITY_EDITOR
        websocket?.DispatchMessageQueue();
#endif

        if (gamepad.IsConnected())
        {
            // Send gamepad state to server
            HandleInputAction();
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
            WebSocketPayload payload = new WebSocketPayload
            {
                action = WebSocketAction.handshake.ToString(),
                id = gamepad.GetID(),
                gamepadType = gamepad.GetGamepadType().ToString(),
                gamepadData = null,
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
            Debug.Log("Message received: " + response.action + " " + response.payload);
            // On ACK
            if (response.action.Equals("handshake_ack"))
            {
                // Handshake acknowledged
                Debug.Log("Conencted! ");
            }
            else if (response.action.Equals("register_ack"))
            {
                gamepad.SetConnected(true);
                gamepad.SetGamepadID(int.Parse(response.payload));
                Debug.Log("Gamepad ID: " + response.payload);
                statusText.text = "Registered! " + response.payload;
                Debug.Log("Registered! " + response.payload);
            }

            // On Pedition
            else if (response.action.Equals("delay_test_request"))
            {
                long recerivedLocalTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                
                WebSocketPingPayload pingPayload = new WebSocketPingPayload
                {
                    action = WebSocketAction.delay_test_request_ack.ToString(),
                    id = gamepad.GetID(),
                    timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    payload = recerivedLocalTime.ToString()
                };

                byte[] msgpackBytes = MessagePackSerializer.Serialize(pingPayload);
                websocket.Send(msgpackBytes);

                isDelayTesting = true;

            }
            //else if (response.action.Equals("delay_test_start"))
            //{

            //}
            else if (response.action.Equals("delay_end"))
            {
                isDelayTesting = false;
            }

        };

        await websocket.Connect();
    }

    public void HandleInputAction()
    {
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            GamepadData data = gamepad.GetGamepadStateAsJson();

            WebSocketPayload payload = new WebSocketPayload
            {
                action = WebSocketAction.input.ToString(),
                id = gamepad.GetID(),
                gamepadType = gamepad.GetGamepadType().ToString(),
                gamepadData = data,
            };

            if(isDelayTesting)
            {
                payload.timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }
            byte[] msgpackBytes = MessagePackSerializer.Serialize(payload);

            websocket.Send(msgpackBytes);
        }
        else
        {
            statusText.text = "Not connected!";
            Debug.Log("Send failed: not connected.");
        }
    }

    public void HandleDisconnectAction()
    {
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            GamepadData data = gamepad.GetGamepadStateAsJson();

            WebSocketPayload payload = new WebSocketPayload
            {
                action = WebSocketAction.disconnect.ToString(),
                id = gamepad.GetID(),
                gamepadType = gamepad.GetGamepadType().ToString(),
                gamepadData = null,
            };

            if (isDelayTesting)
            {
                payload.timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }
            byte[] msgpackBytes = MessagePackSerializer.Serialize(payload);

            websocket.Send(msgpackBytes);
        }
        else
        {
            statusText.text = "Not connected!";
            Debug.Log("Send failed: not connected.");
        }
    }

    public void RegisterMyXboxGamepad()
    {
        Debug.Log("Registering gamepad...");
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            WebSocketPayload payload = new WebSocketPayload
            {
                action = WebSocketAction.register.ToString(),
                id = -1,
                gamepadType = GamepadType.GAMEPAD_XBOX360.ToString(),
                gamepadData = null
            };

            gamepad.SetGamepadType(GamepadType.GAMEPAD_XBOX360);

            byte[] msgpackBytes = MessagePackSerializer.Serialize(payload);

            websocket.Send(msgpackBytes);
            Debug.Log("Gamepad registered");

        }
        else
        {
            statusText.text = "Not connected!";
            Debug.Log("Send failed: not connected.");
        }
    }

    public void RegisterMyDualShockGamepad()
    {
        Debug.Log("Registering gamepad...");
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            WebSocketPayload payload = new WebSocketPayload
            {
                action = WebSocketAction.register.ToString(),
                id = -1,
                gamepadType = GamepadType.GAMEPAD_DUALSHOCK.ToString(),
                gamepadData = null
            };

            gamepad.SetGamepadType(GamepadType.GAMEPAD_DUALSHOCK);

            byte[] msgpackBytes = MessagePackSerializer.Serialize(payload);

            websocket.Send(msgpackBytes);
            Debug.Log("Gamepad registered");

        }
        else
        {
            statusText.text = "Not connected!";
            Debug.Log("Send failed: not connected.");
        }
    }

    //private void StartPingCoroutine()
    //{
    //    // Stop any existing ping coroutine 
    //    StopPingCoroutine();

    //    isPinging = true;
    //    pingCoroutine = StartCoroutine(PingRoutine());
    //}

    //private void StopPingCoroutine()
    //{
    //    if (pingCoroutine != null)
    //    {
    //        StopCoroutine(pingCoroutine);
    //        pingCoroutine = null;
    //    }
    //    isPinging = false;
    //}

    //private void SendPing()
    //{
    //    if (websocket != null && websocket.State == WebSocketState.Open)
    //    {
    //        WebSockerPayload payload = new WebSockerPayload
    //        {
    //            action = WebSocketAction.ping.ToString(),
    //            id = gamepad.GetID(),
    //            gamepadType = gamepad.GetGamepadType().ToString(),
    //            gamepadData = null
    //        };

    //        byte[] msgpackBytes = MessagePackSerializer.Serialize(payload);
    //        websocket.Send(msgpackBytes);
    //        Debug.Log("Ping sent");
    //    }
    //    else
    //    {
    //        Debug.Log("Cannot send ping: connection not open");
    //        StopPingCoroutine();
    //    }
    //}

    //private IEnumerator PingRoutine()
    //{
    //    // First step, send 10 pings in quick succession
    //    for (int i = 0; i < 10 && isPinging; i++)
    //    {
    //        SendPing();
    //        yield return new WaitForSeconds(0.5f);
    //    }

    //    // Second step, send pings every 5 seconds
    //    while (isPinging && websocket != null && websocket.State == WebSocketState.Open)
    //    {
    //        SendPing();
    //        yield return new WaitForSeconds(5f);
    //    }

    //}
}

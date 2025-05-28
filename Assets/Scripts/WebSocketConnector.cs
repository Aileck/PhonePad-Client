using UnityEngine;
using NativeWebSocket;
using TMPro;
using MessagePack;
using System.Collections;
using System;
using System.Threading.Tasks;

public class WebSocketConnector : MonoBehaviour
{
    public bool connectionTesting = false; 
    // For connection
    public TMP_InputField inputField;
    private WebSocket websocket;

    // Connection testing
    private bool isDelayTesting = false;

    private const string defaultPort = "8080";
    private const string ipPrefKey = "server_ip";

    // To send message
    public TMP_InputField messageInputField;

    // For testing
    [SerializeField] private GamepadMocker gamepad;

    private int sessionID = -1;
    private GamepadType sessionGamepad;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the saved IP address from PlayerPrefs
        //string savedIP = PlayerPrefs.GetString(ipPrefKey, "192.168.1.100");

        //inputField.text = savedIP;

        // The ping coroutine is not neccesary for now
        //StartPingCoroutine();
    }

    // Update is called once per frame
    void Update()
    {
#if !UNITY_WEBGL  || UNITY_EDITOR
        websocket?.DispatchMessageQueue();
#endif

        //if (gamepad.IsConnected())
        //{
        //    // Send gamepad state to server
        //    HandleInputAction();
        //}
    }

    private async void OnApplicationQuit()
    {
        if (websocket != null)
        {
            Send_Disconnect();
            await websocket.Close();
            websocket = null;

        }
    }

    public async void TryConnect()
    {
        string ip = (connectionTesting)? "echo.websocket.org" : inputField.text;
        string port = (connectionTesting) ? "8080": defaultPort;

        string wsUrl = "ws://" + ip + ":" + port;


        PlayerPrefs.SetString(ipPrefKey, ip);

        string testing = (connectionTesting) ? "Testing" : "Connecting";

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
            Send_Disconnect();
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            Send_Disconnect();
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

    public async Task<bool> Send_ConnectionPetition(string ip, string port)
    {
        string wsUrl = "ws://" + ip + ":" + port;
        websocket = new WebSocket(wsUrl);

        var connectionTcs = new TaskCompletionSource<bool>();

        websocket.OnOpen += () =>
        {
            WebSocketPayload payload = new WebSocketPayload
            {
                action = WebSocketAction.handshake.ToString(),
                id = -1,
                gamepadType = "",
                gamepadData = null,
            };

            byte[] msgpackBytes = MessagePackSerializer.Serialize(payload);

            websocket.Send(msgpackBytes); 

            Debug.Log("Connection open!");
            connectionTcs.SetResult(true); 
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);

            connectionTcs.SetResult(false); 
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");

            if (!connectionTcs.Task.IsCompleted)
                connectionTcs.SetResult(false);
        };

        websocket.OnMessage += (bytes) =>
        {
            WebSocketResponse response = MessagePackSerializer.Deserialize<WebSocketResponse>(bytes);
            Debug.Log("Message received: " + response.action + " " + response.payload);

            if (response.action.Equals("handshake_ack"))
            {
                Debug.Log("Connected! ");
            }
            else if (response.action.Equals("register_ack"))
            {
                sessionID = (int.Parse(response.payload));

                Debug.Log("Gamepad ID: " + response.payload);
            }
            else if (response.action.Equals("delay_test_request"))
            {
                long receivedLocalTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                WebSocketPingPayload pingPayload = new WebSocketPingPayload
                {
                    action = WebSocketAction.delay_test_request_ack.ToString(),
                    id = sessionID,
                    timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    payload = receivedLocalTime.ToString()
                };

                byte[] msgpackBytes = MessagePackSerializer.Serialize(pingPayload);

                websocket.Send(msgpackBytes);

                isDelayTesting = true;
            }
            else if (response.action.Equals("delay_end"))
            {
                isDelayTesting = false;
            }
        };
        Debug.Log("Try open connection!");

        websocket.Connect();

        var timeoutTask = Task.Delay(5000); // 5s timeout   

        var completedTask = await Task.WhenAny(connectionTcs.Task, timeoutTask);

        if (completedTask == timeoutTask)
        {
            Debug.Log("Connection timeout!");
            return false;
        }

        return connectionTcs.Task.Result;
    }

    public void Send_RegisterGamepad(GamepadType type)
    {
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            WebSocketPayload payload = new WebSocketPayload
            {
                action = WebSocketAction.register.ToString(),
                id = -1,
                gamepadType = type.ToString(),
                gamepadData = null
            };

            sessionGamepad = type;

            byte[] msgpackBytes = MessagePackSerializer.Serialize(payload);
            websocket.Send(msgpackBytes);
            Debug.Log("Gamepad registered");
        }
        else
        {
            Debug.Log("Send failed: not connected.");
        }
    }

    public void Send_Input(GamepadData data)
    {
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            WebSocketPayload payload = new WebSocketPayload
            {
                action = WebSocketAction.input.ToString(),
                id = sessionID,
                gamepadType = sessionGamepad.ToString(),
                gamepadData = data,
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
            Debug.Log("Send failed: not connected.");
        }
    }

    public void Send_Disconnect()
    {
        if (websocket.State == WebSocketState.Open)
        {
            WebSocketPayload payload = new WebSocketPayload
            {
                action = WebSocketAction.disconnect.ToString(),
                id = sessionID,
                gamepadType = sessionGamepad.ToString(),
                gamepadData = null,
            };

            if (isDelayTesting)
            {
                payload.timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }
            byte[] msgpackBytes = MessagePackSerializer.Serialize(payload);

            websocket.Send(msgpackBytes);
        }
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
            Debug.Log("Send failed: not connected.");
        }
    }

    public GamepadType GetSessionGamepadType()
    {
        return sessionGamepad;
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

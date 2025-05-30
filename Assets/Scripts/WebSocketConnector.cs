using UnityEngine;
using NativeWebSocket;
using TMPro;
using MessagePack;
using System.Collections;
using System;
using System.Threading.Tasks;

public class WebSocketConnector : MonoBehaviour
{
    private WebSocket websocket;

    // Connection testing
    private bool isDelayTesting = false;

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
                int id = (int.Parse(response.payload));
                AppLifeTimeManager.Instance.SetSessionID(id);

                Debug.Log("Gamepad ID: " + response.payload);
            }
            else if (response.action.Equals("delay_test_request"))
            {
                long receivedLocalTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                WebSocketPingPayload pingPayload = new WebSocketPingPayload
                {
                    action = WebSocketAction.delay_test_request_ack.ToString(),
                    id = AppLifeTimeManager.Instance.GetSessionID(),
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

            AppLifeTimeManager.Instance.SetSessionGamepad(type);

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
                id = AppLifeTimeManager.Instance.GetSessionID(),
                gamepadType = AppLifeTimeManager.Instance.GetSessionGamepad().ToString(),
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
                id = AppLifeTimeManager.Instance.GetSessionID(),
                gamepadType = AppLifeTimeManager.Instance.GetSessionGamepad().ToString(),
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

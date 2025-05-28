using TMPro;
using UnityEngine;
using MessagePack;
using NativeWebSocket;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;
using System;
using UnityEngine.InputSystem;

public class WebSocketBridge : MonoBehaviour
{
    public TMP_InputField ipInputField;
    public TMP_InputField portInputField;

    private const string defaultPort = "8080";

    private const string ipPrefKey = "server_ip";
    private const string portPrefKey = "server_port";

    private WebSocket websocket;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the saved IP address from PlayerPrefs
        string savedIP = PlayerPrefs.GetString(ipPrefKey, "192.168.1.100");
        string savedPort = PlayerPrefs.GetString(portPrefKey, defaultPort);

        ipInputField.text = savedIP;
        portInputField.text = savedPort;
    }

    // Update is called once per frame
    void Update()
    {
        #if !UNITY_WEBGL || UNITY_EDITOR
                websocket?.DispatchMessageQueue();
        #endif
    }

    private async void OnApplicationQuit()
    {
        if (websocket != null)
            await websocket.Close();
    }

    public async void Connect()
    {
        string ip = ipInputField.text;
        string port = portInputField.text;

        string wsUrl = "ws://" + ip + ":" + port;


        PlayerPrefs.SetString(ipPrefKey, ip);
        PlayerPrefs.SetString(portPrefKey, port);

        websocket = new WebSocket(wsUrl);

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
            Debug.Log("Connection open!");

            // 
            if (websocket.State == WebSocketState.Open)
            {
                websocket.Send(msgpackBytes);
                Debug.Log("Handshake sent!");
            }
            else
            {
                Debug.LogError("WebSocket is not open!");
            }
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };

        websocket.OnMessage += (bytes) =>
        {
            WebSocketResponse response = MessagePackSerializer.Deserialize<WebSocketResponse>(bytes);
            // On ACK
            if (response.action.Equals("handshake_ack"))
            {
                // Handshake acknowledged
                Debug.Log("Conencted! ");
            }
            else if (response.action.Equals("register_ack"))
            {
                
            }
        };

        await websocket.Connect();
    }
}

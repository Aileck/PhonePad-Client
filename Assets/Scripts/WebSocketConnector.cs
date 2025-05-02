using UnityEngine;
using NativeWebSocket;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the saved IP address from PlayerPrefs
        string savedIP = PlayerPrefs.GetString(ipPrefKey, "192.168.1.100");

        inputField.text = savedIP;

        StartCoroutine(TestInternetConnection());

    }

    IEnumerator TestInternetConnection()
    {
        UnityWebRequest request = UnityWebRequest.Get("https://www.google.com"); 
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Internet");
        }
        else
        {
            Debug.LogWarning("No Internet " + request.error);
        }
    }

    // Update is called once per frame
    void Update()
    {
#if !UNITY_WEBGL
//#if !UNITY_WEBGL || UNITY_EDITOR
        websocket?.DispatchMessageQueue();
#endif
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
            statusText.text = "Connection open!";
            websocket.SendText("Hello from Unity!");
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

            var message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("Received OnMessage! (" + bytes.Length + " bytes) " + message);
        };

        await websocket.Connect();
    }

    public void SendMessageToServer()
    {
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            string message = messageInputField.text;
            websocket.SendText(message);
            statusText.text = "Sent: " + message;
            Debug.Log("Sent message: " + message);
        }
        else
        {
            statusText.text = "Not connected!";
            Debug.Log("Send failed: not connected.");
        }
    }
}

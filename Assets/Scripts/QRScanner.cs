using UnityEngine;
using UnityEngine.UI;
using ZXing;

// Json format to handle from QR code
[System.Serializable]
public class ServerInfo
{
    public string ip;
    public string port;
}

public class QRScanner : MonoBehaviour
{
    public GameObject cameraPanel;
    public RawImage cameraDisplay;

    private WebCamTexture webcamTexture;
    private IBarcodeReader barcodeReader = new BarcodeReader();
    private bool isScanning = false;

    void Start()
    {
    }

    void Update()
    {
        if (webcamTexture != null && webcamTexture.isPlaying && webcamTexture.width > 100)
        {
            try
            {
                Color32[] pixels = webcamTexture.GetPixels32();
                int width = webcamTexture.width;
                int height = webcamTexture.height;

                string result = barcodeReader.Decode(pixels, width, height).Text;
                if (result != null)
                {
                    TryParseServerInfo(result, out ServerInfo info);

                    if (info != null)
                    {
                        TryConnect(info.port, info.ip);
                        StopCamera();
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("Error：" + ex.Message);
            }
        }
    }

    public void StartCamera()
    {
        if (webcamTexture == null)
        {
            webcamTexture = new WebCamTexture();
            cameraDisplay.texture = webcamTexture;
            cameraDisplay.material.mainTexture = webcamTexture;
        }

        webcamTexture.Play();
        isScanning = true;
        cameraPanel.SetActive(true);
    }

    public void StopCamera()
    {
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            webcamTexture.Stop();
        }
        isScanning = false;
        cameraPanel.SetActive(false);
    }

    void TryParseServerInfo(string json, out ServerInfo serverInfo)
    {
        serverInfo = null;
        try
        {
            serverInfo = JsonUtility.FromJson<ServerInfo>(json);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to parse JSON: " + ex.Message);
        }
    }

    private async void TryConnect(string port, string ip)
    {
        bool isConnected = await AppLifeTimeManager.Instance.GetWebSocket().Send_ConnectionPetition(ip, port);

        if (isConnected)
        {
            Debug.Log("Connection successful!");
            cameraDisplay.gameObject.SetActive(false);
            AppLifeTimeManager.Instance.ToSelectGamepad();

        }
        else
        {

        }
        Debug.Log("End of function!");
    }

    void OnDestroy()
    {
        if (webcamTexture != null)
        {
            webcamTexture.Stop();
        }
    }
}


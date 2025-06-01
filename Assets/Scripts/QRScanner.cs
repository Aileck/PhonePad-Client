using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;
using System.Collections;
using ZXing;

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
        StartCoroutine(RequestCameraPermissionAndStart());
    }

    private IEnumerator RequestCameraPermissionAndStart()
    {
        if (!HasCameraPermission())
        {
            Debug.Log("Camera permission not granted, requesting permission...");

            RequestCameraPermission();

            yield return new WaitUntil(() => HasCameraPermission() || PermissionDenied());

            if (!HasCameraPermission())
            {
                Debug.LogError("Camera permission denied. Cannot start camera.");
                OnCameraPermissionDenied();
                yield break;
            }
        }

        if (WebCamTexture.devices.Length == 0)
        {
            ShowToast("No camera available on this device");
            yield break;
        }

        Debug.Log("Camera permission granted, starting camera...");
        StartCameraInternal();
    }

    private bool HasCameraPermission()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return Permission.HasUserAuthorizedPermission(Permission.Camera);
#elif UNITY_IOS && !UNITY_EDITOR
        return Application.HasUserAuthorization(UserAuthorization.WebCam);
#else
        return true;
#endif
    }

    private void RequestCameraPermission()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Permission.RequestUserPermission(Permission.Camera);
#elif UNITY_IOS && !UNITY_EDITOR
        Application.RequestUserAuthorization(UserAuthorization.WebCam);
#endif
    }

    private bool PermissionDenied()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return !Permission.HasUserAuthorizedPermission(Permission.Camera) && 
               !AndroidPermissionHelper.IsPermissionRequestInProgress();
#elif UNITY_IOS && !UNITY_EDITOR
        return Application.HasUserAuthorization(UserAuthorization.WebCam) == false;
#else
        return false;
#endif
    }

    private void OnCameraPermissionDenied()
    {
        Debug.LogError("Camera permission is required to scan QR codes.");
        ShowToast("Camera permission denied");
        ShowPermissionDeniedDialog();
    }

    private void ShowPermissionDeniedDialog()
    {
        Debug.Log("Please grant camera permission in device settings to use QR scanner feature.");
    }

    private void ShowToast(string message)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast"))
        {
            currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                using (AndroidJavaObject toast = toastClass.CallStatic<AndroidJavaObject>("makeText", currentActivity, message, 1))
                {
                    toast.Call("show");
                }
            }));
        }
#endif
    }

    private void StartCameraInternal()
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
            cameraDisplay.gameObject.SetActive(false);
            AppLifeTimeManager.Instance.ToSelectGamepad();

        }
        else
        {

        }
    }

    void OnDestroy()
    {
        if (webcamTexture != null)
        {
            webcamTexture.Stop();
        }
    }
}

#if UNITY_ANDROID && !UNITY_EDITOR
public static class AndroidPermissionHelper
{
    private static bool permissionRequestInProgress = false;
    
    public static bool IsPermissionRequestInProgress()
    {
        return permissionRequestInProgress;
    }
    
    public static void SetPermissionRequestInProgress(bool inProgress)
    {
        permissionRequestInProgress = inProgress;
    }
}
#endif
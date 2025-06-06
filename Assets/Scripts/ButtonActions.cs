using UnityEngine;

public class ButtonActions : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OpenURL(string url)
    {
        Application.OpenURL(url);
        Debug.Log("Opening URL: " + url);
    }

    public void CopyToClipboard(string text)
    {
        GUIUtility.systemCopyBuffer = text;
        Debug.Log("Copied to clipboard: " + text);
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast"))
            {
                activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
                    AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
                    AndroidJavaObject toast = toastClass.CallStatic<AndroidJavaObject>("makeText", context, "menu_sponsor_copied", toastClass.GetStatic<int>("LENGTH_SHORT"));
                    toast.Call(i18nManager.Instance.Translate("menu_sponsor_copied"));
                }));
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Toast failed: " + e.Message);
        }
#endif
    }
}

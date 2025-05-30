using UnityEngine;

public class PlayingHandler : MonoBehaviour
{
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Button_Disconnect()
    {
        AppLifeTimeManager.Instance.GetWebSocket().Send_Disconnect();
        AppLifeTimeManager.Instance.ToQuestLogin();
    }
}

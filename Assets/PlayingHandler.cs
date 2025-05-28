using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayingHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

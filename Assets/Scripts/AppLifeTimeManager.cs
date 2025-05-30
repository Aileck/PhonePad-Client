using UnityEngine;
using UnityEngine.SceneManagement;

public class AppLifeTimeManager : MonoBehaviour
{
    public enum AppState
    {
        RequestingLogin,
        SelectingGamepad,
        Playing,
    }

    public enum Error
    {
        None,
        NoGamepad,
        GamepadNotSupported,
    }

    private static AppLifeTimeManager _instance;
    public static AppLifeTimeManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<AppLifeTimeManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("AppLifeTimeManager");
                    _instance = go.AddComponent<AppLifeTimeManager>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    public static event System.Action<AppState> OnStateChanged;

    private AppState[] appStates;
    private AppState currentAppState;

    // Unique global web socket connector instance
    private WebSocketConnector webSocketConnector;

    [SerializeField] int SessionID = -1;
    [SerializeField] GamepadType sessionGamepad = GamepadType.GAMEPAD_XBOX360;
    [SerializeField] int sessionConfigProfileIndex = 0;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (_instance == this)
        {
            appStates = (AppState[])System.Enum.GetValues(typeof(AppState));
        }
    }

    private void Initialize()
    {
        appStates = (AppState[])System.Enum.GetValues(typeof(AppState));
        currentAppState = AppState.RequestingLogin;

        // App configuration
        Application.targetFrameRate = 120;
        QualitySettings.vSyncCount = 0;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        webSocketConnector = gameObject.GetComponent<WebSocketConnector>();
    }

    public AppState GetCurrentState()
    {
        return currentAppState;
    }

    public int GetSessionID()
    {
        return SessionID;
    }

    public void SetSessionID(int sessionID)
    {
        SessionID = sessionID;
    }

    public GamepadType GetSessionGamepad()
    {
        return sessionGamepad;
    }

    public void SetSessionGamepad(GamepadType gamepadType)
    {
        sessionGamepad = gamepadType;
    }

    public int GetSessionConfigProfileIndex()
    {
        return sessionConfigProfileIndex;
    }

    public void SetSessionConfigProfileIndex(int index)
    {
        sessionConfigProfileIndex = index;
    }
    //public AppState CurrentState => currentAppState;


    public void NextState()
    {
        int currentIndex = System.Array.IndexOf(appStates, currentAppState);
        currentAppState = appStates[(currentIndex + 1) % appStates.Length];
        InvokeStateChangeEvent();
    }

    public WebSocketConnector GetWebSocket()
    {
        return webSocketConnector;
    }

    public void ToQuestLogin()
    {
        currentAppState = AppState.RequestingLogin;
        SceneManager.LoadScene("Login");
        InvokeStateChangeEvent();
    }

    public void ToSelectGamepad()
    {
        currentAppState = AppState.SelectingGamepad;
        InvokeStateChangeEvent();
    }

    public void ToPlaying()
    {
        currentAppState = AppState.Playing;

        // Change scene
        SceneManager.LoadScene("Play");
        InvokeStateChangeEvent();
    }

    private void InvokeStateChangeEvent()
    {
        OnStateChanged?.Invoke(currentAppState);
    }
}
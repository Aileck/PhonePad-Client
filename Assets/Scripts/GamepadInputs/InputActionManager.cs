using UnityEngine;
using UnityEngine.InputSystem;

public enum GamepadAction
{
    ButtonEast,
    ButtonSouth,
    ButtonWest,
    ButtonNorth,

    Up,
    Down,
    Left,
    Right,

    LeftShoulder,
    RightShoulder,

    LeftTrigger,
    RightTrigger,

    LeftStickButton,
    RightStickButton,

    LeftStick,
    RightStick,

    Start,
    Select,

    PS_TouchBarPress
}

public class InputActionManager : MonoBehaviour
{
    [SerializeField]
    private InputActionAsset gamepadMockActions;

    private static InputActionManager instance;

    private InputActionMap actionMap;

    private InputAction buttonEast;
    private InputAction buttonSouth;
    private InputAction buttonWest;
    private InputAction buttonNorth;
    private InputAction up;
    private InputAction down;
    private InputAction left;
    private InputAction right;
    private InputAction leftShoulder;
    private InputAction rightShoulder;
    private InputAction leftTrigger;
    private InputAction rightTrigger;
    private InputAction leftStickButton;
    private InputAction rightStickButton;
    private InputAction leftStick;
    private InputAction rightStick;
    private InputAction start;
    private InputAction select;
    private InputAction psTouchBarPress;

    public static InputActionManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("InputActionManager");
                instance = obj.AddComponent<InputActionManager>();
                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        actionMap = gamepadMockActions.FindActionMap("Mock");

        if (actionMap == null)
        {
            Debug.LogError("Failed to find 'Mock' action map in GamepadMock input actions asset.");
            return;
        }

        InitializeActions();

        actionMap.Enable();

        Debug.Log("InputActionManager initialized and action map enabled.");
    }

    private void InitializeActions()
    {
        buttonEast = actionMap.FindAction("ButtonEast");
        buttonSouth = actionMap.FindAction("ButtonSouth");
        buttonWest = actionMap.FindAction("ButtonWest");
        buttonNorth = actionMap.FindAction("ButtonNorth");
        up = actionMap.FindAction("Up");
        down = actionMap.FindAction("Down");
        left = actionMap.FindAction("Left");
        right = actionMap.FindAction("Right");
        leftShoulder = actionMap.FindAction("LeftShoulder");
        rightShoulder = actionMap.FindAction("RightShoulder");
        leftTrigger = actionMap.FindAction("LeftTrigger");
        rightTrigger = actionMap.FindAction("RightTrigger");
        leftStickButton = actionMap.FindAction("LeftStickButton");
        rightStickButton = actionMap.FindAction("RightStickButton");
        leftStick = actionMap.FindAction("LeftStick");
        rightStick = actionMap.FindAction("RightStick");
        start = actionMap.FindAction("Start");
        select = actionMap.FindAction("Select");
        psTouchBarPress = actionMap.FindAction("PS_TouchBarPress");
    }

    private void OnDestroy()
    {
        if (actionMap != null)
        {
            actionMap.Disable();
        }
    }

    public InputAction GetAction(GamepadAction action)
    {
        switch(action)
        {
            case GamepadAction.ButtonEast: return buttonEast;
            case GamepadAction.ButtonSouth: return buttonSouth;
            case GamepadAction.ButtonWest: return buttonWest;
            case GamepadAction.ButtonNorth: return buttonNorth;
            case GamepadAction.Up: return up;
            case GamepadAction.Down: return down;
            case GamepadAction.Left: return left;
            case GamepadAction.Right: return right;
            case GamepadAction.LeftShoulder: return leftShoulder;
            case GamepadAction.RightShoulder: return rightShoulder;
            case GamepadAction.LeftTrigger: return leftTrigger;
            case GamepadAction.RightTrigger: return rightTrigger;
            case GamepadAction.LeftStickButton: return leftStickButton;
            case GamepadAction.RightStickButton: return rightStickButton;
            case GamepadAction.LeftStick: return leftStick;
            case GamepadAction.RightStick: return rightStick;
            case GamepadAction.Start: return start;
            case GamepadAction.Select: return select;
            case GamepadAction.PS_TouchBarPress: return psTouchBarPress;
            default:
                Debug.LogError("Invalid action requested.");
                return null;
        }
    }
}
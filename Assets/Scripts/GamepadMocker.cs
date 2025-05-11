using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadMocker: MonoBehaviour
{
    [SerializeField] private GamepadConfig gamepadConfig;

    [SerializeField] private StickComponent leftStick;
    [SerializeField] private StickComponent rightStick;

    [SerializeField] private DPadComponent dPad;

    [SerializeField] private ButtonComponent buttonNorth;
    [SerializeField] private ButtonComponent buttonSouth;
    [SerializeField] private ButtonComponent buttonWest;
    [SerializeField] private ButtonComponent buttonEast;

    [SerializeField] private ButtonComponent leftStickButton;
    [SerializeField] private ButtonComponent rightStickButton;

    [SerializeField] private ButtonComponent leftTrigger;
    [SerializeField] private ButtonComponent rightTrigger;

    [SerializeField] private ButtonComponent leftShoulder;
    [SerializeField] private ButtonComponent rightShoulder;

    [SerializeField] private ButtonComponent buttonStart;
    [SerializeField] private ButtonComponent buttonSelect;


    private int gamepadID;
    private GamepadType gamepadType;
    private bool connected = false;

    private void Awake()
    {
        gamepadID = -1;

        leftStick.SetConfig(gamepadConfig);
        rightStick.SetConfig(gamepadConfig);

        dPad.SetConfig(gamepadConfig);

        buttonNorth.SetConfig(gamepadConfig);
        buttonSouth.SetConfig(gamepadConfig);
        buttonWest.SetConfig(gamepadConfig);
        buttonEast.SetConfig(gamepadConfig);

        leftStickButton.SetConfig(gamepadConfig);
        rightStickButton.SetConfig(gamepadConfig);

        leftShoulder.SetConfig(gamepadConfig);
        rightShoulder.SetConfig(gamepadConfig);

        leftTrigger.SetConfig(gamepadConfig);
        rightTrigger.SetConfig(gamepadConfig);

        buttonStart.SetConfig(gamepadConfig);
        buttonSelect.SetConfig(gamepadConfig);
    }

    public GamepadMocker(GamepadType type)
    {
        gamepadID = -1;
        gamepadType = type; 
    }

    // ---------------
    // Public methods
    // ---------------

    public int GetID()
    {
        return gamepadID;
    }

    public void SetGamepadType(GamepadType type)
    {
        gamepadType = type;
        // Notify the gamepad type change to the system or other components
    }

    public GamepadType GetGamepadType()
    {
        return gamepadType;
    }

    public Vector2 GetLeftStickInput()
    {
        return leftStick.GetJoystickInput();
    }

    public void SetGamepadID(int id)
    {
        gamepadID = id;
    }

    public void SetConnected(bool isConnected)
    {
        connected = isConnected;
    }

    public bool IsConnected()
    {
        return connected;
    }
    public GamepadData GetGamepadStateAsJson()
    {
        GamepadData data = new GamepadData
        {
            buttonEast = buttonEast.GetButtonInput(),
            buttonWest = buttonWest.GetButtonInput(),
            buttonNorth = buttonNorth.GetButtonInput(),
            buttonSouth = buttonSouth.GetButtonInput(),

            up = dPad.GetUpInput(),
            down = dPad.GetDownInput(),
            left = dPad.GetLeftInput(),
            right = dPad.GetRightInput(),

            leftShoulder = leftShoulder.GetButtonInput(),
            rightShoulder = rightShoulder.GetButtonInput(),

            leftTrigger = leftTrigger.GetButtonInput(),
            rightTrigger = rightTrigger.GetButtonInput(),

            leftStickButton = leftStickButton.GetButtonInput(),
            rightStickButton = rightStickButton.GetButtonInput(),

            leftStickX = leftStick.GetJoystickInput().x,
            leftStickY = leftStick.GetJoystickInput().y,

            rightStickX = rightStick.GetJoystickInput().x,
            rightStickY = rightStick.GetJoystickInput().y,

            buttonStart = buttonStart.GetButtonInput(),
            buttonSelect = buttonSelect.GetButtonInput(),
        };

        return data;
    }
}

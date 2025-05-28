using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static GamepadConfig;

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


    private int gamepadID = -1; // -1 means not assigned
    [SerializeField] private GamepadType gamepadType;
    private bool connected = false;

    private void Start()
    {
        gamepadType = AppLifeTimeManager.Instance.GetWebSocket().GetSessionGamepadType();

        // Should not initialize here
        //gamepadConfig.Initialize();

        // TODO: Fefactr to optimize this S/H/*/T
        if (gamepadType == GamepadType.GAMEPAD_XBOX360)
        {
            XboxProfile profile = gamepadConfig.xboxProfiles[0];

            leftStick.SetProfile(profile);
            leftStick.SetNormalizedPosition(profile.leftStick.position);
            //leftStick.SetScale(profile.leftStick.scale);

            rightStick.SetProfile(profile);
            rightStick.SetNormalizedPosition(profile.rightStick.position);
            //rightStick.SetScale(profile.rightStick.scale);

            dPad.SetProfile(profile);
            dPad.SetNormalizedPosition(profile.dPad.position);
            dPad.SetScale(profile.dPad.scale);

            buttonNorth.SetProfile(profile);
            buttonNorth.SetNormalizedPosition(profile.buttonNorth.position);
            //buttonNorth.SetScale(profile.buttonNorth.scale);
            buttonNorth.SetIcon(profile.buttonNorth.iconImage);

            buttonSouth.SetProfile(profile);
            buttonSouth.SetNormalizedPosition(profile.buttonSouth.position);
            //buttonSouth.SetScale(profile.buttonSouth.scale);
            buttonSouth.SetIcon(profile.buttonSouth.iconImage);

            buttonWest.SetProfile(profile);
            buttonWest.SetNormalizedPosition(profile.buttonWest.position);
            //buttonWest.SetScale(profile.buttonWest.scale);
            buttonWest.SetIcon(profile.buttonWest.iconImage);

            buttonEast.SetProfile(profile);
            buttonEast.SetNormalizedPosition(profile.buttonEast.position);
            //buttonEast.SetScale(profile.buttonEast.scale);
            buttonEast.SetIcon(profile.buttonEast.iconImage);

            leftStickButton.SetProfile(profile);
            leftStickButton.SetNormalizedPosition(profile.leftStickButton.position);
            //leftStickButton.SetScale(profile.leftStickButton.scale);
            leftStickButton.SetIcon(profile.leftStickButton.iconImage);

            rightStickButton.SetProfile(profile);
            rightStickButton.SetNormalizedPosition(profile.rightStickButton.position);
            //rightStickButton.SetScale(profile.rightStickButton.scale);
            rightStickButton.SetIcon(profile.rightStickButton.iconImage);

            leftShoulder.SetProfile(profile);
            leftShoulder.SetNormalizedPosition(profile.leftShoulder.position);
            //leftShoulder.SetScale(profile.leftShoulder.scale);
            leftShoulder.SetIcon(profile.leftShoulder.iconImage);

            rightShoulder.SetProfile(profile);
            rightShoulder.SetNormalizedPosition(profile.rightShoulder.position);
            //rightShoulder.SetScale(profile.rightShoulder.scale);
            rightShoulder.SetIcon(profile.rightShoulder.iconImage);

            leftTrigger.SetProfile(profile);
            leftTrigger.SetNormalizedPosition(profile.leftTrigger.position);
            //leftTrigger.SetScale(profile.leftTrigger.scale);
            leftTrigger.SetIcon(profile.leftTrigger.iconImage);

            rightTrigger.SetProfile(profile);
            rightTrigger.SetNormalizedPosition(profile.rightTrigger.position);
            //rightTrigger.SetScale(profile.rightTrigger.scale);
            rightTrigger.SetIcon(profile.rightTrigger.iconImage);

            buttonStart.SetProfile(profile);
            buttonStart.SetNormalizedPosition(profile.startButton.position);
            //buttonStart.SetScale(profile.startButton.scale);
            buttonStart.SetIcon(profile.startButton.iconImage);

            buttonSelect.SetProfile(profile);
            buttonSelect.SetNormalizedPosition(profile.selectButton.position);
            //buttonSelect.SetScale(profile.selectButton.scale);
            buttonSelect.SetIcon(profile.selectButton.iconImage);
        }
        else {
            DualShockProfile profile = gamepadConfig.dualShockProfiles[0];

            leftStick.SetProfile(profile);
            leftStick.SetNormalizedPosition(profile.leftStick.position);
            //leftStick.SetScale(profile.leftStick.scale);

            rightStick.SetProfile(profile);
            rightStick.SetNormalizedPosition(profile.rightStick.position);
            //rightStick.SetScale(profile.rightStick.scale);

            dPad.SetProfile(profile);
            dPad.SetNormalizedPosition(profile.dPad.position);
            dPad.SetScale(profile.dPad.scale);

            buttonNorth.SetProfile(profile);
            buttonNorth.SetNormalizedPosition(profile.buttonNorth.position);
            //buttonNorth.SetScale(profile.buttonNorth.scale);
            buttonNorth.SetIcon(profile.buttonNorth.iconImage);

            buttonSouth.SetProfile(profile);
            buttonSouth.SetNormalizedPosition(profile.buttonSouth.position);
            //buttonSouth.SetScale(profile.buttonSouth.scale);
            buttonSouth.SetIcon(profile.buttonSouth.iconImage);

            buttonWest.SetProfile(profile);
            buttonWest.SetNormalizedPosition(profile.buttonWest.position);
            //buttonWest.SetScale(profile.buttonWest.scale);
            buttonWest.SetIcon(profile.buttonWest.iconImage);

            buttonEast.SetProfile(profile);
            buttonEast.SetNormalizedPosition(profile.buttonEast.position);
            //buttonEast.SetScale(profile.buttonEast.scale);
            buttonEast.SetIcon(profile.buttonEast.iconImage);

            leftStickButton.SetProfile(profile);
            leftStickButton.SetNormalizedPosition(profile.leftStickButton.position);
            //leftStickButton.SetScale(profile.leftStickButton.scale);
            leftStickButton.SetIcon(profile.leftStickButton.iconImage);

            rightStickButton.SetProfile(profile);
            rightStickButton.SetNormalizedPosition(profile.rightStickButton.position);
            //rightStickButton.SetScale(profile.rightStickButton.scale);
            rightStickButton.SetIcon(profile.rightStickButton.iconImage);

            leftShoulder.SetProfile(profile);
            leftShoulder.SetNormalizedPosition(profile.leftShoulder.position);
            //leftShoulder.SetScale(profile.leftShoulder.scale);
            leftShoulder.SetIcon(profile.leftShoulder.iconImage);

            rightShoulder.SetProfile(profile);
            rightShoulder.SetNormalizedPosition(profile.rightShoulder.position);
            //rightShoulder.SetScale(profile.rightShoulder.scale);
            rightShoulder.SetIcon(profile.rightShoulder.iconImage);

            leftTrigger.SetProfile(profile);
            leftTrigger.SetNormalizedPosition(profile.leftTrigger.position);
            //leftTrigger.SetScale(profile.leftTrigger.scale);
            leftTrigger.SetIcon(profile.leftTrigger.iconImage);

            rightTrigger.SetProfile(profile);
            rightTrigger.SetNormalizedPosition(profile.rightTrigger.position);
            //rightTrigger.SetScale(profile.rightTrigger.scale);
            rightTrigger.SetIcon(profile.rightTrigger.iconImage);

            buttonStart.SetProfile(profile);
            buttonStart.SetNormalizedPosition(profile.startButton.position);
            //buttonStart.SetScale(profile.startButton.scale);
            buttonStart.SetIcon(profile.startButton.iconImage);

            buttonSelect.SetProfile(profile);
            buttonSelect.SetNormalizedPosition(profile.selectButton.position);
            //buttonSelect.SetScale(profile.selectButton.scale);
            buttonSelect.SetIcon(profile.selectButton.iconImage);
        }
         

    }

    public GamepadMocker(GamepadType type)
    {
        //gamepadID = -1;
        gamepadType = type; 
    }

    public void Update()
    {
        AppLifeTimeManager.Instance.GetWebSocket().Send_Input(GetGamepadStateAsJson());
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

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
    private bool connected = false;

    private void Start()
    {
        SetButtons();
    }

    public void Update()
    {
        AppLifeTimeManager.Instance.GetWebSocket().Send_Input(GetGamepadStateAsJson());
    }

    private void OnEnable()
    {
        SetButtons();
    }

    private void SetButtons()
    {
        int currentProfileIndex = AppLifeTimeManager.Instance.GetSessionConfigProfileIndex();
        GamepadType gamepadType = AppLifeTimeManager.Instance.GetSessionGamepad();

        if (gamepadType == GamepadType.GAMEPAD_XBOX360)
        {
            XboxProfile profile = gamepadConfig.xboxProfiles[currentProfileIndex];

            leftStick.SetProfile(profile);
            leftStick.SetNormalizedPosition(profile.leftStick.position);
            //leftStick.SetScale(profile.leftStick.scale);
            leftStick.SetVisibility(profile.leftStick.isVisible);
            leftStick.SetPressToActivate(profile.leftStick.pressToActivate);
            leftStick.SetToggleActive(profile.leftStick.toggle);

            rightStick.SetProfile(profile);
            rightStick.SetNormalizedPosition(profile.rightStick.position);
            //rightStick.SetScale(profile.rightStick.scale);
            rightStick.SetVisibility(profile.rightStick.isVisible);
            rightStick.SetPressToActivate(profile.rightStick.pressToActivate);
            rightStick.SetToggleActive(profile.rightStick.toggle);

            dPad.SetProfile(profile);
            dPad.SetNormalizedPosition(profile.dPad.position);
            dPad.SetScale(profile.dPad.scale);
            dPad.SetVisibility(profile.dPad.isVisible);
            dPad.SetPressToActivate(profile.dPad.pressToActivate);
            dPad.SetToggleActive(profile.dPad.toggle);

            buttonNorth.SetProfile(profile);
            buttonNorth.SetNormalizedPosition(profile.buttonNorth.position);
            //buttonNorth.SetScale(profile.buttonNorth.scale);
            buttonNorth.SetIcon(profile.buttonNorth.iconImage);
            buttonNorth.SetVisibility(profile.buttonNorth.isVisible);
            buttonNorth.SetPressToActivate(profile.buttonNorth.pressToActivate);
            buttonNorth.SetToggleActive(profile.buttonNorth.toggle);

            buttonSouth.SetProfile(profile);
            buttonSouth.SetNormalizedPosition(profile.buttonSouth.position);
            //buttonSouth.SetScale(profile.buttonSouth.scale);
            buttonSouth.SetIcon(profile.buttonSouth.iconImage);
            buttonSouth.SetVisibility(profile.buttonSouth.isVisible);
            buttonSouth.SetPressToActivate(profile.buttonSouth.pressToActivate);
            buttonSouth.SetToggleActive(profile.buttonSouth.toggle);

            buttonWest.SetProfile(profile);
            buttonWest.SetNormalizedPosition(profile.buttonWest.position);
            //buttonWest.SetScale(profile.buttonWest.scale);
            buttonWest.SetIcon(profile.buttonWest.iconImage);
            buttonWest.SetVisibility(profile.buttonWest.isVisible);
            buttonWest.SetPressToActivate(profile.buttonWest.pressToActivate);
            buttonWest.SetToggleActive(profile.buttonWest.toggle);

            buttonEast.SetProfile(profile);
            buttonEast.SetNormalizedPosition(profile.buttonEast.position);
            //buttonEast.SetScale(profile.buttonEast.scale);
            buttonEast.SetIcon(profile.buttonEast.iconImage);
            buttonEast.SetVisibility(profile.buttonEast.isVisible);
            buttonEast.SetPressToActivate(profile.buttonEast.pressToActivate);
            buttonEast.SetToggleActive(profile.buttonEast.toggle);

            leftStickButton.SetProfile(profile);
            leftStickButton.SetNormalizedPosition(profile.leftStickButton.position);
            //leftStickButton.SetScale(profile.leftStickButton.scale);
            leftStickButton.SetIcon(profile.leftStickButton.iconImage);
            leftStickButton.SetVisibility(profile.leftStickButton.isVisible);
            leftStickButton.SetPressToActivate(profile.leftStickButton.pressToActivate);
            leftStickButton.SetToggleActive(profile.leftStickButton.toggle);

            rightStickButton.SetProfile(profile);
            rightStickButton.SetNormalizedPosition(profile.rightStickButton.position);
            //rightStickButton.SetScale(profile.rightStickButton.scale);
            rightStickButton.SetIcon(profile.rightStickButton.iconImage);
            rightStickButton.SetVisibility(profile.rightStickButton.isVisible);
            rightStickButton.SetPressToActivate(profile.rightStickButton.pressToActivate);
            rightStickButton.SetToggleActive(profile.rightStickButton.toggle);

            leftShoulder.SetProfile(profile);
            leftShoulder.SetNormalizedPosition(profile.leftShoulder.position);
            //leftShoulder.SetScale(profile.leftShoulder.scale);
            leftShoulder.SetIcon(profile.leftShoulder.iconImage);
            leftShoulder.SetVisibility(profile.leftShoulder.isVisible);
            leftShoulder.SetPressToActivate(profile.leftShoulder.pressToActivate);
            leftShoulder.SetToggleActive(profile.leftShoulder.toggle);

            rightShoulder.SetProfile(profile);
            rightShoulder.SetNormalizedPosition(profile.rightShoulder.position);
            //rightShoulder.SetScale(profile.rightShoulder.scale);
            rightShoulder.SetIcon(profile.rightShoulder.iconImage);
            rightShoulder.SetVisibility(profile.rightShoulder.isVisible);
            rightShoulder.SetPressToActivate(profile.rightShoulder.pressToActivate);
            rightShoulder.SetToggleActive(profile.rightShoulder.toggle);

            leftTrigger.SetProfile(profile);
            leftTrigger.SetNormalizedPosition(profile.leftTrigger.position);
            //leftTrigger.SetScale(profile.leftTrigger.scale);
            leftTrigger.SetIcon(profile.leftTrigger.iconImage);
            leftTrigger.SetVisibility(profile.leftTrigger.isVisible);
            leftTrigger.SetPressToActivate(profile.leftTrigger.pressToActivate);
            leftTrigger.SetToggleActive(profile.leftTrigger.toggle);

            rightTrigger.SetProfile(profile);
            rightTrigger.SetNormalizedPosition(profile.rightTrigger.position);
            //rightTrigger.SetScale(profile.rightTrigger.scale);
            rightTrigger.SetIcon(profile.rightTrigger.iconImage);
            rightTrigger.SetVisibility(profile.rightTrigger.isVisible);

            buttonStart.SetProfile(profile);
            buttonStart.SetNormalizedPosition(profile.startButton.position);
            //buttonStart.SetScale(profile.startButton.scale);
            buttonStart.SetIcon(profile.startButton.iconImage);
            buttonStart.SetVisibility(profile.startButton.isVisible);
            buttonStart.SetPressToActivate(profile.startButton.pressToActivate);
            buttonStart.SetToggleActive(profile.startButton.toggle);

            buttonSelect.SetProfile(profile);
            buttonSelect.SetNormalizedPosition(profile.selectButton.position);
            //buttonSelect.SetScale(profile.selectButton.scale);
            buttonSelect.SetIcon(profile.selectButton.iconImage);
            buttonSelect.SetVisibility(profile.selectButton.isVisible);
            buttonSelect.SetPressToActivate(profile.selectButton.pressToActivate);
            buttonSelect.SetToggleActive(profile.selectButton.toggle);
        }
        else
        {
            DualShockProfile profile = gamepadConfig.GetDualShockProfile(currentProfileIndex);

            leftStick.SetProfile(profile);
            leftStick.SetNormalizedPosition(profile.leftStick.position);
            //leftStick.SetScale(profile.leftStick.scale);
            leftStick.SetVisibility(profile.leftStick.isVisible);
            leftStick.SetPressToActivate(profile.leftStick.pressToActivate);
            leftStick.SetToggleActive(profile.leftStick.toggle);

            rightStick.SetProfile(profile);
            rightStick.SetNormalizedPosition(profile.rightStick.position);
            //rightStick.SetScale(profile.rightStick.scale);
            rightStick.SetVisibility(profile.rightStick.isVisible);
            rightStick.SetPressToActivate(profile.rightStick.pressToActivate);
            rightStick.SetToggleActive(profile.rightStick.toggle);

            dPad.SetProfile(profile);
            dPad.SetNormalizedPosition(profile.dPad.position);
            // dPad.SetScale(profile.dPad.scale);
            dPad.SetVisibility(profile.dPad.isVisible);
            dPad.SetPressToActivate(profile.dPad.pressToActivate);
            dPad.SetToggleActive(profile.dPad.toggle);

            buttonNorth.SetProfile(profile);
            buttonNorth.SetNormalizedPosition(profile.buttonNorth.position);
            //buttonNorth.SetScale(profile.buttonNorth.scale);
            buttonNorth.SetIcon(profile.buttonNorth.iconImage);
            buttonNorth.SetVisibility(profile.buttonNorth.isVisible);
            buttonNorth.SetPressToActivate(profile.buttonNorth.pressToActivate);
            buttonNorth.SetToggleActive(profile.buttonNorth.toggle);

            buttonSouth.SetProfile(profile);
            buttonSouth.SetNormalizedPosition(profile.buttonSouth.position);
            //buttonSouth.SetScale(profile.buttonSouth.scale);
            buttonSouth.SetIcon(profile.buttonSouth.iconImage);
            buttonSouth.SetVisibility(profile.buttonSouth.isVisible);
            buttonSouth.SetPressToActivate(profile.buttonSouth.pressToActivate);
            buttonSouth.SetToggleActive(profile.buttonSouth.toggle);

            buttonWest.SetProfile(profile);
            buttonWest.SetNormalizedPosition(profile.buttonWest.position);
            //buttonWest.SetScale(profile.buttonWest.scale);
            buttonWest.SetIcon(profile.buttonWest.iconImage);
            buttonWest.SetVisibility(profile.buttonWest.isVisible);
            buttonWest.SetPressToActivate(profile.buttonWest.pressToActivate);
            buttonWest.SetToggleActive(profile.buttonWest.toggle);


            buttonEast.SetProfile(profile);
            buttonEast.SetNormalizedPosition(profile.buttonEast.position);
            //buttonEast.SetScale(profile.buttonEast.scale);
            buttonEast.SetIcon(profile.buttonEast.iconImage);
            buttonEast.SetVisibility(profile.buttonEast.isVisible);
            buttonEast.SetPressToActivate(profile.buttonEast.pressToActivate);
            buttonEast.SetToggleActive(profile.buttonEast.toggle);

            leftStickButton.SetProfile(profile);
            leftStickButton.SetNormalizedPosition(profile.leftStickButton.position);
            //leftStickButton.SetScale(profile.leftStickButton.scale);
            leftStickButton.SetIcon(profile.leftStickButton.iconImage);
            leftStickButton.SetVisibility(profile.leftStickButton.isVisible);
            leftStickButton.SetPressToActivate(profile.leftStickButton.pressToActivate);
            leftStickButton.SetToggleActive(profile.leftStickButton.toggle);

            rightStickButton.SetProfile(profile);
            rightStickButton.SetNormalizedPosition(profile.rightStickButton.position);
            //rightStickButton.SetScale(profile.rightStickButton.scale);
            rightStickButton.SetIcon(profile.rightStickButton.iconImage);
            rightStickButton.SetVisibility(profile.rightStickButton.isVisible);
            rightStickButton.SetPressToActivate(profile.rightStickButton.pressToActivate);
            rightStickButton.SetToggleActive(profile.rightStickButton.toggle);

            leftShoulder.SetProfile(profile);
            leftShoulder.SetNormalizedPosition(profile.leftShoulder.position);
            //leftShoulder.SetScale(profile.leftShoulder.scale);
            leftShoulder.SetIcon(profile.leftShoulder.iconImage);
            leftShoulder.SetVisibility(profile.leftShoulder.isVisible);
            leftShoulder.SetPressToActivate(profile.leftShoulder.pressToActivate);
            leftShoulder.SetToggleActive(profile.leftShoulder.toggle);

            rightShoulder.SetProfile(profile);
            rightShoulder.SetNormalizedPosition(profile.rightShoulder.position);
            //rightShoulder.SetScale(profile.rightShoulder.scale);
            rightShoulder.SetIcon(profile.rightShoulder.iconImage);
            rightShoulder.SetVisibility(profile.rightShoulder.isVisible);
            rightShoulder.SetPressToActivate(profile.rightShoulder.pressToActivate);
            rightShoulder.SetToggleActive(profile.rightShoulder.toggle);

            leftTrigger.SetProfile(profile);
            leftTrigger.SetNormalizedPosition(profile.leftTrigger.position);
            //leftTrigger.SetScale(profile.leftTrigger.scale);
            leftTrigger.SetIcon(profile.leftTrigger.iconImage);
            leftTrigger.SetVisibility(profile.leftTrigger.isVisible);
            leftTrigger.SetPressToActivate(profile.leftTrigger.pressToActivate);
            leftTrigger.SetToggleActive(profile.leftTrigger.toggle);

            rightTrigger.SetProfile(profile);
            rightTrigger.SetNormalizedPosition(profile.rightTrigger.position);
            //rightTrigger.SetScale(profile.rightTrigger.scale);
            rightTrigger.SetIcon(profile.rightTrigger.iconImage);
            rightTrigger.SetVisibility(profile.rightTrigger.isVisible);
            rightTrigger.SetPressToActivate(profile.rightTrigger.pressToActivate);
            rightTrigger.SetToggleActive(profile.rightTrigger.toggle);

            buttonStart.SetProfile(profile);
            buttonStart.SetNormalizedPosition(profile.startButton.position);
            //buttonStart.SetScale(profile.startButton.scale);
            buttonStart.SetIcon(profile.startButton.iconImage);
            buttonStart.SetVisibility(profile.startButton.isVisible);
            buttonStart.SetPressToActivate(profile.startButton.pressToActivate);
            buttonStart.SetToggleActive(profile.startButton.toggle);

            buttonSelect.SetProfile(profile);
            buttonSelect.SetNormalizedPosition(profile.selectButton.position);
            //buttonSelect.SetScale(profile.selectButton.scale);
            buttonSelect.SetIcon(profile.selectButton.iconImage);
            buttonSelect.SetVisibility(profile.selectButton.isVisible);
            buttonSelect.SetPressToActivate(profile.selectButton.pressToActivate);
            buttonSelect.SetToggleActive(profile.selectButton.toggle);
        }
    }

    // ---------------
    // Public methods
    // ---------------

    public int GetID()
    {
        return gamepadID;
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

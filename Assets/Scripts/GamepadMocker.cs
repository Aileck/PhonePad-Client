using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadMocker: MonoBehaviour
{
    [SerializeField] private LeftStickMock leftStick;

    private Guid gamepadID;
    private GamepadType gamepadType;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GamepadMocker(GamepadType type)
    {
        gamepadID = Guid.NewGuid();
        gamepadType = type; 
    }

    // ---------------
    // Public methods
    // ---------------

    public Guid GetUUID()
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

    public string GetGamepadStateAsJson()
    {
        GamepadData data = new GamepadData
        {
            uuid = gamepadID.ToString(),
            leftStick = GetLeftStickInput()
        };

        string json = JsonUtility.ToJson(data);
        Debug.Log("Gamepad JSON: " + json);
        return json;
    }
}

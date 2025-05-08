using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadMocker: MonoBehaviour
{
    [SerializeField] private StickComponent leftStick;
    [SerializeField] private StickComponent rightStick;

    private int gamepadID;
    private GamepadType gamepadType;
    private bool connected = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
            uuid = gamepadID.ToString(),
            leftStick = GetLeftStickInput()
        };

        return data;
    }
}

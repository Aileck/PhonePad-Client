using MessagePack;
using System;
using UnityEngine;

[MessagePackObject(keyAsPropertyName: true)]
public class GamepadData
{
    public string uuid;

    public bool buttonEast;
    public bool buttonWest;
    public bool buttonNorth;
    public bool buttonSouth;

    public bool up;
    public bool down;
    public bool left;
    public bool right;

    public bool leftShoulder;
    public bool rightShoulder;

    public float leftTrigger;
    public float rightTrigger;

    public bool leftStickButton;
    public bool rightStickButton;

    public Vector2 leftStick;
    public Vector2 rightStick;

    public bool buttonStart;
    public bool buttonSelect;
}

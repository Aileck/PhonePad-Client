using MessagePack;
using System;
using UnityEngine;

[MessagePackObject(keyAsPropertyName: true)]
public class GamepadData
{
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

    public bool leftTrigger;
    public bool rightTrigger;

    public bool leftStickButton;
    public bool rightStickButton;

    public float leftStickX;
    public float leftStickY;

    public float rightStickX;
    public float rightStickY;

    public bool buttonStart;
    public bool buttonSelect;
}

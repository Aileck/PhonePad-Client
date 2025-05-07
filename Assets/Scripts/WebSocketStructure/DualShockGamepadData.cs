using MessagePack;
using UnityEngine;

[MessagePackObject(keyAsPropertyName: true)]
public class DualShockGamepadData : GamepadData
{
    public bool buttonPS;
    public bool buttonTouchBar;
    public Vector2 touchBar;
}

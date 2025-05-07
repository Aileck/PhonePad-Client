using MessagePack;

[MessagePackObject(keyAsPropertyName: true)]
public class XboxGamepadData: GamepadData
{
    public bool buttonGuide;
}

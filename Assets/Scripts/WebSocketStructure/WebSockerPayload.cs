using MessagePack;

public enum WebSocketAction
{
    handshake,
    register,
    input,
    disconnect
}

public enum GamepadType
{
    GAMEPAD_XBOX360,
    GAMEPAD_DUALSHOCK4,
};

[MessagePackObject(keyAsPropertyName: true)]
public class WebSockerPayload
{
    public string action;
    public string id;
    public string gamepadType;
    public GamepadData gamepadData;

}

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
    GAMEPAD_DUALSHOCK,
};

[MessagePackObject(keyAsPropertyName: true)]
public class WebSockerPayload
{
    public string action;
    public int id;
    public string gamepadType;
    public GamepadData gamepadData;

}

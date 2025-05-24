using MessagePack;

public enum WebSocketAction
{
    handshake,
    register,
    input,
    disconnect,

    delay_test_request_ack,
}

public enum GamepadType
{
    GAMEPAD_XBOX360,
    GAMEPAD_DUALSHOCK,
};

[MessagePackObject(keyAsPropertyName: true)]
public class WebSocketPayload
{
    public string action;
    public int id;
    public string gamepadType;
    public GamepadData gamepadData;

    // Optional
    public long? timestamp;
}

[MessagePackObject(keyAsPropertyName: true)]
public class WebSocketPingPayload
{
    public string action;
    public int id;
    public long timestamp;
    public string? payload;
}

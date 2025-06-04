using MessagePack;

public static class PayloadType
{
    public const string E_MAX_CONN = "E_MAX_CONN";
}

[MessagePackObject(keyAsPropertyName: true)]
public class WebSocketResponse
{
    public string action;

    public string status;

    public string payload;
}
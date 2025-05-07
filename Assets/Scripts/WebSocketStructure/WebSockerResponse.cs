using MessagePack;

[MessagePackObject(keyAsPropertyName: true)]
public class WebSocketResponse
{
    public string action;

    public string status;

    public string payload;
}
using UnityEngine;

public enum InputType
{
    PYSHICAL,
    VIRTUAL
}

public interface IGamepadComponent
{
    Vector2 GetPosition();
    void SetPosition(Vector2 position);
    void SetConfig(GamepadConfig config);
    void SetLastInputType(InputType type);
}
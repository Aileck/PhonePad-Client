using UnityEngine;

public enum InputType
{
    PYSHICAL,
    VIRTUAL
}

public interface IGamepadComponent
{
    Vector2 GetNormalizedPosition();
    void SetNormalizedPosition(Vector2 position);
    void SetConfig(GamepadConfig config);
    void SetLastInputType(InputType type);
}
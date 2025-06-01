using UnityEngine;
using static GamepadConfig;

public enum InputType
{
    PYSHICAL,
    VIRTUAL
}

public interface IGamepadComponent
{
    Vector2 GetNormalizedPosition();
    void SetNormalizedPosition(Vector2 position);
    void SetProfile(Profile config);
    void SetLastInputType(InputType type);
    void SetIcon(Sprite sprite);
    Vector2 GetScale();
    void SetScale(Vector2 scale);
    void SetVisibility(bool isVisible);
    void SetPressToActivate(bool isPressed);
    void SetToggleActive(bool isActive);
}
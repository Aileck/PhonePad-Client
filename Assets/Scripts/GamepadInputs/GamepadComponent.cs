using UnityEngine;

public interface IGamepadComponent
{
    Vector2 GetPosition();
    void SetPosition(Vector2 position);
}
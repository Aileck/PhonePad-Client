using UnityEngine;

[CreateAssetMenu(fileName = "GamepadConfig", menuName = "Scriptable Objects/GamepadConfig")]
public class GamepadConfig : ScriptableObject
{
    public float buttonPressTransformScale;

    public bool syncVirtualInputWithGamepad;
}

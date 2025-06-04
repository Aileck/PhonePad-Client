using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class GamepadConfigData
{
    public List<XboxProfileData> xboxProfiles = new List<XboxProfileData>();
    public List<DualShockProfileData> dualShockProfiles = new List<DualShockProfileData>();
}

[Serializable]
public class ProfileData
{
    public float buttonPressTransformScale;
    public bool syncVirtualInputWithGamepad;
    public bool ignorePhysicalGamepad;
    public ButtonProfileData buttonEast;
    public ButtonProfileData buttonSouth;
    public ButtonProfileData buttonWest;
    public ButtonProfileData buttonNorth;
    public ButtonProfileData leftShoulder;
    public ButtonProfileData rightShoulder;
    public ButtonProfileData leftTrigger;
    public ButtonProfileData rightTrigger;
    public ButtonProfileData leftStickButton;
    public ButtonProfileData rightStickButton;
    public ButtonProfileData startButton;
    public ButtonProfileData selectButton;
    public ButtonProfileData dPad;
    public ButtonProfileData leftStick;
    public ButtonProfileData rightStick;
}

[Serializable]
public class XboxProfileData : ProfileData { }

[Serializable]
public class DualShockProfileData : ProfileData { }

[Serializable]
public class ButtonProfileData
{
    public ButtonName name;
    public Vector2 position;
    public Vector2 scale;
    public string iconImagePath;
    public string backgroundImagePath;
    public bool isVisible;
    public bool pressToActivate;
    public bool toggle;
} 
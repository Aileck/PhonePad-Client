using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

[CreateAssetMenu(fileName = "GamepadConfig", menuName = "Scriptable Objects/GamepadConfig")]
public class GamepadConfig : ScriptableObject
{
    public List<XboxProfie> xboxProfie = new List<XboxProfie>();
    public float buttonPressTransformScale;

    public bool syncVirtualInputWithGamepad;
    public bool ignorePhysicalGamepad;

    [Serializable]
    public class Profile
    {
        [Header("General Settings")]
        public float buttonPressTransformScale;

        public bool syncVirtualInputWithGamepad;
        public bool ignorePhysicalGamepad;

        [Header("Button Settings")]
        public ButtonProfile buttonEast;
        public ButtonProfile buttonSouth;
        public ButtonProfile buttonWest;
        public ButtonProfile buttonNorth;

        public ButtonProfile leftShoulder;
        public ButtonProfile rightShoulder;

        public ButtonProfile leftTrigger;
        public ButtonProfile rightTrigger;

        public ButtonProfile leftStickButton;
        public ButtonProfile rightStickButton;

        public ButtonProfile startButton;
        public ButtonProfile selectButton;

        public ButtonProfile dPadUp;
        public ButtonProfile dPadDown;
        public ButtonProfile dPadLeft;
        public ButtonProfile dPadRight;

        public ButtonProfile leftStick;
        public ButtonProfile rightStick;
    }

    [Serializable]

    public class XboxProfie : Profile
    {
        public void SetDefaultProfile()
        {
            buttonEast.position = new Vector2(0.5f, 0.5f);
            buttonEast.scale = new Vector2(1f, 1f);
            //buttonEast.iconImage = Resources.Load("../");

            buttonSouth.position = new Vector2(0.5f, 0.5f);
            buttonSouth.scale = new Vector2(1f, 1f);
            buttonWest.position = new Vector2(0.5f, 0.5f);
            buttonWest.scale = new Vector2(1f, 1f);
            buttonNorth.position = new Vector2(0.5f, 0.5f);
            buttonNorth.scale = new Vector2(1f, 1f);
        }
    }

    [Serializable]
    public class ButtonProfile
    {
        public Vector2 position;
        public Vector2 scale;

        public Sprite iconImage;
        public Sprite backgoundImage;

    }
}

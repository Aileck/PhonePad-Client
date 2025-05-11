using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GamepadConfig", menuName = "Scriptable Objects/GamepadConfig")]
public class GamepadConfig : ScriptableObject
{
    public List<XboxProfile> xboxProfiles = new List<XboxProfile>();
    public float buttonPressTransformScale;

    public bool syncVirtualInputWithGamepad;
    public bool ignorePhysicalGamepad;

    //private void OnEnable()
    //{

    //        AddDefaultProfile();

    //}

    //// Method to add a default profile
    //public void AddDefaultProfile()
    //{
    //    XboxProfile defaultProfile = new XboxProfile();
    //    defaultProfile.SetDefaultProfile();
    //    xboxProfiles.Add(defaultProfile);
    //}

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

        public ButtonProfile dPad;

        public ButtonProfile leftStick;
        public ButtonProfile rightStick;
    }

    [Serializable]
    public class XboxProfile : Profile
    {
        public void SetDefaultProfile()
        {
            // Initialize all button profiles if they're null
            buttonEast = buttonEast ?? new ButtonProfile();
            buttonSouth = buttonSouth ?? new ButtonProfile();
            buttonWest = buttonWest ?? new ButtonProfile();
            buttonNorth = buttonNorth ?? new ButtonProfile();
            leftShoulder = leftShoulder ?? new ButtonProfile();
            rightShoulder = rightShoulder ?? new ButtonProfile();
            leftTrigger = leftTrigger ?? new ButtonProfile();
            rightTrigger = rightTrigger ?? new ButtonProfile();
            leftStickButton = leftStickButton ?? new ButtonProfile();
            rightStickButton = rightStickButton ?? new ButtonProfile();
            startButton = startButton ?? new ButtonProfile();
            selectButton = selectButton ?? new ButtonProfile();
            dPad = dPad ?? new ButtonProfile();
            leftStick = leftStick ?? new ButtonProfile();
            rightStick = rightStick ?? new ButtonProfile();

            buttonEast.position = new Vector2(0.34f, 0.22f);
            buttonEast.scale = new Vector2(1f, 1f);
            buttonEast.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/Xbox/Using/xbox_button_b.png");

            buttonSouth.position = new Vector2(0.29f, 0.05f);
            buttonSouth.scale = new Vector2(1f, 1f);
            buttonSouth.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/Xbox/Using/xbox_button_a.png");

            buttonWest.position = new Vector2(0.24f, 0.22f);
            buttonWest.scale = new Vector2(1f, 1f);
            buttonWest.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/Xbox/Using/xbox_button_x.png");

            buttonNorth.position = new Vector2(0.29f, 0.05f);
            buttonNorth.scale = new Vector2(1f, 1f);
            buttonNorth.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/Xbox/Using/xbox_button_y.png");

            leftShoulder.position = new Vector2(-0.22f, 2.81f);
            leftShoulder.scale = new Vector2(1f, 1f);
            leftShoulder.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/Xbox/Using/xbox_button_lb.png");

            rightShoulder.position = new Vector2(0.22f, 2.81f);
            rightShoulder.scale = new Vector2(1f, 1f);
            rightShoulder.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/Xbox/Using/xbox_button_rb.png");

            leftTrigger.position = new Vector2(-0.35f, 2.81f);
            leftTrigger.scale = new Vector2(1f, 1f);
            leftTrigger.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/Xbox/Using/xbox_button_lt.png");

            rightTrigger.position = new Vector2(0.35f, 2.81f);
            rightTrigger.scale = new Vector2(1f, 1f);
            rightTrigger.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/Xbox/Using/xbox_button_rt.png");

            leftStickButton.position = new Vector2(-0.32f, 0.86f);
            leftStickButton.scale = new Vector2(1f, 1f);
            leftStickButton.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/Xbox/Using/xbox_button_l3.png");

            rightStickButton.position = new Vector2(0.32f, -0.86f);
            rightStickButton.scale = new Vector2(1f, 1f);
            rightStickButton.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/Xbox/Using/xbox_button_r3.png");

            startButton.position = new Vector2(0.08f, 1.85f);
            startButton.scale = new Vector2(1f, 1f);
            startButton.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/Xbox/Using/xbox_button_start.png");

            selectButton.position = new Vector2(-0.08f, 1.85f);
            selectButton.scale = new Vector2(1f, 1f);
            selectButton.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/Xbox/Using/xbox_button_select.png");

            dPad.position = new Vector2(-0.19f, -2.05f);
            dPad.scale = new Vector2(1f, 1f);
            // No need to set iconImage for dPad 

            leftStick.position = new Vector2(0.08f, 1.85f);
            leftStick.scale = new Vector2(1f, 1f);
            leftStick.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/Xbox/Using/xbox_left_stick.png");

            rightStick.position = new Vector2(-0.08f, 1.85f);
            rightStick.scale = new Vector2(1f, 1f);
            rightStick.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/Xbox/Using/xbox_right_stick.png");
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
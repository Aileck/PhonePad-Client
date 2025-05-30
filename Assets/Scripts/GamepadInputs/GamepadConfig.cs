using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GamepadConfig", menuName = "Scriptable Objects/GamepadConfig")]
public class GamepadConfig : ScriptableObject
{
    public List<XboxProfile> xboxProfiles = new List<XboxProfile>();
    public List<DualShockProfile> dualShockProfiles = new List<DualShockProfile>();


    //private void OnEnable()
    //{

    //    AddDefaultProfile();

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

            buttonEast.position = new Vector2(0.69f, 0.25f);
            buttonEast.scale = new Vector2(1f, 1f);
            buttonEast.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/Xbox/Using/xbox_button_b.png");
            buttonEast.name = ButtonsName.ButtonEast.ToString();

            buttonSouth.position = new Vector2(0.58f, 0.06f);
            buttonSouth.scale = new Vector2(1f, 1f);
            buttonSouth.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/Xbox/Using/xbox_button_a.png");
            buttonSouth.name = ButtonsName.ButtonSouth.ToString();

            buttonWest.position = new Vector2(0.48f, 0.25f);
            buttonWest.scale = new Vector2(1f, 1f);
            buttonWest.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/Xbox/Using/xbox_button_x.png");
            buttonWest.name = ButtonsName.ButtonWest.ToString();

            buttonNorth.position = new Vector2(0.58f, 0.44f);
            buttonNorth.scale = new Vector2(1f, 1f);
            buttonNorth.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/Xbox/Using/xbox_button_y.png");
            buttonNorth.name = ButtonsName.ButtonNorth.ToString();

            leftShoulder.position = new Vector2(-0.44f, 0.78f);
            leftShoulder.scale = new Vector2(1f, 1f);
            leftShoulder.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/Xbox/Using/xbox_button_lb.png");
            leftShoulder.name = ButtonsName.LeftShoulder.ToString();

            rightShoulder.position = new Vector2(0.44f, 0.78f);
            rightShoulder.scale = new Vector2(1f, 1f);
            rightShoulder.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/Xbox/Using/xbox_button_rb.png");
            rightShoulder.name = ButtonsName.RightShoulder.ToString();

            leftTrigger.position = new Vector2(-0.71f, 0.78f);
            leftTrigger.scale = new Vector2(1f, 1f);
            leftTrigger.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/Xbox/Using/xbox_button_lt.png");
            leftTrigger.name = ButtonsName.LeftTrigger.ToString();

            rightTrigger.position = new Vector2(0.71f, 0.78f);
            rightTrigger.scale = new Vector2(1f, 1f);
            rightTrigger.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/Xbox/Using/xbox_button_rt.png");
            rightTrigger.name = ButtonsName.RightTrigger.ToString();

            leftStickButton.position = new Vector2(-0.65f, -0.24f);
            leftStickButton.scale = new Vector2(1f, 1f);
            leftStickButton.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/Xbox/Using/xbox_button_l3.png");
            leftStickButton.name = ButtonsName.LeftStickButton.ToString();


            rightStickButton.position = new Vector2(0.65f, -0.24f);
            rightStickButton.scale = new Vector2(1f, 1f);
            rightStickButton.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/Xbox/Using/xbox_button_r3.png");
            rightStickButton.name = ButtonsName.RightStickButton.ToString();    

            startButton.position = new Vector2(-0.16f, 0.51f);
            startButton.scale = new Vector2(1f, 1f);
            startButton.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/Xbox/Using/xbox_button_start.png");
            startButton.name = ButtonsName.StartButton.ToString();

            selectButton.position = new Vector2(-0.16f, 0.51f);
            selectButton.scale = new Vector2(1f, 1f);
            selectButton.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/Xbox/Using/xbox_button_select.png");
            selectButton.name = ButtonsName.SelectButton.ToString();

            dPad.position = new Vector2(-0.39f, -0.57f);
            dPad.scale = new Vector2(1f, 1f);
            dPad.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/Xbox/Using/xbox_dpad_none.png");
            dPad.name = ButtonsName.DPad.ToString();

            leftStick.position = new Vector2(-0.58f, 0.25f);
            leftStick.scale = new Vector2(1f, 1f);
            leftStick.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/Xbox/Using/xbox_left_stick.png");
            leftStick.name = ButtonsName.LeftStick.ToString();

            rightStick.position = new Vector2(0.39f, -0.57f);
            rightStick.scale = new Vector2(1f, 1f);
            rightStick.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/Xbox/Using/xbox_right_stick.png");
            rightStick.name = ButtonsName.RightStick.ToString();
        }
    }
   
    [Serializable]
    public class DualShockProfile : Profile {
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

            buttonEast.position = new Vector2(0.69f, 0.25f);
            buttonEast.scale = new Vector2(1f, 1f);
            buttonEast.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/DualShock/Using/playstation_button_circle.png");
            buttonEast.name = ButtonsName.ButtonEast.ToString();

            buttonSouth.position = new Vector2(0.58f, 0.06f);
            buttonSouth.scale = new Vector2(1f, 1f);
            buttonSouth.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/DualShock/Using/playstation_button_square.png");
            buttonSouth.name = ButtonsName.ButtonSouth.ToString();

            buttonWest.position = new Vector2(0.48f, 0.25f);
            buttonWest.scale = new Vector2(1f, 1f);
            buttonWest.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/DualShock/Using/playstation_button_triangle.png");
            buttonWest.name = ButtonsName.ButtonWest.ToString();

            buttonNorth.position = new Vector2(0.58f, 0.44f);
            buttonNorth.scale = new Vector2(1f, 1f);
            buttonNorth.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/DualShock/Using/playstation_button_x.png");
            buttonNorth.name = ButtonsName.ButtonNorth.ToString();

            leftShoulder.position = new Vector2(-0.44f, 0.78f);
            leftShoulder.scale = new Vector2(1f, 1f);
            leftShoulder.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/DualShock/Using/playstation_button_l1.png");
            leftShoulder.name = ButtonsName.LeftShoulder.ToString();

            rightShoulder.position = new Vector2(0.44f, 0.78f);
            rightShoulder.scale = new Vector2(1f, 1f);
            rightShoulder.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/DualShock/Using/playstation_button_r1.png");
            rightShoulder.name = ButtonsName.RightShoulder.ToString();

            leftTrigger.position = new Vector2(-0.71f, 0.78f);
            leftTrigger.scale = new Vector2(1f, 1f);
            leftTrigger.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/DualShock/Using/playstation_button_l2.png");
            leftTrigger.name = ButtonsName.LeftTrigger.ToString();

            rightTrigger.position = new Vector2(0.71f, 0.78f);
            rightTrigger.scale = new Vector2(1f, 1f);
            rightTrigger.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/DualShock/Using/playstation_button_r2.png");
            rightTrigger.name = ButtonsName.RightTrigger.ToString();

            leftStickButton.position = new Vector2(-0.65f, -0.24f);
            leftStickButton.scale = new Vector2(1f, 1f);
            leftStickButton.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/DualShock/Using/playstation_button_l3.png");
            leftStickButton.name = ButtonsName.LeftStickButton.ToString();


            rightStickButton.position = new Vector2(0.65f, -0.24f);
            rightStickButton.scale = new Vector2(1f, 1f);
            rightStickButton.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/DualShock/Using/playstation_button_r3.png");
            rightStickButton.name = ButtonsName.RightStickButton.ToString();    

            startButton.position = new Vector2(-0.16f, 0.51f);
            startButton.scale = new Vector2(1f, 1f);
            startButton.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/DualShock/Using/playstation_button_none.png");
            startButton.name = ButtonsName.StartButton.ToString();

            selectButton.position = new Vector2(-0.16f, 0.51f);
            selectButton.scale = new Vector2(1f, 1f);
            selectButton.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/DualShock/Using/playstation_button_options.png");
            selectButton.name = ButtonsName.SelectButton.ToString();

            dPad.position = new Vector2(-0.58f, 0.25f);
            dPad.scale = new Vector2(1f, 1f);
            dPad.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/Xbox/Using/xbox_dpad_none.png");
            dPad.name = ButtonsName.DPad.ToString();

            leftStick.position = new Vector2(-0.39f, -0.57f);
            leftStick.scale = new Vector2(1f, 1f);
            leftStick.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/Xbox/Using/xbox_left_stick.png");
            leftStick.name = ButtonsName.LeftStick.ToString();

            rightStick.position = new Vector2(0.39f, -0.57f);
            rightStick.scale = new Vector2(1f, 1f);
            rightStick.iconImage = Resources.Load<Sprite>("Assets/Sprites/KennyInput/Xbox/Using/xbox_right_stick.png");
            rightStick.name = ButtonsName.RightStick.ToString();
        }
    }

    [Serializable]
    public class ButtonProfile
    {
        public string name;

        public Vector2 position;
        public Vector2 scale;

        public Sprite iconImage;
        public Sprite backgoundImage;
    }
}
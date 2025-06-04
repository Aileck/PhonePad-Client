using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

[CreateAssetMenu(fileName = "GamepadConfig", menuName = "Scriptable Objects/GamepadConfig")]
public class GamepadConfig : ScriptableObject
{
    [Header("Resource Paths Configuration")]
    public GamepadResourcePaths resourcePaths = new GamepadResourcePaths();

    public List<XboxProfile> xboxProfiles = new List<XboxProfile>();
    public List<DualShockProfile> dualShockProfiles = new List<DualShockProfile>();

    private void OnEnable()
    {
        if (xboxProfiles.Count == 0)
        {
            AddDefaultXboxProfile();
        }

        if (dualShockProfiles.Count == 0)
        {
            AddDefaultDualShockProfile();
        }
    }

    public XboxProfile GetXboxProfile(int index)
    {
        if (xboxProfiles.Count < index)
        {
            XboxProfile newProfile = new XboxProfile();
            newProfile.SetDefaultProfile(resourcePaths.xboxPaths);
            xboxProfiles.Add(newProfile);

            return xboxProfiles[xboxProfiles.Count - 1];
        }

        return xboxProfiles[index];
    }

    public DualShockProfile GetDualShockProfile(int index)
    {
        if (dualShockProfiles.Count < index)
        {
            DualShockProfile newProfile = new DualShockProfile();
            newProfile.SetDefaultProfile(resourcePaths.playStationPaths);
            dualShockProfiles.Add(newProfile);

            return dualShockProfiles[dualShockProfiles.Count - 1];
        }

        return dualShockProfiles[index];
    }

    public void AddDefaultXboxProfile()
    {
        XboxProfile defaultProfile = new XboxProfile();
        defaultProfile.SetDefaultProfile(resourcePaths.xboxPaths);
        xboxProfiles.Add(defaultProfile);

        XboxProfile customProfile = new XboxProfile();
        customProfile.SetDefaultCustomProfile(resourcePaths.xboxPaths);
        xboxProfiles.Add(customProfile);
    }

    public void AddDefaultDualShockProfile()
    {
        DualShockProfile defaultProfile = new DualShockProfile();
        defaultProfile.SetDefaultProfile(resourcePaths.playStationPaths);
        dualShockProfiles.Add(defaultProfile);

        DualShockProfile customProfile = new DualShockProfile();
        customProfile.SetDefaultCustomProfile(resourcePaths.playStationPaths);
        dualShockProfiles.Add(customProfile);
    }

    public void RefreshAllProfileIcons()
    {
        foreach (var profile in xboxProfiles)
        {
            profile.RefreshIcons(resourcePaths.xboxPaths);
        }

        foreach (var profile in dualShockProfiles)
        {
            profile.RefreshIcons(resourcePaths.playStationPaths);
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    private void OnValidate()
    {
        RefreshAllProfileIcons();
    }

    public void SaveToJson(string filePath)
    {
        var configData = new GamepadConfigData();
        
        // Convert Xbox profiles
        foreach (var profile in xboxProfiles)
        {
            var profileData = new XboxProfileData();
            ConvertProfileToData(profile, profileData);
            configData.xboxProfiles.Add(profileData);
        }

        // Convert DualShock profiles
        foreach (var profile in dualShockProfiles)
        {
            var profileData = new DualShockProfileData();
            ConvertProfileToData(profile, profileData);
            configData.dualShockProfiles.Add(profileData);
        }

        string jsonString = JsonConvert.SerializeObject(configData, Formatting.Indented);
        File.WriteAllText(filePath, jsonString);
    }

    public void LoadFromJson(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError($"Config file not found at {filePath}");
            return;
        }

        string jsonString = File.ReadAllText(filePath);
        var configData = JsonConvert.DeserializeObject<GamepadConfigData>(jsonString);

        // Clear existing profiles
        xboxProfiles.Clear();
        dualShockProfiles.Clear();

        // Load Xbox profiles
        foreach (var profileData in configData.xboxProfiles)
        {
            var profile = new XboxProfile();
            ConvertDataToProfile(profileData, profile);
            xboxProfiles.Add(profile);
        }

        // Load DualShock profiles
        foreach (var profileData in configData.dualShockProfiles)
        {
            var profile = new DualShockProfile();
            ConvertDataToProfile(profileData, profile);
            dualShockProfiles.Add(profile);
        }

        RefreshAllProfileIcons();
    }

    private void ConvertProfileToData(Profile profile, ProfileData data)
    {
        data.buttonPressTransformScale = profile.buttonPressTransformScale;
        data.syncVirtualInputWithGamepad = profile.syncVirtualInputWithGamepad;
        data.ignorePhysicalGamepad = profile.ignorePhysicalGamepad;

        data.buttonEast = ConvertButtonToData(profile.buttonEast);
        data.buttonSouth = ConvertButtonToData(profile.buttonSouth);
        data.buttonWest = ConvertButtonToData(profile.buttonWest);
        data.buttonNorth = ConvertButtonToData(profile.buttonNorth);
        data.leftShoulder = ConvertButtonToData(profile.leftShoulder);
        data.rightShoulder = ConvertButtonToData(profile.rightShoulder);
        data.leftTrigger = ConvertButtonToData(profile.leftTrigger);
        data.rightTrigger = ConvertButtonToData(profile.rightTrigger);
        data.leftStickButton = ConvertButtonToData(profile.leftStickButton);
        data.rightStickButton = ConvertButtonToData(profile.rightStickButton);
        data.dPad = ConvertButtonToData(profile.dPad);
        data.leftStick = ConvertButtonToData(profile.leftStick);
        data.rightStick = ConvertButtonToData(profile.rightStick);
    }

    private void ConvertDataToProfile(ProfileData data, Profile profile)
    {
        profile.buttonPressTransformScale = data.buttonPressTransformScale;
        profile.syncVirtualInputWithGamepad = data.syncVirtualInputWithGamepad;
        profile.ignorePhysicalGamepad = data.ignorePhysicalGamepad;

        profile.InitializeButtonProfiles();

        ConvertDataToButton(data.buttonEast, profile.buttonEast);
        ConvertDataToButton(data.buttonSouth, profile.buttonSouth);
        ConvertDataToButton(data.buttonWest, profile.buttonWest);
        ConvertDataToButton(data.buttonNorth, profile.buttonNorth);
        ConvertDataToButton(data.leftShoulder, profile.leftShoulder);
        ConvertDataToButton(data.rightShoulder, profile.rightShoulder);
        ConvertDataToButton(data.leftTrigger, profile.leftTrigger);
        ConvertDataToButton(data.rightTrigger, profile.rightTrigger);
        ConvertDataToButton(data.leftStickButton, profile.leftStickButton);
        ConvertDataToButton(data.rightStickButton, profile.rightStickButton);
        ConvertDataToButton(data.dPad, profile.dPad);
        ConvertDataToButton(data.leftStick, profile.leftStick);
        ConvertDataToButton(data.rightStick, profile.rightStick);
    }

    private ButtonProfileData ConvertButtonToData(ButtonProfile button)
    {
        if (button == null) return null;

        string iconPath = null;
        string backgroundPath = null;

#if UNITY_EDITOR
        iconPath = button.iconImage != null ? UnityEditor.AssetDatabase.GetAssetPath(button.iconImage) : null;
        backgroundPath = button.backgoundImage != null ? UnityEditor.AssetDatabase.GetAssetPath(button.backgoundImage) : null;
#else
        // In runtime, we assume all sprites are in Resources folder
        iconPath = button.iconImage != null ? $"Assets/Resources/{button.iconImage.name}" : null;
        backgroundPath = button.backgoundImage != null ? $"Assets/Resources/{button.backgoundImage.name}" : null;
#endif

        return new ButtonProfileData
        {
            name = button.name,
            position = button.position,
            scale = button.scale,
            iconImagePath = iconPath,
            backgroundImagePath = backgroundPath,
            isVisible = button.isVisible,
            pressToActivate = button.pressToActivate,
            toggle = button.toggle
        };
    }

    private void ConvertDataToButton(ButtonProfileData data, ButtonProfile button)
    {
        if (data == null || button == null) return;

        button.name = data.name;
        button.position = data.position;
        button.scale = data.scale;
        
#if UNITY_EDITOR
        button.iconImage = !string.IsNullOrEmpty(data.iconImagePath) ? 
            UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(data.iconImagePath) : null;
        button.backgoundImage = !string.IsNullOrEmpty(data.backgroundImagePath) ? 
            UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(data.backgroundImagePath) : null;
#else
        button.iconImage = !string.IsNullOrEmpty(data.iconImagePath) ? 
            LoadSpriteFromResources(data.iconImagePath) : null;
        button.backgoundImage = !string.IsNullOrEmpty(data.backgroundImagePath) ? 
            LoadSpriteFromResources(data.backgroundImagePath) : null;
#endif
        button.isVisible = data.isVisible;
        button.pressToActivate = data.pressToActivate;
        button.toggle = data.toggle;
    }

    private Sprite LoadSpriteFromResources(string path)
    {
        // Remove "Assets/Resources/" from the path and the file extension
        string resourcePath = path.Replace("Assets/Resources/", "").Replace(".png", "").Replace(".jpg", "");
        return Resources.Load<Sprite>(resourcePath);
    }

    [Serializable]
    public class GamepadResourcePaths
    {
        [Header("Xbox Resource Paths")]
        public XboxResourcePaths xboxPaths = new XboxResourcePaths();

        [Header("PlayStation Resource Paths")]
        public PlayStationResourcePaths playStationPaths = new PlayStationResourcePaths();
    }

    [Serializable]
    public class XboxResourcePaths
    {
        [Header("Button Icons")]
        public Sprite buttonA;
        public Sprite buttonB;
        public Sprite buttonX;
        public Sprite buttonY;

        [Header("Shoulder & Trigger Icons")]
        public Sprite leftBumper;
        public Sprite rightBumper;
        public Sprite leftTrigger;
        public Sprite rightTrigger;

        [Header("Stick Icons")]
        public Sprite leftStick;
        public Sprite rightStick;
        public Sprite leftStickButton;
        public Sprite rightStickButton;

        [Header("Menu & D-Pad Icons")]
        public Sprite startButton;
        public Sprite selectButton;
        public Sprite dPad;
    }

    [Serializable]
    public class PlayStationResourcePaths
    {
        [Header("Face Button Icons")]
        public Sprite buttonCross;
        public Sprite buttonCircle;
        public Sprite buttonSquare;
        public Sprite buttonTriangle;

        [Header("Shoulder & Trigger Icons")]
        public Sprite l1Button;
        public Sprite r1Button;
        public Sprite l2Trigger;
        public Sprite r2Trigger;

        [Header("Stick Icons")]
        public Sprite l3Button;
        public Sprite r3Button;

        [Header("Menu Icons")]
        public Sprite optionsButton;
        public Sprite shareButton;

        [Header("D-Pad & Stick Icons")]
        public Sprite dPad;
        public Sprite leftStick;
        public Sprite rightStick;
    }

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

        public void InitializeButtonProfiles()
        {
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
        }

        public void UpdateButtonScale(ButtonName buttonName, Vector2 newScale)
        {
            ButtonProfile button = GetButtonProfile(buttonName);
            if (button != null)
            {
                button.scale = newScale;
            }
        }

        public void UpdateButtonPosition(ButtonName buttonName, Vector2 newPosition)
        {
            ButtonProfile button = GetButtonProfile(buttonName);
            if (button != null)
            {
                Debug.Log($"Updating position for {buttonName}: {newPosition}");
                button.position = newPosition;
            }
        }

        public void UpdateButtonVisibility(ButtonName buttonName, bool isVisible)
        {
            ButtonProfile button = GetButtonProfile(buttonName);
            if (button != null)
            {
                button.isVisible = isVisible;
            }
        }

        public void UpdateButtonToggle(ButtonName buttonName, bool toggle)
        {
            ButtonProfile button = GetButtonProfile(buttonName);
            if (button != null)
            {
                button.toggle = toggle;
            }
        }

        public void UpdateButtonPressToActivate(ButtonName buttonName, bool pressToActivate)
        {
            ButtonProfile button = GetButtonProfile(buttonName);
            if (button != null)
            {
                button.pressToActivate = pressToActivate;
            }
        }


        protected ButtonProfile GetButtonProfile(ButtonName buttonName)
        {
            switch (buttonName)
            {
                case ButtonName.ButtonEast: return buttonEast;
                case ButtonName.ButtonSouth: return buttonSouth;
                case ButtonName.ButtonWest: return buttonWest;
                case ButtonName.ButtonNorth: return buttonNorth;
                case ButtonName.LeftShoulder: return leftShoulder;
                case ButtonName.RightShoulder: return rightShoulder;
                case ButtonName.LeftTrigger: return leftTrigger;
                case ButtonName.RightTrigger: return rightTrigger;
                case ButtonName.LeftStickButton: return leftStickButton;
                case ButtonName.RightStickButton: return rightStickButton;
                case ButtonName.StartButton: return startButton;
                case ButtonName.SelectButton: return selectButton;
                case ButtonName.DPad: return dPad;
                case ButtonName.LeftStick: return leftStick;
                case ButtonName.RightStick: return rightStick;
                default: return null;
            }
        }
    }

    [Serializable]
    public class XboxProfile : Profile
    {
        public void SetDefaultProfile(XboxResourcePaths paths)
        {
            buttonPressTransformScale = 0.9f;
            InitializeButtonProfiles();

            SetupButton(buttonEast, new Vector2(0.69f, 0.25f), paths.buttonB, ButtonName.ButtonEast);
            SetupButton(buttonSouth, new Vector2(0.58f, 0.06f), paths.buttonA, ButtonName.ButtonSouth);
            SetupButton(buttonWest, new Vector2(0.48f, 0.25f), paths.buttonX, ButtonName.ButtonWest);
            SetupButton(buttonNorth, new Vector2(0.58f, 0.44f), paths.buttonY, ButtonName.ButtonNorth);

            SetupButton(leftShoulder, new Vector2(-0.44f, 0.78f), paths.leftBumper, ButtonName.LeftShoulder);
            SetupButton(rightShoulder, new Vector2(0.44f, 0.78f), paths.rightBumper, ButtonName.RightShoulder);
            SetupButton(leftTrigger, new Vector2(-0.71f, 0.78f), paths.leftTrigger, ButtonName.LeftTrigger);
            SetupButton(rightTrigger, new Vector2(0.71f, 0.78f), paths.rightTrigger, ButtonName.RightTrigger);

            SetupButton(leftStickButton, new Vector2(-0.65f, -0.24f), paths.leftStickButton, ButtonName.LeftStickButton);
            SetupButton(rightStickButton, new Vector2(0.65f, -0.24f), paths.rightStickButton, ButtonName.RightStickButton);

            SetupButton(startButton, new Vector2(0.16f, 0.51f), paths.startButton, ButtonName.StartButton);
            SetupButton(selectButton, new Vector2(-0.16f, 0.51f), paths.selectButton, ButtonName.SelectButton);

            SetupButton(dPad, new Vector2(-0.39f, -0.57f), paths.dPad, ButtonName.DPad, false);
            SetupButton(leftStick, new Vector2(-0.58f, 0.25f), paths.leftStick, ButtonName.LeftStick);
            SetupButton(rightStick, new Vector2(0.39f, -0.57f), paths.rightStick, ButtonName.RightStick);
        }

        public void SetDefaultCustomProfile(XboxResourcePaths paths)
        {
            SetDefaultProfile(paths);

            dPad.position = new Vector2(-0.58f, 0.25f);
            leftStick.position = new Vector2(-0.39f, -0.57f);
        }

        public void RefreshIcons(XboxResourcePaths paths)
        {
            if (buttonEast != null) buttonEast.iconImage = paths.buttonB;
            if (buttonSouth != null) buttonSouth.iconImage = paths.buttonA;
            if (buttonWest != null) buttonWest.iconImage = paths.buttonX;
            if (buttonNorth != null) buttonNorth.iconImage = paths.buttonY;

            if (leftShoulder != null) leftShoulder.iconImage = paths.leftBumper;
            if (rightShoulder != null) rightShoulder.iconImage = paths.rightBumper;
            if (leftTrigger != null) leftTrigger.iconImage = paths.leftTrigger;
            if (rightTrigger != null) rightTrigger.iconImage = paths.rightTrigger;

            if (leftStickButton != null) leftStickButton.iconImage = paths.leftStickButton;
            if (rightStickButton != null) rightStickButton.iconImage = paths.rightStickButton;

            if (startButton != null) startButton.iconImage = paths.startButton;
            if (selectButton != null) selectButton.iconImage = paths.selectButton;

            if (dPad != null) dPad.iconImage = paths.dPad;
            if (leftStick != null) leftStick.iconImage = paths.leftStick;
            if (rightStick != null) rightStick.iconImage = paths.rightStick;
        }

        private static void SetupButton(ButtonProfile button, Vector2 position, Sprite sprite, ButtonName buttonName, bool pressToActivate = true)
        {
            button.position = position;
            button.scale = new Vector2(1f, 1f);
            button.iconImage = sprite;
            button.name = buttonName;
            button.pressToActivate = pressToActivate;
        }
    }

    [Serializable]
    public class DualShockProfile : Profile
    {
        public void SetDefaultProfile(PlayStationResourcePaths paths)
        {
            buttonPressTransformScale = 0.9f;
            InitializeButtonProfiles();

            SetupButton(buttonEast, new Vector2(0.69f, 0.25f), paths.buttonCircle, ButtonName.ButtonEast);
            SetupButton(buttonSouth, new Vector2(0.58f, 0.06f), paths.buttonCross, ButtonName.ButtonSouth);
            SetupButton(buttonWest, new Vector2(0.48f, 0.25f), paths.buttonSquare, ButtonName.ButtonWest);
            SetupButton(buttonNorth, new Vector2(0.58f, 0.44f), paths.buttonTriangle, ButtonName.ButtonNorth);

            SetupButton(leftShoulder, new Vector2(-0.44f, 0.78f), paths.l1Button, ButtonName.LeftShoulder);
            SetupButton(rightShoulder, new Vector2(0.44f, 0.78f), paths.r1Button, ButtonName.RightShoulder);
            SetupButton(leftTrigger, new Vector2(-0.71f, 0.78f), paths.l2Trigger, ButtonName.LeftTrigger);
            SetupButton(rightTrigger, new Vector2(0.71f, 0.78f), paths.r2Trigger, ButtonName.RightTrigger);

            SetupButton(leftStickButton, new Vector2(-0.65f, -0.24f), paths.l3Button, ButtonName.LeftStickButton);
            SetupButton(rightStickButton, new Vector2(0.65f, -0.24f), paths.r3Button, ButtonName.RightStickButton);

            SetupButton(startButton, new Vector2(-0.16f, 0.51f), paths.optionsButton, ButtonName.StartButton);
            SetupButton(selectButton, new Vector2(0.16f, 0.51f), paths.shareButton, ButtonName.SelectButton);

            SetupButton(dPad, new Vector2(-0.58f, 0.25f), paths.dPad, ButtonName.DPad, false);
            SetupButton(leftStick, new Vector2(-0.39f, -0.57f), paths.leftStick, ButtonName.LeftStick);
            SetupButton(rightStick, new Vector2(0.39f, -0.57f), paths.rightStick, ButtonName.RightStick);
        }

        public void SetDefaultCustomProfile(PlayStationResourcePaths paths)
        {
            SetDefaultProfile(paths);
            dPad.position = new Vector2(-0.39f, -0.57f);
            leftStick.position = new Vector2(-0.58f, 0.25f);
        }

        public void RefreshIcons(PlayStationResourcePaths paths)
        {
            if (buttonEast != null) buttonEast.iconImage = paths.buttonCircle;
            if (buttonSouth != null) buttonSouth.iconImage = paths.buttonCross;
            if (buttonWest != null) buttonWest.iconImage = paths.buttonSquare;
            if (buttonNorth != null) buttonNorth.iconImage = paths.buttonTriangle;

            if (leftShoulder != null) leftShoulder.iconImage = paths.l1Button;
            if (rightShoulder != null) rightShoulder.iconImage = paths.r1Button;
            if (leftTrigger != null) leftTrigger.iconImage = paths.l2Trigger;
            if (rightTrigger != null) rightTrigger.iconImage = paths.r2Trigger;

            if (leftStickButton != null) leftStickButton.iconImage = paths.l3Button;
            if (rightStickButton != null) rightStickButton.iconImage = paths.r3Button;

            if (startButton != null) startButton.iconImage = paths.optionsButton;
            if (selectButton != null) selectButton.iconImage = paths.shareButton;

            if (dPad != null) dPad.iconImage = paths.dPad;
            if (leftStick != null) leftStick.iconImage = paths.leftStick;
            if (rightStick != null) rightStick.iconImage = paths.rightStick;
        }

        private static void SetupButton(ButtonProfile button, Vector2 position, Sprite sprite, ButtonName buttonName, bool pressToActivate = true)
        {
            button.position = position;
            button.scale = new Vector2(1f, 1f);
            button.iconImage = sprite;
            button.name = buttonName;
            button.pressToActivate = pressToActivate;
        }
    }

    [Serializable]
    public class ButtonProfile
    {
        public ButtonName name;

        public Vector2 position;
        public Vector2 scale;

        public Sprite iconImage;
        public Sprite backgoundImage;

        public bool isVisible = true;
        public bool pressToActivate = true;
        public bool toggle = false;

    }
}
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GamepadConfig;

public class ProfileSelector : MonoBehaviour
{
    const int HARCODED_DEFAULT_PROFILE = 0;
    const int HARCODED_CUSTOM_PROFILE = 1;

    [SerializeField] TMP_Dropdown xboxProfileSelector;
    [SerializeField] TMP_Dropdown dualShockProfileSelector;

    // Event that sends both profile indices
    public static event Action<int, int> OnSelectionChanged;

    public class ProfileOptionData : TMP_Dropdown.OptionData
    {
        public int ProfileIndex { get; private set; }

        public ProfileOptionData(string text, int profileIndex) : base(text)
        {
            ProfileIndex = profileIndex;
        }
    }

    void Start()
    {
        InitializeDropdowns();
    }

    void InitializeDropdowns()
    {
        xboxProfileSelector.ClearOptions();
        dualShockProfileSelector.ClearOptions();

        var options = new System.Collections.Generic.List<TMP_Dropdown.OptionData>
        {
            new ProfileOptionData("Default distribution", HARCODED_DEFAULT_PROFILE),
            new ProfileOptionData("Custom distribution", HARCODED_CUSTOM_PROFILE)
        };

        xboxProfileSelector.AddOptions(options);
        dualShockProfileSelector.AddOptions(options);

        xboxProfileSelector.value = 0;
        dualShockProfileSelector.value = 0;

        xboxProfileSelector.onValueChanged.AddListener(OnXboxProfileSelected);
        dualShockProfileSelector.onValueChanged.AddListener(OnDualShockProfileSelected);
    }

    void OnXboxProfileSelected(int index)
    {
        var selectedOption = xboxProfileSelector.options[index] as ProfileOptionData;
        if (selectedOption != null)
        {
            int xboxProfileIndex = selectedOption.ProfileIndex;
            int dualShockProfileIndex = GetCurrentDualShockProfileIndex();
            
            // Fire the event with both indices
            OnSelectionChanged?.Invoke(xboxProfileIndex, dualShockProfileIndex);

            AppLifeTimeManager.Instance.SetSessionConfigProfileIndex(xboxProfileIndex);
        }
    }

    void OnDualShockProfileSelected(int index)
    {
        var selectedOption = dualShockProfileSelector.options[index] as ProfileOptionData;
        if (selectedOption != null)
        {
            int xboxProfileIndex = GetCurrentXboxProfileIndex();
            int dualShockProfileIndex = selectedOption.ProfileIndex;
            
            // Fire the event with both indices
            OnSelectionChanged?.Invoke(xboxProfileIndex, dualShockProfileIndex);

            AppLifeTimeManager.Instance.SetSessionConfigProfileIndex(dualShockProfileIndex);
        }
    }

    private int GetCurrentXboxProfileIndex()
    {
        var selectedOption = xboxProfileSelector.options[xboxProfileSelector.value] as ProfileOptionData;
        return selectedOption?.ProfileIndex ?? HARCODED_DEFAULT_PROFILE;
    }

    private int GetCurrentDualShockProfileIndex()
    {
        var selectedOption = dualShockProfileSelector.options[dualShockProfileSelector.value] as ProfileOptionData;
        return selectedOption?.ProfileIndex ?? HARCODED_DEFAULT_PROFILE;
    }
}
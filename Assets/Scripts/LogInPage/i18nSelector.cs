
using UnityEngine;
using static Languages;
using TMPro;
using System.Collections.Generic;
using System.Linq;
public class i18nSelector : MonoBehaviour
{
    public class LanguageOptionData : TMP_Dropdown.OptionData
    {
        public Language LanguageOption { get; private set; }
        public LanguageOptionData(string text, Language lang) : base(text)
        {
            LanguageOption = lang;
        }
    }

    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private TMP_Text selectedText;
    private const string langPererence = "lang_pererence";
    private List<LanguageOptionData> languageOptions;
    void Start()
    {
        InitializeDropdown();
    }
    void InitializeDropdown()
    {
        dropdown.ClearOptions();
        languageOptions = new List<LanguageOptionData>
        {
            new LanguageOptionData(i18nManager.Instance.Translate("lang_zh"), Language.zh),
            new LanguageOptionData(i18nManager.Instance.Translate("lang_en"), Language.en),
            new LanguageOptionData(i18nManager.Instance.Translate("lang_es"), Language.es)
        };
        dropdown.AddOptions(languageOptions.Cast<TMP_Dropdown.OptionData>().ToList());
        Language currentLang = i18nManager.Instance.GetCurrentLanguage();
        SetDropdownValue(currentLang);
        dropdown.onValueChanged.AddListener(OnLanguageSelected);
    }
    public void SetDropdownValue(Language language)
    {
        for (int i = 0; i < languageOptions.Count; i++)
        {
            if (languageOptions[i].LanguageOption == language)
            {
                dropdown.value = i;
                break;
            }
        }
    }
    public Language GetSelectedLanguage()
    {
        if (dropdown.value >= 0 && dropdown.value < languageOptions.Count)
        {
            return languageOptions[dropdown.value].LanguageOption;
        }
        return Language.en;
    }
    private void OnLanguageSelected(int index)
    {
        if (index >= 0 && index < languageOptions.Count)
        {
            Language selectedLang = languageOptions[index].LanguageOption;
            i18nManager.Instance.SetLanguage(selectedLang);
            PlayerPrefs.SetString(langPererence, selectedLang.ToString());
        }
    }
}
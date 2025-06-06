using System.Collections.Generic;
using UnityEngine;
using static Languages;
using Newtonsoft.Json;
using TMPro;

public class i18nManager : MonoBehaviour
{
    private const string simplifiedChineseFontFamily = "Fonts/NotoSansSC-Regular SDF";
    private const string genericFontFamily = "Fonts/LiberationSerif-Regular SDF";

    public static i18nManager Instance { get; private set; }
    [SerializeField] private Language currentLanguage = Language.en;
    private Dictionary<string, string> translations = new Dictionary<string, string>();
    private Dictionary<TextMeshProUGUI, string> originalKeys = new Dictionary<TextMeshProUGUI, string>();
    private const string langPererence = "lang_pererence";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadConfigurationLanguage();
            LoadTranslation();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        TranslateAllTextMeshPro();
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        TranslateAllTextMeshPro();
    }

    public Language GetCurrentLanguage()
    {
        return currentLanguage;
    }

    public void LoadTranslation()
    {
        string langCode = currentLanguage.ToString();
        string jsonPath = $"Languages/{langCode}";
        TextAsset json = Resources.Load<TextAsset>(jsonPath);
        if (json != null)
        {
            Debug.Log($"Loading translations from: {jsonPath}");
            translations = JsonConvert.DeserializeObject<Dictionary<string, string>>(json.text);
        }
        else
        {
            Debug.LogError($"Translation file not found: {jsonPath}");
            translations = new Dictionary<string, string>();
        }
    }

    public void TranslateAllTextMeshPro()
    {
        TextMeshProUGUI[] allTexts = FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (TextMeshProUGUI textComponent in allTexts)
        {
            if (ShouldSkipTranslation(textComponent))
            {
                if (currentLanguage == Language.zh)
                {
                    textComponent.font = Resources.Load<TMP_FontAsset>(simplifiedChineseFontFamily);
                }
                else
                {
                    textComponent.font = Resources.Load<TMP_FontAsset>(genericFontFamily);
                }

                continue;
            }

            if (!originalKeys.ContainsKey(textComponent))
            {
                originalKeys[textComponent] = textComponent.text;
            }

            string originalKey = originalKeys[textComponent];
            textComponent.text = Translate(originalKey);

            if (currentLanguage == Language.zh)
            {
                textComponent.font = Resources.Load<TMP_FontAsset>(simplifiedChineseFontFamily);
            }
            else
            {
                textComponent.font = Resources.Load<TMP_FontAsset>(genericFontFamily);
            }
        }

        CleanupDestroyedReferences();
    }

    private bool ShouldSkipTranslation(TextMeshProUGUI textComponent)
    {
        TMP_Dropdown dropdown = textComponent.GetComponentInParent<TMP_Dropdown>();
        if (dropdown != null)
        {
            if (textComponent == dropdown.captionText)
            {
                return true;
            }
        }

        return false;
    }

    private void CleanupDestroyedReferences()
    {
        List<TextMeshProUGUI> keysToRemove = new List<TextMeshProUGUI>();
        foreach (var kvp in originalKeys)
        {
            if (kvp.Key == null)
            {
                keysToRemove.Add(kvp.Key);
            }
        }

        foreach (var key in keysToRemove)
        {
            originalKeys.Remove(key);
        }
    }

    public string Translate(string key)
    {
        if (translations.ContainsKey(key))
        {
            return translations[key];
        }
        return key;
    }

    public void SetLanguage(Language language)
    {
        currentLanguage = language;
        PlayerPrefs.SetString(langPererence, language.ToString());
        LoadTranslation();
        TranslateAllTextMeshPro();
    }

    public void LoadConfigurationLanguage()
    {
        string langPreference = PlayerPrefs.GetString(langPererence, Language.en.ToString());
        if (System.Enum.TryParse(langPreference, out Language language))
        {
            currentLanguage = language;
        }
        else
        {
            currentLanguage = Language.en;
        }
        Debug.Log($"Current language set to: {currentLanguage}");
    }

    public void TranslateSpecificText(TextMeshProUGUI textComponent, string key)
    {
        if (textComponent != null)
        {
            textComponent.text = Translate(key);

            if (currentLanguage == Language.zh)
            {
                textComponent.font = Resources.Load<TMP_FontAsset>(simplifiedChineseFontFamily);
            }
            else
            {
                textComponent.font = Resources.Load<TMP_FontAsset>(genericFontFamily);
            }
        }
    }

    public string GetCurrentFont()
    {
        if(currentLanguage == Language.zh)
        {
            return simplifiedChineseFontFamily;
        }
        else
        {
            return genericFontFamily;
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;

public class i18nManager : MonoBehaviour
{
    public static i18nManager Instance { get; private set; }
    [SerializeField] private Languages.Language currentLanguage = Languages.Language.en;
    private Dictionary<string, string> translations = new Dictionary<string, string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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

    public void LoadTranslation()
    {
        string langCode = currentLanguage.ToString();
        string jsonPath = $"Languages/{langCode}";
        TextAsset json = Resources.Load<TextAsset>(jsonPath);
        if (json != null)
        {
            Debug.Log($"Loading translations from: {jsonPath}");
            translations = JsonConvert.DeserializeObject<Dictionary<string, string>>(json.text);
            foreach (var kvp in translations)
            {
                Debug.Log($"Key: {kvp.Key}, Value: {kvp.Value}");
            }
        }
        else
        {
            Debug.LogError($"Translation file not found: {jsonPath}");
            translations = new Dictionary<string, string>();
        }
    }

    public void TranslateAllTextMeshPro()
    {
        TextMeshProUGUI[] allTexts = FindObjectsOfType<TextMeshProUGUI>(true);

        foreach (TextMeshProUGUI textComponent in allTexts)
        {
            textComponent.text = Translate(textComponent.text);
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

    public void SetLanguage(Languages.Language language)
    {
        currentLanguage = language;
        LoadTranslation();
        TranslateAllTextMeshPro();

        // NotoSansSC-Regular SDF for chinese
        // NotoSerif-Regular SDF for the rest of languages
    }
}
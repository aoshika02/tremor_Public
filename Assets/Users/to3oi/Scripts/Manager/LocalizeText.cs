using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocalizeText : SingletonMonoBehaviour<LocalizeText>
{
    private static List<KeyToTextFormat> _localizeTextInstance;
    private static Language _language;
    public static Language Language => _language;

    protected override void Awake()
    {
        if (CheckInstance())
        {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            
            try
            {
                // テキストDBの読み込み
                TextAsset jsonText = Resources.Load<TextAsset>("LocalizeText");
                _localizeTextInstance = JsonHelper.FromJson<KeyToTextFormat>(jsonText.text);

                _language = (Language)PlayerPrefs.GetInt("Language", 0);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }

    public static string GetText(string key)
    {
        var text = _localizeTextInstance.FirstOrDefault(x => x.FindKey == key);
        if (text == null)
        {
            Debug.LogError($"Keyが存在しません log : {key} is null");
            if (key == "")
            {
                return "";
            }
            else
            {
                return $"{key} is null";
            }
        }

        switch (_language)
        {
            case Language.Japanese:
                return text.JP;
            case Language.English:
                return text.EN;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static void SetLanguage(Language language)
    {
        _language = language;
        PlayerPrefs.SetInt("Language", (int)_language);
    }


    private void OnDestroy()
    {
        PlayerPrefs.SetInt("Language", (int)_language);
    }
}
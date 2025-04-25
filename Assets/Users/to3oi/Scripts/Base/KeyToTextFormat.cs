using System;
using System.Collections.Generic;

[Serializable]
public class KeyToTextFormat
{
    public string Type;
    public string Key;
    public string FindKey;
    public string JP;
    public string EN;
    public string Memo;
}

public static class JsonHelper
{
    public static List<T> FromJson<T>(string json)
    {
        Wrapper<T> wrapper = UnityEngine.JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.KeyTexts;
    }

    [Serializable]
    private class Wrapper<T>
    {
        public List<T> KeyTexts;
    }
}
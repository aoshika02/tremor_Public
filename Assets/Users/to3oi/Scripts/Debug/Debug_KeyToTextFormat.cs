using UnityEngine;

public class Debug_KeyToTextFormat : MonoBehaviour
{
    [SerializeField] private string key;
    [SerializeField] private Language language;

    private void OnGUI()
    {
        if (GUILayout.Button("GetText"))
        {
            Debug.Log(LocalizeText.GetText(key));
        }

        if (GUILayout.Button("SetLanguage"))
        {
            LocalizeText.SetLanguage(language);
        }
    }
}
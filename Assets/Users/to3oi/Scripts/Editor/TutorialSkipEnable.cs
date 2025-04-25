#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class TutorialSkipEnable
{
    [MenuItem("Tools/Skip Event/Tutorial Skip")]
    private static void TutorialSkip()
    {
        var b = Utility.IntToBool(PlayerPrefs.GetInt("TutorialSkip", 0));
        PlayerPrefs.SetInt("TutorialSkip",Utility.BoolToInt(!b));
        Debug.Log($"TutorialSkip is {!b}");
    }
}
#endif
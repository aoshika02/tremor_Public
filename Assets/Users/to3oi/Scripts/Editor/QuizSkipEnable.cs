#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class QuizSkipEnable : MonoBehaviour
{
    [MenuItem("Tools/Skip Event/Quiz Skip")]
    private static void QuizSkip()
    {
        var b = Utility.IntToBool(PlayerPrefs.GetInt("QuizSkip", 0));
        PlayerPrefs.SetInt("QuizSkip", Utility.BoolToInt(!b));
        Debug.Log($"QuizSkip is {!b}");
    }
}
#endif

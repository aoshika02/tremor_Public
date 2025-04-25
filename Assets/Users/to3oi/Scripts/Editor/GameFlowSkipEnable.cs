#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class GameFlowSkipEnable : MonoBehaviour
{
    [MenuItem("Tools/Skip Event/GameFlow Skip")]
    private static void GameFlowSkip()
    {
        var b = Utility.IntToBool(PlayerPrefs.GetInt("GameFlowSkip", 0));
        PlayerPrefs.SetInt("GameFlowSkip",Utility.BoolToInt(!b));
        Debug.Log($"GameFlowSkip is {!b}");
    }
}
#endif
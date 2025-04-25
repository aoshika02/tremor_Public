#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class TabletEventSkipEnable 
{
    [MenuItem("Tools/Skip Event/TabletEvent Skip")]
    private static void TabletEventSkip()
    {
        var b = Utility.IntToBool(PlayerPrefs.GetInt("TabletEventSkip", 0));
        PlayerPrefs.SetInt("TabletEventSkip",Utility.BoolToInt(!b));
        Debug.Log($"TabletEventSkip is {!b}");
    }
}
#endif
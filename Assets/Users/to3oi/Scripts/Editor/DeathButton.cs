#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
public class DeathButton : MonoBehaviour
{
    [MenuItem("Tools/Death")]
    private static void Death()
    {
        InGameFlow.Instance.AttackedFlag();
        Debug.Log($"Debug Death");
    }
}
#endif
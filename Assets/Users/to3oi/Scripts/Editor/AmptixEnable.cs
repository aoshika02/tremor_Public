#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.PackageManager;

public class AmptixEnable : MonoBehaviour
{
    private static string _packageName = "com.miraisens.amptixsdk";
    private static string _packageFilePath = "file:../SDKs/com.miraisens.amptixsdk-1.0.0_eval.tgz";

    [MenuItem("Tools/AmptixEnable")]
    private static void SetAmptix()
    {
        if (Utility.IntToBool(PlayerPrefs.GetInt("Amptix", 0)))
        {
            // false
            PlayerPrefs.SetInt("Amptix", 0);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone,
                new string[]
                {
                    "DOTWEEN",
                    "UNITASK_DOTWEEN_SUPPORT",
                    "UNITY_POST_PROCESSING_STACK_V2",
                });
            //Remove();
        }
        else
        {
            // true
            PlayerPrefs.SetInt("Amptix", 1);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone,
                new string[]
                {
                    "DOTWEEN",
                    "UNITASK_DOTWEEN_SUPPORT",
                    "UNITY_POST_PROCESSING_STACK_V2",
                    "Amptix"
                });
            //Import();
        }

        Debug.Log($"Amptix Enable is {Utility.IntToBool(PlayerPrefs.GetInt("Amptix", 0))}");
    }

    private static void Import()
    {
        var request = Client.List();

        while (!request.IsCompleted)
        {
        }

        if (request.Result.Any(x => x.name == "com.miraisens.amptixsdk"))
        {
            Debug.Log("Already Installed");
            return;
        }

        var addRequest = Client.Add(_packageFilePath);

        EditorApplication.update += Progress;


        void Progress()
        {
            if (addRequest.IsCompleted)
            {
                if (addRequest.Status == StatusCode.Success)
                {
                    Debug.Log("Installed: " + addRequest.Result.packageId);
                }
                else if (addRequest.Status >= StatusCode.Failure)
                {
                    Debug.Log(addRequest.Error.message);
                }

                EditorApplication.update -= Progress;
            }
        }
    }

    private static void Remove()
    {
        var request = Client.List();

        while (!request.IsCompleted)
        {
        }

        if (request.Result.Any(x => x.name == "com.miraisens.amptixsdk"))
        {
            var removeRequest = Client.Remove(_packageName);

            EditorApplication.update += Progress;

            void Progress()
            {
                if (removeRequest.IsCompleted)
                {
                    if (removeRequest.Status == StatusCode.Success)
                    {
                        Debug.Log("remove");
                    }
                    else if (removeRequest.Status >= StatusCode.Failure)
                    {
                        Debug.Log(removeRequest.Error.message);
                    }

                    EditorApplication.update -= Progress;
                }
            }
        }
    }
}
#endif
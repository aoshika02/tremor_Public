using UnityEditor;
using UnityEngine;
#if Amptix
using AmptixSdk;
#endif

public class SESetEditor : EditorWindow
{
    public SEClips sObject;
    private Vector2 scrollPos = Vector2.zero;
    private int index = 0;
    private string[] enums;
    private string[] _audioVolumeTypeEnums;
    private int _audioVolumeTypeEnumsIndex;

    [MenuItem("Tools/Set SE")]
    static void Open()
    {
        var window = GetWindow<SESetEditor>();
        window.titleContent = new GUIContent("SESetEditor");
        window.Init();
    }

    public void Init()
    {
        var seTypeLength = System.Enum.GetNames(typeof(SEType)).Length;
        enums = new string[seTypeLength + 1];
        enums[0] = "None";
        for (int i = 0; i < seTypeLength; i++)
        {
            enums[i + 1] = System.Enum.GetNames(typeof(SEType))[i];
        }

        var audioVolumeTypeLength = System.Enum.GetNames(typeof(AudioVolumeType)).Length;
        _audioVolumeTypeEnums = new string[audioVolumeTypeLength];
        for (int i = 0; i < audioVolumeTypeLength; i++)
        {
            _audioVolumeTypeEnums[i] = System.Enum.GetNames(typeof(AudioVolumeType))[i];
        }
    }

    /// <Summary>
    /// ウィンドウのパーツを表示する
    /// </Summary>
    void OnGUI()
    {
        EditorGUILayout.LabelField("SEClips");
        EditorGUILayout.LabelField($"Amptix {PlayerPrefs.GetInt("Amptix", 0) == 1}");
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("Search SEType", GUILayout.Width(100));

            index = EditorGUILayout.Popup(index, enums);
            Texture reload = (Texture)EditorGUIUtility.Load("d_Refresh");
            if (GUILayout.Button(reload, GUILayout.Width(50)))
            {
                index = 0;
            }

            Texture save = (Texture)EditorGUIUtility.Load("d_SaveAs");
            if (GUILayout.Button(save, GUILayout.Width(50)))
            {
                AssetDatabase.SaveAssets();
            }
        }

        //ScriptableObjectの取得
        if (sObject == null)
        {
            var guids = UnityEditor.AssetDatabase.FindAssets("t:SEClips");
            if (guids.Length == 0)
            {
                throw new System.IO.FileNotFoundException("SEClips does not found");
            }

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            sObject = AssetDatabase.LoadAssetAtPath<SEClips>(path);
        }

        if (sObject != null)
        {
            if (GUILayout.Button("Force Reload Amptix Asset", GUILayout.Width(200)))
            {
                foreach (var clip in sObject.SEClipList)
                {
                    clip.SetupAmptixAsset();
                }
            }

            EditorGUILayout.LabelField("========================================");
            using (var h = new EditorGUILayout.HorizontalScope())
            {
                using (var scrollView =
                       new EditorGUILayout.ScrollViewScope(scrollPos))
                {
                    scrollPos = scrollView.scrollPosition;
                    if (index == 0)
                    {
                        foreach (var clip in sObject.SEClipList)
                        {
                            ViewClip(clip);
                        }
                    }
                    else
                    {
                        EditorGUILayout.Space(10);
                        GUIStyle style = new GUIStyle(EditorStyles.label);
                        style.fontSize = 20;
                        var clip = sObject.SEClipList.Find(x => x.SEType.ToString() == enums[index]);
                        if (clip != null)
                        {
                            ViewClip(clip);
                        }
                    }
                }
                EditorUtility.SetDirty(sObject);
            }
        }
    }

    private void ViewClip(SEClip clip)
    {
        GUIStyle style = new GUIStyle(EditorStyles.label);
        style.fontSize = 20;
        EditorGUILayout.LabelField(clip.SEType.ToString(), style);
        EditorGUILayout.Space(10);
        using (new EditorGUILayout.HorizontalScope())
        {
            clip.Clip = (AudioClip)EditorGUILayout.ObjectField(clip.Clip, typeof(AudioClip), false);
            EditorGUILayout.LabelField("Is3D", GUILayout.Width(30));
            clip.Is3D = EditorGUILayout.Toggle(clip.Is3D, GUILayout.Width(25));
            EditorGUILayout.LabelField("IsHaptics", GUILayout.Width(30));
            clip.IsHaptics = EditorGUILayout.Toggle(clip.IsHaptics, GUILayout.Width(25));
        }

        using (new EditorGUILayout.HorizontalScope())
        {
            if (clip.IsHaptics)
            {
                EditorGUILayout.LabelField("HapticsPriority", GUILayout.Width(30));
                clip.HapticsPriority = EditorGUILayout.IntField(clip.HapticsPriority, GUILayout.Width(25));
                EditorGUILayout.LabelField("Amptix Asset");
#if Amptix
                clip.AmptixEffectData =
 (AmptixEffectData)EditorGUILayout.ObjectField(clip.AmptixEffectData, typeof(AmptixEffectData), false);
#else
                clip.Amptix = (DefaultAsset)EditorGUILayout.ObjectField(clip.Amptix, typeof(DefaultAsset), false);
#endif
            }
        }
    }
}
using System;
using UnityEditor;
using UnityEngine;
#if Amptix
using AmptixSdk;
#endif

public class BGMSetEditor : EditorWindow
{
    public BGMClips _sObject;
    private Vector2 _scrollPos = Vector2.zero;
    private string[] _bgmEnums;
    private int _bgmEnumsIndex = 0;
    private string[] _audioVolumeTypeEnums;
    private int _audioVolumeTypeEnumsIndex;


    [MenuItem("Tools/Set BGM")]
    static void Open()
    {
        var window = GetWindow<BGMSetEditor>();
        window.titleContent = new GUIContent("BGMSetEditor");
        window.Init();
    }

    public void Init()
    {
        var bgmTypeLength = System.Enum.GetNames(typeof(BGMType)).Length;
        _bgmEnums = new string[bgmTypeLength + 1];
        _bgmEnums[0] = "None";
        for (int i = 0; i < bgmTypeLength; i++)
        {
            _bgmEnums[i + 1] = System.Enum.GetNames(typeof(BGMType))[i];
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
        EditorGUILayout.LabelField("BGMClips");
        EditorGUILayout.LabelField($"Amptix {PlayerPrefs.GetInt("Amptix", 0) == 1}");
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("Search BGMType", GUILayout.Width(100));

            _bgmEnumsIndex = EditorGUILayout.Popup(_bgmEnumsIndex, _bgmEnums);
            Texture reload = (Texture)EditorGUIUtility.Load("d_Refresh");
            if (GUILayout.Button(reload, GUILayout.Width(50)))
            {
                _bgmEnumsIndex = 0;
            }

            Texture save = (Texture)EditorGUIUtility.Load("d_SaveAs");
            if (GUILayout.Button(save, GUILayout.Width(50)))
            {
                AssetDatabase.SaveAssets();
            }
        }

        //ScriptableObjectの取得
        if (_sObject == null)
        {
            var guids = UnityEditor.AssetDatabase.FindAssets("t:BGMClips");
            if (guids.Length == 0)
            {
                throw new System.IO.FileNotFoundException("BGMClips does not found");
            }

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            _sObject = AssetDatabase.LoadAssetAtPath<BGMClips>(path);
        }

        if (_sObject != null)
        {
            if (GUILayout.Button("Force Reload Amptix Asset", GUILayout.Width(200)))
            {
                foreach (var clip in _sObject.BGMClipList)
                {
                    clip.SetupAmptixAsset();
                }
            }

            EditorGUILayout.LabelField("========================================");
            using (var h = new EditorGUILayout.HorizontalScope())
            {
                using (var scrollView =
                       new EditorGUILayout.ScrollViewScope(_scrollPos))
                {
                    _scrollPos = scrollView.scrollPosition;
                    if (_bgmEnumsIndex == 0)
                    {
                        foreach (var clip in _sObject.BGMClipList)
                        {
                            ViewClip(clip);
                        }
                    }
                    else
                    {
                        EditorGUILayout.Space(10);
                        GUIStyle style = new GUIStyle(EditorStyles.label);
                        style.fontSize = 20;
                        var clip = _sObject.BGMClipList.Find(x => x.BGMType.ToString() == _bgmEnums[_bgmEnumsIndex]);
                        if (clip != null)
                        {
                            ViewClip(clip);
                        }
                    }
                }

                EditorUtility.SetDirty(_sObject);
            }
        }
    }

    private void ViewClip(BGMClip clip)
    {
        GUIStyle style = new GUIStyle(EditorStyles.label);
        style.fontSize = 20;
        EditorGUILayout.LabelField(clip.BGMType.ToString(), style);
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

        using (new EditorGUILayout.HorizontalScope())
        {
            clip.AudioVolumeType = (AudioVolumeType)EditorGUILayout.Popup("AudioVolumeType", (int)clip.AudioVolumeType,
                _audioVolumeTypeEnums);
        }

        EditorGUILayout.Space(20);
    }
}
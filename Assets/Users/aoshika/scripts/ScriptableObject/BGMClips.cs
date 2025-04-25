using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if Amptix
using AmptixSdk;
#endif

[CreateAssetMenu(fileName = "BGMClips", menuName = "Data/BGMClips")]
public class BGMClips : ScriptableObject
{
    public List<BGMClip> BGMClipList = new List<BGMClip>();
    private List<BGMClip> _oldBgmClip = new List<BGMClip>();

    void OnValidate()
    {
        if (BGMClipList.Count == _oldBgmClip.Count)
        {
            for (int i = 0; i < BGMClipList.Count; i++)
            {
                BGMClipList[i].SetupAmptixAsset();
            }
        }

        _oldBgmClip = BGMClipList;
    }
}

[System.Serializable]
public class BGMClip
{
    public BGMType BGMType;
    public AudioClip Clip;
    public bool Is3D;
    public bool IsHaptics;
    public int HapticsPriority;
    public string amptixGUID = String.Empty;
    public AudioVolumeType AudioVolumeType = AudioVolumeType.BGM;
    
#if Amptix
    public AmptixEffectData AmptixEffectData;
#else
#if UNITY_EDITOR
    public DefaultAsset Amptix;
#endif
#endif

    public void SetupAmptixAsset()
    {
#if UNITY_EDITOR
#if Amptix
        if (AmptixEffectData != null)
        {
            amptixGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(AmptixEffectData));
        }
#else
        if (Amptix != null)
        {
            amptixGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(Amptix));
        }
#endif
        else
        {
            if (amptixGUID != String.Empty)
            {
#if Amptix
                AmptixEffectData =
                    AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(amptixGUID),
                            typeof(AmptixEffectData)) as
                        AmptixEffectData;
#else
                Amptix =
                    AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(amptixGUID),
                            typeof(DefaultAsset)) as
                        DefaultAsset;
#endif
            }
            else
            {
                amptixGUID = String.Empty;
            }
        }
#endif
    }
}
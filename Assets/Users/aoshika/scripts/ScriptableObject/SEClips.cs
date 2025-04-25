using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if Amptix
using AmptixSdk;
#endif

[CreateAssetMenu(fileName = "SEClips", menuName = "Data/SEClips")]
public class SEClips : ScriptableObject
{
    public List<SEClip> SEClipList = new List<SEClip>();
    private List<SEClip> _oldSEClip = new List<SEClip>();

    void OnValidate()
    {
        if (SEClipList.Count == _oldSEClip.Count)
        {
            for (int i = 0; i < SEClipList.Count; i++)
            {
                SEClipList[i].SetupAmptixAsset();
            }
        }

        _oldSEClip = SEClipList;
    }
}

[System.Serializable]
public class SEClip
{
    public SEType SEType;
    public AudioClip Clip;
    public bool Is3D;
    public bool IsHaptics;
    public int HapticsPriority;
    public string amptixGUID = String.Empty;
    public AudioVolumeType AudioVolumeType = AudioVolumeType.Default; 
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
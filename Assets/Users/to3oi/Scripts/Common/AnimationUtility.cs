using System;
using System.Collections.Generic;
using UnityEngine;
public class AnimationUtility : MonoBehaviour
{
    [SerializeField]
    private List<AnimationClipSEData> animationClipSeDatas = new List<AnimationClipSEData>();
    public void PlaySE(int i)
    {
        if(i < 0 || animationClipSeDatas.Count <= i ) { Debug.LogError($"AnimationUtility:index�͈͊O {i}"); return;}
        Transform transform = animationClipSeDatas[i].Transform;
        SEType type = animationClipSeDatas[i].SEType;
        SoundManager.Instance.PlaySE(type,transform);
    }
}

[Serializable]
public class AnimationClipSEData
{
    public Transform Transform;
    public SEType SEType;
}
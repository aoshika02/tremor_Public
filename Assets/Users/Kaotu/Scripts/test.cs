using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField] private List<SEClipSEData> seClipSeData = new List<SEClipSEData>();

    [SerializeField] private bool isOneShot = true;
    [SerializeField] private Transform start;
    [SerializeField] private Transform end;
    private int _playSECount = 0;

    public void CallOnTriggerEnter(Collider collider, CallOnTrigger _)
    {
        if (isOneShot && _playSECount == 0)
        {
            PlaySE(0);
            _playSECount++;
        }
    }

    public void PlaySE(int index)
    {
        if (index < 0 || seClipSeData.Count <= index)
        {
            Debug.LogError($"AnimationUtility:index�͈͊O{index}");
            return;
        }

        Transform t = seClipSeData[index].SEPosition;
        SEType type = seClipSeData[index].SEType;
        SoundManager.Instance.PlaySE(type, t);

        start.transform.DOMove(end.position, 60f);
       

    }

    [Serializable]
    public class SEClipSEData
    {
        public Transform SEPosition;
        public SEType SEType;
    }
}

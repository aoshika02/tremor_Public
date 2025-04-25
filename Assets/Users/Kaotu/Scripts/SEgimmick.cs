using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SEGimmick : MonoBehaviour, OnTriggerEnterInterface
{
    [SerializeField] private List<SEClipSEData> seClipSeData = new List<SEClipSEData>();

    [SerializeField] private bool isOneShot = true;
    [SerializeField] private bool moveSound = false;
    [SerializeField] private float SoundTime = 0;
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

    public async UniTask  tutorialSE()
    {
        PlaySE(0);
        if (moveSound == true)
        {
            await start.transform.DOMove(end.position, SoundTime);
            Destroy(this.gameObject);
        }
    }

    public void PlaySE(int index)
    {
        if (index < 0 || seClipSeData.Count <= index)
        {
            Debug.LogError($"AnimationUtility:index範囲外{index}");
            return;
        }

        Transform t = seClipSeData[index].SEPosition;
        SEType type = seClipSeData[index].SEType;
        SoundManager.Instance.PlaySE(type, t);
    }


    [Serializable]
    public class SEClipSEData
    {
        public Transform SEPosition;
        public SEType SEType;
    }
}
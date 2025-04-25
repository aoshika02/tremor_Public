using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.Playables;

public class HandGimmickTimeLine : MonoBehaviour, OnTriggerEnterInterface
{
    [SerializeField] private int _playTimeLineCount = 0;
    [SerializeField] private PlayableDirector _handgimmick;
    [SerializeField] private GameObject _handgimmicks;
    [SerializeField] private Transform _transform;

    

    public async void CallOnTriggerEnter(Collider collider, CallOnTrigger _)
    {
        if (_playTimeLineCount == 0)
        {
            _handgimmicks.SetActive(true);
            _handgimmick.Play();
            await UniTask.Delay(TimeSpan.FromSeconds(0.8f));
            PlayerMove.Instance.SetMove(false);
            await PlayerMove.Instance.SetPlayerRotation(_transform, 0.1f);
            await UniTask.Delay(TimeSpan.FromSeconds(3f));
            PlayerMove.Instance.SetMove(true);
            _playTimeLineCount++;
            
        }
    }
}
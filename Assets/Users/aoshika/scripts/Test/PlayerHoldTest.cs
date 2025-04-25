using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerHoldTest : MonoBehaviour
{
    [SerializeField] private Transform _transformP;
    [SerializeField] private Transform _transformT;
    [SerializeField] private Transform _gameObject;
    private bool _isArea = false;
    private bool _isHold = false;
    private void Start()
    {
        InputManager.Instance.Decision.Subscribe(x =>
        {
            if (x != 1) { return; }
            if (!_isArea) { return; }
            HoldTest();
        }).AddTo(this);
    }
    private void HoldTest()
    {
        if (!_isHold)
        {
            PlayerItemHold.Instance.PlayerGetItem(_gameObject);
            _isHold = PlayerItemHold.Instance.IsHold;
        }
        else
        {
            PlayerItemHold.Instance.PlayerReleaseItem(_gameObject, _transformP, _transformT);
            _isHold = PlayerItemHold.Instance.IsHold;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isArea = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isArea = false;
        }
    }
}

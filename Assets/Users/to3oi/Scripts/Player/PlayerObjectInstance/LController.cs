using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class LController : SingletonMonoBehaviour<LController>
{
#if UNITY_ANDROID
    private List<Collider> _inHandColliders = new List<Collider>();

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.TryGetComponent<ILIndexTriggerAction>(out _))
        {
            _inHandColliders.Add(collider);
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.TryGetComponent<ILIndexTriggerAction>(out _))
        {
            _inHandColliders.Remove(collider);
        }
    }


    private void Start()
    {
        InputManager.Instance.LIndexTrigger.Subscribe(x =>
        {
            //TODO:いい感じの値
            if (1.0f <= x)
            {
                if (_inHandColliders.Count <= 0)
                {
                    return;
                }

                foreach (var collider in _inHandColliders)
                {
                    if (collider.TryGetComponent<ILIndexTriggerAction>(out ILIndexTriggerAction action))
                    {
                        action.LIndexTriggerAction();
                    }
                }
            }
        }).AddTo(this);
    }
#endif
}
using UnityEngine;
using UniRx;

public class GetNewsPaper : MonoBehaviour, OnTriggerEnterInterface, OnTriggerExitInterface
{
    private bool _isTriggered = false;
    private CallOnTrigger _callOnTrigger;
    private void Start()
    {
        InputManager.Instance.Decision.Subscribe(x =>
        {
            if (_isTriggered is false) { return; }
            if (x == 1f)
            {
                TutorialFlowManager.Instance.GetNewsPaperFlag();
                _callOnTrigger.ActionComplete();
                _callOnTrigger.RemoveActionButtonUIViewer();
            }
            if (x == 0.0f) return;
        }).AddTo(this);
    }
    public void CallOnTriggerEnter(Collider other, CallOnTrigger callOnTrigger)
    {
        _isTriggered = true;
        _callOnTrigger = callOnTrigger;
    }

    public void CallOnTriggerExit(Collider other, CallOnTrigger _)
    {
        _isTriggered = false;
    }
}

using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;

public class TabletTrigger : MonoBehaviour, OnTriggerEnterInterface, OnTriggerExitInterface
{
    private bool isTriggered = false;
    private CallOnTrigger _callOnTrigger;

    private void Start()
    {
        CancelToken cancelToken = new CancelToken();
        InputManager.Instance.Decision.Subscribe(x =>
        {
            if (isTriggered is false) { return; }
            if (TutorialFlowManager.Instance.IsGetItemEvent) return;
            if (x == 1f)
            {
                if (TutorialFlowManager.Instance.IsGetDocument is false)
                {
                    TutorialFlowManager.Instance.GetDocumentFlag();
                }
                else
                {
                    TutorialFlow.Instance.TabletEvent();
                    _callOnTrigger.ActionComplete();
                    _callOnTrigger.RemoveActionButtonUIViewer();
                    
                    cancelToken.Cancel();
                }
            }

            if (x == 0.0f) return;
        }).AddTo(cancelToken.GetToken());
    }

    public void CallOnTriggerEnter(Collider other, CallOnTrigger callOnTrigger)
    {
        isTriggered = true;
        _callOnTrigger = callOnTrigger;
    }

    public void CallOnTriggerExit(Collider other, CallOnTrigger _)
    {
        isTriggered = false;
    }
}
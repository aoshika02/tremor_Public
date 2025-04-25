using UnityEngine;
using UniRx;
using System.Threading.Tasks;

public class GetSawHandle : MonoBehaviour, OnTriggerEnterInterface, OnTriggerExitInterface
{
    private bool _isTriggered = false;
    private CallOnTrigger _callOnTrigger;
    [SerializeField] GameObject _sawHandleObj;
    private bool _isCalled = false;
    private void Start()
    {
        InputManager.Instance.Decision.Subscribe(async x =>
        {
            if (_isTriggered is false) { return; }
            if (x == 1f)
            {
                _callOnTrigger.ActionComplete();
                _callOnTrigger.RemoveActionButtonUIViewer();
                await GetSawHandleEventAsync();
            }
            if (x == 0.0f) return;
        }).AddTo(this);
    }
    private async Task GetSawHandleEventAsync()
    {
        if (_isCalled) return;
        _isCalled = true;
        await GetItemEvent.Instance.ViewItem(ItemEventName.Saw_Hand);
        InGameFlow.Instance.GetSawHandleFlag();
        _sawHandleObj.SetActive(false);
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

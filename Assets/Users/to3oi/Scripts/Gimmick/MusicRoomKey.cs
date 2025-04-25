using UnityEngine;
using UniRx;

public class MusicRoomKey : MonoBehaviour, OnTriggerEnterInterface, OnTriggerExitInterface
{
    private bool _isColliderEnter;
    private CallOnTrigger _callOnTrigger;

    private void Start()
    {
        InputManager.Instance.Decision.Subscribe(async x =>
        {
            if(x != 1.0f){return;}
            if(_isColliderEnter is false){ return;}
            
            await GetItemEvent.Instance.ViewItem(ItemEventName.MusicRoom_GetKey);
            MusicRoomManager.Instance.GetKeyFlag();
            await MessageViewEvent.Instance.ViewText(MessageEventName.MusicRoom_GetKey);
            _callOnTrigger.ActionComplete();
        }).AddTo(this);
    }

    public void CallOnTriggerEnter(Collider other, CallOnTrigger callOnTrigger)
    {
        _isColliderEnter = true;
        _callOnTrigger = callOnTrigger;
    }

    public void CallOnTriggerExit(Collider other, CallOnTrigger callOnTrigger)
    {
        _isColliderEnter = false;
    }
}

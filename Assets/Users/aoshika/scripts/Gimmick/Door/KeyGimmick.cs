using UniRx;
using UnityEngine;

public class KeyGimmick : MonoBehaviour, OnTriggerEnterInterface, OnTriggerExitInterface
{
    private bool _isTriggerEnter = false;
    [SerializeField] private GameObject _keyItem;
    private CallOnTrigger _callOnTrigger;
    private bool _isEvent;
    private void Start()
    {
        InputManager.Instance.Decision.Subscribe(async x =>
        {
            if (_isTriggerEnter == false) return;
            if (x != 1) return;
            if(_isEvent) { return;}
            _isEvent = true;
            _callOnTrigger.ActionComplete();
            _callOnTrigger.RemoveActionButtonUIViewer();
            MusicRoomManager.Instance.GetKeyFlag();
            _keyItem.SetActive(false);
            SoundManager.Instance.PlaySE(SEType.Get_Key_SE,_keyItem.transform);
            await GetItemEvent.Instance.ViewItem(ItemEventName.MusicRoom_GetKey);
            Destroy(gameObject);
        }).AddTo(this);
    }

    public void CallOnTriggerEnter(Collider collider, CallOnTrigger callOnTrigger)
    {
        _callOnTrigger = callOnTrigger;
        _isTriggerEnter = true;
    }

    public void CallOnTriggerExit(Collider collider, CallOnTrigger _)
    {
        _isTriggerEnter = false;
    }
}

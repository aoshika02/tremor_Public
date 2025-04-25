using UnityEngine;
using UniRx;

public class MusicRoomRadio : MonoBehaviour, OnTriggerEnterInterface, OnTriggerExitInterface
{
    [SerializeField] private GameObject _cassetteTape;
    private bool _isColliderEnter;
    private CallOnTrigger _callOnTrigger;
    private bool _isEvent = false;

    private void Start()
    {
        InputManager.Instance.Decision.Subscribe(async x =>
        {
            if (x != 1.0f)
            {
                return;
            }

            if (_isColliderEnter is false)
            {
                return;
            }

            if (_isEvent is true)
            {
                return;
            }

            _isEvent = true;
            
            _callOnTrigger.ActionComplete();
            _callOnTrigger.RemoveActionButtonUIViewer();
            
            PlayerMove.Instance.SetMove(false);
            // ラジオの処理
            await MessageViewEvent.Instance.ViewText(MessageEventName.TakeRadio_1_1);
            // 暗転
            await FadeInOut.FadeIn();
            await SoundManager.Instance.PlaySEAsync(SEType.SetCassette_SE, transform);
            _cassetteTape.SetActive(false);
            await FadeInOut.FadeOut();
            await MessageViewEvent.Instance.ViewText(MessageEventName.TakeRadio_1_2);
            PlayerMove.Instance.SetMove(true);

            MusicRoomManager.Instance.SearchRadioFlag();

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
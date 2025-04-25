using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;

public class PreparationRoomRadioGimmick : MonoBehaviour, OnTriggerEnterInterface, OnTriggerExitInterface
{
    [SerializeField] private Transform _radioTransform;
    [SerializeField] private GameObject _cassetteTape;
    private bool _isTriggered = false;
    private bool _isFuncActive = false;
    private CallOnTrigger _callOnTrigger;

    void Start()
    {
        InputManager.Instance.Decision.Subscribe(async x =>
        {
            if (_isTriggered is false)
            {
                return;
            }

            if (x == 1f)
            {
                if (_isFuncActive) return;
                _isFuncActive = true;
                _callOnTrigger?.ActionComplete();
                _callOnTrigger?.RemoveActionButtonUIViewer();
                
                PlayerMove.Instance.SetMove(false);
                await PlayerMove.Instance.SetPlayerRotation(_radioTransform, 1.5f);
                // ラジオの処理
                await MessageViewEvent.Instance.ViewText(MessageEventName.TakeRadio_2_1);
                // 暗転
                await FadeInOut.FadeIn();
                await SoundManager.Instance.PlaySEAsync(SEType.SetCassette_SE, transform);
                _cassetteTape.SetActive(false);
                await FadeInOut.FadeOut();
                await MessageViewEvent.Instance.ViewText(MessageEventName.TakeRadio_2_2);
                PlayerMove.Instance.SetMove(true);
                PreparationRoomManager.Instance.RadioCheckFlag().Forget();
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
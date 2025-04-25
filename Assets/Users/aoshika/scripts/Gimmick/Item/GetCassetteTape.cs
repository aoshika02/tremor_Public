using UnityEngine;
using UniRx;
using System.Threading.Tasks;

public class GetCassetteTape : MonoBehaviour, OnTriggerEnterInterface, OnTriggerExitInterface
{
    private bool _isTriggered = false;
    private CallOnTrigger _callOnTrigger;
    [SerializeField] GameObject _tapeObj;
    private bool _isCalled = false;
    private void Start()
    {
        InputManager.Instance.Decision.Subscribe(async x =>
        {
            if (_isTriggered is false) { return; }
            if (x == 1f)
            {
                VibrationQuiz.Instance.GetTapeFlag();
                _callOnTrigger.ActionComplete();
                _callOnTrigger.RemoveActionButtonUIViewer();
                await GetTapeEventAsync();
            }
            if (x == 0.0f) return;
        }).AddTo(this);
    }
    private async Task GetTapeEventAsync()
    {
        if (_isCalled) return;
        _isCalled = true;
        PlayerMove.Instance.SetMove(false);
        //カセットテープ取得演出
        await GetItemEvent.Instance.ViewItem(ItemEventName.PreparationRoom_Cassette);
        PlayerMove.Instance.SetMove(true);
        _tapeObj.SetActive(false);
        PreparationRoomManager.Instance.TapeCheckFlag();
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

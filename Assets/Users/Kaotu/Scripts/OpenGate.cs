using Cysharp.Threading.Tasks;
using UnityEngine;
using UniRx;
using UnityEngine.Playables;
using DG.Tweening;

public class OpenGate : MonoBehaviour, OnTriggerEnterInterface, OnTriggerExitInterface
{
    [SerializeField]
    private PlayableDirector _opengate;
    [SerializeField]
    private Transform _transform;
    [SerializeField]
    private Transform _rotation;
    private CallOnTrigger _callOnTrigger;
    private bool _isColliderEnter = false;

    private bool _isViewText = false;
    void Start()
    {
        InputManager.Instance.Decision.Subscribe(GateTriggerAction).AddTo(this);
    }
     private async UniTask GateAnim()
    {
        await FadeInOut.FadeIn(1f);
        await Player.Instance.transform.DOMove(_transform.position,0f);
        PlayerMove.Instance.SetMove(false);
        await PlayerMove.Instance.SetPlayerRotation(_rotation, 0f);
        await FadeInOut.FadeOut(1f);
        await _opengate.PlayAsync();
        PlayerMove.Instance.SetMove(true);
    }

    public void CallOnTriggerEnter(Collider collider, CallOnTrigger _coCallOnTrigger)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            _isColliderEnter = true;
            _callOnTrigger = _coCallOnTrigger;
        }
    }

    public void CallOnTriggerExit(Collider collider, CallOnTrigger _)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            _isColliderEnter = false;
        }
    }

    public async void GateTriggerAction(float x)
    {
        if(x != 1.0f){return;}
        if(_isColliderEnter is false) { return;}
        
        // チュートリアル後にドアを確認するフラグを立てる
        if(TutorialFlowManager.Instance.IsCheckMiddleDoor is false)
        {
            TutorialFlowManager.Instance.CheckMiddleDoorFlag();
            return;
        }
        // ドアを開ける
        
        if(InGameFlow.Instance.IsGetSaw is true)
        {
            _callOnTrigger?.ActionComplete();
            _callOnTrigger?.RemoveActionButtonUIViewer();
            InGameFlow.Instance.GateOpenFlag();
            return;
        }
        
        
        // ドアが開かないときのテキスト
        if(TutorialFlowManager.Instance.IsMovie is false &&
           InGameFlow.Instance.IsMovie is false)
        {
            if(_isViewText) {return;}

            PlayerMove.Instance.SetMove(false);
            _isViewText = true;
            await MessageViewEvent.Instance.ViewText(MessageEventName.LockedGate);
            _isViewText = false;
            PlayerMove.Instance.SetMove(true);
            return;
        }
    }
}

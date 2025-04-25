using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;


public class PeepHoleGimmick : MonoBehaviour, OnTriggerEnterInterface, OnTriggerExitInterface
{
    [SerializeField]private Camera _animationCamera;
    [SerializeField]private PlayableDirector _peepHoleTimeline;
    [SerializeField] private Transform _holeTransform;
    [SerializeField] private AudioListener _animationCameraAL;
    [SerializeField] CancelToken _pauseCancelToken = new CancelToken();
    
    [SerializeField]private CallOnTrigger _callOnTrigger;
    private bool _isTriggered = false;
    private bool _called = false;
    private bool _pause = false;
    private double _pausetime;
    [SerializeField] private double _pausedTime;
    void Start()
    {
        
        _animationCamera.enabled = false;
        _animationCameraAL.enabled = false;
        _called = false;    
        Pause(_pauseCancelToken.GetToken()).Forget();

        InputManager.Instance.Decision.Subscribe(async x =>
        {
            if (!TutorialFlowManager.Instance.IsTutorialFinish) return;
            if (_isTriggered is false) return;
            if (x == 1)
            {
                _callOnTrigger.ActionComplete();
                _callOnTrigger.RemoveActionButtonUIViewer();
                await PlayAnimation();
            }
        }).AddTo(this);
    }
    private async UniTask PlayAnimation() 
    {
        if (_called) return;
        _called = true;
        
        PlayerMove.Instance.SetMove(false);
        await PlayerMove.Instance.SetPlayerRotation(_holeTransform,2);
        //TODO:壁の穴を調査時にスムーズに覗くアニメーションをしたい
        //オーディオリスナーの有効切り替え
        await FadeInOut.FadeIn();
        Player.Instance.Listener.enabled = false;
        _animationCameraAL.enabled = true;
        //カメラの有効切り替え
        _animationCamera.enabled = true;
        Player.Instance.Camera.enabled = false;
        //プレイヤーのライト切り替え
        Player.Instance.LightSetActive(false);
        //タイムライン再生       
        _pauseCancelToken.Cancel();
        await UniTask.Delay(TimeSpan.FromSeconds(0.5));
        await FadeInOut.FadeOut();
        await _peepHoleTimeline.PlayAsync();


        //オーディオリスナーの有効切り替え
        _animationCameraAL.enabled = false;
        Player.Instance.Listener.enabled = true;
        //カメラの有効切り替え   
        Player.Instance.Camera.enabled = true;
        _animationCamera.enabled = false;
        //プレイヤーのライト切り替え
        Player.Instance.LightSetActive(true);

        PreparationRoomManager.Instance.HoleCheckFlag();
        await MessageViewEvent.Instance.ViewText(MessageEventName.LookedPeepHole);
        SonarConverter.Instance.HideSonar(SonarType.Hole);
        PlayerMove.Instance.SetMove(true);
    }
    public void CallOnTriggerEnter(Collider other, CallOnTrigger callOnTrigger)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _isTriggered = true;
            _callOnTrigger = callOnTrigger;
        }
    }
    public void CallOnTriggerExit(Collider other, CallOnTrigger _)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _isTriggered = false;
        }
    }

    private async UniTask Pause(CancellationToken token)
    {
        _peepHoleTimeline.Play();
        while (true)
        {
            _peepHoleTimeline.time = _pausetime;
            await UniTask.Yield(cancellationToken:token);
        }
    }
    public void SetIsActionBlock(bool isActionBlock)
    {
        _callOnTrigger.isActionBlock = isActionBlock;
    }
}
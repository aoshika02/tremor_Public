using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;

public class BoxGimmick : MonoBehaviour, OnTriggerEnterInterface, OnTriggerExitInterface
{
    [SerializeField] private Transform _boxTransform;
    [SerializeField] private MessageEventName _messageEventName;
    [SerializeField] private SEType _sEType;
    [SerializeField] private Animator _boxAnim;
    [SerializeField] private Animator _allFloating;
    [SerializeField] private GameObject _boxlid;
    [SerializeField] private GameObject _item;
    private bool _isArea = false;
    private bool _isFuncActive = false;
    private bool _isOpen = false;
    ChoicesGimmickManager _choiceGimmicManager;
    PlayerMove _playerMove;
    CallOnTrigger _callOnTrigger;
    private static readonly int IsOpen = Animator.StringToHash("isOpen");
    private static readonly int Isfloating = Animator.StringToHash("isfloating");

    private void Start()
    {
        _isFuncActive = false;
        _choiceGimmicManager = ChoicesGimmickManager.Instance;
        _playerMove = PlayerMove.Instance;
        InputManager.Instance.Decision.Subscribe(x =>
        {
            if (x != 1) return;
            if (_isArea is false) return;
            if (VibrationQuiz.Instance.IsCheckRadio is false) return;
            if (_isFuncActive)
            {
                _choiceGimmicManager.ContinueFlag();
            }
            else
            {
                BoxOpen();
            }
        }).AddTo(this);
        InputManager.Instance.Cancel.Subscribe(x =>
        {
            if (x != 1) return;
            _choiceGimmicManager.CancelFlag();
        }).AddTo(this);
    }
    /// <summary>
    /// 箱開閉処理
    /// </summary>
    private async void BoxOpen()
    {
        if (_isArea is false) return;
        if (_isOpen) return;
        if (VibrationQuiz.Instance.IsQuizFlow is false)
        {
            _callOnTrigger.RemoveActionButtonUIViewer();
            _callOnTrigger.ActionComplete();
            return;
        }
        _isFuncActive = true;
        //プレイヤーを動けなくする
        _playerMove.SetMove(false);
        //箱をあけるときの確認テキスト
        await SoundManager.Instance.PlaySEAsync(_sEType, transform,volume:0);
        await MessageViewEvent.Instance.ViewText(MessageEventName.OpenBox);
        await MessageViewEvent.Instance.ChoicesView(1, 0.5f, ChoiceType.All,SelectElementType.OpenBox);
        if (await _choiceGimmicManager.ContinueOrCancel() is false)
        {
            _playerMove.SetMove(true);
            await MessageViewEvent.Instance.ChoicesView(0, 0.5f, ChoiceType.No, SelectElementType.OpenBox);
            _isFuncActive = false;
            return;
        }
        await MessageViewEvent.Instance.ChoicesView(0, 0.5f, ChoiceType.Yes, SelectElementType.OpenBox);
        _callOnTrigger.RemoveActionButtonUIViewer();
        _callOnTrigger.ActionComplete();
        _callOnTrigger.isUiView = false;
        //箱のほうを向く
        await _playerMove.SetPlayerRotation(_boxTransform, 1);

        TabletObjViewManager.Instance.TabletObjActive();
        SoundManager.Instance.PlaySE(SEType.Get_Heart_SE);
        await UniTask.WaitForSeconds(1);
        //開閉アニメーション
        _boxAnim.SetTrigger(IsOpen);  
        SoundManager.Instance.PlaySE(SEType.Open_box);
        //await UniTask.WaitForSeconds(1);
        // 開閉アニメーション待機時間でアイテムの方を向いておく
        await _playerMove.SetPlayerRotation(_item.transform,1);
        _allFloating.SetTrigger(Isfloating);
        
        // 時間ないので無理やり実装
        float time = 0;
        while (time <= /*アニメーションの長さ*/1)
        {
            await _playerMove.SetPlayerRotation(_item.transform,0);
            time += Time.deltaTime;
        }
        
        await MessageViewEvent.Instance.ViewText(_messageEventName);
        await UniTask.WaitForSeconds(1);
        TabletObjViewManager.Instance.TabletObjDisable();
        SoundManager.Instance.PlaySE(SEType.Get_Heart_SE);
        _boxlid.SetActive(false);
        _item.SetActive(false);

        //_messageEventNameがQuizClearなら成功
        if (_messageEventName == MessageEventName.QuizClear)
        {
            VibrationQuiz.Instance.SuccessFlag();
        }
        else if ((_messageEventName == MessageEventName.QuizHeart) ||
            (_messageEventName == MessageEventName.QuizSkull))
        {
            VibrationQuiz.Instance.FailedFlag();
        }
        else 
        {
            UnityEngine.Debug.LogError(_messageEventName + " は想定していないMessageEventNameです");
        }
        _isOpen = true;
        _isFuncActive = false;
    }
    #region Trigger内部にいるかどうかのフラグ
    public void CallOnTriggerEnter(Collider collider, CallOnTrigger callOnTrigger)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            _callOnTrigger = callOnTrigger;
            _isArea = true;
        }
    }
    public void CallOnTriggerExit(Collider collider, CallOnTrigger callOnTrigger)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            _callOnTrigger = callOnTrigger;
            _isArea = false;
        }
    }
    #endregion
}

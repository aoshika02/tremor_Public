using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;

public class GetQuizItems : MonoBehaviour, OnTriggerEnterInterface, OnTriggerExitInterface
{
    private bool _isTriggered;
    private bool _isFuncActive = false;
    private bool _isChoicesActive = false;
    //このRootのCallOnTrigger
    private CallOnTrigger _callOnTrigger;
    private void Start()
    {
        InputManager.Instance.Decision.Subscribe(async x =>
        {
            if (_isTriggered is false) { return; }

            //イベント中ならリターン
            if (VibrationQuiz.Instance.IsGetItemEvent) return;
            if (VibrationQuiz.Instance.IsQuizFinish is true) return;
            if (x == 1f)
            {
                //ラジオとメモを取得してるなら
                if (VibrationQuiz.Instance.IsQuizTutorialFinish)
                {
                    if (_isChoicesActive) 
                    {
                        ChoicesGimmickManager.Instance.ContinueFlag();
                    }
                    else 
                    {
                        if (_isFuncActive) return;
                        _isFuncActive = true;
                        await ReGetQuizItem();
                        _isFuncActive = false;
                        return;
                    }
                }
                //メモを確認してないならメモのフラグを起こす
                if (VibrationQuiz.Instance.IsCheckMemo is false)
                {
                    VibrationQuiz.Instance.CheckMemoFlag();
                    return;
                }
                //カセットテープを取得していないならテキスト表示してリターン
                if (VibrationQuiz.Instance.IsGetTape == false)
                {
                    PlayerMove.Instance.SetMove(false);
                    await MessageViewEvent.Instance.ViewText(MessageEventName.NotGetCassette);
                    PlayerMove.Instance.SetMove(true);
                    return;
                }
                //ラジオ取得フラグ
                if (VibrationQuiz.Instance.IsCheckRadio is false)
                {
                    GetItemIconViewManager.Instance.RemoveItemView(ItemEventName.PreparationRoom_Cassette);
                    VibrationQuiz.Instance.CheckRadioFlag();
                    return;
                }
            }
            if (x == 0.0f) return;
        }).AddTo(this);
        InputManager.Instance.Cancel.Subscribe(x =>
        {
            if (x != 1) return;
            if (_isChoicesActive)
            {
                ChoicesGimmickManager.Instance.CancelFlag();
            }
        }).AddTo(this);
    }
    private async UniTask ReGetQuizItem() 
    {
        PlayerMove.Instance.SetMove(false);
        await VibrationQuiz.Instance.LookAtMemo();
        await MessageViewEvent.Instance.ChoicesView(1, 0.5f, ChoiceType.All, SelectElementType.QuizItems);
        _isChoicesActive = true;
        if (await ChoicesGimmickManager.Instance.ContinueOrCancel() is true)
        {
            //Memoをもう一度
            await MessageViewEvent.Instance.ChoicesView(0, 0.5f, ChoiceType.Yes, SelectElementType.QuizItems);
            await VibrationQuiz.Instance.LookAtMemo();
            //メモ取得イベント
            await GetItemEvent.Instance.ViewItem(ItemEventName.Quiz_Memo, false);
            _isChoicesActive = false;
            PlayerMove.Instance.SetMove(true);
            await UniTask.WaitForSeconds(1);
            return;
        }
        //ラジオをもう一度
        await MessageViewEvent.Instance.ChoicesView(0, 0.5f, ChoiceType.No, SelectElementType.QuizItems);
        await VibrationQuiz.Instance.LookAtRadio();
        //テキスト再生
        await MessageViewEvent.Instance.ViewText(MessageEventName.ReTakeRadio);
        await UniTask.WaitForSeconds(2);
        _isChoicesActive = false;
        PlayerMove.Instance.SetMove(true);
        await UniTask.WaitForSeconds(1);
    }

    public void CallOnTriggerEnter(Collider other, CallOnTrigger callOnTrigger)
    {
        if (_callOnTrigger == null)
        {
            _callOnTrigger = callOnTrigger;
        }
        _isTriggered = true;
        if (VibrationQuiz.Instance.IsQuizFinish is true)
        {
            _callOnTrigger.RemoveActionButtonUIViewer();
            _callOnTrigger.ActionComplete();
            return;
        }
    }

    public void CallOnTriggerExit(Collider other, CallOnTrigger callOnTrigger)
    {

        if (_callOnTrigger == null)
        {
            _callOnTrigger = callOnTrigger;
        }
        _isTriggered = false;
        if (VibrationQuiz.Instance.IsQuizFinish is true)
        {
            _callOnTrigger.RemoveActionButtonUIViewer();
            _callOnTrigger.ActionComplete();
            return;
        }
    }
}

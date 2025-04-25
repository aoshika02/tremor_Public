using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Playables;

public class VibrationQuiz : SingletonMonoBehaviour<VibrationQuiz>
{
    #region　変数
    //失敗回数
    private int _failedCount = 0;
    //失敗できる回数
    private int _maxFailedCount = 2;
    //TransformのList（ゴーストがいる（と見せかける）場所）
    [SerializeField] private List<GameObject> ghostObjs = new List<GameObject>();
    //タブレットのTransform
    [SerializeField] private Transform _tabletTransform;
    //箱3つのど真ん中のTransform
    [SerializeField] private Transform _boxiesTransform;
    //メモのTransform
    [SerializeField] private Transform _memoTransform;
    //ラジオのTransform
    [SerializeField] private Transform _radioTransform;

    [SerializeField] private Transform _deadTransform;

    [SerializeField] private PlayableDirector _opendead;

    [SerializeField] private PlayableDirector _killdemon;

    [SerializeField] private GameObject cam1;

    [SerializeField] private GameObject cam2;
    //アイテムイベント中フラグ
    public bool IsGetItemEvent { get; private set; } = false;
    //失敗フラグ
    private bool _isFailed = false;
    //成功フラグ
    private bool _isSuccess = false;
    //カセットテープ取得フラグ
    public bool IsGetTape { get; private set; } = false;
    //メモ確認フラグ
    public bool IsCheckMemo { get; private set; } = false;
    //ラジオ確認フラグ
    public bool IsCheckRadio { get; private set; } = false;
    //クイズ実行中フラグ
    public bool IsQuizFlow { get; private set; } = false;
    //クイズ実行中フラグ
    public bool IsQuizTutorialFinish { get; private set; } = false;
    public bool IsQuizFinish { get; private set; } = false;
    //プレイヤーのカメラ
    [SerializeField] private Camera _camera;
    PlayerMove _playerMove;
    CancelToken _cancelToken = new CancelToken();
    SonarObject _sonarObject;
    [SerializeField] private OpenQuizRoomDoor[] _openQuizRoomDoor;
    [SerializeField] private Animator[] _doorAnimators;
    [SerializeField] private List<CallOnTrigger> _callOnTriggers = new List<CallOnTrigger>();
    [SerializeField] private CallOnTrigger _itemCallOnTrigger;
    #endregion
    private void Start()
    {
        _itemCallOnTrigger.isUiView = true;
        //ItemButtonView(false);
        BoxButtonUIView(false);
        _playerMove = PlayerMove.Instance;
        _sonarObject = SonarObjectPool.Instance.GetSonarObject();
        IsQuizFlow = false;
    }
    /// <summary>
    /// フロー開始関数（他オブジェクトのトリガーで呼び出し）
    /// </summary>
    public void StartVibrationQuizFlow(CancellationToken cancellationToken)
    {
        CancelToken flowCancelToken = new CancelToken();
        FailedFunc(cancellationToken: cancellationToken, flowCancelToken).Forget();
        CancellationTokenSource tokenSource = CancellationTokenSource.CreateLinkedTokenSource(new[] { cancellationToken, flowCancelToken.GetToken() });
        // 渡す
        VibrationQuizFlow(cancellationToken: tokenSource.Token).Forget();
    }
    /// <summary>
    /// クイズフロー
    /// </summary>
    /// <returns></returns>
    private async UniTask VibrationQuizFlow(CancellationToken cancellationToken)
    {
       
        _playerMove.SetMove(false);
        //箱のほうを見る
        await _playerMove.SetPlayerRotation(_boxiesTransform, 1);
        //部屋に入ったときのテキスト
        await MessageViewEvent.Instance.ViewText(MessageEventName.QuizEnterRoom);
        _playerMove.SetMove(true);

        IsCheckMemo = false;
        // メモを調べるまで待機
        await UniTask.WaitUntil(() => IsCheckMemo, cancellationToken: cancellationToken);

        //イベント中
        IsGetItemEvent = true;
        _playerMove.SetMove(false);
        //メモの方を向く
        await _playerMove.SetPlayerRotation(_memoTransform, 1);
        //メモ取得イベント
        await GetItemEvent.Instance.ViewItem(ItemEventName.Quiz_Memo,false);
        //イベント終了
        IsGetItemEvent = false;

        IsCheckRadio = false;
        // ラジオを調べるまで待機
        await UniTask.WaitUntil(() => IsCheckRadio, cancellationToken: cancellationToken);
        //クイズ開始
        IsQuizFlow = true;
        //イベント中
        IsGetItemEvent = true;
        _playerMove.SetMove(false);
        //ドアを閉める
        CloseDoor().Forget();
        //ラジオの方を向く
        await _playerMove.SetPlayerRotation(_radioTransform, 1);
        //ラジオ演出
        await MessageViewEvent.Instance.ViewText(MessageEventName.TakeRadio_3_1);
        _playerMove.SetMove(false);
        await MovieFrame.Instance.FrameIn();
        // ソナー演出処理呼び出し
        await VibrationQuizGuide(cancellationToken: cancellationToken, _sonarObject);
        await MessageViewEvent.Instance.ViewText(MessageEventName.GhostShow);
        _playerMove.SetMove(false);
        //箱(3つ)のほうを見る
        await _playerMove.SetPlayerRotation(_boxiesTransform, 2);
        //クイズ開始
        await MessageViewEvent.Instance.ViewText(MessageEventName.QuizStart);
        BoxButtonUIView(true);
        _playerMove.SetMove(true);
        await UniTask.WaitForSeconds(1);
        IsQuizTutorialFinish = true;
        //イベント終了
        IsGetItemEvent = false;

        // 成功するまで待機
        await UniTask.WaitUntil(() => _isSuccess, cancellationToken: cancellationToken);
        //ソナー破棄
        SonarObjectPool.Instance.ReleaseSonarObject(_sonarObject);
        for (int i = 0; i < _openQuizRoomDoor.Length; i++)
        {
            // 開かない表示解除
            _openQuizRoomDoor[i].SetIsActionBlock(false);
        }
        //のこぎりもらえる
        await GetItemEvent.Instance.ViewItem(ItemEventName.Saw_Blade);
        InGameFlow.Instance.GetSawBladeFlag();
        BoxButtonUIView(false);

        //クイズ終了
        IsQuizFlow = false;
        IsQuizFinish = true;
        //クイズの部屋のソナー破棄
        SonarConverter.Instance.HideSonar(SonarType.ClassRoom3);
    }
    /// <summary>
    /// 失敗処理
    /// </summary>
    /// <returns></returns>
    private async UniTask FailedFunc(CancellationToken cancellationToken, CancelToken flowCancelToken)
    {
        //ラジオを確認するまで待機
        await UniTask.WaitUntil(() => IsCheckRadio, cancellationToken: cancellationToken);
        /// 間違えるたびループ
        while (_failedCount < _maxFailedCount)
        {
            //フラグをfalseにする
            _isFailed = false;
            //失敗するまで待機
            await UniTask.WaitUntil(() => _isFailed, cancellationToken: cancellationToken);
            //失敗回数を加算
            _failedCount++;
            //ソナー演出処理呼び出し
            await VibrationQuizGuide(cancellationToken: cancellationToken, _sonarObject);
            if (_failedCount == 1)
            {
                await MessageViewEvent.Instance.ViewText(MessageEventName.QuizMiss1);
                _playerMove.SetMove(true);
            }
        }
        flowCancelToken.Cancel();
        await MessageViewEvent.Instance.ViewText(MessageEventName.QuizMiss2);
        //ゴーストに襲われる
        await _playerMove.SetPlayerRotation(_deadTransform, 1f);
        await _opendead.PlayAsync();
        await FadeInOut.FadeIn(0f);
        cam1.SetActive(false);
        cam2.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(2f));
        FadeInOut.FadeOut(2.0f).Forget();
        await _killdemon.PlayAsync();
        await FadeInOut.FadeIn();
        await SceneLoadManager.Instance.AddLoadScene(SceneType.GameOver);
        SceneLoadManager.Instance.LoadedSceneActive(SceneType.GameOver, isOutFade: true).Forget();
        _cancelToken.Cancel();
    }
    /// <summary>
    /// ソナー演出処理
    /// </summary>
    /// <returns></returns>
    private async UniTask VibrationQuizGuide(CancellationToken cancellationToken, SonarObject sonarObject)
    {
        //Playerを動けなくする
        _playerMove.SetMove(false);
        //　カメラの回転角を取得
        float lastRotionX = _camera.transform.rotation.eulerAngles.x;
        //タブレットを見ているかどうかの判断
        bool isRotationOver = 40 <= lastRotionX && lastRotionX <= 46;
        //TODO:タブレットの起動音的な（）
        if (!isRotationOver)
        {
            // タブレットの方に視線固定
            await _playerMove.SetPlayerRotation(_tabletTransform, 2);
        }
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }
        await UniTask.WaitForSeconds(1f);
        // ソナーのアルファを0にする
        sonarObject.Deactivate();
        await UniTask.WaitForSeconds(1f);
        //ゴーストの座標を取得してソナーの場所を変更
        sonarObject.Initialize(Color.red,
            //失敗カウント数に応じて参照Transfromを変更
            ghostObjs[_failedCount % ghostObjs.Count].transform.position);

        //TODO：ゴースト声を鳴らす
        await UniTask.WaitForSeconds(1.5f);
        if (!isRotationOver)
        {
            //角度-45～0を考慮
            lastRotionX = (lastRotionX > 46) ? lastRotionX - 360 : lastRotionX;
            //カメラを元のアングルに戻る
            await _playerMove.SetCameraRotation(lastRotionX, 2);
        }
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }
        await MovieFrame.Instance.FrameOut();
    }
    /// <summary>
    /// 箱の調べるボタン表示の切り替え
    /// </summary>
    /// <param name="isView"></param>
    private void BoxButtonUIView(bool isView)
    {
        foreach (CallOnTrigger cot in _callOnTriggers)
        {
            if (isView is false)
            {
                cot.RemoveActionButtonUIViewer();
            }
            cot.isUiView = isView;
        }
    }
    /// <summary>
    /// アイテムの調べるボタン表示の切り替え
    /// </summary>
    /// <param name="isView"></param>
    private void ItemButtonView(bool isView)
    {
        if (isView is false)
        {
            _itemCallOnTrigger.RemoveActionButtonUIViewer();
            _itemCallOnTrigger.isUiView = false;
            return;
        }
        _itemCallOnTrigger.AddActionButtonUIViewer();
        _itemCallOnTrigger.isUiView = true;
    }
    private async UniTask CloseDoor() 
    {
        foreach (var doorAnimator in _doorAnimators)
        {
            doorAnimator.ResetTrigger("isOpen");
        }
        await UniTask.Yield();

        for (int i = 0; i < _openQuizRoomDoor.Length; i++)
        {
            // フロー開始時に扉が閉まる
            _doorAnimators[i].SetTrigger("isClose");
            // 開かない表示
            _openQuizRoomDoor[i].SetIsActionBlock(true);
            _openQuizRoomDoor[i].DoorUIView();
        }
        await UniTask.Yield();
        foreach (var doorAnimator in _doorAnimators)
        {
            doorAnimator.ResetTrigger("isClose");
        }
    }
    public async UniTask LookAtMemo() 
    {
        await _playerMove.SetPlayerRotation(_memoTransform, 1);
    }
    public async UniTask LookAtRadio()
    {
        await _playerMove.SetPlayerRotation(_radioTransform, 1);
    }
    #region 外部公開:フラグ用関数
    //失敗フラグ
    public void FailedFlag()
    {
        _isFailed = true;
    }
    //成功フラグ
    public void SuccessFlag()
    {
        _isSuccess = true;
    }
    //メモ確認フラグ
    public void CheckMemoFlag()
    {
        IsCheckMemo = true;
    }
    //ラジオ確認フラグ
    public void CheckRadioFlag()
    {
        IsCheckRadio = true;
    }
    public void GetTapeFlag()
    {
        IsGetTape = true;
    }
    #endregion
}

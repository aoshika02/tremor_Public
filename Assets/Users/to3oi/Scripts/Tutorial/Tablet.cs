using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class Tablet : MonoBehaviour
{
    /*[SerializeField] private List<TabletCameraData> _tabletCameraDatas;
    [SerializeField] private RawImage _output;
    [SerializeField] private float _lookGhostTime = 5;
    [SerializeField] private AreaTablet _areaTablet;
    [SerializeField] private ObjectTablet _objectTablet;
    [SerializeField] private GameObject _canvasGameObject;

    private int _index = 0;
    private bool _isTabletLooking = false;
    private CancelToken _lookGhostCancelToken = new CancelToken();
    private bool _cleared = false;

    private void Start()
    {
        //タブレットで見えているカメラを初期化
        UpdateCamera();
        //タブレットを非表示に
        _canvasGameObject.SetActive(false);
    }

    public void TabletEventStart(CancelToken eventCancelToken)
    {
        //入力を取得
        InputManager.Instance.LeftCrossKeyRight.Subscribe(x =>
        {
            if (!_isTabletLooking)
            {
                return;
            }

            if (x == 0.0f) return;
            _index++;

            UpdateCamera();
        }).AddTo(cancellationToken:eventCancelToken.GetToken());

        InputManager.Instance.LeftCrossKeyLeft.Subscribe(x =>
        {
            if (!_isTabletLooking)
            {
                return;
            }

            if (x == 0.0f) return;
            _index--;

            UpdateCamera();
        }).AddTo(cancellationToken:eventCancelToken.GetToken());

        InputManager.Instance.Cancel.Subscribe(x =>
        {
            if (!_isTabletLooking)
            {
                return;
            }

            if (x == 0.0f) return;
            UnLookingTablet();
        }).AddTo(cancellationToken:eventCancelToken.GetToken());
    }

    private void UpdateCamera()
    {
        _lookGhostCancelToken.Cancel();
        var index = GetIndex();
        _output.texture = _tabletCameraDatas[index].RenderTexture;

        if (!_tabletCameraDatas[index].isGhostCamera) return;

        _lookGhostCancelToken.ReCreate();
        TabletBreakWaiter(_lookGhostCancelToken.GetToken()).Forget();
    }

    private async UniTask TabletBreakWaiter(CancellationToken ctx)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_lookGhostTime), cancellationToken: ctx);

        _cleared = true;
        //TODO:Canvasの画面が割れたような演出を追加する
        //await 
        //タブレットの画面を割る
        _objectTablet.BreakTablet();
        //タブレットの画面を閉じる
        UnLookingTablet();
        //エリアイベントの処理を停止する
        _areaTablet.ClearEvent();
        //チュートリアルを進める
        TutorialFlow.Instance.LookedGhost();
    }

    private int GetIndex() => Mathf.Abs(_index) % _tabletCameraDatas.Count;

    /// <summary>
    /// タブレットを見ている状態を呼び出す
    /// </summary>
    public void LookingTablet()
    {
        _isTabletLooking = true;
        //タブレットで最初に見える画面を初期化
        //正と負の境目で表示順が意図しないものになるので一旦indexをずらして対応
        _index = _tabletCameraDatas.Count * 10000;
        UpdateCamera();
        //タブレットのハイライトを非表示
        _objectTablet.OutlineHide();
        //タブレットを表示
        _canvasGameObject.SetActive(true);
        //Playerの移動を停止
        PlayerMove.Instance.SetMove(false);
    }

    /// <summary>
    /// タブレットを見ている状態を解除する
    /// </summary>
    private void UnLookingTablet()
    {
        _isTabletLooking = false;
        //タブレット画面を消す
        _canvasGameObject.SetActive(false);
        //タブレットのハイライトを表示
        if (!_cleared)
        {
            _objectTablet.OutlineView();
        }
        //プレイヤーの移動を有効にする
        PlayerMove.Instance.SetMove(true);
        //タブレット画面に移行する直前の座標に移動させる
        _areaTablet.ExitEvent();
    }*/
}

[Serializable]
public class TabletCameraData
{
    public bool isGhostCamera = false;
    public Camera Camera;
    public RenderTexture RenderTexture;
}
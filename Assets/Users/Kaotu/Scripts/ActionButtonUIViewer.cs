using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ActionButtonUIViewer : MonoBehaviour
{
    // UIを表示させる対象オブジェクト
    private Transform _targetPoint;
    private RectTransform _targetUI;
    private RectTransform _parentUI;

    [SerializeField] private CanvasGroup _actionUICanvasGroup;
    [SerializeField] private TextMeshProUGUI _searchText;
    [SerializeField] private GameObject _cancelObj;
    public bool IsView = false;
    private bool _isVisible;
    private CancelToken _cancelToken = new CancelToken();

    private void Awake()
    {
        _actionUICanvasGroup.DOFade(0, 0);
        _searchText.text = LocalizeText.GetText("UI_Text_Search");
        _cancelObj.SetActive(false);
    }

    public void Init(RectTransform parentRTF)
    {
        _parentUI = parentRTF;
        _targetUI = GetComponent<RectTransform>();
    }

    public async UniTask SetData(Transform target, bool isActionBlock = false)
    {
        IsView = true;
        _targetPoint = target;
        _cancelObj.SetActive(isActionBlock);
        _cancelToken.ReCreate();

        _isVisible = IsInsideCamera();
        if (_isVisible)
        {
            await _actionUICanvasGroup.DOFade(1, 0.25f).ToUniTask(cancellationToken: _cancelToken.GetToken());
        }
    }


    private void Update()
    {
        if (IsView)
        {
            OnUpdatePosition();

            if (IsInsideCamera())
            {
                if (_isVisible is false)
                {
                    _isVisible = true;
                    _cancelToken.ReCreate();
                    _actionUICanvasGroup.DOFade(1, 0.25f).ToUniTask(cancellationToken: _cancelToken.GetToken()).Forget();
                }
            }
            else
            {
                if (_isVisible is true)
                {
                    _isVisible = false;
                    _cancelToken.ReCreate();
                    _actionUICanvasGroup.DOFade(0, 0.25f).ToUniTask(cancellationToken: _cancelToken.GetToken()).Forget();
                }
            }
        }
        else
        {
            _actionUICanvasGroup.alpha = 0;
        }
    }

    private bool IsInsideCamera()
    {
        Vector3 targetToCameraDirection_N = (Camera.main.transform.position - _targetPoint.position).normalized;

        // 正規化したベクトルの内積が一定以下なら見たことにする
        return Vector3.Dot(targetToCameraDirection_N, Camera.main.transform.forward.normalized) < -0.75f;
    }

    public void OnUpdatePosition()
    {
        var targetScreenPos = Camera.main.WorldToScreenPoint(_targetPoint.position);
        // スクリーン座標変換→UIローカル座標変換
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _parentUI,
            targetScreenPos,
            null,
            out var uiLocalPos
        );
        _targetUI.anchoredPosition = uiLocalPos;
    }

    public async UniTask Release()
    {
        _cancelToken.ReCreate();
        await _actionUICanvasGroup.DOFade(0, 0.25f).ToUniTask(cancellationToken: _cancelToken.GetToken());
        IsView = false;
    }
}
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MovieFrame : SingletonMonoBehaviour<MovieFrame>
{
    [SerializeField] private RawImage _upImage;
    [SerializeField] private RawImage _downImage;

    private bool _isAnimation = false;
    private bool _isFrameIn = false;

    private readonly float _frameDefaultPosX = 1920f;
    private readonly float _frameInPosX = 0f;
    private readonly float _frameOutPosX = -1920f;

    protected override void Awake()
    {
        if (CheckInstance())
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public async UniTask FrameIn(float duration = 1f)
    {
        if (_isFrameIn)
        {
            return;
        }

        if (_isAnimation)
        {
            return;
        }

        _isFrameIn = true;
        _isAnimation = true;

        // 座標の初期化
        var upImageRectAnchorPosition = _upImage.rectTransform.anchoredPosition;
        upImageRectAnchorPosition.x = _frameDefaultPosX;
        _upImage.rectTransform.anchoredPosition = upImageRectAnchorPosition;

        var downImageRectAnchorPosition = _downImage.rectTransform.anchoredPosition;
        downImageRectAnchorPosition.x = _frameDefaultPosX * -1;
        _downImage.rectTransform.anchoredPosition = downImageRectAnchorPosition;

        _upImage.gameObject.SetActive(true);
        _downImage.gameObject.SetActive(true);
        
        // アニメーション
        UniTask[] task = new UniTask[2];
        
        task[0] = _upImage.rectTransform.DOAnchorPosX(_frameInPosX, duration).SetEase(Ease.InQuint).ToUniTask();
        task[1] = _downImage.rectTransform.DOAnchorPosX(_frameInPosX * -1, duration).SetEase(Ease.InQuint).ToUniTask();
        await UniTask.WhenAll(task);
        
        _isAnimation = false;
    }

    public async UniTask FrameOut(float duration = 1f)
    {
        if (!_isFrameIn)
        {
            return;
        }

        if (_isAnimation)
        {
            return;
        }

        _isAnimation = true;

        // アニメーション
        UniTask[] task = new UniTask[2];
        task[0] = _upImage.rectTransform.DOAnchorPosX(_frameOutPosX, duration).SetEase(Ease.OutQuint).ToUniTask();
        task[1] = _downImage.rectTransform.DOAnchorPosX(_frameOutPosX * -1, duration).SetEase(Ease.OutQuint).ToUniTask();
        await UniTask.WhenAll(task);

        _upImage.gameObject.SetActive(false);
        _downImage.gameObject.SetActive(false);

        _isFrameIn = false;
        _isAnimation = false;
    }
}
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GetItemIconView : MonoBehaviour
{
    [SerializeField] private RawImage _rawImage;

    public async UniTask SetUp(Texture2D texture, float startYValue, float endYValue, float duration = 0.25f,
        CancellationToken cancellationToken = default)
    {
        _rawImage.texture = texture;
        var rectTransform = transform.GetComponent<RectTransform>();
        await rectTransform.DOLocalMoveY(startYValue, 0);
        await _rawImage.DOFade(0, 0);

        Sequence sequence = DOTween.Sequence();
        sequence.Join(_rawImage.DOFade(1f, duration))
            .Join(rectTransform.DOLocalMoveY(endYValue, duration));

        await sequence.ToUniTask(cancellationToken: cancellationToken);
    }

    public async UniTask Move(float endYValue, float duration = 0.25f, CancellationToken cancellationToken = default)
    {
        var rectTransform = transform.GetComponent<RectTransform>();
        await rectTransform.DOLocalMoveY(endYValue, duration).ToUniTask(cancellationToken: cancellationToken);
    }

    public void Release()
    {
        Destroy(gameObject);
    }
}
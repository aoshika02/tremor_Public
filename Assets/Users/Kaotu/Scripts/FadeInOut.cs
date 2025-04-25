using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
[RequireComponent(typeof(Canvas))]
public class FadeInOut : SingletonMonoBehaviour<FadeInOut>
{
    [SerializeField]
    UnityEngine.UI.Image _image;
    static UnityEngine.UI.Image image;

    protected override void Awake()
    {
        if (CheckInstance())
        {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            image = _image;
            image.DOFade(0, 0);
            var canvas = GetComponent<Canvas>();
            canvas.enabled = true;
            canvas.sortingOrder = 1000;
        }
    }
    public static async UniTask FadeOut(float durationtime = 1f)
    {
        await image.DOFade(endValue: 0f, duration: durationtime);
    }

    public static async UniTask FadeIn(float durationtime = 1f)
    {
        await image.DOFade(endValue: 1f, duration: durationtime);
    }
}

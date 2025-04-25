using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SonarObject : MonoBehaviour
{
    [SerializeField] private Color _sonarColor;
    [SerializeField] private CanvasGroup _sonarCanvasGroup;
    //[SerializeField] private GameObject _sonarObj;
    [SerializeField] private Image _sonarImage;
    [SerializeField] private Image _sonarAnimImage;
    //円が広がりきるまでの時間
    [SerializeField] private float _spreadTime = 10;
    private bool _isOnce = false;
    public bool IsUse = false;
    public void Initialize(Color color, Vector3 vector)
    {
        _sonarImage.color = color;
        TabletPoint(_sonarImage, vector);
        _sonarCanvasGroup.alpha = 1;
        if (!_isOnce)
        {
            _isOnce = true;
            _sonarAnimImage.material = Instantiate(_sonarAnimImage.material);
            _sonarAnimImage.material.SetColor("_Color", color);
            PlayerSonarAnim(_sonarAnimImage.material, _spreadTime).Forget();
        }
    }
    private async UniTask PlayerSonarAnim(Material _material,float spreadTime)
    {
        var _sColor = _material.color;
        _material.SetFloat("_circleRadius", 0);
        _material.SetColor("_Color", new Color(_sColor.r, _sColor.g, _sColor.b, 1));

        await DOVirtual.Float(0, 0.5f, spreadTime, f =>
        {
            _material.SetFloat("_circleRadius", f);
            _material.SetColor("_Color", new Color(_sColor.r, _sColor.g, _sColor.b, (1 - f * 2)));
        }).SetLoops(-1).ToUniTask(cancellationToken: destroyCancellationToken);
    }
    private Vector3 TabletPoint(Image pointImage, Vector3 objPoint)
    {
        var rect = pointImage.GetComponent<RectTransform>();
        //rect.anchoredPosition = new Vector3(objPoint.z * -13 + 867, objPoint.x * 13 - 1080, 0);TGSでの式
        rect.anchoredPosition = new Vector3(objPoint.z * 12.5f - 124.75f, objPoint.x * -12.5f + 139.75f, 0);
        return rect.anchoredPosition;
    }
    public void Deactivate() 
    {
        _sonarCanvasGroup.alpha = 0;
       // _sonarObj.SetActive(false);
    }
}

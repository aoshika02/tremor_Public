using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SonarPlayer : SingletonMonoBehaviour<SonarPlayer>
{
    private Material _sonarCurrentMaterial;
    [SerializeField] private Image _sonarCurrentImage;
    private Material _sonarItemMaterial;
    [SerializeField] private Image _sonarItemImage;
    private Material _sonarKeyMaterial;
    [SerializeField] private Image _sonarKeyImage;
    private Material _sonarGoalMaterial;
    [SerializeField] private Image _sonarGoalImage;
    //円が広がりきるまでの時間
    [SerializeField] private float _spreadTime = 2.5f;
    //円が消えるまでの時間
    [SerializeField] private float _fadeTime = 10;
    
    public void StartSonerLoop()
    {
        _sonarCurrentMaterial = _sonarCurrentImage.material;
        _sonarItemMaterial = _sonarItemImage.material;
        _sonarKeyMaterial = _sonarKeyImage.material;
        _sonarGoalMaterial = _sonarGoalImage.material;
        PlayerSonarAnim(_sonarCurrentMaterial).Forget();
        PlayerSonarAnim(_sonarItemMaterial).Forget();
        PlayerSonarAnim(_sonarKeyMaterial).Forget();
        PlayerSonarAnim(_sonarGoalMaterial).Forget();
    }
    public void StartPlayerSonar() 
    {
        _sonarCurrentMaterial = _sonarCurrentImage.material;
        PlayerSonarAnim(_sonarCurrentMaterial).Forget();
    }
    private async UniTask PlayerSonarAnim(Material _material)
    {
        var _sColor = _material.color;
        _material.SetFloat("_circleRadius", 0);
        _material.SetColor("_Color", new Color(_sColor.r, _sColor.g, _sColor.b, 1));

        await DOVirtual.Float(0, 0.5f, _spreadTime, f =>
        {
            _material.SetFloat("_circleRadius", f);
            _material.SetColor("_Color", new Color(_sColor.r, _sColor.g, _sColor.b, (1- f *2)));
        }).SetLoops(-1).ToUniTask(cancellationToken:destroyCancellationToken);
       
    }
}

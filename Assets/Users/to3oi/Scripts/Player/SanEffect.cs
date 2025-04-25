using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Volume))]
public class SanEffect : MonoBehaviour
{
    private Material _grayscaleMaterial;
    private static readonly int Blend = Shader.PropertyToID("_Blend");

    private Volume _volume;
    private Vignette _vignette;
    [SerializeField] private AnimationCurve _volumeInsensityCurve;
    [SerializeField] private AnimationCurve _volumeSmoothnessCurve;

    enum SanState
    {
        MentalLevel0 = 0,
        MentalLevel1 = 1,
        MentalLevel2 = 2,
        MentalLevel3 = 3
    }

    [SerializeField] private float _statusChangeTime = 1f; 
    private SanState _sanState = SanState.MentalLevel0;
    CancelToken _ctHallucination = new CancelToken();
    CancelToken _ctHallucinationBGM = new CancelToken();
    CancelToken _ctStrongHallucination = new CancelToken();
    CancelToken _ctSaturation = new CancelToken();
    CancelToken _ctIntensityWave = new CancelToken();

    void Start()
    {
        _volume = GetComponent<Volume>();
        if (_volume.profile.TryGet<Vignette>(out _vignette))
        {
            _vignette.intensity.value = _volumeInsensityCurve.Evaluate(0);
            _vignette.smoothness.value = _volumeSmoothnessCurve.Evaluate(0);
        }

        _grayscaleMaterial = Grayscale.Instance.GrayscaleMaterial;
        _grayscaleMaterial.SetFloat(Blend, 0f);
        _sanState = SanState.MentalLevel0;
        PlayerStatus.Instance.SanObservable.Subscribe(async x =>
        {
            /*
             * 80まで      変動なし
             * 80-50      幻聴
             * 50-30      幻聴＋低彩度
             * 30-        強い幻聴＋彩度無し
             */
            SanState newSanState = x switch
            {
                > 80 => SanState.MentalLevel0,
                > 50 => SanState.MentalLevel1,
                > 30 => SanState.MentalLevel2,
                _ => SanState.MentalLevel3
            };
            if (_sanState == newSanState)
            {
                return;
            }

            switch (x)
            {
                case > 80:
                {
                    // 80まで 変動なし
                    //TODO:幻聴実装時に調整
                    /*_ctHallucination.Cancel();
                    _ctStrongHallucination.Cancel();
                    _ctSaturation.Cancel();*/
                    Saturation(0f).Forget();
                    
                    //キャンセル
                    _ctIntensityWave.Cancel();
                }
                    break;
                case > 50:
                {
                    // 80-50 幻聴
                    Hallucination().Forget();
                    Saturation(0f).Forget();
                    
                    //キャンセル
                    _ctIntensityWave.Cancel();
                }
                    break;
                case > 30:
                {
                    // 50-30 幻聴＋低彩度
                    Hallucination().Forget();
                    Saturation(0.5f).Forget();
                    
                    //キャンセル
                    _ctIntensityWave.Cancel();
                }
                    break;
                default:
                {
                    // 30- 強い幻聴＋彩度無し
                    Hallucination().Forget();
                    await Saturation(1f);
                    _ctIntensityWave.ReCreate();
                    IntensityWave(_volumeInsensityCurve.Evaluate(1f),_ctIntensityWave.GetToken()).Forget();
                }
                    break;
            }

            _sanState = newSanState;
        }).AddTo(this);
    }

    /// <summary>
    /// 幻聴
    /// </summary>
    private async UniTask Hallucination()
    {
        //TODO:幻聴の演出
        //TODO:強い幻聴から幻聴への切り替えの対応
        await DOVirtual.Float(0f, 1f, _statusChangeTime, f => { }).ToUniTask();
    }

    /// <summary>
    /// 強い幻聴
    /// </summary>
    private async UniTask StrongHallucination()
    {
        //TODO:強い幻聴の演出
        //TODO:幻聴から強い幻聴への切り替えの対応
        await DOVirtual.Float(0f, 1f, _statusChangeTime, f => { }).ToUniTask();
    }

    /// <summary>
    /// 彩度
    /// </summary>
    private async UniTask Saturation(float end)
    {
        var startValue = _grayscaleMaterial.GetFloat(Blend);
        if (startValue == end)
        { return; }

        await DOVirtual.Float(startValue, end, _statusChangeTime, f =>
        {
            _grayscaleMaterial.SetFloat(Blend, f);
            _vignette.intensity.value = _volumeInsensityCurve.Evaluate(f);
            _vignette.smoothness.value = _volumeSmoothnessCurve.Evaluate(f);
        }).ToUniTask();
    }
    
    private async UniTask IntensityWave(float baseValue,CancellationToken ct,float waveValue = 0.025f)
    {
        float time = 0;
        while (true)
        {
            //これを行うことでvalueの値がbaseValueから始まるのでスムーズに演出が見える
            time += Time.deltaTime;
            var value = baseValue + Mathf.Sin(time * 2) * waveValue;
            _vignette.intensity.value = value;
            await UniTask.Yield(ct);
        }
    }
}
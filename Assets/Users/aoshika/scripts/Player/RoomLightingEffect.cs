using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RoomLightingEffect : SingletonMonoBehaviour<RoomLightingEffect>
{
    [SerializeField] private Volume volume;
    private ShadowsMidtonesHighlights _roomShadow;
    private float _fogLastValue = 0;
    private float _shadowLastValue = 0;
    [SerializeField] private float _fogLowValue = 0;
    [SerializeField] private float _fogHightValue = 0.07f;
    [SerializeField] private float _shadowLowValue = 0.5f;
    [SerializeField] private float _shadowHightValue = -0.25f;

    CancelToken _cancelToken = new CancelToken();

    public async UniTask CallValueChange(bool InToRoom,float time=1)
    {
        _cancelToken.Cancel();
        _cancelToken.ReCreate();
        if (volume.profile.TryGet<ShadowsMidtonesHighlights>(out var _tmpRoomShadow))
        {
            _tmpRoomShadow.shadows.overrideState = true;
            if (InToRoom == true)
            {
                UniTask[] task = new UniTask[2];
                task[0] = RoomShadowsValueChange(_shadowHightValue, time, _tmpRoomShadow, _cancelToken.GetToken());
                task[1] = FogDensityValueChange(_fogHightValue, time, _cancelToken.GetToken());
                await UniTask.WhenAll(task);
            }
            else
            {
                UniTask[] task = new UniTask[2];
                task[0] = RoomShadowsValueChange(_shadowLowValue, time, _tmpRoomShadow, _cancelToken.GetToken());
                task[1] = FogDensityValueChange(_fogLowValue, time, _cancelToken.GetToken());
                await UniTask.WhenAll(task);
            }
        }
    }

    /// <summary>
    /// FOGの値変更
    /// </summary>
    /// <param name="endValue"></param>
    /// <param name="duration"></param>
    /// <param name="cancellationToken"></param>
    private async UniTask FogDensityValueChange(float endValue, float duration, CancellationToken cancellationToken)
    {
        await DOVirtual.Float(_fogLastValue, endValue, duration, f =>
        {
            f = (float)Math.Round(f, 5);
            _fogLastValue = f;
            RenderSettings.fogDensity = f;
        }).ToUniTask(cancellationToken: cancellationToken);
    }

    /// <summary>
    /// ShadowsMidtonesHighlightsのshadowsの値変更
    /// </summary>
    /// <param name="startValue"></param>
    /// <param name="endValue"></param>
    /// <param name="duration"></param>
    /// <param name="smh"></param>
    /// <param name="cancellationToken"></param>
    private async UniTask RoomShadowsValueChange(float endValue, float duration, ShadowsMidtonesHighlights smh,
        CancellationToken cancellationToken)
    {
        Vector4 shadowVal = smh.shadows.value;

        await DOVirtual.Float(_shadowLastValue, endValue, duration, f =>
        {
            smh.shadows.overrideState = true;
            f = (float)Math.Round(f, 5);
            _shadowLastValue = f;
            smh.shadows.SetValue(
                new UnityEngine.Rendering.Vector4Parameter(new Vector4(shadowVal.x, shadowVal.y, shadowVal.z, f)));

            volume.profile.TryGet(out ShadowsMidtonesHighlights updatedSMH);
            if (updatedSMH != null)
            {
                updatedSMH.shadows.value = smh.shadows.value;
            }
        }).ToUniTask(cancellationToken: cancellationToken);
    }
}
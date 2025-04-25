using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlinkingLights : MonoBehaviour
{
    private Light _light;
    [SerializeField] private float _threshold = 0.75f;
    [SerializeField] private AnimationCurve[] blinkCurves;

    void Start()
    {
        _light = GetComponent<Light>();
        Blink().Forget();
    }

    private async UniTask Blink()
    {
        CancellationToken ct = destroyCancellationToken;
        while (true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(3, 10)), cancellationToken: ct);
            var blinkCurve = blinkCurves[Random.Range(0, blinkCurves.Length)];

            if (blinkCurve is null)
            {
                return;
            }

            await UniTask.DelayFrame(Random.Range(1, 5), cancellationToken: ct);
            float time = Random.Range(1f, 3f);
            float deltaTime = 0f;
            while (time > deltaTime)
            {
                deltaTime += Time.deltaTime;
                var value = blinkCurve.Evaluate(deltaTime / time);
                _light.enabled = value > _threshold;
                await UniTask.Yield(ct);
            }

            _light.enabled = true;
        }
    }
}
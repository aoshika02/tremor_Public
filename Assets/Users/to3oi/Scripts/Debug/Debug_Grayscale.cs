using Cysharp.Threading.Tasks;
using UnityEngine;

public class Debug_Grayscale : MonoBehaviour
{
    [SerializeField]
    public Material _grayscaleMaterial;
    private static readonly int Blend = Shader.PropertyToID("_Blend");

    async void Start()
    {
        _grayscaleMaterial = Grayscale.Instance.GrayscaleMaterial;
        while (true)
        {
            float sinWave = Mathf.Sin(Time.time) * 0.5f + 0.5f;
            _grayscaleMaterial.SetFloat(Blend, sinWave);
            await UniTask.Yield();
        }
    }
}

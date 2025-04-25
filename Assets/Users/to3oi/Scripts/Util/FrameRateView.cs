using System.Globalization;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class FrameRateView : SingletonMonoBehaviour<FrameRateView>
{
    [SerializeField] private TextMeshProUGUI _frameRateView;

    protected override void Awake()
    {
        if (CheckInstance())
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    void Update()
    {
        _frameRateView.text = $"{(1f / Time.deltaTime).ToString("F2", CultureInfo.CurrentCulture)}";
    }
}

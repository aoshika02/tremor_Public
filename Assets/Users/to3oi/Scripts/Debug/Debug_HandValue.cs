using TMPro;
using UnityEngine;
using UniRx;

public class Debug_HandValue : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMeshProUGUI;

    void Start()
    {
#if UNITY_ANDROID
        InputManager.Instance.LIndexTrigger.Subscribe(x =>
        {
            _textMeshProUGUI.text = x.ToString();
        }).AddTo(this);
#endif
    }
}
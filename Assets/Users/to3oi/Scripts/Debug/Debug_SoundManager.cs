using UniRx;
using UnityEngine;

public class Debug_SoundManager : MonoBehaviour
{
    [SerializeField] private SEType _seType1;
    [SerializeField] private SEType _seType2;
    [SerializeField] private SEType _seType3;

    [SerializeField] private Transform _seType1Transform;
    [SerializeField] private Transform _seType2Transform;
    [SerializeField] private Transform _seType3Transform;

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 100), "_seType1"))
        {
            SoundManager.Instance.PlaySE(_seType1, _seType1Transform);
        }

        if (GUI.Button(new Rect(10, 40, 150, 100), "_seType2"))
        {
            SoundManager.Instance.PlaySE(_seType2, _seType2Transform);
        }

    }

    bool _isPlaying = false;
    private void Start()
    {
        InputManager.Instance.Cancel.Subscribe(x =>
        {
            if (x !=0)
            {
                if (!_isPlaying)
                {
                    _isPlaying = true;
                    SoundManager.Instance.PlaySE(_seType1, _seType1Transform);
                    SoundManager.Instance.PlaySE(_seType2, _seType2Transform);
                    SoundManager.Instance.PlaySE(_seType3, _seType3Transform);
                }
            }
            else
            {
                _isPlaying = false;
            }
        }).AddTo(this);
        
        InputManager.Instance.Decision.Subscribe(x =>
        {
            if (x != 0)
            {
                SoundManager.Instance.UpdateHapticsPrioritySE();
            }            
        }).AddTo(this); 
        
        InputManager.Instance.Dash.Subscribe(x =>
        {
            if (x != 0)
            {
                SoundManager.Instance.PlaySE(_seType1, _seType1Transform,hapVolume:1);
            }
        }).AddTo(this);

    }
}
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] Animator _animator;
    private static readonly int X = Animator.StringToHash("X");
    private static readonly int Y = Animator.StringToHash("Y");
    private static readonly int MotionSpeed = Animator.StringToHash("MotionSpeed");
    private static readonly int Idle = Animator.StringToHash("Idle");

    private Vector2 _moveVector2 = Vector2.zero;
    private float _dashValue = 0;
    private bool _dash = false;
    CancelToken _cancelToken = new CancelToken();

    void Start()
    {
        InputManager.Instance.Move.Subscribe(v => { _moveVector2 = v; }).AddTo(this);
        InputManager.Instance.Dash.Subscribe(f =>
        {
            if (f == 0 && _dash)
            {
                _cancelToken.ReCreate();
                //ダッシュ解除
                _dash = false;
                DOVirtual.Float(_dashValue, 0, 0.25f, f => { _dashValue = f; })
                    .WithCancellation(_cancelToken.GetToken());
            }
            else if (f == 1 && !_dash)
            {
                _cancelToken.ReCreate();
                //ダッシュ開始
                _dash = true;
                DOVirtual.Float(_dashValue, 1, 0.25f, f => { _dashValue = f; })
                    .WithCancellation(_cancelToken.GetToken());
            }
            _dashValue = f;
        }).AddTo(this);
    }

    private void Update()
    {
        _animator.SetBool(Idle, _moveVector2 == Vector2.zero);
        _animator.SetFloat(MotionSpeed, 1 + _dashValue);
        _animator.SetFloat(X, _moveVector2.x * (1 + _dashValue));
        _animator.SetFloat(Y, _moveVector2.y * (1 + _dashValue));
    }
}
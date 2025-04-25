using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : SingletonMonoBehaviour<PlayerMove>
{
    private Vector2 _moveDirection;
    private Rigidbody _rb;

    //歩行時の速度
    [SerializeField] private float _moveSpeed_low;

    //走行時の速度
    [SerializeField] private float _moveSpeed_high;

    [SerializeField] private Transform _cameraShakeRoot;
    [SerializeField] private Transform _camera;

    //playermoveで持っておくスタミナ
    private float _playerStamina;

    //回転系統
    [Tooltip("最小角度-180～180")] [SerializeField]
    float _maxCameraRotateY;

    [Tooltip("最大角度-180～180")] [SerializeField]
    float _minCameraRotateY;

    [SerializeField] Camera _playercamera;
    [SerializeField] float fovspeed;

    private float _cameraAngleY = 0;
    [SerializeField] private float _rotationSpeed = 1f;
    private Vector2 _cameraRotation;

    private bool _isMove = false;

    private bool _isRun = false;

    //走るボタンのしきい値
    private float _runThreshold = 0.25f;

    //息切れ状態のフラグ
    public bool _dashStan { private set; get; } = false;
    [SerializeField] private int _stanValue = 20;

    [SerializeField] private int _cancelStanValue = 100;

    //
    private float _runningTime;

    //走ることができる最大の秒数
    [SerializeField] private int _maxRunningTime;

    PlayerStatus playerStatus;

    [SerializeField] private Transform _footTransform;
    private List<SEType> _seTypeList = SEType.GetValues(typeof(SEType)).Cast<SEType>().ToList();
    private SEType _savedSeType;

    private bool _isSound = false;

    //チュートリアル用制御フラグ
    private bool _isMoveLock = false;

    //チェイス開始フラグ
    private bool _isStartChase = false;

    DecisionGround decisionGround;
    List<DecisionGround.GroundTypeData> _groundType;
    List<SoundHash> soundHashes;

    [SerializeField] private List<HartSeProperty> _propertyTable = new List<HartSeProperty>()
    {
        new HartSeProperty() { _lowIntervalPoint = 0, _highIntervalPoint = 3, _intervalTime = 0.05f },
        new HartSeProperty() { _lowIntervalPoint = 3, _highIntervalPoint = 6, _intervalTime = 0.1f },
        new HartSeProperty() { _lowIntervalPoint = 6, _highIntervalPoint = 10, _intervalTime = 0.15f },
    };

    [Serializable]
    private class HartSeProperty
    {
        //待機時間切り替え領域
        [SerializeField] public float _lowIntervalPoint;

        [SerializeField] public float _highIntervalPoint;

        //待機時間
        [SerializeField] public float _intervalTime;
    }
    
    [SerializeField] private float shakePower = 0.001f;
    [SerializeField] private float shakeTime = 5f;
    [SerializeField] private int vibrato = 6;


    void Start()
    {
        _isStartChase = false;
        _isSound = false;
        _rb = GetComponent<Rigidbody>();
        //1秒あたりに減らす、増やすスタミナの値の計算
        _runningTime = 100 / _maxRunningTime;
        SetMove(true);

        //playerの移動イベントの登録
        InputManager.Instance.SetMoveStartedAction(PressedInputMovement);
        InputManager.Instance.SetMovePerformedAction(PressedInputMovement);
        InputManager.Instance.SetMoveCanceledAction(CanceledMoveDirection);

        InputManager.Instance.SetCameraStartedAction(PressedInputCameraRotation);
        InputManager.Instance.SetCameraPerformedAction(PressedInputCameraRotation);
        InputManager.Instance.SetCameraCanceledAction(CanceledCameraRotation);

        InputManager.Instance.SetRunStartedAction(PressedInputRunButton);
        InputManager.Instance.SetRunPerformedAction(PressedInputRunButton);
        InputManager.Instance.SetRunCanceledAction(CanceledRunButton);

        playerStatus = PlayerStatus.Instance;
        playerStatus.StaminaObservable.Subscribe(Stamina =>
        {
            _playerStamina = Stamina;
            if (Stamina <= _stanValue)
            {
                _dashStan = true;
            }
            else if (Stamina >= _cancelStanValue)
            {
                _dashStan = false;
            }
        });
    }

    void Update()
    {
        if (!_isMove)
        {
            return;
        }

        if (_isMoveLock)
        {
            return;
        }

        transform.Rotate(0, _cameraRotation.x * _rotationSpeed, 0);

        _cameraAngleY += -_cameraRotation.y * _rotationSpeed;

        if (_maxCameraRotateY <= _cameraAngleY)
        {
            _cameraAngleY = _maxCameraRotateY;
        }

        if (_cameraAngleY <= _minCameraRotateY)
        {
            _cameraAngleY = _minCameraRotateY;
        }

        var sampleAngle = _camera.eulerAngles;
        sampleAngle.x = _cameraAngleY;
        _camera.eulerAngles = sampleAngle;

        var cameraAngle = Quaternion.Euler(0, _camera.transform.rotation.eulerAngles.y, 0);

        var v = cameraAngle * new Vector3(_moveDirection.x, 0, _moveDirection.y) * _moveSpeed_high / 60;

        if (_isRun == true && _dashStan == false && v != Vector3.zero)
        {
            _rb.velocity = v;

            if (_playercamera.fieldOfView < 70)
            {
                _playercamera.fieldOfView += fovspeed;
            }

            if (v == Vector3.zero)
            {
                return;
            }

            playerStatus.SubStamina(Time.deltaTime * _runningTime);
        }
        else
        {
            _rb.velocity = cameraAngle * new Vector3(_moveDirection.x, 0, _moveDirection.y) *
                _moveSpeed_low / 60;
            if (_playercamera.fieldOfView > 60)
            {
                _playercamera.fieldOfView -= fovspeed;
            }

            playerStatus.AddStamina(Time.deltaTime * _runningTime);
        }

        if (!_isStartChase)
        {
            //if (地面の種類)
            if (_moveDirection.x != 0 || _moveDirection.y != 0)
            {
                if (_isSound) return;
                WalkSe(destroyCancellationToken).Forget();
            }
        }
        else
        {
            if (_isSound) return;
            if (InGameFlow.Instance.IsAttackedGhost) return;
            HartSe(destroyCancellationToken).Forget();
        }
    }

    private async UniTask WalkSe(CancellationToken ct)
    {
        _isSound = true;

        decisionGround = DecisionGround.Instance;

        _groundType = decisionGround.GetGroundType();

        var _waitTime = (_isRun == true && _dashStan == false)
            ? UnityEngine.Random.Range(0.05f, 0.1f)
            : UnityEngine.Random.Range(0.3f, 0.4f);
        var _selist = new List<UniTask>();

        if (_groundType.Count != 0)
        {
            for (int i = 0; i < _groundType.Count; i++)
            {
                var _setype =
                    _seTypeList[UnityEngine.Random.Range(0 + _groundType[i].Type * 4, 4 + _groundType[i].Type * 4)];
                while (_savedSeType == _setype)
                {
                    _setype = _seTypeList[
                        UnityEngine.Random.Range(0 + _groundType[i].Type * 4, 4 + _groundType[i].Type * 4)];
                }

                _selist.Add(SoundManager.Instance.PlaySEAsync(_setype, _footTransform, cancellationToken: ct,
                    _groundType[i].Ratio));
                _savedSeType = _setype;
            }

            for (int i = 0; i < _groundType.Count; i++)
            {
                //取得している地面の比率
                Debug.Log($"_groundType[{i}]:{_groundType[i].Type},Ratio:{_groundType[i].Ratio}");
            }
        }
        else
        {
            var _setype = _seTypeList[UnityEngine.Random.Range(0, 4)];
            while (_savedSeType == _setype)
            {
                _setype = _seTypeList[UnityEngine.Random.Range(0, 4)];
            }

            _savedSeType = _setype;
            _selist.Add(SoundManager.Instance.PlaySEAsync(_setype, _footTransform, cancellationToken: ct, 1.0f));
        }

        await _selist;
        await UniTask.Delay(TimeSpan.FromSeconds(_waitTime), cancellationToken: ct);
        _isSound = false;
    }

    private async UniTask HartSe(CancellationToken ct)
    {
        Debug.Log(Vector3.Distance(InGameFlow.Instance.EnemyGhost.transform.position, Player.Instance.Position));
        _isSound = true;

        SoundManager.Instance.PlaySE(SEType.ChaseHeart);

        var time = 0f;
        while (time <= GetHartSeProperty()._intervalTime)
        {
            time += Time.deltaTime;
            await UniTask.Yield(cancellationToken: ct);
        }

        _isSound = false;
    }

    private HartSeProperty GetHartSeProperty()
    {
        var dis = Vector3.Distance(InGameFlow.Instance.EnemyGhost.transform.position, Player.Instance.Position);

        for (int i = 0; i < _propertyTable.Count; i++)
        {
            if (_propertyTable[i]._lowIntervalPoint <= dis && dis < _propertyTable[i]._highIntervalPoint)
            {
                return _propertyTable[i];
            }
        }

        return _propertyTable.Last();
    }

    public void StartChaseFlag()
    {
        _isStartChase = true;
    }

    public void SetMove(bool isMove)
    {
        _isMove = isMove;
        if (isMove is false)
        {
            _rb.velocity = Vector3.zero;
        }
    }

    public void InvertMoveLockFlag()
    {
        _isMoveLock = !_isMoveLock;
    }

    public void SetPlayerTransform(Transform setTransform)
    {
        transform.position = setTransform.position;
    }

    public async UniTask SetCameraRotation(float eulerAngles, float time)
    {
        await DOVirtual.Float(_camera.rotation.eulerAngles.x, eulerAngles, time, f =>
        {
            _cameraAngleY = f;
            var sampleAngle = _camera.eulerAngles;
            sampleAngle.x = _cameraAngleY;
            _camera.eulerAngles = sampleAngle;
        }).ToUniTask();
    }

    public async UniTask SetPlayerRotation(Transform setTransform, float time)
    {
        UniTask[] task = new UniTask[2];

        float BetweenY = Mathf.Abs(setTransform.position.y - _camera.transform.position.y);
        float dis = Vector3.Distance(setTransform.transform.position, _camera.transform.position);

        float TargetCameraX = Mathf.Asin(BetweenY / dis) * Mathf.Rad2Deg;
        if (_maxCameraRotateY <= TargetCameraX)
        {
            TargetCameraX = _maxCameraRotateY;
        }

        if (TargetCameraX <= _minCameraRotateY)
        {
            TargetCameraX = _minCameraRotateY;
        }

        var NowAngleX = _cameraAngleY;
        task[0] =
            DOVirtual.Float(NowAngleX, TargetCameraX, time, f =>
            {
                _cameraAngleY = f;
                var sampleAngle = _camera.eulerAngles;
                sampleAngle.x = _cameraAngleY;
                _camera.eulerAngles = sampleAngle;
            }).ToUniTask();
        Vector3 targetDirection = setTransform.position - transform.position;
        targetDirection.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        task[1] = transform.DORotate(new Vector3(0, targetRotation.eulerAngles.y, 0), time, RotateMode.Fast)
            .ToUniTask();
        await UniTask.WhenAll(task).AttachExternalCancellation(cancellationToken: destroyCancellationToken);
    }

    public enum ShakeDirection
    {
        All,
        Vertical,
        Horizontal,
    }

    public Vector3 GetShakeDirection2Vector3(ShakeDirection shakeDirection) => shakeDirection switch
    {
        ShakeDirection.All => new Vector3(1, 1, 0),
        ShakeDirection.Vertical => new Vector3(0, 1, 0),
        ShakeDirection.Horizontal => new Vector3(1, 0, 0),
    };
    
    public async UniTask ShakeCamera(float shakePower = 1f, float shakeTime = 0.25f, int vibrato = 1,
        ShakeDirection shakeDirection = ShakeDirection.All, CancellationToken cancellationToken = default)
    {
        await _cameraShakeRoot.DOShakePosition(shakeTime, GetShakeDirection2Vector3(shakeDirection) * shakePower, vibrato: vibrato)
            .ToUniTask(cancellationToken: cancellationToken);
    }

    public bool GetRun() => _isRun;

    #region PlayerMove

    private void PressedInputMovement(InputAction.CallbackContext context)
    {
        _moveDirection = context.ReadValue<Vector2>();
    }

    private void CanceledMoveDirection(InputAction.CallbackContext context)
    {
        _moveDirection = Vector2.zero;
    }

    #endregion

    #region CameraMove

    private void PressedInputCameraRotation(InputAction.CallbackContext context)
    {
        _cameraRotation = context.ReadValue<Vector2>();
    }

    private void CanceledCameraRotation(InputAction.CallbackContext context)
    {
        _cameraRotation = Vector2.zero;
    }

    #endregion

    #region Run

    private void PressedInputRunButton(InputAction.CallbackContext context)
    {
        _isRun = _runThreshold <= context.ReadValue<float>();
    }

    private void CanceledRunButton(InputAction.CallbackContext context)
    {
        _isRun = false;
    }

    #endregion

    private void OnDestroy()
    {
        //playerの移動イベントの登録解除
        InputManager.Instance.RemoveMoveStartedAction(PressedInputMovement);
        InputManager.Instance.RemoveMovePerformedAction(PressedInputMovement);
        InputManager.Instance.RemoveMoveCanceledAction(CanceledMoveDirection);

        InputManager.Instance.RemoveCameraStartedAction(PressedInputCameraRotation);
        InputManager.Instance.RemoveCameraPerformedAction(PressedInputCameraRotation);
        InputManager.Instance.RemoveCameraCanceledAction(CanceledCameraRotation);

        InputManager.Instance.RemoveRunStartedAction(PressedInputRunButton);
        InputManager.Instance.RemoveRunPerformedAction(PressedInputRunButton);
        InputManager.Instance.RemoveRunCanceledAction(CanceledRunButton);
    }
}
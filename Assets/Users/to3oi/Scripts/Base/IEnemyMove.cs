using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵の移動処理を行う基底クラス
/// </summary>
[RequireComponent(typeof(Collider))]
public class IEnemyMove: MonoBehaviour
{
    [SerializeField]
    protected Transform[] _targetPositions;
    protected int _targetIndex = 0;
    private bool _isIndexAdd = true;
    protected Rigidbody _rigidbody = null;
    protected EnemyState _enemyState = EnemyState.None;
    [SerializeField] protected Transform _head;

    [SerializeField]
    public IEnemyAnimation EnemyAnimation;

    /*------------------------------------------------------------------*/
    //移動に関するプロパティ
    [SerializeField,]
    protected float _chasemoveSpeed = 2.0f;
    public float ChaseMoveSpeed
    {
        get { return _chasemoveSpeed; }
    }
    //プレイヤーを捕獲する距離    
    protected float _captureDistance = 2;
    /// <summary>
    /// プレイヤーを捕獲する距離
    /// </summary>
    public float CaptureDistance
    {
        get { return _captureDistance; }
    }
    /*------------------------------------------------------------------*/
    
    /*------------------------------------------------------------------*/
    //移動に関するプロパティ
    [SerializeField,]
    protected float _moveSpeed = 1.0f;
    public float MoveSpeed
    {
        get { return _moveSpeed; }
    }
    [SerializeField]
    protected float _moveCompletionDistance = 0.1f;
    /// <summary>
    /// 移動を完了するしきい値
    /// </summary>
    public float MoveCompletionDistance
    {
        get { return _moveCompletionDistance; }
    }
    
    public bool IsHit
    {
        get { return _hitCollisionList.Count > 0; }
    }
    
    [SerializeField]
    protected float _rotationSpeed = 3.0f;
    public float RotationSpeed
    {
        get { return _rotationSpeed; }
    }

    [SerializeField]
    protected float _playerFindDistance = 7f;

    [SerializeField]
    protected float _playerFoundedPower = 1.25f;

    public bool IsPlayerFound = false;
    /// <summary>
    /// プレイヤーを索敵する距離
    /// </summary>
    public float PlayerFindDistance
    {
        get { return _playerFindDistance * (IsPlayerFound ? 1f : _playerFoundedPower); }
    } 
    
    /*------------------------------------------------------------------*/
    
    
    protected List<Collision> _hitCollisionList = new List<Collision>();
    [SerializeField] private LayerMask _layerMask;
    private Collider _collider;

    protected virtual void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    public virtual void StartEnemy()
    {
        //敵の移動開始
    }
    //割り込みなどで次のステートを指定するための変数
    public EnemyState ExtraNextState { get; private set; } = EnemyState.None;
    
    public void SetExtraNextState(EnemyState state)
    {
        ExtraNextState = state;
    }
    public Transform GetNextPosition()
    {
        return _targetPositions[_targetIndex];
    }

    public Rigidbody GetRigidbody()
    {
        return _rigidbody;
    }
    public void UpdatePosition()
    {
        //端まで行ったら逆に戻る
        if(_isIndexAdd) _targetIndex++;
        else _targetIndex--;
        
        //配列外参照になったら範囲内に戻す
        if(_targetIndex >= _targetPositions.Length) { _isIndexAdd = false; _targetIndex = _targetPositions.Length - 2; }
        else if(_targetIndex < 0) { _isIndexAdd = true; _targetIndex = 1; }
    }
    
    protected virtual void OnCollisionEnter(Collision collision)
    {
        //何かしらのコライダーに触れたらListにいれる
        if(CompareLayer(collision.gameObject.layer))
        {
            Physics.IgnoreCollision(_collider, collision.collider, true);
            _hitCollisionList.Add(collision);
            //todo:当たったら移動速度を一定時間落とす
        }
    }
    private bool CompareLayer(int layer)
    {
        return ((1 << layer) & _layerMask) != 0;
    }
    public void ResetCollision()
    {
        foreach (var col in _hitCollisionList)
        {
            Physics.IgnoreCollision(_collider, col.collider, false);
        }
        _hitCollisionList.Clear();
    }
}

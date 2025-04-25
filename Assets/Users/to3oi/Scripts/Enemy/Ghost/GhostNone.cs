using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GhostNone : EnemyStateBase
{
    //todo:コードの内容すべて書き換え
    //2024/06/22 GhostMoveからコピペのみ
    protected EnemyState _enemyState { get; set; }
     
    protected Rigidbody _rigidbody = null;
    protected Transform _thisTransform = null;
     
    private float _distance = 0.1f;
    private float _moveSpeed = 1.0f;
     
    public GhostNone(IEnemyMove enemyMove)
    {
        _enemyMove = enemyMove;
        _enemyState = EnemyState.Move;
        _rigidbody = _enemyMove.GetRigidbody();
        _thisTransform = enemyMove.transform;
    }
     
    public override async UniTask Entry(EnemyState enemy, CancellationToken ct)
    {
    }
     
    public override async UniTask Do(EnemyState enemy, CancellationToken ct)
    {
    }
     
    public override async UniTask<EnemyState> Exit(EnemyState enemy, CancellationToken ct)
    {
        return EnemyState.Stay;
    }
}

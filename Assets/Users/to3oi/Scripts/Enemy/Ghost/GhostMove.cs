using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// ゴーストの移動ステート
/// </summary>
public class GhostMove : EnemyStateBase
{
    protected EnemyState _enemyState { get; set; }

    protected Rigidbody _rigidbody = null;
    protected Transform _thisTransform = null;


    public GhostMove(IEnemyMove enemyMove)
    {
        _enemyMove = enemyMove;
        _enemyState = EnemyState.Move;
        _rigidbody = _enemyMove.GetRigidbody();
        _thisTransform = enemyMove.transform;
    }

    public override async UniTask Entry(EnemyState enemy, CancellationToken ct)
    {
        _enemyState = enemy;
        _enemyMove.EnemyAnimation.SetBool(GhostAnimationType.Move,true);
    }

    public override async UniTask Do(EnemyState enemy, CancellationToken ct)
    {
        var t = _enemyMove.GetNextPosition();
        var v = t.position - _thisTransform.position;
        v.y = 0;
        while (Vector3.Distance(t.position, _thisTransform.position) > _enemyMove.MoveCompletionDistance)
        {
            _rigidbody.MovePosition(_thisTransform.position + v.normalized * _enemyMove. MoveSpeed * Time.deltaTime);
            await UniTask.Yield(cancellationToken: ct);
        }
    }

    public override async UniTask<EnemyState> Exit(EnemyState enemy, CancellationToken ct)
    {
        _enemyMove.EnemyAnimation.SetBool(GhostAnimationType.Move,false);
        return EnemyState.Stay;
    }
}
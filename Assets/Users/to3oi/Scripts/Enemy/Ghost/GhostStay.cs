using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Stay
/// </summary>
public class GhostStay : EnemyStateBase
{
    protected  EnemyState _enemyState { get; set; }
    float _duration = 0;
    public GhostStay(IEnemyMove enemyMove)
    {
        _enemyMove = enemyMove;
        _enemyState = EnemyState.Stay;
    }
    
    public override async UniTask Entry(EnemyState enemy,CancellationToken ct)
    {
        _enemyMove.EnemyAnimation.ResetAll();
        _enemyMove.ResetCollision();
        _duration = Random.Range(0.25f, 0.75f);
    }

    public override async UniTask Do(EnemyState enemy,CancellationToken ct)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(_duration),cancellationToken: ct);
    }

    public override async UniTask<EnemyState> Exit(EnemyState enemy,CancellationToken ct)
    {
        _enemyMove.UpdatePosition();
        return EnemyState.FindPlayer;
    }
}
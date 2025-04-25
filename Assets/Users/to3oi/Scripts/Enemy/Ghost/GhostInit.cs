using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// ゴーストの初期化を行う
/// </summary>
public class GhostInit : EnemyStateBase
{
    protected EnemyState _enemyState { get; set; }

    public GhostInit(IEnemyMove enemyMove)
    {
        _enemyMove = enemyMove;
        _enemyState = EnemyState.Init;
    }

    public override async UniTask Entry(EnemyState enemy, CancellationToken ct)
    {
    }

    public override async UniTask Do(EnemyState enemy, CancellationToken ct)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(3), cancellationToken: ct);
    }

    public override async UniTask<EnemyState> Exit(EnemyState enemy, CancellationToken ct)
    {
        if (EnemyState.None == _enemyMove.ExtraNextState)
        {
            return EnemyState.Stay;
        }
        else
        {
            var state = _enemyMove.ExtraNextState;
            _enemyMove.SetExtraNextState(EnemyState.None);
            return state;
        }
    }
}
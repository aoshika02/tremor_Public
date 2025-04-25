using System;
using System.Threading;
using Cysharp.Threading.Tasks;

/// <summary>
/// ゴーストがプレイヤーを攻撃するステート
/// </summary>
public class GhostAttack : EnemyStateBase
{    
    protected EnemyState _enemyState { get; set; }
     
    public GhostAttack(IEnemyMove enemyMove)
    {
        _enemyMove = enemyMove;
    }
     
    public override async UniTask Entry(EnemyState enemy, CancellationToken ct)
    {
    }
     
    public override async UniTask Do(EnemyState enemy, CancellationToken ct)
    {
        //todo:プレイヤーを攻撃する演出を再生
        _enemyMove.EnemyAnimation.SetTrigger(GhostAnimationType.Attack2);
        await UniTask.Yield(ct);
    }
     
    public override async UniTask<EnemyState> Exit(EnemyState enemy, CancellationToken ct)
    {
        //todo:ゴーストのすべての処理を停止させる
        return EnemyState.FlowBreak;
    }
}

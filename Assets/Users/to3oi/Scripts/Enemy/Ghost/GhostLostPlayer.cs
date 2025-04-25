using System.Threading;
using Cysharp.Threading.Tasks;
/// <summary>
/// プレイヤーを見失ったときの処理
/// </summary>
public class GhostLostPlayer  : EnemyStateBase
{
    protected  EnemyState _enemyState { get; set; }
    public GhostLostPlayer(IEnemyMove enemyMove)
    {
        _enemyMove = enemyMove;
        _enemyState = EnemyState.LostPlayer;
    }
    
    public override async UniTask Entry(EnemyState enemy,CancellationToken ct)
    {
    }

    public override async UniTask Do(EnemyState enemy,CancellationToken ct)
    {
        //todo:プレイヤーを見失ったアニメーションを再生
        //await UniTask.Delay(TimeSpan.FromSeconds(3),cancellationToken: ct);
    }

    public override async UniTask<EnemyState> Exit(EnemyState enemy,CancellationToken ct)
    {
        return EnemyState.ReturnPosition;
    }
}

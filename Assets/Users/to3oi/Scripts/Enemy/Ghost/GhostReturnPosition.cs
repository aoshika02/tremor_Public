using System.Threading;
using Cysharp.Threading.Tasks;

public class GhostReturnPosition : EnemyStateBase
{
    protected  EnemyState _enemyState { get; set; }
    private Player _player;

    public GhostReturnPosition(IEnemyMove enemyMove)
    {
        _enemyMove = enemyMove;
        _enemyState = EnemyState.ReturnPosition;
        _player = Player.Instance;
    }
    
    public override async UniTask Entry(EnemyState enemy,CancellationToken ct)
    {
    }

    public override async UniTask Do(EnemyState enemy,CancellationToken ct)
    {
        //元のターゲットの位置に移動
        //別にRotationからやり直してもいい
    }

    public override async UniTask<EnemyState> Exit(EnemyState enemy,CancellationToken ct)
    {
        return EnemyState.Stay;
    }
}

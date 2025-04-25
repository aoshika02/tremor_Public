using System.Threading;
using Cysharp.Threading.Tasks;

public class EnemyStateBase
{
    protected EnemyState _enemyState { get; set; }
    protected IEnemyMove _enemyMove { get; set; }
    
    //TODO:EnemyStateの必要性を検討
    public virtual async UniTask Entry(EnemyState enemy,CancellationToken ct)
    {
    }

    public virtual async UniTask Do(EnemyState enemy,CancellationToken ct)
    {
    }

    public virtual async UniTask<EnemyState> Exit(EnemyState enemy,CancellationToken ct)
    {
        return EnemyState.None;
    }
}
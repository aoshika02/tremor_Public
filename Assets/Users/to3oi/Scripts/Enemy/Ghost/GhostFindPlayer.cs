using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// プレイヤーを探すモーションを再生するステート
/// </summary>
public class GhostFindPlayer : EnemyStateBase
{
    protected  EnemyState _enemyState { get; set; }
    private Transform _thisTransform = null;

    public GhostFindPlayer(IEnemyMove enemyMove)
    {
        _enemyMove = enemyMove;
        _thisTransform = enemyMove.transform;
        _enemyState = EnemyState.FindPlayer;
    }
    
    public override async UniTask Entry(EnemyState enemy,CancellationToken ct)
    {
    }

    public override async UniTask Do(EnemyState enemy,CancellationToken ct)
    {
        // プレイヤーを探すアニメーションを再生
        _enemyMove.EnemyAnimation.SetTrigger(GhostAnimationType.SearchType1);
        SoundManager.Instance.PlaySE(SEType.GhostVoice01,_thisTransform);
        //todo:このステートのときだけ索敵範囲を少し広げてもいいかも
    }

    public override async UniTask<EnemyState> Exit(EnemyState enemy,CancellationToken ct)
    {
        await _enemyMove.EnemyAnimation.WaitSearchTriggerAnimationExit(ct);
        // Tポーズになるので先に一旦Moveを再生
        _enemyMove.EnemyAnimation.SetBool(GhostAnimationType.Move,true);
        return EnemyState.Rotation;
    }
}

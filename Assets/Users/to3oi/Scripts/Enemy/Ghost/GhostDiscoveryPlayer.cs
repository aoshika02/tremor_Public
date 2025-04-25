using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// ゴーストがプレイヤーを見つけたときの処理
/// </summary>
public class GhostDiscoveryPlayer : EnemyStateBase
{
    protected  EnemyState _enemyState { get; set; }
    private Transform _thisTransform = null;

    public GhostDiscoveryPlayer(IEnemyMove enemyMove)
    {
        _enemyMove = enemyMove;
        _thisTransform = enemyMove.transform;
        _enemyState = EnemyState.DiscoveryPlayer;
    }
    
    public override async UniTask Entry(EnemyState enemy,CancellationToken ct)
    {
        _enemyMove.EnemyAnimation.ResetAll();
        SoundManager.Instance.PlaySE(SEType.GhostVoice02,_thisTransform);
    }

    public override async UniTask Do(EnemyState enemy,CancellationToken ct)
    {
        
        //プレイヤーの方向を向かせる
        var targetTransform = Player.Instance.transform;
        var ratio = 0.0f;
        Vector3 targetDirection = targetTransform.position - _thisTransform.position;
        targetDirection.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion currentRotation = _thisTransform.rotation;
        //回転方向を計算する
        var q = Quaternion.Lerp(_thisTransform.rotation, targetRotation, 1f);
        var diffRotation = Quaternion.Inverse(q) * _thisTransform.rotation;

        Vector3 axis = Vector3.zero;
        diffRotation.ToAngleAxis(out _, out axis);
        //axis.y == -1の場合右回転
        //axis.y == 1の場合左回転
        //todo:回転アニメーションを追加すると一瞬Tポーズになるので保留
        switch (axis.y)
        {
            case -1:
            {
                //_enemyMove.EnemyAnimation.SetTrigger(GhostAnimationType.TurnRight);
            }
                break;
            case 1:
            default:
            {
                //_enemyMove.EnemyAnimation.SetTrigger(GhostAnimationType.TurnLeft);
            }
                break;
        }
        
        UniTask[] task = new UniTask[2];
        task[0] = UniTask.Create(async () =>
        {
            //アニメーションがすぐに再生されないので少し待つ
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f),cancellationToken: ct);
            while (ratio < _enemyMove.RotationSpeed)
            {
                ratio += Time.deltaTime;

                _thisTransform.rotation =
                    Quaternion.Lerp(currentRotation, targetRotation, ratio / _enemyMove.RotationSpeed);

                await UniTask.Yield(cancellationToken: ct);
            }
        });
        //task[1] = _enemyMove.EnemyAnimation.WaitTurnTriggerAnimationExit(ct);

        await UniTask.WhenAll(task);
        
        //========================================================================================
        Debug.Log("プレイヤーを発見したときのモーションを再生");

        //プレイヤーを発見したときのモーションを再生
        _enemyMove.EnemyAnimation.SetTrigger(GhostAnimationType.FindPlayer);
        List<UniTask> tasks = new List<UniTask>();
        // todo:モーションに合わせてSEを鳴らす処理を時間がてきたら同期する
        await UniTask.Delay(TimeSpan.FromSeconds(1.5f),cancellationToken:ct);
        tasks.Add( SoundManager.Instance.PlaySEAsync(SEType.GhostVoice03,_thisTransform, ct));
        tasks.Add( _enemyMove.EnemyAnimation.WaitFindPlayerTriggerAnimationExit(ct));
        await UniTask.WhenAll(tasks);
    }

    public override async UniTask<EnemyState> Exit(EnemyState enemy,CancellationToken ct)
    {
        //プレイヤーを追いかける処理に移行
        return EnemyState.MovePlayer;
    }
}

using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// ゴーストが次の移動ポイントの方向にその場で回転するステート
/// </summary>
public class GhostRotation : EnemyStateBase
{
    protected EnemyState _enemyState { get; set; }
    private Transform _thisTransform = null;

    public GhostRotation(IEnemyMove enemyMove)
    {
        _enemyMove = enemyMove;
        _thisTransform = enemyMove.transform;
        _enemyState = EnemyState.Rotation;
    }

    public override async UniTask Entry(EnemyState enemy, CancellationToken ct)
    {
    }

    public override async UniTask Do(EnemyState enemy, CancellationToken ct)
    {
        var targetTransform = _enemyMove.GetNextPosition();
        var ratio = 0.0f;
        Vector3 targetDirection = targetTransform.position - _thisTransform.position;
        targetDirection.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion currentRotation = _thisTransform.rotation;
        //回転方向を計算する
        var q = Quaternion.Lerp(_thisTransform.rotation, targetRotation, 1f);
        var diffRotation = Quaternion.Inverse(q) * _thisTransform.rotation;

        Vector3 axis = Vector3.zero;
        float angle = 0;
        diffRotation.ToAngleAxis(out angle, out axis);
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
            while (ratio < _enemyMove.RotationSpeed * (angle / 90))
            {
                ratio += Time.deltaTime;

                Debug.Log($"ratio: {ratio}");
                // * (angle / 90)
                _thisTransform.rotation =
                    Quaternion.Lerp(currentRotation, targetRotation, ratio / (_enemyMove.RotationSpeed * (angle / 90)));

                await UniTask.Yield(cancellationToken: ct);
            }
        });
        //task[1] = _enemyMove.EnemyAnimation.WaitTurnTriggerAnimationExit(ct);

        await UniTask.WhenAll(task);
    }

    public override async UniTask<EnemyState> Exit(EnemyState enemy, CancellationToken ct)
    {
        return EnemyState.Move;
    }
}
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// プレイヤーを追いかけるステート
/// ゴーストをプレイヤーが見失うことは想定していない
/// </summary>
public class GhostMovePlayer: EnemyStateBase
{
    protected  EnemyState _enemyState { get; set; }
    private Player _player;
    private Transform _thisTransform = null;
    protected Rigidbody _rigidbody = null;
    private float _f = 0f;

    public GhostMovePlayer(IEnemyMove enemyMove)
    {
        _enemyMove = enemyMove;
        _thisTransform = enemyMove.transform;
        _enemyState = EnemyState.MovePlayer;
        _rigidbody = _enemyMove.GetRigidbody();
        _player = Player.Instance;
    }
    
    public override async UniTask Entry(EnemyState enemy,CancellationToken ct)
    {
        _enemyMove.EnemyAnimation.SetBool(GhostAnimationType.Run, true);
    }

    public override async UniTask Do(EnemyState enemy,CancellationToken ct)
    {
        var distance = _enemyMove.CaptureDistance;
        bool whileBreak = false;
        var targetTransform = Player.Instance?.transform;

        if(targetTransform is null) { return; }
        while(!whileBreak)
        {
            // プレイヤーの方向を向くように常に修正
            Vector3 targetDirection = targetTransform.position - _thisTransform.position;
            targetDirection.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            _thisTransform.rotation = Quaternion.Lerp(_thisTransform.rotation, targetRotation,1);
            
            // プレイヤーの方向に移動
            var v = _player.transform.position - _thisTransform.position;
            _rigidbody.MovePosition(_thisTransform.position + v.normalized * _enemyMove.ChaseMoveSpeed * Time.deltaTime);
            
            _f += Time.deltaTime;
            // SEを一定間隔で再生
            if (_f > 10f)
            {
                SoundManager.Instance.PlaySE(SEType.GhostVoice05,_thisTransform);
                _f = 0f;
            }
            await UniTask.Yield(ct);
            // 判定更新
            // 見失ったときの判定処理はEnemyGhostにあるので
            // ここではプレイヤーが捕獲範囲内にいるかどうかの判定のみ
            if (Vector3.Distance(_player.transform.position, _enemyMove.transform.position) < distance)
            {
                whileBreak = true;
            }
        }
        
    }

    public override async UniTask<EnemyState> Exit(EnemyState enemy,CancellationToken ct)
    {
        //プレイヤーへの攻撃演出へ移行
        return EnemyState.Attack;
    }
}

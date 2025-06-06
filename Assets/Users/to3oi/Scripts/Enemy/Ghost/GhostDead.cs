using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GhostDead : EnemyStateBase
{
    //todo:コードの内容すべて書き換え
    //2024/06/22 GhostMoveからコピペのみ
    protected EnemyState _enemyState { get; set; }
     
         protected Rigidbody _rigidbody = null;
         protected Transform _thisTransform = null;
     
         private float _distance = 0.1f;
         private float _moveSpeed = 1.0f;
     
         public GhostDead(IEnemyMove enemyMove)
         {
             _enemyMove = enemyMove;
             _enemyState = EnemyState.Move;
             _rigidbody = _enemyMove.GetRigidbody();
             _thisTransform = enemyMove.transform;
         }
     
         public override async UniTask Entry(EnemyState enemy, CancellationToken ct)
         {
             _enemyState = enemy;
         }
     
         public override async UniTask Do(EnemyState enemy, CancellationToken ct)
         {
             Debug.Log("GhostMove");
             //TODO:この座標へ移動する
             var t = _enemyMove.GetNextPosition();
             var v = t.position - _thisTransform.position;
             v.y = 0;
             while (Vector3.Distance(t.position, _thisTransform.position) > _distance)
             {
                 _rigidbody.MovePosition(_thisTransform.position + v.normalized * _moveSpeed * Time.deltaTime);
                 await UniTask.Yield(cancellationToken: ct);
             }
             Debug.Log("Move to " + _enemyMove.GetNextPosition().name);
         }
     
         public override async UniTask<EnemyState> Exit(EnemyState enemy, CancellationToken ct)
         {
             return EnemyState.Stay;
         }
}

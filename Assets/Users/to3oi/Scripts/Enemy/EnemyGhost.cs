using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EnemyGhost : IEnemyMove
{
    private Transform _nextTargetPosition;

    private CancelToken _cancelToken;

    //ステートのクラスを管理
    private Dictionary<EnemyState, EnemyStateBase> _enemyStateDic;

    private Player _player;
    private bool _isPlayerFound = false;
    private bool _isForceLookPlayer = false;

    private void Awake()
    {
        base.Awake();
        _player = Player.Instance;
        _rigidbody = GetComponent<Rigidbody>();
        //ステートの初期化
        _enemyStateDic =
            new Dictionary<EnemyState, EnemyStateBase>()
            {
                /*----------------------------------------------------------------------*/
                // ゴーストの移動処理に必要なステート
                { EnemyState.None, new GhostNone(this) },
                { EnemyState.Init, new GhostInit(this) },
                { EnemyState.Stay, new GhostStay(this) },
                { EnemyState.FindPlayer, new GhostFindPlayer(this) },
                { EnemyState.Rotation, new GhostRotation(this) },
                { EnemyState.Move, new GhostMove(this) },
                /*----------------------------------------------------------------------*/

                /*----------------------------------------------------------------------*/
                // ゴーストがプレイヤーを追いかける時に必要なステート

                { EnemyState.DiscoveryPlayer, new GhostDiscoveryPlayer(this) },
                { EnemyState.MovePlayer, new GhostMovePlayer(this) },

                { EnemyState.ChaseDiscoveryPlayer, new GhostChaseDiscoveryPlayer(this) },

                { EnemyState.Attack, new GhostAttack(this) },
                { EnemyState.Dead, new GhostDead(this) },
                
                { EnemyState.LostPlayer, new GhostLostPlayer(this) },
                { EnemyState.ReturnPosition, new GhostReturnPosition(this) }
                /*----------------------------------------------------------------------*/
            };
    }

    public override void StartEnemy()
    {
        //初期化処理
        _cancelToken = new CancelToken();
        _enemyState = EnemyState.Init;
        //敵の移動開始
        Flow(_cancelToken.GetToken()).Forget();
    }


    private async UniTask Flow(CancellationToken ct)
    {
        while (true)
        {
            if (ct.IsCancellationRequested) { break; }

            var state =  GetState(_enemyState);
            if(state == null){ break; }
            await state.Entry(_enemyState, _cancelToken.GetToken());
            
            if (ct.IsCancellationRequested) { break; }

            state =  GetState(_enemyState);
            if(state == null){ break; }
            await state.Do(_enemyState, _cancelToken.GetToken());
            if (ct.IsCancellationRequested)
            { break; }

            state =  GetState(_enemyState);
            if(state == null){ break; }

            _enemyState = await state.Exit(_enemyState, _cancelToken.GetToken());
            if (_enemyState == EnemyState.FlowBreak || ct.IsCancellationRequested)
            {
                Debug.Log("Break");
                Debug.Log("todo:ゲームオーバー処理");
                //todo:ゲームオーバー処理

                InGameFlow.Instance.AttackedFlag();

                EnemyAnimation.ResetAll();
                break;
            }
        }
    }

    private EnemyStateBase GetState(EnemyState state)
    {
        var stateBase = _enemyStateDic[state];
        if (stateBase == null)
        {
            Debug.Log($"{name}にState:[{state}]が登録されていません");
        }

        return stateBase;
    }

    private async void Update()
    {
        return;
        if (_isForceLookPlayer)
        {
            return;
        }

        Debug.DrawRay(_head.position, (_player.Head.position - _head.position).normalized, Color.red, 0.1f);
        //プレイヤーを発見したときの割り込み処理
        if (Vector3.Distance(_player.transform.position, transform.position) < PlayerFindDistance &&
            _isPlayerFound == false)
        {
            if (Physics.Raycast(_head.position, (_player.Head.position - _head.position).normalized,
                    out var hit, float.PositiveInfinity))
            {
                if (!hit.collider.gameObject.CompareTag("Player"))
                {
                    return;
                }

                Debug.Log($"hit {hit.collider.gameObject.name}");
                Debug.Log($"割り込み");

                _isPlayerFound = true;

                //プレイヤーを発見したときの割り込み処理

                //ステートを変更
                ForceStateUpdate(EnemyState.DiscoveryPlayer);
                SetExtraNextState(EnemyState.LostPlayer);
                IsPlayerFound = true;

                // 連続で割り込みが入らないように適当な秒数待機
                await UniTask.Delay(TimeSpan.FromSeconds(2));
            }
        }
        //プレイヤーを見失ったときの割り込み処理
        else if (PlayerFindDistance < Vector3.Distance(_player.transform.position, transform.position) &&
                 _isPlayerFound == true)
        {
            _isPlayerFound = false;
            Debug.Log($"割り込み解除");

            //プレイヤーを見失ったときの処理
            ForceStateUpdate(EnemyState.LostPlayer);
            IsPlayerFound = false;

            // 連続で割り込みが入らないように適当な秒数待機
            await UniTask.Delay(TimeSpan.FromSeconds(2));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //プレイヤーを発見したときの割り込み処理
        if (other.CompareTag("Player"))
        {
            //現在の処理を停止して、プレイヤーを発見した状態に遷移
            _cancelToken.Cancel();
            _cancelToken.ReCreate();
            _enemyState = EnemyState.DiscoveryPlayer;
            SetExtraNextState(EnemyState.LostPlayer);
            Flow(_cancelToken.GetToken()).Forget();
        }
    }

    private void ForceStateUpdate(EnemyState state)
    {
        _cancelToken.Cancel();
        _cancelToken.ReCreate();
        _enemyState = state;
        Flow(_cancelToken.GetToken()).Forget();
    }

    public void InitForceLookPlayer(Transform position)
    {
        gameObject.transform.position = position.position;
        gameObject.SetActive(true);
        
        //EnemyAnimation.SetBool(GhostAnimationType.Move,true);
    }
    public void ForceLookPlayer(Transform position)
    {
        gameObject.transform.position = position.position;
        
        //プレイヤーの方向を向かせる
        Vector3 targetDirection = _player.Position - transform.position;
        targetDirection.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = targetRotation;
        
        ForceStateUpdate(EnemyState.ChaseDiscoveryPlayer);
        SetExtraNextState(EnemyState.LostPlayer);
        _isForceLookPlayer = true;
    }

    public void ForceStop()
    {
        _cancelToken?.Cancel();
        _cancelToken?.ReCreate();
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _cancelToken?.Cancel();
    }
}
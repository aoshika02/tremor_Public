using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class IEnemyAnimation : MonoBehaviour
{
    [SerializeField] protected Animator _animator;
    protected Dictionary<Enum, AnimationData> _animatorHash = new Dictionary<Enum, AnimationData>();
    protected CancelToken _cancelToken = new CancelToken();
    protected ObservableStateMachineTrigger[] _animatorTriggers;
    private readonly int Random = Animator.StringToHash("Random");
    private readonly int Type = Animator.StringToHash("Type");
    bool _isTurnTriggerAnimationExit = false;
    bool _isSearchTriggerAnimationExit = false;
    bool _isFindPlayerTriggerAnimationExit = false;
    bool _isAttackAnimationExit = false;

    protected virtual void Start()
    {
        SetTriggers(_cancelToken.GetToken());
    }

    protected virtual void OnEnable()
    {
        _cancelToken.ReCreate();
        SetTriggers(_cancelToken.GetToken());
    }

    protected void OnDisable()
    {
        _cancelToken.Cancel();
    }

    protected virtual void SetTriggers(CancellationToken ct)
    {
        _animatorTriggers = _animator.GetBehaviours<ObservableStateMachineTrigger>();

        foreach (var animatorTrigger in _animatorTriggers)
        {
            animatorTrigger.OnStateExitAsObservable()
                .Subscribe(state =>
                {
                    if (state.StateInfo.IsName("TurnTriggerAnimationExit"))
                    {
                        Debug.Log("TurnTriggerAnimationExit");

                        _isTurnTriggerAnimationExit = true;
                    }

                    if (state.StateInfo.IsName("SearchTriggerAnimationExit"))
                    {
                        Debug.Log("SearchTriggerAnimationExit");
                        _isSearchTriggerAnimationExit = true;
                    }

                    if (state.StateInfo.IsName("FindPlayerTriggerAnimationExit"))
                    {
                        Debug.Log("FindPlayerTriggerAnimationExit");

                        _isFindPlayerTriggerAnimationExit = true;
                    }
                    if (state.StateInfo.IsName("AttackAnimationExit"))
                    {
                        Debug.Log("AttackAnimationExit");

                        _isAttackAnimationExit = true;
                    }
                }).AddTo(ct);
        }
    }
    public virtual void SetTrigger(Enum type)
    {
        UpdateRandom(type);
        _animator.SetTrigger(_animatorHash[type].Hash);
    }

    public virtual void ResetTrigger(Enum type)
    {
        _animator.ResetTrigger(_animatorHash[type].Hash);
    }

    public virtual void SetBool(Enum type, bool value)
    {
        UpdateRandom(type);
        _animator.SetBool(_animatorHash[type].Hash, value);
    }

    public virtual void SetFloat(Enum type, float value)
    {
        UpdateRandom(type);
        _animator.SetFloat(_animatorHash[type].Hash, value);
    }

    public virtual void SetInt(Enum type, int value)
    {
        UpdateRandom(type);
        _animator.SetInteger(_animatorHash[type].Hash, value);
    }

    public virtual void ResetAll()
    {
    }
    public virtual void ResetAllTrigger()
    {
    }

    private void UpdateRandom(Enum type)
    {
        if (_animatorHash[type].RandomRange != 0)
        {
            _animator.SetInteger(Random, UnityEngine.Random.Range(0, _animatorHash[type].RandomRange));
        }
    }

    public virtual async UniTask WaitTurnTriggerAnimationExit(CancellationToken ct = default)
    {
        await UniTask.WaitUntil(() => _isTurnTriggerAnimationExit, cancellationToken: ct);
        _isTurnTriggerAnimationExit = false;
    }

    public virtual async UniTask WaitSearchTriggerAnimationExit(CancellationToken ct = default)
    {
        await UniTask.WaitUntil(() => _isSearchTriggerAnimationExit, cancellationToken: ct);
        _isSearchTriggerAnimationExit = false;
    }

    public virtual async UniTask WaitFindPlayerTriggerAnimationExit(CancellationToken ct = default)
    {
        await UniTask.WaitUntil(() => _isFindPlayerTriggerAnimationExit, cancellationToken: ct);
        _isFindPlayerTriggerAnimationExit = false;
    }
    
    public virtual async UniTask WaitAttackAnimationExit(CancellationToken ct = default)
    {
        await UniTask.WaitUntil(() => _isAttackAnimationExit, cancellationToken: ct);
        _isAttackAnimationExit = false;
    }
}
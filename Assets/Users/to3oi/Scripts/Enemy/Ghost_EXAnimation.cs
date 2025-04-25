using UnityEngine;

public class Ghost_EXAnimation : IEnemyAnimation
{
    private void Awake()
    {
        _animatorHash.Add(
            GhostAnimationType.Idle,
            new AnimationData()
            {
                Hash = Animator.StringToHash("Idle"),
                RandomRange = 2
            });
        _animatorHash.Add(
            GhostAnimationType.Move,
            new AnimationData()
            {
                Hash = Animator.StringToHash("Move")
            });
        _animatorHash.Add(
            GhostAnimationType.Run,
            new AnimationData()
            {
                Hash = Animator.StringToHash("Run")
            });
        _animatorHash.Add(GhostAnimationType.SearchType1,
            new AnimationData()
            {
                Hash = Animator.StringToHash("SearchType1")
            });
        _animatorHash.Add(GhostAnimationType.SearchType2,
            new AnimationData()
            {
                Hash = Animator.StringToHash("SearchType2")
            });
        _animatorHash.Add(GhostAnimationType.FindPlayer,
            new AnimationData()
            {
                Hash = Animator.StringToHash("FindPlayer")
            });
        _animatorHash.Add(GhostAnimationType.TurnRight,
            new AnimationData()
            {
                Hash = Animator.StringToHash("TurnRight")
            });
        _animatorHash.Add(GhostAnimationType.TurnLeft,
            new AnimationData()
            {
                Hash = Animator.StringToHash("TurnLeft")
            });
        _animatorHash.Add(GhostAnimationType.Attack1,
            new AnimationData()
            {
                Hash = Animator.StringToHash("Attack1")
            });
        _animatorHash.Add(GhostAnimationType.Attack2,
            new AnimationData()
            {
                Hash = Animator.StringToHash("Attack2")
            });
    }

    public override void ResetAll()
    {
        SetBool(GhostAnimationType.Move,false);
        SetBool(GhostAnimationType.Run,false);
        
        
        ResetTrigger(GhostAnimationType.SearchType1);
        ResetTrigger(GhostAnimationType.SearchType2);
        ResetTrigger(GhostAnimationType.TurnRight);
        ResetTrigger(GhostAnimationType.TurnLeft);
        ResetTrigger(GhostAnimationType.Attack1);
        ResetTrigger(GhostAnimationType.Attack2);
    } 
    public override void ResetAllTrigger()
    {
        ResetTrigger(GhostAnimationType.SearchType1);
        ResetTrigger(GhostAnimationType.SearchType2);
        ResetTrigger(GhostAnimationType.TurnRight);
        ResetTrigger(GhostAnimationType.TurnLeft);
        ResetTrigger(GhostAnimationType.Attack1);
        ResetTrigger(GhostAnimationType.Attack2);
    }
}

public enum GhostAnimationType
{
    Idle,
    Move,
    Run,
    SearchType1,
    SearchType2,
    FindPlayer,
    TurnRight,
    TurnLeft,
    Attack1,
    Attack2,
}

public class AnimationData
{
    /// <summary>
    /// AnimationHash
    /// </summary>
    public int Hash = 0;
    /// <summary>
    /// ランダムの範囲 
    /// </summary>
    public int RandomRange = 0;
}
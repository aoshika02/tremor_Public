using UnityEngine;

public class WallFall : SingletonMonoBehaviour<WallFall>
{
    [SerializeField] private Animator _firstWallAnimator;
    [SerializeField] private Animator _entranceSideWallAnimator;
    [SerializeField] private Animator _classroomSideWallAnimator;

    private bool _entranceSideWall = false;
    private bool _classroomSideWall = false;
    
    public void ActFirstWallFall()
    {
        InGameFlow _inGameFlow = InGameFlow.Instance;
        if (!_inGameFlow.IsFirstWall)
        {
            _inGameFlow.FirstWallFallFlag();
            _firstWallAnimator.SetTrigger("isFall");
            SoundManager.Instance.PlaySE(SEType.door_don,transform);
        }
    }
    public void ActEntranceSideWallFall()
    {
        InGameFlow _inGameFlow = InGameFlow.Instance;
        MapLocation _mapLocation = MapLocation.Instance;
        if (!_inGameFlow.IsSecondtWall)
        {
            _entranceSideWall = true;
            _inGameFlow.SecondWallFallFlag();
            _entranceSideWallAnimator.SetTrigger("isFall");
            _mapLocation.EntranceWallAlpha(1);
            SoundManager.Instance.PlaySE(SEType.door_don,transform);
        }
        else if (_inGameFlow.IsGetCursedItem && _classroomSideWall) 
        {
            _inGameFlow.SecondWallFallFlag();
            _entranceSideWallAnimator.SetTrigger("isFall");
            _mapLocation.EntranceWallAlpha(1);
            SoundManager.Instance.PlaySE(SEType.door_don, transform);
        }
    }
    public void ActClassroomSideWallFall()
    {
        InGameFlow _inGameFlow = InGameFlow.Instance;
        MapLocation _mapLocation = MapLocation.Instance;
        if (!_inGameFlow.IsSecondtWall)
        {

            _classroomSideWall = true;
            _inGameFlow.SecondWallFallFlag();
            _classroomSideWallAnimator.SetTrigger("isFall");
            _mapLocation.ClassRoomWallAlpha(1);
            SoundManager.Instance.PlaySE(SEType.door_don, transform);
        }
        else if (_inGameFlow.IsGetCursedItem && _entranceSideWall)
        {
            _inGameFlow.SecondWallFallFlag();
            _classroomSideWallAnimator.SetTrigger("isFall");
            _mapLocation.ClassRoomWallAlpha(1);
            SoundManager.Instance.PlaySE(SEType.door_don, transform);
        }
    }


}

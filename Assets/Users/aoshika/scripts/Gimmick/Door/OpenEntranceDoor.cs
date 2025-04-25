public class OpenEntranceDoor : OpenDoor
{
    public override void DoorOpenAction(float x)
    {
        //if(!InGameFlow.Instance.IsGetCursedItem)return;
        if(InGameFlow.Instance.IsWaitGoal is false)return;
        if (IsDoorOpen(x))
        {
            InGameFlow.Instance.GoalFlag();
        }
    }
}

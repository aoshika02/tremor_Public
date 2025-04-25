public class OpenScienceRoomDoor : OpenDoor
{
    public override void DoorOpenAction(float x)
    {
        if (!TutorialFlowManager.Instance.IsTutorialFinish) return;
        if (IsDoorOpen(x))
        {
            _callOnTrigger?.ActionComplete();
            _callOnTrigger?.RemoveActionButtonUIViewer();
        }
    }
}

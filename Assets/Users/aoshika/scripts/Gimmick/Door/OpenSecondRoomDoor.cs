public class OpenSecondRoomDoor : OpenDoor
{
    private bool _isOutRoom = false;
    public override void DoorOpenAction(float x)
    {
        if (!TutorialFlowManager.Instance.IsTutorialFinish) return;
        if (IsDoorOpen(x))
        {
            if (_isOutRoom) return;
            _isOutRoom = true;
            _callOnTrigger?.ActionComplete();
            _callOnTrigger?.RemoveActionButtonUIViewer();
        }
    }
}

public class OpenTutorialSecondDoor : OpenDoor
{
    private bool _isOutRoom = false;
    public override void DoorOpenAction(float x)
    {
        if (!TutorialFlowManager.Instance.IsGetNewsPaper) return;
        if (IsDoorOpen(x))
        {
            if (_isOutRoom) return;
            _isOutRoom = true;
            _callOnTrigger?.RemoveActionButtonUIViewer();
            _callOnTrigger?.ActionComplete();
        }
    }
}

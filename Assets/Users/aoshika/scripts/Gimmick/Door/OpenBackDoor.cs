public class OpenBackDoor : OpenDoor
{
    private bool _inToSchool = false;
    public override void DoorOpenAction(float x)
    {
        //if(!TutorialTablet.Instance.IsFinishTutorial)return;
        if(!TutorialFlowManager.Instance.IsGetTablet) return;
        if (IsDoorOpen(x))
        {
            if (_inToSchool) return;
            _inToSchool = true;
            _callOnTrigger?.ActionComplete();
            _callOnTrigger?.RemoveActionButtonUIViewer();

            TutorialFlowManager.Instance.OpenFirstDoorFlag();
            //InGameFlow.Instance.StartInGame();
        }
    }
}

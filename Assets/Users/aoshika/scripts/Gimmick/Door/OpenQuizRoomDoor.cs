public class OpenQuizRoomDoor : OpenDoor
{
    public override void DoorOpenAction(float x)
    {
        if (!TutorialFlowManager.Instance.IsTutorialFinish) return;
        if (VibrationQuiz.Instance.IsQuizFlow) return;
        if (IsDoorOpen(x))
        {
            if (VibrationQuiz.Instance.IsQuizFinish is false)
            {
                _callOnTrigger?.RemoveActionButtonUIViewer();
                if (_callOnTrigger == null) return;
                _callOnTrigger.isUiView = false;
                return;
            }
            else
            {
                _callOnTrigger?.ActionComplete();
                _callOnTrigger?.RemoveActionButtonUIViewer();
            }
        }
    }
    public void DoorUIView() 
    {
        if (_callOnTrigger == null) return;
        _callOnTrigger.isUiView = true;
    }
}

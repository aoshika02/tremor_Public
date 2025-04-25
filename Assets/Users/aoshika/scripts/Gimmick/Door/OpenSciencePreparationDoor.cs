/// <summary>
/// 理科準備室のドア
/// </summary>
public class OpenSciencePreparationDoor : OpenDoor
{
    private bool _isEventEntry = false;

    public override async void DoorOpenAction(float x)
    {
        if (!TutorialFlowManager.Instance.IsTutorialFinish) return;
        if (_isEventEntry)
        {
            return;
        }

        if (x != 1 || _isDoor is false)
        {
            return;
        }
        
        if (PreparationRoomManager.Instance.IsHoleCheck == false)
        {
            _isEventEntry = true;
            PlayerMove.Instance.SetMove(false);
            await MessageViewEvent.Instance.ViewText(MessageEventName.PreparationRoom_NotPeek);
            PlayerMove.Instance.SetMove(true);
            _isEventEntry = false;
        }
        //鍵取得してないフラグ
        else if (MusicRoomManager.Instance.GetKey == false)
        {
            _isEventEntry = true;
            PlayerMove.Instance.SetMove(false);
            await MessageViewEvent.Instance.ViewText(MessageEventName.PreparationRoom_NotGetGey);
            PlayerMove.Instance.SetMove(true);
            _isEventEntry = false;
        }
        else if (IsDoorOpen(x))
        {
            GetItemIconViewManager.Instance.RemoveItemView(ItemEventName.MusicRoom_GetKey);
            _callOnTrigger?.ActionComplete();
            _callOnTrigger?.RemoveActionButtonUIViewer();
        }
    }
}
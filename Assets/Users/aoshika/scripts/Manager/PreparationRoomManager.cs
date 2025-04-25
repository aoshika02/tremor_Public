using Cysharp.Threading.Tasks;
using System.Threading;

public class PreparationRoomManager : SingletonMonoBehaviour<PreparationRoomManager>
{
    public bool IsHoleCheck { get; private set; } = false;
    private bool _isRadioCheck = false;
    public bool IsRadioCheck => _isRadioCheck;
    private bool _isTapeCheck = false;
    public void StartPreparationRoomFlow(CancellationToken ct) 
    {
        PreparationRoomFlow(ct).Forget();
    }
    private async UniTask PreparationRoomFlow(CancellationToken ct)
    {
        _isRadioCheck = false;
        _isTapeCheck = false;
        await UniTask.WaitUntil(() => _isRadioCheck && _isTapeCheck, cancellationToken: ct);
        SonarConverter.Instance.HideSonar(SonarType.PreparationRoom);
    }
    public void HoleCheckFlag()
    {
        IsHoleCheck = true;
    }
    public async UniTask RadioCheckFlag()
    {
        _isRadioCheck = true;
        
        if (MusicRoomManager.Instance.SearchRadio)
        {
            PlayerMove.Instance.SetMove(false);
            await MessageViewEvent.Instance.ViewText(MessageEventName.RadioChecked);
            PlayerMove.Instance.SetMove(true);
        }
    }
    public void TapeCheckFlag()
    {
        _isTapeCheck = true;
    }
}
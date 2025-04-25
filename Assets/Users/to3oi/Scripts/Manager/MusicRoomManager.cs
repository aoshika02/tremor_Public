using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MusicRoomManager : SingletonMonoBehaviour<MusicRoomManager>
{
    [SerializeField] private GameObject _musicRoomExitGameObject;
    public bool SearchRadio => _searchRadio;
    private bool _searchRadio = false;
    public bool GetKey => _getKey;
    private bool _getKey = false;

    private void Start()
    {
        _musicRoomExitGameObject.SetActive(false);
        StartFlow(destroyCancellationToken).Forget();
    }

    public async UniTask StartFlow(CancellationToken ctx)
    {
        await UniTask.WhenAll(
            UniTask.WaitUntil(() => GetKey, cancellationToken: ctx),
            UniTask.WaitUntil(() => SearchRadio, cancellationToken: ctx));
        
        _musicRoomExitGameObject.SetActive(true);
        SonarConverter.Instance.HideSonar(SonarType.MusicRoom);
    }

    public async UniTask SearchRadioFlag()
    {
        _searchRadio = true;
        if (PreparationRoomManager.Instance.IsRadioCheck)
        {
            PlayerMove.Instance.SetMove(false);
            await MessageViewEvent.Instance.ViewText(MessageEventName.RadioChecked);
            PlayerMove.Instance.SetMove(true);
        }
    }

    public void GetKeyFlag()
    {
        _getKey = true;
    }
}
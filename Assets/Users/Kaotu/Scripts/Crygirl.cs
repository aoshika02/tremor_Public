using UnityEngine;
using UniRx;
using UnityEngine.Playables;

public class Crygirl : MonoBehaviour, OnTriggerEnterInterface, OnTriggerExitInterface
{
    [SerializeField] protected CallOnTrigger _callOnTrigger;
    [SerializeField] private PlayableDirector _girltimeline;
    [SerializeField] private Transform _crygirl;
    [SerializeField] private GameObject _sawHandleObj;
    SoundHash _soundHash;
    [SerializeField]
    private bool _girlcry = false;

    private bool _isEvent = false;
    void Start()
    {
        InputManager.Instance.Decision.Subscribe(Girl).AddTo(this);
        

    }
    public void CallOnTriggerEnter(Collider collider, CallOnTrigger _)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            _girlcry = true;
        }
    }

    public void CallOnTriggerExit(Collider collider, CallOnTrigger _)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            _girlcry = false;
        }
    }

    //チュートリアルフローで呼び出して
    public void GirlSound()
    {
        _soundHash = SoundManager.Instance.PlayBGM(BGMType.GirlCryBGM,_crygirl, volume: 0.05f);
    }
    public async void Girl(float x)
    {
        if (x == 1 && _girlcry == true && _isEvent is false)
        {
            _isEvent = true;
            PlayerMove.Instance.SetMove(false);
            await SoundManager.Instance.StopBGM(_soundHash);
            _callOnTrigger?.ActionComplete();
            _callOnTrigger?.RemoveActionButtonUIViewer();
            // 女の子初回テキスト表示
            await MessageViewEvent.Instance.ViewText(MessageEventName.CryGirlBefore);
            await _girltimeline.PlayAsync();
            // 女の子消失テキスト表示
            await MessageViewEvent.Instance.ViewText(MessageEventName.CryGirlAfter);
            await PlayerMove.Instance.SetPlayerRotation(_sawHandleObj.transform, 1.5f);
            await GetItemEvent.Instance.ViewItem(ItemEventName.Saw_Hand);
            _sawHandleObj.SetActive(false);
            PlayerMove.Instance.SetMove(true);
            InGameFlow.Instance.GetSawHandleFlag();
            Destroy(gameObject);
            SonarConverter.Instance.HideSonar(SonarType.ClassRoom2);
        }
        else
        {
            return;
        }
    }
}

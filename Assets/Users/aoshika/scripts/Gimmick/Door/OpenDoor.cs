using UnityEngine;
using UniRx;

[RequireComponent(typeof(Animator))]
public class OpenDoor : MonoBehaviour,OnTriggerEnterInterface,OnTriggerExitInterface
{
    protected bool _isDoor = false;
    private Animator _anim;

    [SerializeField] protected CallOnTrigger _callOnTrigger;
    void Start()
    {
        _anim = GetComponent<Animator>();
        InputManager.Instance.Decision.Subscribe(DoorOpenAction).AddTo(this);
    }

    public void CallOnTriggerEnter(Collider collider, CallOnTrigger _)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            _isDoor = true;
        }
    }

    public void CallOnTriggerExit(Collider collider, CallOnTrigger _)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            _isDoor = false;
        }
    }

    public virtual void DoorOpenAction(float x)
    {
        if (x == 1 && _isDoor)
        {
            _anim.SetTrigger("isOpen");
            _callOnTrigger?.ActionComplete();
            _callOnTrigger?.RemoveActionButtonUIViewer();
        }
        else
        {
            return;
        }
    }
    protected bool IsDoorOpen(float x)
    {
        if (x == 1 && _isDoor)
        {
            _anim.SetTrigger("isOpen");
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public void SetIsActionBlock(bool isActionBlock)
    {
        _callOnTrigger.isActionBlock = isActionBlock;
    }
}

using UnityEngine;

public class AreaEventBase : MonoBehaviour
{
    protected Vector3 _lastFramePlayerMoveDirection = Vector3.zero;
    protected float _returnDistance = 0.5f;
    protected bool _cleared = false;
    protected bool _isEventStart = false;
    protected CancelToken _eventCancelToken = new CancelToken();

    private Player _player;

    private void Start()
    {
        _player = Player.Instance;
    }

    protected void OnTriggerEnter(Collider collider)
    {
        if(!_isEventStart){return; }
        if(_cleared) { return;}
        
        if (collider.transform.parent.TryGetComponent<Player>(out _))
        {
            PlayerEnterEvent();
            _lastFramePlayerMoveDirection = _player.MoveDirection;
        }
    }
    
    protected virtual void PlayerEnterEvent() {}
    
    public virtual void ExitEvent()
    {
        _player.UpdatePosition(_player.Position + -_lastFramePlayerMoveDirection * _returnDistance);
    }

    public virtual void ClearEvent()
    {
        _eventCancelToken.Cancel();
        _cleared = true;
    }

    public virtual void StartEvent()
    {
        _eventCancelToken.ReCreate();
        _isEventStart = true;
    }
}
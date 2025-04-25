using UnityEngine;
using TNRD;
using Cysharp.Threading.Tasks;
using System.Linq;

public class CallOnTrigger : MonoBehaviour
{
    [SerializeField] private SerializableInterface<OnTriggerEnterInterface> enter;
    [SerializeField] private SerializableInterface<OnTriggerStayInterface> stay;
    [SerializeField] private SerializableInterface<OnTriggerExitInterface> exit;
    private ActionButtonUIViewer _actionButtonUIViewer = null;
    [SerializeField] private Transform[] _actionButtonUIViewerTransform;

    public bool isUiView = false;
    public bool isActionBlock = false;
    public bool isActionComplete = false;

    private async UniTask OnTriggerEnter(Collider other)
    {
        if (isActionComplete)
        {
            return;
        }

        if (other.gameObject.CompareTag("Player"))
        {
            if (isUiView)
            {
                _actionButtonUIViewer = ActionButtonUIViewerManager.Instance.GetActionButtonUIViewer();
                _actionButtonUIViewer.SetData(GetActionButtonTransform(), isActionBlock).Forget();
            }

            enter?.Value?.CallOnTriggerEnter(other, this);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isActionComplete)
        {
            return;
        }
        if (other.gameObject.CompareTag("Player"))
        {
            stay?.Value?.CallOnTriggerStay(other, this);
        }
    }

    private async UniTask OnTriggerExit(Collider other)
    {
        if (isActionComplete)
        {
            return;
        }

        if (other.gameObject.CompareTag("Player"))
        {
            if (isUiView)
            {
                _actionButtonUIViewer?.Release().Forget();
                _actionButtonUIViewer = null;
            }

            exit?.Value?.CallOnTriggerExit(other, this);
        }
    }

    private Transform GetActionButtonTransform()
    {
        return _actionButtonUIViewerTransform
            .OrderBy(t => Vector3.Distance(t.transform.position, Player.Instance.transform.position))
            .FirstOrDefault();
    }
    public void AddActionButtonUIViewer()
    {
        if (isUiView)
        {
            _actionButtonUIViewer = ActionButtonUIViewerManager.Instance.GetActionButtonUIViewer();
            _actionButtonUIViewer.SetData(GetActionButtonTransform(), isActionBlock).Forget();
        }
    }
    public void ActionComplete()
    {
        isActionComplete = true;
    }
    public void RemoveActionButtonUIViewer()
    {
        // if (isUiView)
        //isUiViewをfalseにするタイミングによってはReleaseされないのでisUiViewを見ない
        {
            _actionButtonUIViewer?.Release().Forget();
            _actionButtonUIViewer = null;
        }
    }
}
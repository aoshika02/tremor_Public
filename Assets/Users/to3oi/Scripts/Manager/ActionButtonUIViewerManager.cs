using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ActionButtonUIViewerManager : SingletonMonoBehaviour<ActionButtonUIViewerManager>
{
    private List<ActionButtonUIViewer> _actionButtonUIViewers = new List<ActionButtonUIViewer>();
    [SerializeField] private GameObject _actionButtonUIViewerInstance;
    [SerializeField] private Canvas _canvas;
    private RectTransform _canvasRTF;
    private Transform _actionButtonUIViewerRoot; 
    
    private void Start()
    {
        _canvasRTF = _canvas.GetComponent<RectTransform>();
        _actionButtonUIViewerRoot = new GameObject("ActionButtonUIViewerManager").transform;
        _actionButtonUIViewerRoot.parent = _canvas.transform;
        _actionButtonUIViewerRoot.localPosition = Vector3.zero;
        
        // オブジェクトプールを初期化
        for (int i = 0; i < 3; i++)
        {
            _actionButtonUIViewers.Add(CreateActionButtonUIViewer());
        }
    }

    public ActionButtonUIViewer GetActionButtonUIViewer()
    {
        for (int i = 0; i < _actionButtonUIViewers.Count; i++)
        {
            if (_actionButtonUIViewers[i].IsView == false)
            {
                return _actionButtonUIViewers[i];
            }
        }

        return CreateActionButtonUIViewer();
    }

    private ActionButtonUIViewer CreateActionButtonUIViewer()
    {
        // ActionButtonUIViewerを生成する実態
        var actionButtonUIViewer = Instantiate(_actionButtonUIViewerInstance,_actionButtonUIViewerRoot).GetComponent<ActionButtonUIViewer>();
        actionButtonUIViewer.Init(_canvasRTF);
        _actionButtonUIViewers.Add(actionButtonUIViewer);
        return actionButtonUIViewer;
    }

    public void AllRelease()
    {
        for (int i = 0; i < _actionButtonUIViewers.Count; i++)
        {
            if (_actionButtonUIViewers[i].IsView)
            {
                _actionButtonUIViewers[i].Release().Forget();
            } 
        }
    }
}
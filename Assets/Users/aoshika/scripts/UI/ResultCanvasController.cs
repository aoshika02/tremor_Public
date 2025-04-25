using Cysharp.Threading.Tasks;
using UnityEngine;

public class ResultCanvasController : SingletonMonoBehaviour<ResultCanvasController>
{
    [SerializeField] private Animator _resultCanvasAnimator;
    private bool _isClear = false;
    private bool _isOver = false;
    CancelToken _cancelToken = new CancelToken();

    public void StartResultView()
    {
        _cancelToken.GetToken();
        ResultView().Forget();
    }
    private async UniTask ResultView() 
    {
        await UniTask.WaitUntil(()=> _isClear||_isOver);
        _cancelToken.Cancel();
    }
    public void GameClearCanvas() 
    {
        _isClear = true;
        _resultCanvasAnimator.SetTrigger("GameClear");
    }
    public void GameOverCanvas() 
    {
        _isOver = true;
        _resultCanvasAnimator.SetTrigger("GameOver");
    }
}

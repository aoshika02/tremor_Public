using Cysharp.Threading.Tasks;

public class ChoicesGimmickManager : SingletonMonoBehaviour<ChoicesGimmickManager>
{
    private bool _isDecision = false;
    private bool _isCancel = false;
    /// <summary>
    /// 選択肢の分岐時の処理
    /// </summary>
    /// <returns>継続の場合true</returns>
    public async UniTask<bool> ContinueOrCancel()
    {
        _isDecision = false;
        _isCancel = false;

        await UniTask.WaitUntil(() => _isDecision || _isCancel);
        return _isDecision;
    }
    public void ContinueFlag()
    {
        _isDecision = true;
    }
    public void CancelFlag()
    {
        _isCancel = true;
    }
}

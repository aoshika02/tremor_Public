using System.Threading;
public class CancelToken
{
    private CancellationTokenSource _cancellationTokenSource;
    private CancellationToken _cancellationToken;

    public CancelToken()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _cancellationToken = _cancellationTokenSource.Token;
    }

    public CancellationToken GetToken()
    {
        return _cancellationToken;
    }

    public void Cancel()
    {
        _cancellationTokenSource.Cancel();
    }

    public void ReCreate()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        _cancellationToken = _cancellationTokenSource.Token;
    }
}

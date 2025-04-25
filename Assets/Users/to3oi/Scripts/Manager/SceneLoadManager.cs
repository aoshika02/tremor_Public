using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class SceneLoadManager : SingletonMonoBehaviour<SceneLoadManager>
{
    private List<SceneType> _loadingSceneTypes = new List<SceneType>();

    private List<(SceneType SceneType, AsyncOperation AsyncOperation)> _loadedSceneTypes =
        new List<(SceneType SceneType, AsyncOperation AsyncOperation)>();

    bool _call = true;

    protected override void Awake()
    {
        if (CheckInstance())
        {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
    }

    public async UniTask AddLoadScene(SceneType type)
    {
        // リストにtypeが存在していたら重複して読み込んでしまうので処理を終了
        if (_loadingSceneTypes.Any(x => x == type) ||
            _loadedSceneTypes.Any(x => x.SceneType == type))
        {
            return;
        }

        _loadingSceneTypes.Add(type);
        var sceneCount = SceneManager.loadedSceneCount;
        AsyncOperation loadSceneAsync;
        loadSceneAsync = SceneManager.LoadSceneAsync((int)type, LoadSceneMode.Additive);
        loadSceneAsync.allowSceneActivation = false;
        while (loadSceneAsync.progress < 0.9f)
        {
            await UniTask.Yield();
        }

        _loadingSceneTypes.Remove(type);
        _loadedSceneTypes.Add((type, loadSceneAsync));

        await UniTask.WaitUntil(() => sceneCount + 1 <= SceneManager.loadedSceneCount + _loadedSceneTypes.Count);
        await UniTask.Yield();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="loadedSceneType"></param>
    /// <param name="isFade">フェードイン、フェードアウト両方</param>
    /// <param name="isOutFade">フェードアウトのみ</param>
    public async UniTask LoadedSceneActive(SceneType loadedSceneType, bool isFade = false, bool isOutFade = false)
    {
        await UniTask.Yield();
        UniTask fadeInTask = new UniTask();
        if (isFade)
        {
            fadeInTask = FadeInOut.FadeIn();
        }

        // ロードが終わるまで待機
        while (_loadedSceneTypes.Any(x => x.SceneType != loadedSceneType))
        {
            // リストにloadedSceneTypeが存在していない場合読み込む必要がある
            if (_loadingSceneTypes.Any(x => x != loadedSceneType))
            {
                await AddLoadScene(loadedSceneType);
            }
            await UniTask.Yield();
        }

        var oldScene = SceneManager.GetActiveScene();

        if (isFade)
        {
            await fadeInTask;
        }

        var loadedContents = _loadedSceneTypes.First(x => x.SceneType == loadedSceneType);
        loadedContents.AsyncOperation.allowSceneActivation = true;

        await UniTask.WaitUntil(() => SceneManager.loadedSceneCount <= 2);
        await UniTask.Yield();

        await UniTask.WaitUntil(() => SceneManager.GetAllScenes().Any(scene => scene.buildIndex == (int)loadedSceneType));
        await UniTask.Yield();

        var scene = SceneManager.GetSceneByBuildIndex((int)loadedSceneType);
        await UniTask.WaitUntil(() => scene.isLoaded);
    
        await UniTask.WaitUntil(()=> SceneManager.SetActiveScene(scene));
        
        _loadedSceneTypes.Remove(loadedContents);
        
        await UniTask.Yield();

        // 念の為もう一度
        while (SceneManager.GetActiveScene().name  != scene.name)
        {
            await UniTask.WaitUntil(()=> SceneManager.SetActiveScene(scene));
            await UniTask.Yield();
        }
        
        await SceneManager.UnloadSceneAsync(oldScene,UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
        
        await UniTask.Yield();

        if (isFade || isOutFade)
        {
            await FadeInOut.FadeOut();
        }
    }

    // TODO:時間ができたらunloadも実装する
    /*public async UniTask UnloadScene(SceneType unloadSceneType)
    {
        // どちらかに登録されていたらunloadの準備をする
        if (_loadedSceneTypes.Any(x => x.SceneType == unloadSceneType) ||
            _loadingSceneTypes.Any(x => x == unloadSceneType))
        {
            // 読込中判定
            await UniTask.WaitWhile(() => _loadingSceneTypes.Any(x => x == unloadSceneType));

            // 読み込み済み判定 _loadedSceneTypesにloadedSceneTypeが入っているときに
            await UniTask.WaitUntil(() => _loadedSceneTypes.Any(x => x.SceneType == unloadSceneType));

            var loadedContents = _loadedSceneTypes.First(x => x.SceneType == unloadSceneType);
            loadedContents.AsyncOperation.allowSceneActivation = true;
            await UniTask.Yield();

            await SceneManager.UnloadSceneAsync(SceneManager.GetSceneByBuildIndex((int)unloadSceneType));
        }
    }*/
}

public enum SceneType
{
    Title = 0,
    LoadScene = 1,
    InGame = 2,
    GameOver = 3,
    GameClear = 4,
    ThankYouForPlaying = 5,
}
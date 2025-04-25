using Cysharp.Threading.Tasks;

public class TabletTaskUI : SingletonMonoBehaviour<TabletTaskUI>
{
    /*
     * 周囲を探索
     * 学校に入る
     * 昇降口を見つける
     * かぎを取る
     * かぎの掛かった教室をあけろ
     * 呪物を見つけろ
    */
    
    /*
     * タスク１をクリア
     * タスク１をクリア	
     */
    //チュートリアル終了フラグ
    private bool _isClearTutorial = false;
    //探索フラグ
    private bool _isResearch = false;
    //玄関発見フラグ
    private bool _isDiscoverEntranceGate = false;
    //校舎侵入フラグ
    private bool _isIntoScool = false;
    //鍵取得フラグ
    private bool _isGetKey = false;
    //鍵使用フラグ
    private bool _isUseKey = false;
    //呪物取得フラグ
    private bool _isGetCursedItem = false;
    private bool _goExit = false;
    CancelToken _cancelToken = new CancelToken();
    public async UniTask InGameTaskUI()
    {
        _cancelToken.GetToken();

        _isClearTutorial = false;
        await UniTask.WaitUntil(() => _isClearTutorial);

        //var researchTask = MissionUIManager.Instance.AddMission("Research");
        //_isResearch = false;
        //await UniTask.WaitUntil(() => _isResearch);
        //await MissionUIManager.Instance.ClearMission(researchTask);
        //var _intoScoolTask = MissionUIManager.Instance.AddMission("IntoScool");

        //_isIntoScool = false;
        //await UniTask.WaitUntil(() => _isIntoScool);
        //await MissionUIManager.Instance.ClearMission(_intoScoolTask);
        //DiscoverEntrance().Forget();

        var _getKeyTask = MissionUIManager.Instance.AddMission("鍵を入手する");

        _isGetKey = false;
        await UniTask.WaitUntil(() => _isGetKey);
        await MissionUIManager.Instance.ClearMission(_getKeyTask);

        var _openRoomTask = MissionUIManager.Instance.AddMission("鍵のかかった部屋を開ける");

        _isUseKey = false;
        await UniTask.WaitUntil(() => _isUseKey);
        await MissionUIManager.Instance.ClearMission(_openRoomTask);

        var _getCursedItemTask = MissionUIManager.Instance.AddMission("呪物を\n  入手する");

        _isGetCursedItem = false;
        await UniTask.WaitUntil(() => _isGetCursedItem);
        await MissionUIManager.Instance.ClearMission(_getCursedItemTask);

        var _goExitTask = MissionUIManager.Instance.AddMission("出口へ向かう");

        _goExit = false;
        await UniTask.WaitUntil(() => _goExit);
        await MissionUIManager.Instance.ClearMission(_goExitTask);
        _cancelToken.Cancel();
    }
    public async UniTask DiscoverEntrance()
    {
        var _discoverTask = MissionUIManager.Instance.AddMission("Discover");
        _isDiscoverEntranceGate = false;
        await UniTask.WaitUntil(() => _isDiscoverEntranceGate);
        await MissionUIManager.Instance.ClearMission(_discoverTask);
        _cancelToken.Cancel();
    }
    public void ClearTutorialFlag()
    {
        _isClearTutorial = true;
    }

    public void ResearchFlag()
    {
        _isResearch = true;
    }
    public void DiscoverEntranceGateFlag()
    {
        _isDiscoverEntranceGate = true;
    }
    public void IntoScoolFlag()
    {
        _isIntoScool = true;
    }
    public void GetKeyFlag()
    {
        _isGetKey = true;
    }
    public void UseKeyFlag()
    {
        _isUseKey = true;
    }
    public void GetCursedItemFlag()
    {
        _isGetCursedItem = true;
    }
    public void GoExitFlag()
    {
        _goExit = true;
    }
}

using Cysharp.Threading.Tasks;
using UnityEngine;
public class TutorialFlowManager : SingletonMonoBehaviour<TutorialFlowManager>
{
    MapHideOpen _mapHideOpen;
    SonarObjectPool _sonarObjectPool;
    PlayerMove _playerMove;
    SonarConverter _sonarConverter;
    public bool IsGetItemEvent { get; private set; } = false;
    public bool IsGetDocument { get; private set; } = false;
    public bool IsGetTablet { get; private set; } = false;
    private bool _isOpenFirstDoor = false;
    private bool _isOpenSecondDoor = false;
    public bool IsCheckMiddleDoor { get;private set; } = false;
    public bool IsGetNewsPaper { get; private set; } = false;
    public bool IsTutorialFinish { get; private set; } = false;
    public bool IsMovie { get; private set; } = false;
    [SerializeField] private Transform _tabletTransForm;
    [SerializeField] private Transform _secondRoomDoorTransForm;
    [SerializeField] private OpenBackDoor _openBackDoor;
    [SerializeField] private OpenTutorialSecondDoor _openSecondDoor;
    [SerializeField] private OpenSecondRoomDoor[] _openSecondRoomDoor;
    [SerializeField] private OpenQuizRoomDoor[] _openQuizRoomDoor;
    [SerializeField] private OpenScienceRoomDoor _openScienceRoomDoor;
    [SerializeField] private OpenSciencePreparationDoor _openSciencePreparationDoor;
    [SerializeField] private PeepHoleGimmick _peepHoleGimmick;
    
    [SerializeField]private SEGimmick _sEGimmick;

    [SerializeField] private GameObject _documentObj;
    [SerializeField] private GameObject _newsPaperObj;
    [SerializeField] private GameObject _flashlightObj;

    private void Start()
    {
        _mapHideOpen = MapHideOpen.Instance;
        _sonarObjectPool = SonarObjectPool.Instance;
        _playerMove = PlayerMove.Instance;
        _sonarConverter = SonarConverter.Instance;
        //各扉の開かない表示
        _openBackDoor.SetIsActionBlock(true);
        _openSecondDoor.SetIsActionBlock(true);
        _openScienceRoomDoor.SetIsActionBlock(true);
        _openSciencePreparationDoor.SetIsActionBlock(true);
        _peepHoleGimmick.SetIsActionBlock(true);
        for (int i = 0; i < _openSecondRoomDoor.Length; i++) 
        { _openSecondRoomDoor[i].SetIsActionBlock(true); }
        for (int i = 0; i < _openQuizRoomDoor.Length; i++) 
        { _openQuizRoomDoor[i].SetIsActionBlock(true); }
        StartTutorialFlow();
    }
    public void StartTutorialFlow() 
    {
        TutorialFlowFirstRoom().Forget();
    }
    /// <summary>
    /// チュートリアルフロー最初の部屋
    /// </summary>
    /// <returns></returns>
    private async UniTask TutorialFlowFirstRoom() 
    {
        //書類取得待ち
        IsGetDocument = false;
        await UniTask.WaitUntil(() => IsGetDocument);

#if UNITY_EDITOR
        if (Utility.GetTutorialSkip() is false)
        {
#endif
            IsGetItemEvent = true;
            _playerMove.SetMove(false);
            _documentObj.SetActive(false);
            //書類取得アニメーションを再生 テキスト表示
            await GetItemEvent.Instance.ViewItem(ItemEventName.TutorialFormCheck,false);
            IsGetItemEvent = false;
            //タブレット取得待ち
            IsGetTablet = false;
            await UniTask.WaitUntil(() => IsGetTablet);

            //懐中電灯取得演出
            _playerMove.SetMove(false);
            _flashlightObj.SetActive(false);
            await GetItemEvent.Instance.ViewItem(ItemEventName.Flashlight,false);
            _playerMove.SetMove(false);
            await Player.Instance.Flashlight(true);

            await MovieFrame.Instance.FrameIn();
            //タブレットに視線を固定する
            await _playerMove.SetPlayerRotation(_tabletTransForm, 1);
            //最初の部屋と二番目の部屋のマップを開ける
            _mapHideOpen.OpenMap(RoomType.Start);
            await UniTask.WaitForSeconds(0.5f);
            _mapHideOpen.OpenMap(RoomType.Corridor_1);
            await UniTask.WaitForSeconds(0.5f);
            _mapHideOpen.OpenMap(RoomType.Room_1);
            //Mapが開く秒数分待機
            await UniTask.WaitForSeconds(2.2f);
            //扉にソナー表示
            _sonarConverter.ViewSonar(SonarType.TutorialFirstDoor);
            await UniTask.WaitForSeconds(1);
            //プレイヤーのソナー表示
            MapLocation.Instance.currentPosAlpha(1);
            SonarPlayer.Instance.StartPlayerSonar();
            await UniTask.WaitForSeconds(1);
            await MovieFrame.Instance.FrameOut();

#if UNITY_EDITOR
        }
        else
        {
            IsGetItemEvent = false;
            IsGetTablet = false;
            _sonarConverter.ViewSonar(SonarType.TutorialFirstDoor);
            //プレイヤーのソナー表示
            MapLocation.Instance.currentPosAlpha(1);
            SonarPlayer.Instance.StartPlayerSonar();
            Player.Instance.Flashlight(true).Forget();
        }
#endif
        _playerMove.SetMove(true);
        _isOpenFirstDoor = false;
        _openBackDoor.SetIsActionBlock(false);
        //最初の部屋のドアを開けるまで待機
        await UniTask.WaitUntil(() => _isOpenFirstDoor);
        _sonarConverter.HideSonar(SonarType.TutorialFirstDoor);
        TutorialFlowSecondRoom().Forget();
    }
    /// <summary>
    /// チュートリアルフロー2番目の部屋
    /// </summary>
    /// <returns></returns>
    private async UniTask TutorialFlowSecondRoom()
    {
        //２つ目の部屋の扉を開ける
        _isOpenSecondDoor = false;
        await UniTask.WaitUntil(() => _isOpenSecondDoor);
#if UNITY_EDITOR
        if (Utility.GetTutorialSkip() is false)
        {
#endif
        //黒板にソナー表示
        _sonarConverter.ViewSonar(SonarType.TutorialBlackoard);
        //新聞を見るまで待機
        IsGetNewsPaper = false;
        await UniTask.WaitUntil(() => IsGetNewsPaper);
        _newsPaperObj.SetActive(false);
        _sonarConverter.HideSonar(SonarType.TutorialBlackoard);
        _openSecondDoor.SetIsActionBlock(false);
        //新聞取得アニメーション テキスト表示
        await GetItemEvent.Instance.ViewItem(ItemEventName.NewsPaper,false);
        _playerMove.SetMove(false);
        await MovieFrame.Instance.FrameIn();
        //数秒待機
        await UniTask.WaitForSeconds(1);

        //お化けの足音
        _sEGimmick.tutorialSE();
        await UniTask.WaitForSeconds(2.5f);
        
        //プレイヤーの視線誘導
        await _playerMove.SetPlayerRotation(_secondRoomDoorTransForm, 1);

        //テキスト表示
        await MessageViewEvent.Instance.ViewText(MessageEventName.FootStepSound);
        
        //タブレットに視線を固定する
        await _playerMove.SetPlayerRotation(_tabletTransForm, 1);
#if UNITY_EDITOR
        }
#endif
        //順にミニマップ表示
        _mapHideOpen.OpenMap(RoomType.Room_2);
        _mapHideOpen.OpenMap(RoomType.Corridor_3);
        _mapHideOpen.OpenMap(RoomType.SciencePreparationRoom);
        await UniTask.WaitForSeconds(0.5f);
        _mapHideOpen.OpenMap(RoomType.Room_3);
        _mapHideOpen.OpenMap(RoomType.ScienceRoom);
        await UniTask.WaitForSeconds(0.5f);
        _mapHideOpen.OpenMap(RoomType.Corridor_2);
        _mapHideOpen.OpenMap(RoomType.Room_4);
        _mapHideOpen.OpenMap(RoomType.Entrance);
        _mapHideOpen.OpenMap(RoomType.Toilet_1);
        await UniTask.WaitForSeconds(0.5f);
        _mapHideOpen.OpenMap(RoomType.Toilet_2);
        _mapHideOpen.OpenMap(RoomType.MusicRoom);
        await UniTask.WaitForSeconds(0.5f);
        _mapHideOpen.OpenMap(RoomType.Corridor_4);
        _mapHideOpen.OpenMap(RoomType.Room_5);
        await UniTask.WaitForSeconds(2.2f);

        //門にソナーを表示
        _sonarConverter.ViewSonar(SonarType.MiddleDoor);
        await UniTask.WaitForSeconds(1);
        await MovieFrame.Instance.FrameOut();

        _playerMove.SetMove(true);
        //中間ドアを確認するまで待機
        IsCheckMiddleDoor = false;
        await UniTask.WaitUntil(() => IsCheckMiddleDoor);
        _playerMove.SetMove(false);

        //チュートリアル終了
        IsTutorialFinish = true;
        IsMovie = true;
        
        //門を調べたときのテキスト
        await MessageViewEvent.Instance.ViewText(MessageEventName.FrontLatch);
        _playerMove.SetMove(true);
        
        //Room_02とクイズの部屋の開かない表示解除
        for (int i = 0; i < _openSecondRoomDoor.Length; i++)
        { _openSecondRoomDoor[i].SetIsActionBlock(false); }
        for (int i = 0; i < _openQuizRoomDoor.Length; i++)
        { _openQuizRoomDoor[i].SetIsActionBlock(false); }
        _openScienceRoomDoor.SetIsActionBlock(false);
        _openSciencePreparationDoor.SetIsActionBlock(false);
        _peepHoleGimmick.SetIsActionBlock(false);
        IsMovie = false;
        //InGameFlow呼び出し
        InGameFlow.Instance.StartInGame();
    }

    #region 外部公開:フラグ用関数
    /// <summary>
    /// 最初の部屋：書類確認フラグ
    /// </summary>
    public void GetDocumentFlag() 
    {
        IsGetDocument = true;
    }
    /// <summary>
    /// 最初の部屋：タブレット取得フラグ
    /// </summary>
    public void GetTabletFlag() 
    {
        IsGetTablet = true;
    }
    /// <summary>
    /// 最初の部屋：扉を開けたフラグ
    /// </summary>
    public void OpenFirstDoorFlag() 
    {
        _isOpenFirstDoor= true;
    }
    /// <summary>
    /// 2番目の部屋:部屋に入るドア開けたフラグ
    /// </summary>
    public void OpenSecondDoorFlag() 
    {
        _isOpenSecondDoor= true;
    }
    /// <summary>
    /// 2番目の部屋:新聞取得フラグ
    /// </summary>
    public void GetNewsPaperFlag()
    {
        IsGetNewsPaper = true;
    } 
    /// <summary>
    /// 門確認フラグ
    /// </summary>
    public void CheckMiddleDoorFlag()
    {
        IsCheckMiddleDoor = true;
    }
    #endregion
}

using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;
using System.Threading.Tasks;

public class InGameFlow : SingletonMonoBehaviour<InGameFlow>
{
    [SerializeField] private PlayableDirector _killdemon;
    [SerializeField] private GameObject cam1;
    [SerializeField] private GameObject cam2;
    [SerializeField] private Crygirl _crygirl;
    [SerializeField] private GameObject girl;

    SceneLoadManager sceneLoadManager;
    MapLocation _mapLocation;
    TabletTaskUI tabletTaskUI;
    SonarConverter _sonarConverter;
    PlayerMove _playerMove;
    TabletObjViewManager _tabletObjViewManager;

    private Player _player;

    //幽霊に襲われたかどうかのフラグ
    private bool _isAttackedGhost = false;
    public bool IsAttackedGhost => _isAttackedGhost;

    //呪物を入手したかどうかのフラグ
    public bool IsGetCursedItem { get; private set; } = false;

    //鍵を入手したかどうかのフラグ
    //public bool IsGetKeyItem { get; private set; } = false;

    //校舎に入った際に最初に落ちてくる壁のフラグ
    public bool IsFirstWall { get; private set; } = false;

    //玄関側の壁または教室側の壁のフラグ
    public bool IsSecondtWall { get; private set; } = false;

    //チェイス準備フラグ
    public bool IsChasePreparation { get; private set; } = false;

    //チェイス開始フラグ
    public bool IsChaseStart { get; private set; } = false;

    //のこぎり取得フラグ
    public bool IsGetSaw { get; private set; } = false;

    //のこぎり持ち手取得フラグ
    public bool IsGetSawHandle { get => isGetSawHandle; private set
        {
            if (isGetSawHandle == value) return;
            isGetSawHandle = value;
             GetSawAsync().Forget();
        } 
    }
    //のこぎり刃先取得フラグ
    public bool IsGetSawBlade { get => isGetSawBlade; private set 
        {
            if (isGetSawBlade == value) return;
            isGetSawBlade = value;
             GetSawAsync().Forget();
        }
    }
    //クイズ開始フラグ
    private bool _isQuizStart = false;

    //門チェックフラグ
    private bool _isGateOpenFlag = false;

    //ゴールのフラグ
    private bool _isGoal = false;

    //時間切れのフラグ
    private bool _isTimeOver = false;
    CancelToken _cancelToken = new CancelToken();

    private float _time = 0;
    public float TimeLimit { get; private set; } = 600;

    public IObservable<float> TimeObservable => _inGameTime;
    private Subject<float> _inGameTime = new Subject<float>();

    [SerializeField] private EnemyGhost _enemyGhost;
    [SerializeField] private EnemyDebuffs _enemyGhostDebuffs;
    [SerializeField] private GameObject _enemyGhostObject;

    public EnemyGhost EnemyGhost => _enemyGhost;
    public bool IsMovie { get; private set; }
    public bool IsWaitGoal { get; private set; }
    public bool IsLastChase { get; private set; }
    [SerializeField] private Light _playerLight;
    [SerializeField] private Light _tabletLight;
    [SerializeField] private Animator _tabletDirectionAnimator;
    [SerializeField] private GameObject _tabletImage;
    [SerializeField] private Transform _playerBack;
    [SerializeField] private Transform _tabletTransForm;
    [SerializeField] private PlayableDirector _ghostTransformTimeline;
    [SerializeField] private PlayableDirector _middleDoorTimeline;
    [SerializeField] private Transform _middleDoorPlayerTransform;
    [SerializeField] private Transform _middleDoorPlayerRotation;
    [SerializeField] private Transform _playerLastTransform;
    [SerializeField] private Transform _playerLastRotation;
    [SerializeField] private Transform _playermiddle;
    private bool isGetSawHandle = false;
    private bool isGetSawBlade = false;
    public void StartInGame()
    {
        _cancelToken.ReCreate();
        //_enemyGhost?.StartEnemy();

        Time(_cancelToken.GetToken()).Forget();
        GhostAction(_cancelToken.GetToken()).Forget();
        GameFlow(_cancelToken.GetToken()).Forget();
        PreparationRoomManager.Instance.StartPreparationRoomFlow(_cancelToken.GetToken());
    }

    private async UniTask Time(CancellationToken ct)
    {
        _isTimeOver = false;
        while (!_isTimeOver)
        {
            /*if (!IsGetKeyItem)
            {
                _time++;
            }*/

            if (_isAttackedGhost)
            {
                return;
            }

            _inGameTime.OnNext(TimeLimit - _time);

            await UniTask.WaitForSeconds(1f, cancellationToken: ct);
            if (_time < TimeLimit) continue;
            _isTimeOver = true;
            ActionButtonUIViewerManager.Instance.AllRelease();
            PlayerMove.Instance.InvertMoveLockFlag();
            await PlayerMove.Instance.SetPlayerRotation(TutorialTablet.Instance.TabletTransform, 1f);
            _tabletDirectionAnimator.SetTrigger("TurnOff");
            MissionUIManager.Instance.AllClearMission();
            _mapLocation = MapLocation.Instance;
            _mapLocation.currentPosAlpha();
            //   mapLocation.KeyPosAlpha();
            //  mapLocation.ItemPosAlpha();
            // mapLocation.FirstWallAlpha();
            // mapLocation.EntranceWallAlpha();
            //  mapLocation.ClassRoomWallAlpha();
            _playerLight.enabled = false;
            _tabletLight.enabled = false;
            _tabletImage.SetActive(true);
            TutorialTablet.Instance.TurnOffTablet();
            InGameUtility.SetDeathPosition(Player.Instance.transform.position);
            await UniTask.WaitForSeconds(1.7f);
            SoundManager.Instance.PlaySE(SEType.GhostVoice03, _playerBack);
            await UniTask.WaitForSeconds(3);
            await FadeInOut.FadeIn(1);
            cam1.SetActive(false);
            cam2.SetActive(true);
            _enemyGhost?.ForceStop();
            FadeInOut.FadeOut(2.0f);
            await _killdemon.PlayAsync();
            await FadeInOut.FadeIn();

            await SceneLoadManager.Instance.AddLoadScene(SceneType.GameOver);
            SceneLoadManager.Instance.LoadedSceneActive(SceneType.GameOver, isOutFade: true).Forget();
            _cancelToken.Cancel();
        }
    }

    private async UniTask GhostAction(CancellationToken ct)
    {
        _isAttackedGhost = false;
        await UniTask.WaitUntil(() => _isAttackedGhost, cancellationToken: ct);

        if (_isGoal)
        {
            return;
        }
        if (_isTimeOver)
        {
            return;
        }

        ActionButtonUIViewerManager.Instance.AllRelease();
        RoomLightingEffect.Instance.CallValueChange(false).Forget();
        PlayerMove.Instance.InvertMoveLockFlag();
        InGameUtility.SetDeathPosition(Player.Instance.transform.position);
        await FadeInOut.FadeIn();
        RoomLightingEffect.Instance.CallValueChange(true, 0).Forget();
        cam1.SetActive(false);
        cam2.SetActive(true);
        await UniTask.Delay(TimeSpan.FromSeconds(2f));
        FadeInOut.FadeOut(2.0f).Forget();
        await _killdemon.PlayAsync();
        await FadeInOut.FadeIn();
        await SceneLoadManager.Instance.AddLoadScene(SceneType.GameOver);
        SceneLoadManager.Instance.LoadedSceneActive(SceneType.GameOver, isOutFade: true).Forget();
        _cancelToken.Cancel();
    }

    private async UniTask GameFlow(CancellationToken ct)
    {
        IsGetSawHandle = false;
        IsGetSawBlade = false;
        IsGetSaw = false;
        _tabletObjViewManager = TabletObjViewManager.Instance;
        _playerMove = PlayerMove.Instance;
        _sonarConverter = SonarConverter.Instance;
        //動けないようにする
        _playerMove.SetMove(false);
        //TODO:門開かないテキスト
        //タブレットに視線を固定する
        await _playerMove.SetPlayerRotation(_tabletTransForm, 1);
        //門のソナーを消す
        _sonarConverter.HideSonar(SonarType.MiddleDoor);
        await UniTask.WaitForSeconds(1);
        //ソナー表示（教室2，3）
        _sonarConverter.ViewSonar(SonarType.ClassRoom2);
        _sonarConverter.ViewSonar(SonarType.ClassRoom3);
        _sonarConverter.ViewSonar(SonarType.MusicRoom);
        _sonarConverter.ViewSonar(SonarType.PreparationRoom);
        _sonarConverter.ViewSonar(SonarType.Hole);
        await UniTask.WaitForSeconds(1);
        //バッテリー表示
        TutorialTablet.Instance.BatteryActive();
        await UniTask.WaitForSeconds(1);
        //動けるようにする
        _playerMove.SetMove(true);
        //女の子なく
        _crygirl.GirlSound();

#if UNITY_EDITOR
        if (!Utility.GetQuizSkip())
        {
#endif
            //クイズ開始待機
            _isQuizStart = false;
            await UniTask.WaitUntil(() => _isQuizStart, cancellationToken: ct);
            //クイズフロー呼び出し
            VibrationQuiz.Instance.StartVibrationQuizFlow(ct);
            //のこぎり入手待機
            IsGetSaw = false;
            await UniTask.WaitUntil(() => IsGetSaw, cancellationToken: ct);
#if UNITY_EDITOR
        }
        else
        {
            IsGetSaw = true;
        }
#endif
        //ソナー表示（門）
        _sonarConverter.ViewSonar(SonarType.MiddleDoor);
        //門調べ待機
        _isGateOpenFlag = false;
        await UniTask.WaitUntil(() => _isGateOpenFlag, cancellationToken: ct);
        IsMovie = true;
        //ソナー非表示
        _sonarConverter.HideSonar(SonarType.MiddleDoor);

        // 中央のドア開放演出
        {
            await FadeInOut.FadeIn(1f);
            // 暗転中に座標を移動するなど
            GetItemIconViewManager.Instance.RemoveItemView(ItemEventName.Saw);
            await Player.Instance.transform.DOMove(_middleDoorPlayerTransform.position, 0f);
            PlayerMove.Instance.SetMove(false);
            await PlayerMove.Instance.SetPlayerRotation(_playermiddle, 2f);
            await FadeInOut.FadeOut(1f);

            // 開放演出
            _tabletObjViewManager.TabletObjActive();
            await UniTask.WaitForSeconds(1, cancellationToken: ct);
            await _middleDoorTimeline.PlayAsync();
            _tabletObjViewManager.TabletObjDisable();
            await UniTask.WaitForSeconds(1, cancellationToken: ct);
            //TODO:12/13用 対応
            //PlayerMove.Instance.SetMove(true);
            await PlayerMove.Instance.SetPlayerRotation(_playermiddle, 2f);
        }

        // チェイス周り
        // 12/13用 対応
        // ゴーストの変身演出
        {
            girl.SetActive(true);
            PlayerMove.Instance.SetMove(false);
            // プレイヤー移動
            Player.Instance.transform.DOMove(_playerLastTransform.position, 2f);
            var task = UniTask.Create(async () =>
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.25f),cancellationToken:ct);
                await MessageViewEvent.Instance.ViewText(MessageEventName.GateOpened);
            });
            await UniTask.WhenAll(task,
                Player.Instance.transform.DOMove(_playerLastTransform.position, 2f).ToUniTask(cancellationToken:ct));
            //await Player.Instance.transform.DOMove(_playerLastTransform.position, 2f);
            
            // ゴーストの方向を向く
            await PlayerMove.Instance.SetPlayerRotation(_playerLastRotation, 1f);
            // ゴーストに変身前テキスト
            // 使わない
            //await MessageViewEvent.Instance.ViewText(MessageEventName.TransformGirl);
            await _ghostTransformTimeline.PlayAsync();
            // ゴーストに変身後テキスト
            await MessageViewEvent.Instance.ViewText(MessageEventName.TransformedGirl);
            PlayerMove.Instance.SetMove(true);
        }

        // チェイス本体
        {
            IsLastChase = true;
            _enemyGhost.enabled = true;
            _enemyGhostDebuffs.enabled = true;
            _enemyGhost.StartEnemy();
            PlayerMove.Instance.StartChaseFlag();
            _enemyGhost.ForceLookPlayer(_enemyGhost.transform);
        }

        IsMovie = false;
        //ゴール待機
        IsWaitGoal = true;
        _isGoal = false;
        await UniTask.WaitUntil(() => _isGoal, cancellationToken: ct);
        ActionButtonUIViewerManager.Instance.AllRelease();
        await UniTask.WaitForSeconds(0.5f, cancellationToken: ct);

        await FadeInOut.FadeIn();
        await SceneLoadManager.Instance.AddLoadScene(SceneType.GameClear);
        SceneLoadManager.Instance.LoadedSceneActive(SceneType.GameClear, isOutFade: true).Forget();
    }
    private async UniTask GetSawAsync()
    {
        if ((IsGetSawHandle == true) && (IsGetSawBlade == true))
        {
            IsGetSawHandle = false;
            IsGetSawBlade = false;
            GetItemIconViewManager.Instance.RemoveItemView(ItemEventName.Saw_Hand);
            GetItemIconViewManager.Instance.RemoveItemView(ItemEventName.Saw_Blade);
            
            PlayerMove.Instance.SetMove(false);
            await GetItemEvent.Instance.ViewItem(ItemEventName.Saw);
            PlayerMove.Instance.SetMove(true);

            GetSawFlag();
            PlayerMove.Instance.SetMove(false);
            await MessageViewEvent.Instance.ViewText(MessageEventName.SawSynthesis);
            PlayerMove.Instance.SetMove(true);
        }
    }

    private async UniTask TGSGameFlow(CancellationToken ct)
    {
        tabletTaskUI = TabletTaskUI.Instance;
        _mapLocation = MapLocation.Instance;

#if UNITY_EDITOR
        if (!Utility.GetGameFlowSkip())
        {
#endif
            IsFirstWall = false;
            await UniTask.WaitUntil(() => IsFirstWall, cancellationToken: ct);
            //1枚目の壁を落とす
            IsSecondtWall = false;
            await UniTask.WaitUntil(() => IsSecondtWall, cancellationToken: ct);
            //対応する2枚目の壁を落とす

            //IsGetKeyItem = false;
            //await UniTask.WaitUntil(() => IsGetKeyItem, cancellationToken: ct);

            tabletTaskUI.GetKeyFlag();
            _mapLocation.ItemPosAlpha(255);
            _mapLocation.KeyPosAlpha();
            IsGetCursedItem = false;
           // GameClearManager.gameob = DateTime.Now.AddHours(12).ToString("M月dd日 HH時mm分");
            await UniTask.WaitUntil(() => IsGetCursedItem, cancellationToken: ct);
            tabletTaskUI.GetCursedItemFlag();
            TutorialTablet.Instance.SetAfterGoalTexture();
            _mapLocation.GoalPosAlpha(255);
            _mapLocation.ItemPosAlpha();

            IsChasePreparation = false;
            await UniTask.WaitUntil(() => IsChasePreparation, cancellationToken: ct);

            IsChaseStart = false;
            await UniTask.WaitUntil(() => IsChaseStart, cancellationToken: ct);

#if UNITY_EDITOR
        }
        else
        {
            IsFirstWall = true;
            IsSecondtWall = true;
            //IsGetKeyItem = true;
            IsGetCursedItem = true;
            IsChasePreparation = true;
            IsChaseStart = true;
        }
#endif

        _isGoal = false;
        await UniTask.WaitUntil(() => _isGoal, cancellationToken: ct);
        await UniTask.WaitForSeconds(0.5f);

        ActionButtonUIViewerManager.Instance.AllRelease();
        await FadeInOut.FadeIn();
        await SceneLoadManager.Instance.AddLoadScene(SceneType.GameClear);
        SceneLoadManager.Instance.LoadedSceneActive(SceneType.GameClear, isOutFade: true).Forget();
    }

    public void AttackedFlag()
    {
        _isAttackedGhost = true;
    }

    public void CursedItemFlag()
    {
        _enemyGhost?.ForceStop();
        IsGetCursedItem = true;
    }

    public void ChasePreparationFlag(Transform position)
    {
        _enemyGhost?.InitForceLookPlayer(position);
        IsChasePreparation = true;
    }

    public void ChaseStartFlag(Transform position)
    {
        _enemyGhost?.ForceLookPlayer(position);
        IsChaseStart = true;
    }

    /*public void KeyFlag()
    {
        IsGetKeyItem = true;
    }*/

    public void FirstWallFallFlag()
    {
        IsFirstWall = true;
    }

    public void SecondWallFallFlag()
    {
        IsSecondtWall = true;
    }

    public void GoalFlag()
    {
        _isGoal = true;
    }

    [ContextMenu("GetSawFlag")]
    public void GetSawFlag()
    {
        IsGetSaw = true;
    }
    public void GetSawHandleFlag()
    {
        IsGetSawHandle = true;
    }
    public void GetSawBladeFlag()
    {
        IsGetSawBlade = true;
    }
    public void GateOpenFlag()
    {
        _isGateOpenFlag = true;
    }

    public void QuizStartFlag()
    {
        _isQuizStart = true;
    }

    [ContextMenu("LastChaseStart")]
    public void LastChaseStart()
    {
        IsLastChase = true;
    }
}
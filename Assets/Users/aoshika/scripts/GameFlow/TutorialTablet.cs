using System;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using DG.Tweening;

public class TutorialTablet : SingletonMonoBehaviour<TutorialTablet>
{
    [SerializeField] private List<Transform> _cameraTransforms;

    [SerializeField] private Texture _renderTexture;
    [SerializeField] private Texture _mapTextureAterGoal;
    [SerializeField] private Texture _mapTextureBeforeGoal;
    [SerializeField] private Texture _mapBTexture;

    [SerializeField] private Transform _surveillanceCamera;

    [SerializeField] private Transform _tutorialPlayerTransform;
    [SerializeField] private Transform _afterTutorialTransform;
    [SerializeField] private Transform _tabletTransform;
    public Transform TabletTransform => _tabletTransform;
    [SerializeField] private RawImage _tabletRawImage;
    [SerializeField] private RawImage _mapRawImage;
    [SerializeField] private RawImage _tabletNoiseImageCover;
    [SerializeField] private Image _tabletNoiseImage;

    [SerializeField] private TextMeshProUGUI _tabletText;
    [SerializeField] private TextMeshProUGUI _tabletText2;
    [SerializeField] private TextMeshProUGUI _fasttext;
    [SerializeField] private GameObject _triangle;
    [SerializeField] private GameObject _triangle2;
    [SerializeField] private GameObject _triangle3;
    [SerializeField] private GameObject _battery;

    [SerializeField] private PlayableDirector _walkGhostTimeline;
    [SerializeField] private PlayableDirector _searchGhostTimeline;
    [SerializeField] private PlayableDirector _breakCameraTimeline;
    [SerializeField] private PlayableDirector _dropTabletTimeline;

    [SerializeField] private GameObject _tutorialghost1;
    [SerializeField] private GameObject _tutorialghost2;
    [SerializeField] private GameObject _tutorialghost3;

    [SerializeField] private Material _ipadCanvasGlitchnoise;
    [SerializeField] private GameObject _Panel;
    //置いてあるタブレット
    [SerializeField] private GameObject _placedTablet;

    //持つタブレット
    [SerializeField] private GameObject _haveTablet;

    [SerializeField] private Animator _tabletDirectionAnimator;
    [SerializeField] private AudioListener _tabletAudioListener;
    [SerializeField] private AudioListener _playerAudioListener;

    [SerializeField] private CanvasGroup _tutorialItemImg;
    [SerializeField] private CanvasGroup _tutorialKeyImg;

    [SerializeField] private Material _material;
    private bool _stopNoise = false;

    PlayerMove playerMove;
    TabletTaskUI tabletTaskUI;

    private int Clickcount = 0;

    //入力フラグ
    private bool _isClick = false;

    //入力拒否用フラグ
    private bool _isWaitInput = false;

    //チュートリアル終了フラグ
    public bool IsFinishTutorial { get; private set; } = false;
    [SerializeField] private OpenBackDoor _openBackDoor;
    CancelToken _cancelToken = new CancelToken();

    public void StartTutorialTablet()
    {
        //TabletFlow().Forget();
        GetTablet();
    }

    void Start()
    {
        TabletClear();
    }
    /// <summary>
    /// マップ以外を非表示
    /// </summary>
    private void TabletClear()
    {
        _mapRawImage.texture = _mapBTexture;
        _tabletAudioListener.enabled = false;
        _playerAudioListener.enabled = true;
        _fasttext.text = "";
        _tabletText.text = "";
        _tabletText2.text = "";
        _tabletRawImage.enabled = false;
        _tabletNoiseImageCover.enabled = false;
        _tabletNoiseImage.enabled = false;
        _tutorialItemImg.alpha = 0;
        _tutorialKeyImg.alpha = 0;
        _triangle.SetActive(false);
        _triangle2.SetActive(false);
        _battery.SetActive(false);
        //タブレット初期状態
        _placedTablet?.SetActive(true);
        _haveTablet?.SetActive(false);
    }
    private void GetTablet()
    {
        if (TutorialFlowManager.Instance.IsGetDocument == false) return;
        TutorialFlowManager.Instance.GetTabletFlag();
        //タブレット取得
        _placedTablet.SetActive(false);
        _haveTablet.SetActive(true);

    }
    public void BatteryActive()
    {
        _battery.SetActive(true);
    }
    private void TGSStart()
    {
        _tabletAudioListener.enabled = false;
        _playerAudioListener.enabled = true;
        //タブレットのCanvas内部を全て非表示
        _fasttext.color = new Color(255, 255, 255);
        _fasttext.text = "取レ";
        _tabletText.text = "";
        _tabletText2.text = "";
        _tabletRawImage.texture = null;
        _tabletRawImage.color = new Color(0, 0, 0);
        _tabletRawImage.enabled = false;
        _tabletNoiseImage.enabled = true;
        _tabletNoiseImageCover.enabled = true;
        _triangle.SetActive(false);
        _triangle2.SetActive(false);
        _battery.SetActive(false);

        //タブレット初期状態
        _placedTablet?.SetActive(true);
        _haveTablet?.SetActive(false);
        _tabletRawImage?.gameObject.SetActive(false);
        _mapRawImage?.gameObject.SetActive(false);
        _ipadCanvasGlitchnoise?.SetFloat("_GlitchActive", 0);

        _tutorialItemImg.alpha = 0;
        _tutorialKeyImg.alpha = 0;

        NoiseAnim().Forget();


        InputManager.Instance.Decision.Subscribe(x =>
        {
            if (_isWaitInput) return;
            ClickFlag();
        }).AddTo(this);
    }

    private async UniTask TabletFlow()
    {
        _cancelToken.GetToken();

        playerMove = PlayerMove.Instance;
        playerMove.InvertMoveLockFlag();
        await FadeInOut.FadeIn();
        _tabletRawImage.enabled = true;
        _tabletNoiseImage.enabled = false;
        _tabletNoiseImageCover.enabled = false;

        _stopNoise = true;
        _fasttext.text = "";
        //プレイヤーの移動停止と所定位置に移動

        _placedTablet.SetActive(false);
        _haveTablet.SetActive(true);
        playerMove.SetPlayerTransform(_tutorialPlayerTransform);
        await FadeInOut.FadeOut();
        await playerMove.SetPlayerRotation(_tutorialPlayerTransform, 2);

        _tabletAudioListener.enabled = true;
        _playerAudioListener.enabled = false;
#if UNITY_EDITOR
        if (!Utility.GetTabletEventSkip())
        {
#endif
            //TODO:鍵呪物の画像の表示

            _tabletText.color = new Color(255, 255, 255);
            _tabletText2.color = new Color(255, 255, 255);
            await TutorialTabletText(_tabletText, "学校にある呪物を、、、\nモチカエレ");
            await TutorialImageView(_tutorialItemImg);
            _triangle.SetActive(true);
            _isClick = false;
            await UniTask.WaitUntil(() => _isClick);
            _tutorialItemImg.alpha = 0;

            _tabletText.text = "";
            _triangle.SetActive(false);

            await TutorialTabletText(_tabletText2, "呪物ハ鍵ノカカッタ部屋ニ\nカクサレテイル \n鍵ヲ、、ミツケロ");
            await TutorialImageView(_tutorialKeyImg);

            _triangle.SetActive(true);

            _isClick = false;
            await UniTask.WaitUntil(() => _isClick);
            _tutorialKeyImg.alpha = 0;

            _triangle.SetActive(false);
            _isWaitInput = true;

            //テキスト非表示と画像差し替え
            _tabletText2.text = "";
            _tabletRawImage.color = new Color(255, 255, 255);
            _tabletRawImage.texture = _renderTexture;
            CameraTransformChanger(Clickcount++);

            await UniTask.Yield();

            _tabletRawImage?.gameObject.SetActive(true);
            await UniTask.WaitForSeconds(2);
            _triangle.SetActive(true);

            _isWaitInput = false;
            _isClick = false;
            await UniTask.WaitUntil(() => _isClick);

            _triangle.SetActive(false);
            CameraTransformChanger(Clickcount++);

            //cam2
            _isWaitInput = true;
            await _walkGhostTimeline.PlayAsync();

            _triangle.SetActive(true);
            _isWaitInput = false;

            _isClick = false;
            await UniTask.WaitUntil(() => _isClick);
            Destroy(_tutorialghost1);
            _triangle.SetActive(false);
            //cam3
            CameraTransformChanger(Clickcount++);
            _isWaitInput = true;
            await _searchGhostTimeline.PlayAsync();

            _triangle.SetActive(true);
            _isWaitInput = false;

            _isClick = false;
            await UniTask.WaitUntil(() => _isClick);
            Destroy(_tutorialghost2);
            _triangle.SetActive(false);
            //cam4
            CameraTransformChanger(Clickcount);
            _isWaitInput = true;
            await _breakCameraTimeline.PlayAsync();
            //_tabletDirectionAnimator.SetTrigger("Break");

            _triangle.SetActive(true);
            _isWaitInput = false;
            Destroy(_tutorialghost3);

            await UniTask.Delay(TimeSpan.FromSeconds(1));
#if UNITY_EDITOR
        }
#endif
        _Panel.SetActive(false);

        _tabletRawImage?.gameObject.SetActive(false);
        _mapRawImage.texture = _mapTextureBeforeGoal;
        _mapRawImage?.gameObject.SetActive(true);

        MapLocation.Instance.currentPosAlpha(255);
        MapLocation.Instance.KeyPosAlpha(255);
        SonarPlayer.Instance.StartSonerLoop();

        tabletTaskUI = TabletTaskUI.Instance;
        tabletTaskUI.InGameTaskUI().Forget();
        tabletTaskUI.ClearTutorialFlag();

        _battery.SetActive(true);
        _triangle.SetActive(false);
        _triangle3.SetActive(false);

        await UniTask.WaitForSeconds(2);
        await playerMove.SetPlayerRotation(_afterTutorialTransform, 2);

      //  GameClearManager.textend = DateTime.Now.AddHours(12).ToString("M月dd日 HH時mm分");
        _tabletAudioListener.enabled = false;
        _playerAudioListener.enabled = true;

        playerMove.InvertMoveLockFlag();
        IsFinishTutorial = true;
        _cancelToken.Cancel();
    }

    private void CameraTransformChanger(int index)
    {
        _surveillanceCamera.position = _cameraTransforms[index].position;
        _surveillanceCamera.rotation = _cameraTransforms[index].rotation;
    }

    private void ClickFlag()
    {
        _isClick = true;
    }

    private async UniTask TutorialTabletText(TextMeshProUGUI tutorialText, string str)
    {
        // テキスト全体の長さ
        var length = str.Length;
        tutorialText.text = str;
        // １文字ずつ表示する演出
        for (var i = 0; i <= length; i++)
        {
            // 徐々に表示文字数を増やしていく
            tutorialText.maxVisibleCharacters = i;

            await UniTask.WaitForSeconds(0.1f);
        }
    }
    public void TurnOffTablet()
    {
        _fasttext.color = new Color(255, 255, 255);
        _fasttext.text = "";
        _tabletText.text = "";
        _tabletRawImage.texture = null;
        _tabletRawImage.color = new Color(0, 0, 0);
        _mapRawImage.texture = null;
        _mapRawImage.color = new Color(0, 0, 0);
        _triangle.SetActive(false);
        _triangle2.SetActive(false);
        _battery.SetActive(false);
    }
    public void SetAfterGoalTexture()
    {
        _mapRawImage.texture = _mapTextureAterGoal;
    }

    public void SetIpadCanvasGlitchNoise()
    {
        _ipadCanvasGlitchnoise?.SetFloat("_GlitchActive", 1);

    }
    private async UniTask NoiseAnim()
    {
        var seed = 0;
        while (!_stopNoise)
        {
            _material.SetInt("_Seed", seed);
            seed++;
            await UniTask.Yield();
        }
    }
    private async UniTask TutorialImageView(CanvasGroup canvasGroup, float start = 0, float end = 1, float time = 2)
    {
        await DOVirtual.Float(start, end, time, f =>
        {
            canvasGroup.alpha = f;
        });
    }
}
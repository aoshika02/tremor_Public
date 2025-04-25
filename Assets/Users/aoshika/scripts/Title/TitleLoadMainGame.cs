using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UniRx;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class TitleLoadMainGame : SingletonMonoBehaviour<TitleLoadMainGame>
{
    [SerializeField] private GameObject _handRoot;
    [SerializeField] private RawImage _pressButtonUIRawImage;
    [SerializeField] private float _blinkDuration = 1f;

    // ボタン降下時変数
    [SerializeField] private GameObject _pressButtonUIGameObject;
    [SerializeField] private float _pressEffectTime = 0.2f;
    [SerializeField] private GameObject _demonGameObject;
    [SerializeField] private RawImage _title_BG_Effect;
    private CancelToken _blinkDurationToken = new CancelToken();
    private SoundHash _bgmHash;
    private Color _pressButtonUIColor;
    //プレイ人数を見る登録するとこ
    [SerializeField]
    private GameObject _countob;
    [SerializeField]
    private GameObject _clearob;
    [SerializeField]
    private GameObject _gameoverob;
    [SerializeField]
    private Text _counttext;
    [SerializeField]
    private Text _clearText;
    [SerializeField]
    private Text _gameoverText;
    bool _iscounttext = false;

    // グリッチノイズ変数
    private CancelToken _glitchToken = new CancelToken();

    [SerializeField] private RawImage[] _glitchRawImages;
    private Material _glitchMaterial;

    // グリッチが再生される時間
    [SerializeField] private Vector2 _glitchActiveDurationRandomRange = Vector2.zero;

    // 次にグリッチが再生されるまでの時間
    [SerializeField] private Vector2 _glitchDelayDurationRandomRange = Vector2.zero;
    [SerializeField] private GameObject[] _glitchActiveObjects;
    private float _randomActiveDurationTime = 0f;
    private float _glitchDelayDurationTime = 0f;
    private float _glitchDurationTime = 0f;
    private float _glitchActiveTime = 0f;
    private static readonly int GlitchActive = Shader.PropertyToID("_GlitchActive");
    
    bool _isLoad = false;
    public bool IsLoad => _isLoad;
    async void Start()
    {
        // 初期化処理
        _handRoot.SetActive(false);
        _pressButtonUIGameObject.SetActive(false);
        _demonGameObject.SetActive(false);
        _pressButtonUIColor = _pressButtonUIRawImage.color;
        _pressButtonUIColor.a = 0;
        _pressButtonUIRawImage.color = _pressButtonUIColor;
        foreach (var g in _glitchActiveObjects)
        {
            g.SetActive(false);
        }

        _glitchMaterial = _glitchRawImages[0].material;

        foreach (var rawImage in _glitchRawImages)
        {
            rawImage.material = _glitchMaterial;
        }

        _randomActiveDurationTime =
            Random.Range(_glitchActiveDurationRandomRange.x, _glitchActiveDurationRandomRange.y);
        _glitchDelayDurationTime = Random.Range(_glitchDelayDurationRandomRange.x, _glitchDelayDurationRandomRange.y);
        _glitchMaterial.SetFloat(GlitchActive, 0);

        StartGlitch(_glitchToken.GetToken());

        ///////////////////////////////////////////////////////////////////////
        _pressButtonUIGameObject.SetActive(true);
        PlayBGM();
        DOVirtual.Float(0, 1, _blinkDuration, f =>
            {
                _pressButtonUIColor.a = f;
                _pressButtonUIRawImage.color = _pressButtonUIColor;
            }).SetEase(Ease.InOutQuint).SetLoops(-1, LoopType.Yoyo)
            .ToUniTask(cancellationToken: _blinkDurationToken.GetToken()).Forget();

        // ボタン押したとき処理
        InputManager.Instance.Decision.Subscribe(PressButtonAction).AddTo(this);

    }

    private async void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) && _iscounttext == false)
        {
            _countob.SetActive(true);
            _clearob.SetActive(true);
            _gameoverob.SetActive(true);
            _iscounttext = true;
            CoutSytem();
        }
        else if(Input.GetKeyDown(KeyCode.G) &&  _iscounttext == true)
        {
            _countob.SetActive(false);
            _clearob.SetActive(false);
            _gameoverob.SetActive(false);
            _iscounttext = false;
        }
        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Delete) && _iscounttext == true)
        {
            PlayerPrefs.DeleteAll();
            CoutSytem();
        }
    }
    private void CoutSytem()
    {
        _counttext.text = $"プレイ人数 : {PlayerPrefs.GetInt("PlayCount", 0)}";
        _clearText.text = $"クリア人数 : {PlayerPrefs.GetInt("ClearCount", 0)}";
        _gameoverText.text = $" 死亡人数 : {PlayerPrefs.GetInt("GameOverCount", 0)}";
    }

    private async void PressButtonAction(float x)
    {
        // UIが表示状態なら処理しない
        if(LicenseGenerator.Instance.IsView == true) { return; }
        if (AudioSetting.Instance?.IsView == true) { return; }

        if (x == 1 && !_isLoad)
        {
            _isLoad = true;
            _iscounttext = false;
            _blinkDurationToken.Cancel();
            _glitchToken.Cancel();
            _glitchMaterial.SetFloat(GlitchActive, 0);
            _demonGameObject.SetActive(false);
            List<UniTask> loadTask = new List<UniTask>();
            List<UniTask> effectTask = new List<UniTask>();

            PlayerPrefs.SetInt("PlayCount", PlayerPrefs.GetInt("PlayCount", 0) + 1);
            PlayerPrefs.Save();
           // GameClearManager.title = DateTime.Now.AddHours(12).ToString("M月dd日 HH時mm分");

            SoundManager.Instance.PlaySE(SEType.Title_piano_SE);

            // 読み込み処理
            SceneLoadManager.Instance.AddLoadScene(SceneType.InGame).Forget();

            // BGM停止
            StopBGM();

            // UI表示
            _pressButtonUIColor.a = 1f;
            _pressButtonUIRawImage.color = _pressButtonUIColor;

            // UI拡大
            effectTask.Add(_pressButtonUIGameObject.transform
                .DOScale(new Vector3(1.1f, 1.1f, 1.1f), _pressEffectTime)
                .ToUniTask());
            // UI透明化
            effectTask.Add(
                DOVirtual.Float(1, 0, _pressEffectTime * 0.75f, f =>
                    {
                        _pressButtonUIColor.a = f;
                        _pressButtonUIRawImage.color = _pressButtonUIColor;
                    }).SetEase(Ease.InOutQuint)
                    .ToUniTask());

            await UniTask.WhenAll(effectTask);

            await UniTask.Delay(TimeSpan.FromSeconds(1f));

            // 手の表示
            SoundManager.Instance.PlaySE(SEType.Title_impact_SE);
            _handRoot.SetActive(true);
            _glitchMaterial.SetFloat(GlitchActive, 1);

            await _title_BG_Effect.DOColor(new Color(0.2830189f, 0f, 0f), 0);
            _handRoot.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
            await _handRoot.transform
                .DOScale(Vector3.one, 0.025f)
                .ToUniTask();

            // 演出終了まで適当に待機
            await UniTask.Delay(TimeSpan.FromSeconds(1f));

            // 読み込みが終わってからシーン移動
            //await UniTask.WhenAll(loadTask);
            SceneLoadManager.Instance.LoadedSceneActive(SceneType.InGame, true).Forget();
        }
    }

    private async void StartGlitch(CancellationToken ct)
    {
        while (true)
        {
            _glitchDurationTime += Time.deltaTime;
            // グリッチ再生開始判定
            if (_glitchDelayDurationTime <= _glitchDurationTime)
            {
                _glitchDurationTime = 0;
                bool isFirst = false;

                // グリッチの再生中
                while (_glitchActiveTime <= _randomActiveDurationTime)
                {
                    if (!isFirst)
                    {
                        _glitchMaterial.SetFloat(GlitchActive, 1);
                        foreach (var g in _glitchActiveObjects)
                        {
                            g.SetActive(true);
                        }

                        isFirst = true;
                    }

                    _glitchActiveTime += Time.deltaTime;

                    if (ct.IsCancellationRequested) { return; }
                    await UniTask.Yield();
                }

                _glitchActiveTime = 0f;
                _glitchMaterial.SetFloat(GlitchActive, 0);
                foreach (var g in _glitchActiveObjects)
                {
                    g.SetActive(false);
                }

                _randomActiveDurationTime =
                    Random.Range(_glitchActiveDurationRandomRange.x, _glitchActiveDurationRandomRange.y);
                _glitchDelayDurationTime =
                    Random.Range(_glitchDelayDurationRandomRange.x, _glitchDelayDurationRandomRange.y);
            }

            if (ct.IsCancellationRequested) { return; }
            await UniTask.Yield();
        }
    }

    public void PlayBGM()
    {
        _bgmHash = SoundManager.Instance.PlayBGM(BGMType.title_BGM);
    }

    public void StopBGM()
    {
        SoundManager.Instance.StopBGM(_bgmHash).Forget();
    }
}
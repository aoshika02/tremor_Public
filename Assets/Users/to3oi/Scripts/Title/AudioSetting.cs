using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UniRx;
using UnityEngine.InputSystem;

public class AudioSetting : SingletonMonoBehaviour<AudioSetting>
{
    [Serializable]
    public class AudioSettingData
    {
        public string ViewName;
        public AudioMixerGroup AudioMixerGroup;
        public bool IsSE = false;
        public string AudioName;
    }

    [SerializeField] private List<AudioSettingData> _data = new List<AudioSettingData>();

    [SerializeField] private GameObject _root;
    [SerializeField] private GameObject _viewArea;
    [SerializeField] private GameObject _audioSettingFieldPrefab;
    List<AudioSettingField> _audioSettingFields = new List<AudioSettingField>();
    private bool _isPlaySound = false;
    private int _index = 0;
    private Vector2 _moveDirection;

    private SoundHash _soundHash;

    private bool _isView = false;
    public bool IsView => _isView;

    private void Start()
    {
        _root.SetActive(false);
        foreach (var data in _data)
        {
            var field = Instantiate(_audioSettingFieldPrefab, _viewArea.transform);
            var audioSettingField = field.GetComponent<AudioSettingField>();
            audioSettingField.Init(data);
            _audioSettingFields.Add(audioSettingField);
        }

        InputManager.Instance.SetMoveStartedAction(PressedInputMovement);
        InputManager.Instance.SetMovePerformedAction(PressedInputMovement);
        InputManager.Instance.SetMoveCanceledAction(CanceledMoveDirection);

        InputManager.Instance.Setting.Subscribe(x =>
        {
            if(IsActionCheck()) { return; }
            if (x == 1)
            {
                _isView = !_isView;
                if (_isView)
                {
                    // 表示処理
                    _root.SetActive(true);
                    _audioSettingFields.ForEach(x => x.UnSelect());
                    _audioSettingFields[_index].Select();
                    TitleLoadMainGame.Instance.StopBGM();
                }
                else
                {
                    // 非表示処理
                    _root.SetActive(false);
                    TitleLoadMainGame.Instance.PlayBGM();

                    if (_isPlaySound)
                    {
                        SoundManager.Instance.StopSE(_soundHash).Forget();
                        SoundManager.Instance.StopBGM(_soundHash).Forget();
                    }

                    _isPlaySound = false;
                }
            }
        }).AddTo(this);

        InputManager.Instance.Move.Subscribe(vec =>
        {
            if(IsActionCheck()) { return; }
            if(_isView == false) { return; }

            if (vec == Vector2.zero)
            {
                return;
            }

            // 音声の種類を選択
            if (Mathf.Abs(vec.y) == 1)
            {
                _audioSettingFields.ForEach(x => x.UnSelect());

                // Y軸の反転
                _index += (int)vec.y * -1;
                if (_index < 0)
                {
                    _index = _audioSettingFields.Count - 1;
                }
                else if (_index >= _audioSettingFields.Count)
                {
                    _index = 0;
                }

                _audioSettingFields[_index].Select();
            }
        }).AddTo(this);

        InputManager.Instance.Decision.Subscribe(x =>
        {
            if(IsActionCheck()) { return; }
            if(_isView == false) { return; }

            if (x != 1)
            {
                return;
            }

            ;

            if (_isPlaySound)
            {
                if (_soundHash != null)
                {
                    SoundManager.Instance.StopSE(_soundHash).Forget();
                    SoundManager.Instance.StopBGM(_soundHash).Forget();
                }
            }

            _isPlaySound = true;

            if (_audioSettingFields[_index].IsSE)
            {
                Debug.Log($"SE Play: {_audioSettingFields[_index].AudioName}");
                _soundHash = SoundManager.Instance.PlaySE(GetSEType(_audioSettingFields[_index].AudioName),
                    Camera.main.transform);
            }
            else
            {
                Debug.Log($"BGM Play: {_audioSettingFields[_index].AudioName}");
                _soundHash = SoundManager.Instance.PlayBGM(GetBGMType(_audioSettingFields[_index].AudioName),
                    Camera.main.transform);
            }

            _audioSettingFields[_index].SetPlaySound(true);
        }).AddTo(this);
    }

    public BGMType GetBGMType(string name)
    {
        return (BGMType)Enum.Parse(typeof(BGMType), name);
    }

    public SEType GetSEType(string name)
    {
        return (SEType)Enum.Parse(typeof(SEType), name);
    }

    #region スライダー調整用

    private void PressedInputMovement(InputAction.CallbackContext context)
    {
        _moveDirection = context.ReadValue<Vector2>();
    }

    private void CanceledMoveDirection(InputAction.CallbackContext context)
    {
        _moveDirection = Vector2.zero;
    }

    private void Update()
    {
        if(IsActionCheck()) { return; }

        if (Mathf.Abs(_moveDirection.x) != 1)
        {
            return;
        }

        _audioSettingFields[_index].MoveSlider((float)(_moveDirection.x * 0.01));
    }

    #endregion

    private bool IsActionCheck()
    {
        return LicenseGenerator.Instance.IsView == true ||
               TitleLoadMainGame.Instance.IsLoad == true;
    }
}
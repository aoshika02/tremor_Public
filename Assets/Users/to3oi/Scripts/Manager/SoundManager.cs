using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;


#if Amptix
using AmptixSdk;
using static AmptixSdk.AmptixManager;
#endif

[Serializable]
public class MixerGroup
{
    public AudioVolumeType AudioVolumeType;
    public AudioMixerGroup AudioMixerGroup;
}

public class SoundInfo
{
    public GameObject GameObject;
#if Amptix
    public AmptixSound AmptixSound;
#else
    public AudioSource AudioSource;
#endif
    public SoundHash SoundHash;
    public bool IsHaptics;
    public float HapticsBaseVolume = 1;
    public int HapticsPriority = 0;
}

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
#if Amptix
    private List<AmptixSound> _amptixSounds = new List<AmptixSound>();
#else
    private List<AudioSource> _audioSource = new List<AudioSource>();
#endif
    private Transform _soundRoot;
    private BGMType _bgmtype;
    private SEType _setype;

    List<SoundInfo> _playingSEAudioSources = new List<SoundInfo>();
    List<SoundInfo> _playingBGMAudioSources = new List<SoundInfo>();

    [SerializeField] private SEClips _seClips;

    [SerializeField] private BGMClips _bgmClips;
    [SerializeField] private List<MixerGroup> _mixerGroups;

    private int DeviceID = 0;

    protected override void Awake()
    {
        if (CheckInstance())
        {
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
            _soundRoot = new GameObject("SoundRoot").transform;
            DontDestroyOnLoad(_soundRoot);

            //適当な数を初期生成
            for (int i = 0; i < 5; i++)
            {
                CreateAudioSource();
            }

#if Amptix
            gameObject.AddComponent<AmptixManager>();
            AmptixManager.AddOnDeviceStateChangedListener(DeviceActive);
            void DeviceActive(int deviceID, DeviceEvent deviceEvent)
            {
                var b = AmptixManager.ActivateDevice(deviceID);
                DeviceID = deviceID;
                Debug.Log($"Device ID {deviceID} : {b}");
            }
#endif
        }
    }

    #region PlaySE

    public SoundHash PlaySE(SEType seType, float volume = 1, float hapVolume = 1)
    {
#if Amptix
        AmptixSound amptixSound = GetAmptixSoundSE();
#else
        AudioSource audioSource = GetAudioSourceSE();
#endif

        SEClip seClip = GetSEClip(seType);

        var soundHash = new SoundHash(seClip.Clip);

        if (SeClipNullCheck(seClip))
        {
            return null;
        }

#if Amptix
        SetAmptixSoundSE(ref amptixSound, seClip, volume, hapVolume, soundHash);
#else
        SetAudioClipSE(ref audioSource, seClip, volume, soundHash);
#endif

        var soundInfo = new SoundInfo
        {
#if Amptix
            AmptixSound = amptixSound,
#else
            AudioSource = audioSource,
#endif
            SoundHash = soundHash,
            IsHaptics = seClip.IsHaptics,
            HapticsPriority = seClip.HapticsPriority,
            HapticsBaseVolume = hapVolume
        };

        _playingSEAudioSources.Add(soundInfo);
        return soundInfo.SoundHash;
    }

    public SoundHash PlaySE(SEType seType, Transform parent, float volume = 1, float hapVolume = 1)
    {
#if Amptix
        AmptixSound amptixSound = GetAmptixSoundSE();
#else
        AudioSource audioSource = GetAudioSourceSE();
#endif
        
        SEClip seClip = GetSEClip(seType);

        var soundHash = new SoundHash(seClip.Clip);

        if (SeClipNullCheck(seClip))
        {
            return null;
        }
#if Amptix
        SetAmptixSoundSE(ref amptixSound, seClip, volume, hapVolume, soundHash, parent);
#else
        SetAudioClipSE(ref audioSource, seClip, volume, soundHash, parent);
#endif

        var soundInfo = new SoundInfo
        {
#if Amptix
            AmptixSound = amptixSound,
#else
            AudioSource = audioSource,
#endif
            SoundHash = soundHash,
            IsHaptics = seClip.IsHaptics,
            HapticsPriority = seClip.HapticsPriority,
            HapticsBaseVolume = hapVolume
        };

        _playingSEAudioSources.Add(soundInfo);
        return soundInfo.SoundHash;
    }

    public async UniTask PlaySEAsync(SEType seType, Transform parent, CancellationToken cancellationToken = default,
        float volume = 1, float hapVolume = 1)
    {
#if Amptix
        AmptixSound amptixSound = GetAmptixSoundSE();
#else
        AudioSource audioSource = GetAudioSourceSE();
#endif
        SEClip seClip = GetSEClip(seType);

        var soundHash = new SoundHash(seClip.Clip);

        if (SeClipNullCheck(seClip))
        {
            return;
        }

#if Amptix
        SetAmptixSoundSE(ref amptixSound, seClip, volume, hapVolume, soundHash, parent);
#else
        SetAudioClipSE(ref audioSource, seClip, volume, soundHash, parent);
#endif

        var soundInfo = new SoundInfo
        {
#if Amptix
            AmptixSound = amptixSound,
#else
            AudioSource = audioSource,
#endif
            SoundHash = soundHash,
            IsHaptics = seClip.IsHaptics,
            HapticsPriority = seClip.HapticsPriority,
            HapticsBaseVolume = hapVolume
        };

        _playingSEAudioSources.Add(soundInfo);
        try
        {
#if Amptix
            await UniTask.WaitUntil(() => !amptixSound.IsPlaying, cancellationToken: cancellationToken);
#else
            await UniTask.WaitUntil(() => !audioSource.isPlaying, cancellationToken: cancellationToken);
#endif
        }
        catch (OperationCanceledException e)
        {
            await StopSE(soundInfo.SoundHash);
        }

        return;
    }

    #endregion

    #region PlayBGM

    public SoundHash PlayBGM(BGMType bgmType, float volume = 1, float hapVolume = 1)
    {
#if Amptix
        AmptixSound amptixSound = GetAmptixSoundBGM();
#else
        AudioSource audioSource = GetAudioSourceBGM();
#endif
        BGMClip bgmClip = GetBGMClip(bgmType);

        var soundHash = new SoundHash(bgmClip.Clip);


        if (BGMClipNullCheck(bgmClip))
        {
            return null;
        }

#if Amptix
        SetAudioClipBGM(ref amptixSound, bgmClip, volume, soundHash);
        amptixSound.SetVolume(hapVolume);
#else
        SetAudioClipBGM(ref audioSource, bgmClip, volume, soundHash);
#endif

        var soundInfo = new SoundInfo
        {
#if Amptix
            AmptixSound = amptixSound,
#else
            AudioSource = audioSource,
#endif
            SoundHash = soundHash,
            IsHaptics = bgmClip.IsHaptics,
            HapticsPriority = bgmClip.HapticsPriority,
            HapticsBaseVolume = hapVolume
        };

        _playingBGMAudioSources.Add(soundInfo);
        return soundInfo.SoundHash;
    }

    public SoundHash PlayBGM(BGMType bgmType, Transform parent, float volume = 1, float hapVolume = 1)
    {
#if Amptix
        AmptixSound amptixSound = GetAmptixSoundBGM();
#else
        AudioSource audioSource = GetAudioSourceBGM();
#endif
        BGMClip bgmClip = GetBGMClip(bgmType);

        var soundHash = new SoundHash(bgmClip.Clip);

        if (BGMClipNullCheck(bgmClip))
        {
            return null;
        }

#if Amptix
        SetAudioClipBGM(ref amptixSound, bgmClip, volume, soundHash, parent);
        amptixSound.SetVolume(hapVolume);
#else
        SetAudioClipBGM(ref audioSource, bgmClip, volume, soundHash, parent);
#endif

        var soundInfo = new SoundInfo
        {
#if Amptix
            AmptixSound = amptixSound,
#else
            AudioSource = audioSource,
#endif
            SoundHash = soundHash,
            IsHaptics = bgmClip.IsHaptics,
            HapticsPriority = bgmClip.HapticsPriority,
            HapticsBaseVolume = hapVolume
        };

        _playingBGMAudioSources.Add(soundInfo);
        return soundInfo.SoundHash;
    }

    public async UniTask PlayBGMAsync(BGMType bgmType, Transform parent, CancellationToken cancellationToken = default,
        float volume = 1, float hapVolume = 1)
    {
#if Amptix
        AmptixSound amptixSound = GetAmptixSoundBGM();
        amptixSound.SetVolume(hapVolume);
#else
        AudioSource audioSource = GetAudioSourceBGM();
#endif
        BGMClip bgmClip = GetBGMClip(bgmType);

        var soundHash = new SoundHash(bgmClip.Clip);

        if (BGMClipNullCheck(bgmClip))
        {
            return;
        }

#if Amptix
        SetAudioClipBGM(ref amptixSound, bgmClip, volume, soundHash);
#else
        SetAudioClipBGM(ref audioSource, bgmClip, volume, soundHash);
#endif

        var soundInfo = new SoundInfo
        {
#if Amptix
            AmptixSound = amptixSound,
#else
            AudioSource = audioSource,
#endif
            SoundHash = soundHash,
            IsHaptics = bgmClip.IsHaptics,
            HapticsPriority = bgmClip.HapticsPriority,
            HapticsBaseVolume = hapVolume
        };

        _playingBGMAudioSources.Add(soundInfo);

        try
        {
#if Amptix
            await UniTask.WaitUntil(() => !amptixSound.IsPlaying, cancellationToken: cancellationToken);
#else
            await UniTask.WaitUntil(() => !audioSource.isPlaying, cancellationToken: cancellationToken);
#endif
        }
        catch (OperationCanceledException e)
        {
            await StopBGM(soundInfo.SoundHash);
        }
    }

    #endregion

    #region Stop

    public async UniTask StopSE(SoundHash soundHash, float fadeTime = 0.25f)
    {
        SoundInfo asInfo = null;
        for (int i = 0; i < _playingSEAudioSources.Count; i++)
        {
            if (_playingSEAudioSources[i].SoundHash == soundHash &&

#if Amptix
                _playingSEAudioSources[i].AmptixSound.IsPlaying)
#else
                _playingSEAudioSources[i].AudioSource.isPlaying)
#endif

            {
                asInfo = _playingSEAudioSources[i];
#if Amptix
                await DOVirtual.Float(asInfo.AmptixSound.AudioSource.volume, 0, fadeTime, v =>
#else
                await DOVirtual.Float(asInfo.AudioSource.volume, 0, fadeTime, v =>
#endif
                {
#if Amptix
                    asInfo.AmptixSound.AudioSource.volume = v;
#else
                    asInfo.AudioSource.volume = v;
#endif

                    if (v == 0)
                    {
#if Amptix
                        asInfo.AmptixSound.AudioSource.Stop();
#else
                        asInfo.AudioSource.Stop();
#endif
                        if (asInfo != null)
                        {
                            RemoveSoundInfoSE(asInfo.SoundHash);
                        }
                    }
                }).ToUniTask();
            }
        }
    }

    public async UniTask StopBGM(SoundHash soundHash, float fadeTime = 0.25f)
    {
        SoundInfo asInfo = null;
        for (int i = 0; i < _playingBGMAudioSources.Count; i++)
        {
            if (_playingBGMAudioSources[i].SoundHash.SoundHashID == soundHash.SoundHashID &&
#if Amptix
                _playingBGMAudioSources[i].AmptixSound.AudioSource.isPlaying)
#else
                _playingBGMAudioSources[i].AudioSource.isPlaying)
#endif
            {
                asInfo = _playingBGMAudioSources[i];
#if Amptix
                await DOVirtual.Float(asInfo.AmptixSound.AudioSource.volume, 0, fadeTime, v =>
#else
                await DOVirtual.Float(asInfo.AudioSource.volume, 0, fadeTime, v =>
#endif
                {
#if Amptix
                    asInfo.AmptixSound.AudioSource.volume = v;
#else
                    asInfo.AudioSource.volume = v;
#endif
                    if (v == 0)
                    {
#if Amptix
                        asInfo.AmptixSound.AudioSource.Stop();
#else
                        asInfo.AudioSource.Stop();
#endif
                        if (asInfo != null)
                        {
                            RemoveSoundInfoBGM(asInfo.SoundHash);
                        }
                    }
                }).ToUniTask();
            }
        }
    }

    public async UniTask AllStopSE()
    {
        List<UniTask> task = new List<UniTask>();
        foreach (var asInfo in _playingSEAudioSources)
        {
            task.Add(
#if Amptix
                DOVirtual.Float(asInfo.AmptixSound.AudioSource.volume, 0, 0.25f, v =>
#else
                DOVirtual.Float(asInfo.AudioSource.volume, 0, 0.25f, v =>
#endif

                {
#if Amptix
                    asInfo.AmptixSound.AudioSource.volume = v;
#else
                    asInfo.AudioSource.volume = v;
#endif

                    if (v == 0)
                    {
#if Amptix
                        asInfo.AmptixSound.AudioSource.Stop();
#else
                        asInfo.AudioSource.Stop();
#endif
                        if (asInfo != null)
                        {
                            RemoveSoundInfoSE(asInfo.SoundHash);
                        }
                    }
                }).ToUniTask());
        }

        await UniTask.WhenAll(task);
    }

    public async UniTask AllStopBGM()
    {
        List<UniTask> task = new List<UniTask>();
        foreach (var asInfo in _playingBGMAudioSources)
        {
            task.Add(
#if Amptix
                DOVirtual.Float(asInfo.AmptixSound.AudioSource.volume, 0, 0.25f, v =>
#else
                DOVirtual.Float(asInfo.AudioSource.volume, 0, 0.25f, v =>
#endif
                {
#if Amptix
                    asInfo.AmptixSound.AudioSource.volume = v;
#else
                    asInfo.AudioSource.volume = v;
#endif
                    if (v == 0)
                    {
#if Amptix
                        asInfo.AmptixSound.AudioSource.Stop();
#else
                        asInfo.AudioSource.Stop();
#endif
                        if (asInfo != null)
                        {
                            RemoveSoundInfoBGM(asInfo.SoundHash);
                        }
                    }
                }).ToUniTask());
        }

        await UniTask.WhenAll(task);
    }

    #endregion

    #region GetAudioSource

#if Amptix
    public void SetAmptixSoundSE(ref AmptixSound amptixSound, SEClip seClip, float volume, float hapVolume, SoundHash soundHash, Transform parent
 = null)
    {
        if (seClip.IsHaptics)
        {
            amptixSound.SetData(seClip.AmptixEffectData);
            amptixSound.SetVolume(hapVolume);
        }
        amptixSound.AudioSource.clip = seClip?.Clip;
        amptixSound.AudioSource.volume = volume;
        amptixSound.AudioSource.spatialBlend = Convert.ToInt32(seClip.Is3D);
        if (seClip.Is3D)
        {
            amptixSound.AudioSource.transform.parent = parent;
            amptixSound.AudioSource.transform.localPosition = Vector3.zero;
        }
        else
        {
            amptixSound.AudioSource.transform.localPosition = Vector3.zero;
            amptixSound.AudioSource.transform.parent = null;
            amptixSound.AudioSource.transform.localPosition = Vector3.zero;
        }

        if (seClip.IsHaptics)
        {
            amptixSound.AudioSource.outputAudioMixerGroup = GetAudioMixerGroup(seClip.AudioVolumeType);
            amptixSound.Play(DeviceID);
            UpdateHapticsPrioritySE();
        }
        else
        {
            amptixSound.AudioSource.outputAudioMixerGroup = GetAudioMixerGroup(seClip.AudioVolumeType);
            amptixSound.AudioSource.Play();
        }

        var observer = amptixSound.GetComponent<AudiosourceObserver>();
        observer.Play(soundHash, AudiosourceObserver.SoundType.SE);
    }
#else
    public void SetAudioClipSE(ref AudioSource audioSource, SEClip seClip, float volume, SoundHash soundHash,
        Transform parent = null)
    {
        audioSource.clip = seClip?.Clip;
        audioSource.volume = volume;
        audioSource.spatialBlend = Convert.ToInt32(seClip.Is3D);
        if (seClip.Is3D)
        {
            audioSource.transform.parent = parent;
            audioSource.transform.localPosition = Vector3.zero;
        }
        else
        {
            audioSource.transform.localPosition = Vector3.zero;
            audioSource.transform.parent = null;
            audioSource.transform.localPosition = Vector3.zero;
        }

        audioSource.outputAudioMixerGroup = GetAudioMixerGroup(seClip.AudioVolumeType);
        audioSource.Play();

        var observer = audioSource.GetComponent<AudiosourceObserver>();
        observer.Play(soundHash, AudiosourceObserver.SoundType.SE);
    }
#endif
#if Amptix
    public void SetAudioClipBGM(ref AmptixSound amptixSound, BGMClip bgmClip, float volume, SoundHash soundHash, Transform parent
 = null)
    {
        if (bgmClip.IsHaptics)
        {
            amptixSound.SetData(bgmClip.AmptixEffectData);
        }
        amptixSound.AudioSource.clip = bgmClip?.Clip;
        amptixSound.AudioSource.volume = volume;
        amptixSound.AudioSource.spatialBlend = Convert.ToInt32(bgmClip.Is3D);
        if (bgmClip.Is3D)
        {
            amptixSound.AudioSource.transform.parent = parent;
            amptixSound.AudioSource.transform.localPosition = Vector3.zero;
        }
        else
        {
            amptixSound.AudioSource.transform.localPosition = Vector3.zero;
            amptixSound.AudioSource.transform.parent = null;
            amptixSound.AudioSource.transform.localPosition = Vector3.zero;
        }

        if (bgmClip.IsHaptics)
        {
            amptixSound.AudioSource.outputAudioMixerGroup = GetAudioMixerGroup(bgmClip.AudioVolumeType);
            amptixSound.Play(DeviceID);
            UpdateHapticsPrioritySE();
        }
        else
        {
            amptixSound.AudioSource.outputAudioMixerGroup = GetAudioMixerGroup(bgmClip.AudioVolumeType);
            amptixSound.AudioSource.Play();
        }

        var observer = amptixSound.GetComponent<AudiosourceObserver>();
        observer.Play(soundHash, AudiosourceObserver.SoundType.SE);
    }
#else
    public void SetAudioClipBGM(ref AudioSource audioSource, BGMClip bgmClip, float volume, SoundHash soundHash,
        Transform parent = null)
    {
        audioSource.clip = bgmClip?.Clip;
        audioSource.volume = volume;
        audioSource.spatialBlend = Convert.ToInt32(bgmClip.Is3D);
        if (bgmClip.Is3D)
        {
            audioSource.transform.parent = parent;
            audioSource.transform.localPosition = Vector3.zero;
        }
        else
        {
            audioSource.transform.localPosition = Vector3.zero;
            audioSource.transform.parent = null;
            audioSource.transform.localPosition = Vector3.zero;
        }

        audioSource.outputAudioMixerGroup = GetAudioMixerGroup(bgmClip.AudioVolumeType);
        audioSource.Play();

        var observer = audioSource.GetComponent<AudiosourceObserver>();
        observer.Play(soundHash, AudiosourceObserver.SoundType.SE);
    }
#endif

#if Amptix
    private AmptixSound GetAmptixSoundSE()
    {
        var amptix = GetAmptixSound();
        amptix.AudioSource.loop = false;
        amptix.AudioSource.volume = 1.0f;
        return amptix;
    }
#else
    private AudioSource GetAudioSourceSE()
    {
        var audioSource = GetAudioSource();
        audioSource.loop = false;
        audioSource.volume = 1.0f;
        return audioSource;
    }
#endif

#if Amptix
    private AmptixSound GetAmptixSoundBGM()
    {
        var amptix = GetAmptixSound();
        amptix.AudioSource.loop = true;
        amptix.AudioSource.volume = 0.1f;
        return amptix;
    }
#else
    private AudioSource GetAudioSourceBGM()
    {
        var audioSource = GetAudioSource();
        audioSource.loop = true;
        audioSource.volume = 0.1f;
        return audioSource;
    }
#endif

#if Amptix
    private AmptixSound GetAmptixSound()
    {

         for (int i = 0; i < _amptixSounds.Count; i++)
        {
            AmptixSound amptix = _amptixSounds[i];
            if (amptix == null)
            {
            }else if (amptix != null && !amptix.AudioSource.isPlaying)
            {
                amptix.AudioSource.volume = 1;
                return amptix;
            }
        }

        _amptixSounds.RemoveAll(x => x == null || x.AudioSource == null);

        return CreateAudioSource();
    }
#else
    private AudioSource GetAudioSource()
    {
        for (int i = 0; i < _audioSource.Count; i++)
        {
            AudioSource audioSource = _audioSource[i];
            if (audioSource == null)
            {
            }
            else if (audioSource != null && !audioSource.isPlaying)
            {
                audioSource.volume = 1;
                return audioSource;
            }
        }

        _audioSource.RemoveAll(x => x == null);

        return CreateAudioSource();
    }
#endif
#if Amptix
    private AmptixSound CreateAudioSource()
    {
        var amptixSound = AmptixSound.Create();
        amptixSound.transform.parent = _soundRoot.transform;
        amptixSound.AudioSource.playOnAwake = false;
        amptixSound.gameObject.AddComponent<AudiosourceObserver>();
        _amptixSounds.Add(amptixSound);
        return amptixSound;
    }
#else
    private AudioSource CreateAudioSource()
    {
        var g = new GameObject("AudioSource");
        g.transform.parent = _soundRoot.transform;
        var audioSource = g.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.gameObject.AddComponent<AudiosourceObserver>();
        _audioSource.Add(audioSource);
        return audioSource;
    }
#endif

    #endregion

    #region Null

    private static bool BGMClipNullCheck(BGMClip bgmClip)
    {
        if (bgmClip != null) return false;
        Debug.LogError($"{bgmClip.BGMType}にClipが設定されていません");
        return true;
    }

    private static bool SeClipNullCheck(SEClip seClip)
    {
        if (seClip != null) return false;
        Debug.LogError($"{seClip.SEType}にClipが設定されていません");
        return true;
    }

    #endregion

    #region Get

    private SEClip GetSEClip(SEType seType)
    {
        SEClip seClip = null;

        for (int i = 0; i < _seClips.SEClipList.Count; i++)
        {
            if (_seClips.SEClipList[i].SEType == seType)
            {
                seClip = _seClips.SEClipList[i];
                break;
            }
        }

        return seClip;
    }

    private BGMClip GetBGMClip(BGMType bgmType)
    {
        BGMClip bgmClip = null;

        for (int i = 0; i < _bgmClips.BGMClipList.Count; i++)
        {
            if (_bgmClips.BGMClipList[i].BGMType == bgmType)
            {
                bgmClip = _bgmClips.BGMClipList[i];
                break;
            }
        }

        return bgmClip;
    }

    private AudioMixerGroup GetAudioMixerGroup(AudioVolumeType audioType)
    {
        var mixer = _mixerGroups.FirstOrDefault(x => x.AudioVolumeType == audioType);
        return mixer!.AudioMixerGroup;
    }

    #endregion

    #region Remove

    public void RemoveSoundInfoSE(SoundHash soundHash)
    {
        _playingSEAudioSources.RemoveAll(x =>
        {
            if (x.SoundHash == soundHash)
            {
#if Amptix
                x.AmptixSound.transform.parent = _soundRoot;
#else
                x.AudioSource.transform.parent = _soundRoot;
#endif
            }

            return x.SoundHash == soundHash;
        });
    }

    public void RemoveSoundInfoBGM(SoundHash soundHash)
    {
        _playingBGMAudioSources.RemoveAll(x =>
        {
            if (x.SoundHash == soundHash)
            {
#if Amptix
                x.AmptixSound.transform.parent = _soundRoot;
#else
                x.AudioSource.transform.parent = _soundRoot;
#endif
            }

            return x.SoundHash == soundHash;
        });
    }

    #endregion

    #region Update

    public void UpdateHapticsPrioritySE()
    {
#if Amptix
        //Priorityの数
        var priorityList = _playingSEAudioSources.Where(x => x.IsHaptics).Select(x => x.HapticsPriority).Distinct();
        var prioritySum = priorityList.Sum();
            
        foreach (var soundInfo in _playingSEAudioSources.Where(x => x.IsHaptics))
        {
            var volume = (float)soundInfo.HapticsPriority / (float)prioritySum * soundInfo.HapticsBaseVolume;
            soundInfo.AmptixSound.SetVolume(volume);
        }
#endif
    }

    #endregion
}
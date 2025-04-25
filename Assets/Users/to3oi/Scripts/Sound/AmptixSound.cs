#if Amptix
using AmptixSdk;
using UnityEngine;

public class AmptixSound : AmptixEffectSolo
{
    protected AudioSource _audioSource;
    public AudioSource AudioSource => _audioSource;
    public new static AmptixSound Create()
    {
        GameObject gameObject = new GameObject("AmptixSound");
        var amptix = gameObject.AddComponent<AmptixSound>();
        amptix.Setup();
        return amptix;
    }

    public void Setup()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();
    }

    #region PublicFunctions

    /// <summary>
    /// Play Amptix effect on targetDevice.
    /// </summary>
    /// <param name="playerId">Target player id. (0-3)</param>
    public override void Play(int playerId)
    {
        base.Play(playerId);
        if (IsPlaying && _audioSource != null && _audioSource.clip.length > GetCurrentTime())
        {
            _audioSource.loop = Loop;
            _audioSource.time = GetCurrentTime();
            _audioSource.Play();
        }
    }

    /// <summary>
    /// Stop playing Amptix effect.
    /// </summary>
    public override void Stop()
    {
        base.Stop();
        if (_audioSource != null)
        {
            _audioSource.Stop();
        }
    }

    /// <summary>
    /// Pause playing Amptix effect.
    /// </summary>
    public override void Pause()
    {
        base.Pause();
        if (_audioSource != null)
        {
            _audioSource.Pause();
        }
    }

    /// <summary>
    /// Seek AmptixEffect to time(sec).
    /// </summary>
    /// <param name="time">Target time(sec).</param>
    public override void SetCurrentTime(float time)
    {
        base.SetCurrentTime(time);
        if (_audioSource.clip.length > time)
        {
            _audioSource.time = time;
        }
        else
        {
            _audioSource.Stop();
        }
    }
    #endregion
}
#endif
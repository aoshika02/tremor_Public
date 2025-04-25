using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class AudiosourceObserver : MonoBehaviour
{
    public enum SoundType
    {
        SE = 0,
        BGM
    }
    private AudioSource audioSource;
    private bool isAudioPlay = false;
    private SoundHash soundHash;
    private SoundType soundType;
    private void Awake()
    {
        audioSource  = GetComponent<AudioSource>();
    }

    public void Play(SoundHash soundHash , SoundType soundType)
    {
        this.isAudioPlay = true;
        this.soundHash = soundHash;
        this.soundType = soundType;
    }
    void Update()
    {
        if (!isAudioPlay) { return; }
        if (!audioSource.isPlaying)
        {
            isAudioPlay = false;
            if(soundType == SoundType.SE)
            {
                SoundManager.Instance.RemoveSoundInfoSE(soundHash);
            }else if(soundType == SoundType.BGM)
            {
                SoundManager.Instance.RemoveSoundInfoBGM(soundHash);
            }
        }
    }
}

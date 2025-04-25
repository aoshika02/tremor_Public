using System.Security.Cryptography;
using UnityEngine;

public class SoundHash
{
    public SHA1 SoundHashID { get; set; }
    public AudioClip AudioClip { get; set; }
    public SoundHash(AudioClip clip)
    {
        AudioClip = clip;
        SoundHashID = SHA1.Create();
    }
    public float GetClipTime => this.AudioClip.length;
}
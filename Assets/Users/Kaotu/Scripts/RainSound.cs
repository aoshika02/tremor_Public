using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainSound : MonoBehaviour
{
    public void PlayAt(Vector3 position)
    {
        SoundManager.Instance.PlaySE(SEType.Water_Drop02);
    }
}

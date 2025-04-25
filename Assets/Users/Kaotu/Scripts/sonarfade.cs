using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sonarfade : MonoBehaviour
{
  

    public async void FadeIn()
    {
        await FadeInOut.FadeIn(0f);
    }

    public async void FadeOut()
    {
        await FadeInOut.FadeOut(0f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
   public async void fadein()
    {
        await FadeInOut.FadeIn();
    }
}

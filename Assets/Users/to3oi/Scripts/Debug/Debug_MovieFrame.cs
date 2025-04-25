using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Debug_MovieFrame : MonoBehaviour
{
    private async UniTask Start()
    {
        
        /*
        await MovieFrame.Instance.FrameIn();
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        await MovieFrame.Instance.FrameOut();
    */
    }

    private void OnGUI()
    {
        if (GUILayout.Button("FrameIn"))
        {
             MovieFrame.Instance.FrameIn();
        }

        if (GUILayout.Button("FrameOut"))
        {
            MovieFrame.Instance.FrameOut();
        }
    }
}

using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestRenderMaterial : MonoBehaviour
{
    private async void Start()
    {
        await UniTask.WaitForSeconds(2);
        for (int i = 0; i < 16; i++) {
            MapHideOpen.Instance.OpenMap((RoomType)i);
            await UniTask.WaitForSeconds(2);
        }
    }

    //private async UniTask ChangeValue(Material material ,float duration)
    private async UniTask ChangeValue(RawImage rawImage ,float duration)
    {
        rawImage.material = Instantiate(rawImage.material);

        await DOVirtual.Float(0, 1, duration, f =>
        {
            rawImage.material.SetFloat("_border", f);
        }).ToUniTask();
    }
}

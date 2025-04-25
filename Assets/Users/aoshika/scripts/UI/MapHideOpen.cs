using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapHideOpen : SingletonMonoBehaviour<MapHideOpen>
{
    [Serializable]
    class RoomParam 
    {
        public RawImage MapHideImage;
        public RoomType RoomType;
    }
    [SerializeField] private List<RoomParam> _roomParams = new List<RoomParam>();
    private int _duration = 2;
    public void OpenMap(RoomType roomType) 
    {
        for(int i=0;i< _roomParams.Count; i++) 
        {
            if (_roomParams[i].RoomType== roomType) 
            {
                _roomParams[i].MapHideImage.material = Instantiate(_roomParams[i].MapHideImage.material);
                ChangeValue(_roomParams[i].MapHideImage, _duration).Forget();
                break;
            }
        }
    }
    private async UniTask ChangeValue(RawImage rawImage, float duration)
    {
        rawImage.material = Instantiate(rawImage.material);

        await DOVirtual.Float(0, 1, duration, f =>
        {
            rawImage.material.SetFloat("_border", f);
        }).ToUniTask(cancellationToken:destroyCancellationToken);
    }
}
public enum RoomType 
{
    Start,
    Entrance,
    MusicRoom,
    ScienceRoom,
    SciencePreparationRoom,
    Toilet_1,
    Toilet_2,
    Room_1,
    Room_2,
    Room_3,
    Room_4,
    Room_5,
    Corridor_1,
    Corridor_2, 
    Corridor_3,
    Corridor_4,
}

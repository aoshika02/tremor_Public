using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable,DisplayName("PlaySeTimeline")]
public class SeSignal :Marker,INotification
{
    public int Index;

    public PropertyName id
    {
        get
        {
            return new PropertyName("GetIndex");
        }
    }
}



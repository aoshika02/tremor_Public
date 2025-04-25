using UnityEngine;
using UnityEngine.Playables;

public class SeSignakReceiver : MonoBehaviour,INotificationReceiver
{
    public void OnNotify(Playable origin, INotification notification, object context)
    {
        var element = notification as SeSignal;
        if (element == null)
            return;

       Debug.Log($"element.Index {element.Index}");
    }
} 


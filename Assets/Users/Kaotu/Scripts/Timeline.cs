using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class Timeline : MonoBehaviour
{
    [SerializeField] private TimelineAsset[] timelines;

    private PlayableDirector director;
    void Start()
    {
        director = this.GetComponent<PlayableDirector>();
    }

   public void TimelineStart(int i)
    {
        director.Play(timelines[i]);
    }
}

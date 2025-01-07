using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineController : MonoBehaviour
{
    public PlayableDirector timelineDirector;

    void Start()
    {
        PlayTimeline();
    }

    public void PlayTimeline()
    {
        if(timelineDirector != null)
        {
            timelineDirector.Play();
        }
        else
        {
            Debug.LogError("PlayableDirector가 할당되지 않았음");
        }
    }

    public void PauseTimeline()
    {
        if (timelineDirector != null)
        {
            timelineDirector.Pause();
        }
    }

    public void StopTimeline()
    {
        if(timelineDirector != null)
        {
            timelineDirector.Stop();
        }
    }
    void Update()
    {
        
    }
}

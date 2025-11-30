using UnityEngine;
using UnityEngine.Playables;

public class TimelineGameplayStarter : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The PlayableDirector playing the StartingTimeline")]
    public PlayableDirector playableDirector;

    [Tooltip("The Ayan player GameObject to enable after the timeline finishes")]
    public GameObject ayanPlayer;

    [Tooltip("The CameraManager GameObject to enable after the timeline finishes")]
    public GameObject cameraManager;

    void OnEnable()
    {
        if (playableDirector != null)
        {
            playableDirector.stopped += OnTimelineStopped;
        }
    }

    void OnDisable()
    {
        if (playableDirector != null)
        {
            playableDirector.stopped -= OnTimelineStopped;
        }
    }

    void OnTimelineStopped(PlayableDirector director)
    {
        if (director == playableDirector)
        {
            EnableGameplay();
        }
    }

    void EnableGameplay()
    {
        if (ayanPlayer != null)
        {
            ayanPlayer.SetActive(true);
        }

        if (cameraManager != null)
        {
            cameraManager.SetActive(true);
        }
    }
}

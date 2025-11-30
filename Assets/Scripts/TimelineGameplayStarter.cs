using UnityEngine;
using UnityEngine.Playables;

public class TimelineGameplayStarter : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The PlayableDirector playing the StartingTimeline")]
    [SerializeField] private PlayableDirector playableDirector;

    [Tooltip("The Ayan player GameObject to enable after the timeline finishes")]
    [SerializeField] private GameObject ayanPlayer;

    [Tooltip("The CameraManager GameObject to enable after the timeline finishes")]
    [SerializeField] private GameObject cameraManager;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = true;

    void OnEnable()
    {
        if (playableDirector != null)
        {
            playableDirector.stopped += OnTimelineStopped;
        }
        else if (debugLogs)
        {
            Debug.LogWarning("[TimelineGameplayStarter] PlayableDirector is not assigned. Timeline completion will not be detected.");
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
        if (debugLogs) Debug.Log("[TimelineGameplayStarter] Timeline finished. Enabling gameplay...");

        if (ayanPlayer != null)
        {
            ayanPlayer.SetActive(true);
            if (debugLogs) Debug.Log("[TimelineGameplayStarter] Ayan player enabled.");
        }
        else if (debugLogs)
        {
            Debug.LogWarning("[TimelineGameplayStarter] Ayan player reference is not assigned.");
        }

        if (cameraManager != null)
        {
            cameraManager.SetActive(true);
            if (debugLogs) Debug.Log("[TimelineGameplayStarter] CameraManager enabled.");
        }
        else if (debugLogs)
        {
            Debug.LogWarning("[TimelineGameplayStarter] CameraManager reference is not assigned.");
        }

        if (debugLogs) Debug.Log("[TimelineGameplayStarter] Gameplay started successfully.");
    }
}

using UnityEngine;
using UnityEngine.Playables;

public class CutsceneTrigger : MonoBehaviour
{
    [Header("Timeline")]
    [SerializeField] private PlayableDirector playableDirector;
    
    [Header("Settings")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool playOnce = true;
    [SerializeField] private bool disablePlayerControl = true;
    
    [Header("Cinemachine")]
    [SerializeField] private GameObject cinemachineBrain; // Main-Brain GameObject
    [SerializeField] private GameObject[] cinemachineCameras; // 1, 2, 3, 4 camera GameObjects
    
    [Header("Optional")]
    [SerializeField] private GameObject[] uiToHide;
    [SerializeField] private MonoBehaviour[] scriptsToDisable; // e.g., player controller
    
    private bool hasPlayed = false;

    private void Start()
    {
        // Ensure timeline doesn't play on awake
        if (playableDirector != null)
        {
            playableDirector.playOnAwake = false;
            playableDirector.stopped += OnTimelineFinished;
        }

        // Disable Cinemachine Brain at start
        if (cinemachineBrain != null)
        {
            cinemachineBrain.SetActive(false);
        }

        // Disable all Cinemachine cameras at start
        foreach (GameObject cam in cinemachineCameras)
        {
            if (cam != null)
            {
                cam.SetActive(false);
            }
        }
    }

    private void OnDestroy()
    {
        if (playableDirector != null)
        {
            playableDirector.stopped -= OnTimelineFinished;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (playOnce && hasPlayed) return;

        PlayCutscene();
    }

    private void PlayCutscene()
    {
        hasPlayed = true;

        // Enable Cinemachine Brain
        if (cinemachineBrain != null)
        {
            cinemachineBrain.SetActive(true);
        }

        // Enable all Cinemachine cameras
        foreach (GameObject cam in cinemachineCameras)
        {
            if (cam != null)
            {
                cam.SetActive(true);
            }
        }

        // Hide UI elements
        foreach (GameObject ui in uiToHide)
        {
            if (ui != null)
                ui.SetActive(false);
        }

        // Disable player control scripts
        if (disablePlayerControl)
        {
            foreach (MonoBehaviour script in scriptsToDisable)
            {
                if (script != null)
                    script.enabled = false;
            }
        }

        // Play the timeline
        if (playableDirector != null)
        {
            playableDirector.Play();
        }
    }

    private void OnTimelineFinished(PlayableDirector director)
    {
        // Disable Cinemachine Brain after cutscene
        if (cinemachineBrain != null)
        {
            cinemachineBrain.SetActive(false);
        }

        // Disable all Cinemachine cameras after cutscene
        foreach (GameObject cam in cinemachineCameras)
        {
            if (cam != null)
            {
                cam.SetActive(false);
            }
        }

        // Re-enable UI elements
        foreach (GameObject ui in uiToHide)
        {
            if (ui != null)
                ui.SetActive(true);
        }

        // Re-enable player control scripts
        if (disablePlayerControl)
        {
            foreach (MonoBehaviour script in scriptsToDisable)
            {
                if (script != null)
                    script.enabled = true;
            }
        }

        // Optional: Disable the trigger so it can't be triggered again
        if (playOnce)
        {
            gameObject.SetActive(false);
        }
    }
}

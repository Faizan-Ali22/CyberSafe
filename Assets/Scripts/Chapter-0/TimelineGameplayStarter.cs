using UnityEngine;
using UnityEngine.Playables;
using System.Collections;
using System.Collections.Generic;   

public class TimelineGameplayStarter : MonoBehaviour
{
    [Header("Timeline")]
    [SerializeField] private PlayableDirector playableDirector;
    
    [Header("Components to Enable")]
    [SerializeField] private MonoBehaviour[] componentsToEnable;

    [Header("GameObjects")]
    [SerializeField] private GameObject gameplayCharacter;
    [SerializeField] private GameObject cutsceneCharacter;
    [SerializeField] private GameObject cutsceneCharacter2;
    [SerializeField] private GameObject cinemachineBrain;
    public GameObject CineCamera;

    [Header("UI")]
    [SerializeField] private GameObject[] gameplayUIElements;
    [SerializeField] private GameObject skipButton;

    [Header("Cameras")]
    public GameObject cameramanager;
    [SerializeField] private Camera gameplayCamera;
    [SerializeField] private Camera timelineCamera;

    [Header("Player Scripts")]
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private InputManager inputManager;

    private bool hasSkipped = false;

    private void Awake()
    {
        SetComponentsEnabled(false);
        
        // Disable player scripts during cutscene
        SetPlayerScriptsEnabled(false);
        
        if (gameplayCharacter != null)
        {
            gameplayCharacter.SetActive(false);
        }

        SetUIActive(false);
        
        if (gameplayCamera != null)
        {
            gameplayCamera.enabled = false;
        }

        // Ensure skip button is visible at start
        if (skipButton != null)
        {
            skipButton.SetActive(true);
        }
    }

    private void OnEnable()
    {
        if (playableDirector != null)
        {
            playableDirector.stopped += OnTimelineStopped;
        }
    }

    private void OnDisable()
    {
        if (playableDirector != null)
        {
            playableDirector.stopped -= OnTimelineStopped;
        }
    }

    // Call this method from the Skip Button's OnClick event
    public void SkipCutscene()
    {
        if (hasSkipped) return;
        hasSkipped = true;

        if (playableDirector != null)
        {
            // Fast forward to end to apply any final state
            playableDirector.time = playableDirector.duration;
            playableDirector.Evaluate();
            playableDirector.Stop();
        }

        // Hide skip button
        if (skipButton != null)
        {
            skipButton.SetActive(false);
        }

        // Transition to gameplay
        TransitionToGameplay();

        #if UNITY_EDITOR
        Debug.Log("Cutscene skipped!");
        #endif
    }

    private void OnTimelineStopped(PlayableDirector director)
    {
        // Hide skip button when timeline ends naturally
        if (skipButton != null)
        {
            skipButton.SetActive(false);
        }

        if (!hasSkipped)
        {
            TransitionToGameplay();
        }

        #if UNITY_EDITOR
        Debug.Log("Timeline ended - Gameplay started!");
        #endif
    }

    private void TransitionToGameplay()
    {
        SetComponentsEnabled(true);
        SetGameplayActive(true);
        SetCutsceneActive(false);
        SetUIActive(true);
        
        if (timelineCamera != null)
        {
            timelineCamera.enabled = false;
        }
        
        // Enable player scripts and reinitialize camera after a short delay
        StartCoroutine(EnablePlayerAndCamera());
    }

    private IEnumerator EnablePlayerAndCamera()
    {
        // Wait one frame to ensure all GameObjects are properly activated
        yield return null;
        
        // Enable player scripts
        SetPlayerScriptsEnabled(true);
        
        // Reinitialize camera manager to find the player
        ReinitializeCameraManager();
    }

    private void SetPlayerScriptsEnabled(bool enabled)
    {
        if (playerManager != null)
        {
            playerManager.enabled = enabled;
        }
        
        if (inputManager != null)
        {
            inputManager.enabled = enabled;
        }
    }

    private void ReinitializeCameraManager()
    {
        if (cameramanager == null) return;
    
        var camManager = cameramanager.GetComponent<CameraManager>();
        if (camManager != null)
        {
            // Call InitializeCamera if it exists, otherwise use SendMessage
            camManager.SendMessage("InitializeCamera", SendMessageOptions.DontRequireReceiver);
        }
    }

    private void SetComponentsEnabled(bool enabled)
    {
        if (componentsToEnable == null) return;
           
        foreach (var component in componentsToEnable)
        {
            if (component != null)
            {
                component.enabled = enabled;
            }
        }
    }

    private void SetUIActive(bool active)
    {
        if (gameplayUIElements == null) return;

        foreach (var uiElement in gameplayUIElements)
        {
            if (uiElement != null)
            {
                uiElement.SetActive(active);
            }
        }
    }

    private void SetGameplayActive(bool active)
    {
        if (gameplayCharacter != null)
        {
            gameplayCharacter.SetActive(active);
        }

        if (gameplayCamera != null)
        {
            gameplayCamera.enabled = active;
        }

        if (cameramanager != null)
        {
            cameramanager.SetActive(active);
        }
    }

    private void SetCutsceneActive(bool active)
    {
        if (cutsceneCharacter != null)
        {
            cutsceneCharacter.SetActive(active);
        }
        if (cutsceneCharacter2 != null)
        {
            cutsceneCharacter2.SetActive(active);
        }
        if (CineCamera != null)
        {
            CineCamera.SetActive(active);
        }
        if (cinemachineBrain != null)
        {
            cinemachineBrain.SetActive(active);
        }
    }
}

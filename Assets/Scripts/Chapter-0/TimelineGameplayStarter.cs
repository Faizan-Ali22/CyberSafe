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

    [Header("Cameras")]
    public GameObject cameramanager;
    [SerializeField] private Camera gameplayCamera;
    [SerializeField] private Camera timelineCamera;

    private void Awake()
    {
        SetComponentsEnabled(false);
        
        if (gameplayCharacter != null)
        {
            gameplayCharacter.SetActive(false);
        }

        SetUIActive(false);
        
        if (gameplayCamera != null)
        {
            gameplayCamera.enabled = false;
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

    private void OnTimelineStopped(PlayableDirector director)
    {
        SetComponentsEnabled(true);
        
        if (gameplayCharacter != null)
        {
            gameplayCharacter.SetActive(true);
        }
        
        if (cutsceneCharacter != null)
        {
            cutsceneCharacter.SetActive(false);
        }
        if (cutsceneCharacter2 != null)
        {
            cutsceneCharacter2.SetActive(false);
        }
        if (CineCamera != null)
        {
            CineCamera.SetActive(false);
        }
        if (cinemachineBrain != null)
        {
            cinemachineBrain.SetActive(false);
        }
        
        SetUIActive(true);
        
        if (timelineCamera != null)
        {
            timelineCamera.enabled = false;
        }
        if (cameramanager != null)
        {
            cameramanager.SetActive(true);
            // Force re-initialization of camera target
            ReinitializeCameraManager();
        }
        if (gameplayCamera != null)
        {
            gameplayCamera.enabled = true;
        }
        
        #if UNITY_EDITOR
        Debug.Log("Timeline ended - Gameplay started!");
        #endif
    }

    private void ReinitializeCameraManager()
    {
        // Get the CameraManager component
        var camManager = cameramanager.GetComponent<CameraManager>();
        if (camManager != null && gameplayCharacter != null)
        {
            // Re-assign the player target
            // camManager.target = gameplayCharacter.transform; // Commented out - 'target' property doesn't exist
            
            // If CameraManager has an initialization method, call it
            // camManager.Initialize(); // Uncomment if such method exists
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
}

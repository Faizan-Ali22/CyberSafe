using UnityEngine;
using UnityEngine.Playables;

public class TimelineGameplayStarter : MonoBehaviour
{
    [Header("Timeline")]
    [SerializeField] private PlayableDirector playableDirector;
    
    [Header("Components to Enable")]
    [SerializeField] private MonoBehaviour[] componentsToEnable;

    [Header("GameObjects")]
    [SerializeField] private GameObject gameplayCharacter;
    [SerializeField] private GameObject cutsceneCharacter;
    [SerializeField] private GameObject cinemachineBrain;  // The Main Camera with Cinemachine Brain

    [Header("UI")]
    [SerializeField] private GameObject[] gameplayUIElements;

    [Header("Cameras")]
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

        // Disable Cinemachine Brain camera
        if (cinemachineBrain != null)
        {
            cinemachineBrain.SetActive(false);
        }
        
        SetUIActive(true);
        
        if (timelineCamera != null)
        {
            timelineCamera.enabled = false;
        }
        
        if (gameplayCamera != null)
        {
            gameplayCamera.enabled = true;
        }
        
        #if UNITY_EDITOR
        Debug.Log("Timeline ended - Gameplay started!");
        #endif
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

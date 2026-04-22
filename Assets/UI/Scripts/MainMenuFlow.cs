using UnityEngine;
using System.Collections;

public class MainMenuFlow : MonoBehaviour
{
    [Header("Animators")]
    public Animator mainMenuAnimator;
    public Animator selectMenuAnimator;
    
    [Header("Canvas Groups")]
    [SerializeField] private CanvasGroup mainMenuGroup;
    [SerializeField] private CanvasGroup selectMenuGroup;
    
    [Header("UI Elements")]
    [SerializeField] private GameObject[] mainUIElements; 
    [SerializeField] private GameObject[] selectUIElements;
    [SerializeField] private GameObject[] settingsUIElements;
    //public GameObject BottomBar;

    [Header("BG Intro")]
    [SerializeField] private GameObject bgRoot;
    [SerializeField] private float bgIntroTime = 2.0f;

    [Header("Timing")]
    [SerializeField] private float introAnimTime = 1f;
    [SerializeField] private float animStartDelay = 0.5f;
    [SerializeField] private float firstSelectAnimTime = 1f;
    [SerializeField] private float secondSelectAnimTime = 1f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip transitionAudio; // Assign your 2-second clip here

    void Awake()
    {
        if (bgRoot != null)
        { 
            bgRoot.SetActive(true);
        }
        SetCanvas(mainMenuGroup, 0f, false);
        SetCanvas(selectMenuGroup, 0f, false);
        SetMainUIActive(false);
        SetSelectUIActive(false);
        SetSettingsUIActive(false);
        StartCoroutine(SceneStartupFlow());
    }

    private IEnumerator SceneStartupFlow()
    {
        yield return new WaitForSeconds(bgIntroTime);

        if (bgRoot != null)
            bgRoot.SetActive(false);   

        SetCanvas(mainMenuGroup, 1f, false);
        yield return PlayIntroAnimation();
    }

    private IEnumerator PlayIntroAnimation()
    {
        yield return new WaitForSeconds(animStartDelay);
        PlayTransitionAudio(); // <--- Add this here
        mainMenuAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(introAnimTime);
        SetMainUIActive(true);
        SetCanvasInteractable(mainMenuGroup, true);
    }

    private void PlayTransitionAudio()
    {
        if (audioSource != null && transitionAudio != null)
        {
            audioSource.PlayOneShot(transitionAudio);
        }
    }

    public void OnClickStartButton()
    {
        if (mainMenuGroup.alpha < 1f || selectMenuGroup.alpha > 0f)
        {
            SetCanvas(mainMenuGroup, 1f, true);
            SetCanvas(selectMenuGroup, 0f, false);
            SetMainUIActive(true);
            SetSelectUIActive(false);
            SetSettingsUIActive(false);
        }
        StartCoroutine(TransitionToSelectMenu());
    }

    public void OnClickSettingstButton()
    {
        if (mainMenuGroup.alpha < 1f || selectMenuGroup.alpha > 0f)
        {
            SetCanvas(mainMenuGroup, 1f, true);
            SetCanvas(selectMenuGroup, 0f, false);
            SetMainUIActive(true);
            SetSelectUIActive(false);
            SetSettingsUIActive(false);
        }
        StartCoroutine(TransitionToSettingsMenu());
    }

    public void OnClickExit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void OnClickBackButton()
    {
        StartCoroutine(TransitionToMainMenu());
    }

    private IEnumerator TransitionToSelectMenu()
    {
        selectMenuAnimator.ResetTrigger("Start");
        selectMenuAnimator.ResetTrigger("Select");

        SetCanvas(mainMenuGroup, 0f, false);
        SetMainUIActive(false);
        SetSettingsUIActive(false);
        
        SetCanvas(selectMenuGroup, 1f, false);
        yield return new WaitForSeconds(animStartDelay);
        
        PlayTransitionAudio(); // <--- Add this here (First 60-frame transition)
        selectMenuAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(firstSelectAnimTime);
        
        PlayTransitionAudio(); // <--- Add this here (if needed for the second phase)
        selectMenuAnimator.SetTrigger("Select");
        yield return new WaitForSeconds(secondSelectAnimTime);
        
        SetSelectUIActive(true);
        SetCanvasInteractable(selectMenuGroup, true);
        //BottomBar.SetActive(false);
    }
     private IEnumerator TransitionToSettingsMenu()
    {
        selectMenuAnimator.ResetTrigger("Start");
        selectMenuAnimator.ResetTrigger("Select");

        SetCanvas(mainMenuGroup, 0f, false);
        SetMainUIActive(false);
        SetSelectUIActive(false);
        
        SetCanvas(selectMenuGroup, 1f, false);
        yield return new WaitForSeconds(animStartDelay);
        
        PlayTransitionAudio(); // <--- Add this here
        selectMenuAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(firstSelectAnimTime);
        
        PlayTransitionAudio(); // <--- Add this here (if needed for the second phase)
        selectMenuAnimator.SetTrigger("Select");
        yield return new WaitForSeconds(secondSelectAnimTime);
        
        SetSettingsUIActive(true);
        SetCanvasInteractable(selectMenuGroup, true);
        //BottomBar.SetActive(false);
    }
    private IEnumerator TransitionToMainMenu()
    {
        mainMenuAnimator.ResetTrigger("Start");
        mainMenuAnimator.ResetTrigger("Back");

        SetCanvas(mainMenuGroup, 1f, false);
        SetCanvas(selectMenuGroup, 0f, false);
        SetSelectUIActive(false);
        SetSettingsUIActive(false);

        
        ResetSelectMenuAnimator();

        PlayTransitionAudio(); // <--- Add this here for the Back animation
        mainMenuAnimator.SetTrigger("Back");
        yield return new WaitForSeconds(introAnimTime);
        
        PlayTransitionAudio(); // <--- Add this here for the UI revealing again
        mainMenuAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(introAnimTime);

        SetMainUIActive(true);
        SetCanvasInteractable(mainMenuGroup, true);
    }

    private void ResetSelectMenuAnimator()
    {
        if (!selectMenuAnimator) return;

        selectMenuAnimator.ResetTrigger("Start");
        selectMenuAnimator.ResetTrigger("Select");
        selectMenuAnimator.Play("Select-Idle", 0, 0f); 
        selectMenuAnimator.Update(0f); 
    }

    private void SetMainUIActive(bool active)
    {
        if (mainUIElements == null) return;
        foreach (var go in mainUIElements) if (go) go.SetActive(active);
    }
    
    private void SetSelectUIActive(bool active)
    {
        if (selectUIElements == null) return;
        foreach (var go in selectUIElements) if (go) go.SetActive(active);
    }
    private void SetSettingsUIActive(bool active)
    {
        if (settingsUIElements == null) return;
        foreach (var go in settingsUIElements) if (go) go.SetActive(active);
    }
    private void SetCanvasInteractable(CanvasGroup cg, bool interactable)
    {
        if (!cg) return;
        cg.interactable = interactable;
        cg.blocksRaycasts = interactable;
    }
    
    private void SetCanvas(CanvasGroup cg, float alpha, bool interactable)
    {
        if (!cg) return;
        cg.alpha = alpha;
        cg.interactable = interactable;
        cg.blocksRaycasts = interactable;
    }
}

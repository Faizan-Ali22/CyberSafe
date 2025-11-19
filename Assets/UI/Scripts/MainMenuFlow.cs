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
    public GameObject BottomBar;

    [Header("BG Intro")]
    [SerializeField] private GameObject bgRoot;
    [SerializeField] private float bgIntroTime = 0.1f;

    [Header("Timing")]
    [SerializeField] private float introAnimTime = 1f;
    [SerializeField] private float animStartDelay = 0.5f;
    [SerializeField] private float firstSelectAnimTime = 1f;
    [SerializeField] private float secondSelectAnimTime = 1f;

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

        StartCoroutine(SceneStartupFlow());
    }

    private IEnumerator SceneStartupFlow()
    {
        yield return new WaitForSeconds(bgIntroTime);

        if (bgRoot != null)
            bgRoot.SetActive(false);   // hide BG before showing main menu

        SetCanvas(mainMenuGroup, 1f, false);
        yield return PlayIntroAnimation();
    }

    private IEnumerator PlayIntroAnimation()
    {
        yield return new WaitForSeconds(animStartDelay);
        mainMenuAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(introAnimTime);
        SetMainUIActive(true);
        SetCanvasInteractable(mainMenuGroup, true);
    }

    // --- CHANGED ---
    public void OnClickStartButton()
    {
        // Ensure the canvases are in the expected state first
        if (mainMenuGroup.alpha < 1f || selectMenuGroup.alpha > 0f)
        {
            // force main menu visible, select menu hidden
            SetCanvas(mainMenuGroup, 1f, true);
            SetCanvas(selectMenuGroup, 0f, false);
            SetMainUIActive(true);
            SetSelectUIActive(false);
        }

        // Now run the usual transition
        StartCoroutine(TransitionToSelectMenu());
    }
    // ---------------

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
        
        SetCanvas(selectMenuGroup, 1f, false);
        yield return new WaitForSeconds(animStartDelay);
        selectMenuAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(firstSelectAnimTime);
        
        selectMenuAnimator.SetTrigger("Select");
        yield return new WaitForSeconds(secondSelectAnimTime);
        
        SetSelectUIActive(true);
        SetCanvasInteractable(selectMenuGroup, true);
        BottomBar.SetActive(false);
    }

    private IEnumerator TransitionToMainMenu()
    {
        mainMenuAnimator.ResetTrigger("Start");
        mainMenuAnimator.ResetTrigger("Back");

        SetCanvas(mainMenuGroup, 1f, false);
        SetCanvas(selectMenuGroup, 0f, false);
        SetSelectUIActive(false);

        // return select animator to idle before we leave it hidden
        ResetSelectMenuAnimator();

        mainMenuAnimator.SetTrigger("Back");
        yield return new WaitForSeconds(introAnimTime);
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
        selectMenuAnimator.Play("Select-Idle", 0, 0f); // use your actual idle state name
        selectMenuAnimator.Update(0f); // force immediate evaluation
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

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
    
    [Header("Timing")]
    [SerializeField] private float introAnimTime = 1f;
    [SerializeField] private float animStartDelay = 0.5f;
    [SerializeField] private float firstSelectAnimTime = 1f;
    [SerializeField] private float secondSelectAnimTime = 1f;
    
    void Awake()
    {
        SetCanvas(mainMenuGroup, 1f, false);
        SetCanvas(selectMenuGroup, 0f, false);
        SetMainUIActive(false);
        SetSelectUIActive(false);
        StartCoroutine(PlayIntroAnimation());
    }
    
    private IEnumerator PlayIntroAnimation()
    {
        yield return new WaitForSeconds(animStartDelay);
        mainMenuAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(introAnimTime);
        SetMainUIActive(true);
        SetCanvasInteractable(mainMenuGroup, true);
    }

    public void OnClickStartButton()
    {
        StartCoroutine(TransitionToSelectMenu());
    }
    
    public void OnClickExit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    private IEnumerator TransitionToSelectMenu()
    {
        // Hide main menu
        SetCanvas(mainMenuGroup, 0f, false);
        SetMainUIActive(false);
        
        // Show select menu and play first animation
        SetCanvas(selectMenuGroup, 1f, false);
        yield return new WaitForSeconds(animStartDelay);
        selectMenuAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(firstSelectAnimTime);
        
        // Play second animation
        selectMenuAnimator.SetTrigger("Select");
        yield return new WaitForSeconds(secondSelectAnimTime);
        
        // Show UI after both animations complete
        SetSelectUIActive(true);
        SetCanvasInteractable(selectMenuGroup, true);
        BottomBar.SetActive(false);
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

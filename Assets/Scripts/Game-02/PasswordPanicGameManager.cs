using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PasswordPanicGameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PasswordStrengthChecker passwordChecker;
    [SerializeField] private GameObject completedPanel;
    [SerializeField] private Button continueButton;
    [SerializeField] private GameObject passwordUiCanvas; // <- drag the parent Canvas here
    
    [Header("Debug Settings")]
    [Tooltip("Check this in play mode to reset progress for testing")]
    public bool resetProgress = false;

    private bool isCompleting = false;
    private const string AVATAR_PREF_PREFIX = "AvatarPasswordSet_";
    private const int TOTAL_AVATARS = 5;

    private void Start()
    {
        // 1. Ensure the completed panel starts turned off
        if (completedPanel != null)
        {
            completedPanel.SetActive(false);
        }

        // 2. Hook up the Continue Button to load the next scene
        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(OnContinueClicked);
        }
    }

    private void Update()
    {
        // Reset Logic for Testing
        if (resetProgress)
        {
            ResetProgress();
            resetProgress = false;
        }

        // Continually check if all passwords are set
        if (!isCompleting && CheckAllPasswordsSet())
        {
            StartCoroutine(HandleGameCompletion());
        }
    }

    private bool CheckAllPasswordsSet()
    {
        int setPasswordsCount = 0;
        bool newlySaved = false;

        for (int i = 0; i < TOTAL_AVATARS; i++)
        {
            // Check if the player set the password securely in this active session
            bool isSetNow = passwordChecker != null && passwordChecker.IsPasswordSet(i);
            
            // Save state to PlayerPrefs if they set it AND it wasn't saved before
            if (isSetNow && PlayerPrefs.GetInt(AVATAR_PREF_PREFIX + i, 0) == 0)
            {
                PlayerPrefs.SetInt(AVATAR_PREF_PREFIX + i, 1);
                newlySaved = true; // Mark that we need to hit the disk once
            }

            // Check how many have been saved in PlayerPrefs as Complete
            if (PlayerPrefs.GetInt(AVATAR_PREF_PREFIX + i, 0) == 1)
            {
                setPasswordsCount++;
            }
        }

        // Fix: ONLY save if we changed something. Calling Save() 60 times a second freezes Android devices
        if (newlySaved)
        {
            PlayerPrefs.Save();
        }
        
        return setPasswordsCount >= TOTAL_AVATARS;
    }

    private IEnumerator HandleGameCompletion()
    {
        isCompleting = true;
        
        Debug.Log("✅ All 5 Avatar passwords set! Waiting 2 seconds...");
        
        // Wait 2 seconds
        yield return new WaitForSeconds(2.0f);

        // Turn on the Completed panel and bring it to the very front
        if (completedPanel != null)
        {
            // NEW: Push the Completed Panel to the front of the screen
            completedPanel.transform.SetAsLastSibling();
            completedPanel.SetActive(true);
            
            // NEW: Hide the password checker UI completely so it doesn't overlap
            if (passwordChecker != null)
            {
                passwordChecker.gameObject.SetActive(false);
            }

            // Force hide the entire UI Canvas
            if (passwordUiCanvas != null)
            {
                Destroy(passwordUiCanvas);
            }

            // Find and play the attached AudioSource sound
            AudioSource completedAudio = completedPanel.GetComponent<AudioSource>();
            if (completedAudio != null)
            {
                completedAudio.Play();
            }
            else
            {
                Debug.LogWarning("No AudioSource found attached to the Completed Panel!");
            }
        }

        UpdatePlayerProgress();
    }

    private void UpdatePlayerProgress()
    {
        if (ProgressManager.Instance != null)
        {
            // Chapters Completed: Element 0, 1, 2, 3
            for (int i = 0; i <= 3; i++)
            {
                ProgressManager.Instance.SetChapterCompletedOnly(i, true);
            }

            // Chapters Unlocked: Element 0, 1, 2, 3, 4
            for (int i = 0; i <= 4; i++)
            {
                ProgressManager.Instance.SetChapterUnlocked(i, true);
            }
            
            Debug.Log("🏆 Player progress saved successfully via ProgressManager!");
        }
        else
        {
            Debug.LogWarning("ProgressManager.Instance is null! Could not update progress.");
        }
    }

    public void OnContinueClicked()
    {
        // Load the next scene automatically based on Build Settings index
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("No next scene available in Build Settings.");
        }
    }

    public void ResetProgress()
    {
        // Wipe all 5 avatar keys
        for (int i = 0; i < TOTAL_AVATARS; i++)
        {
            PlayerPrefs.DeleteKey(AVATAR_PREF_PREFIX + i);
        }
        PlayerPrefs.Save();
        
        isCompleting = false;

        if (completedPanel != null)
            completedPanel.SetActive(false);

        Debug.Log("🔄 Avatar password progress has been reset for testing!");
    }
}

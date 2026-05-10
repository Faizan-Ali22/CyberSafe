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
    [SerializeField] private GameObject passwordUiCanvas; 
    
    [Header("Debug Settings")]
    [Tooltip("Check this in play mode to reset progress for testing")]
    public bool resetProgress = false;

    private bool isCompleting = false;
    private const string AVATAR_PREF_PREFIX = "AvatarPasswordSet_";
    private const int TOTAL_AVATARS = 5;

    private void Start()
    {
        ResetProgress(); // reset when this scene loads

        if (completedPanel != null)
        {
            completedPanel.SetActive(false);
        }

        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(OnContinueClicked);
        }
    }

    private void Update()
    {
        if (resetProgress)
        {
            ResetProgress();
            resetProgress = false;
        }

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
            bool isSetNow = passwordChecker != null && passwordChecker.IsPasswordSet(i);
            
            if (isSetNow && PlayerPrefs.GetInt(AVATAR_PREF_PREFIX + i, 0) == 0)
            {
                PlayerPrefs.SetInt(AVATAR_PREF_PREFIX + i, 1);
                newlySaved = true; 
            }

            if (PlayerPrefs.GetInt(AVATAR_PREF_PREFIX + i, 0) == 1)
            {
                setPasswordsCount++;
            }
        }

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
        
        // FIX: Use Realtime to prevent Android OS keyboard suspensions from freezing the coroutine
        yield return new WaitForSecondsRealtime(2.0f);

        if (completedPanel != null)
        {
            // NOTE: Make sure the Completed Canvas 'Order in Layer' is set to 1 in the Unity Inspector!
            completedPanel.SetActive(true);
            
            if (passwordChecker != null)
            {
                passwordChecker.gameObject.SetActive(false);
            }

            if (passwordUiCanvas != null)
            {
                Destroy(passwordUiCanvas);
            }
            else
            {
                Debug.LogWarning("⚠️ passwordUiCanvas is null! Cannot destroy the main canvas.");
            }

            AudioSource completedAudio = completedPanel.GetComponent<AudioSource>();
            if (completedAudio != null)
            {
                completedAudio.Play();
            }
        }
        else
        {
             Debug.LogError("⚠️ completedPanel is null! The badge cannot be activated.");
        }

        UpdatePlayerProgress();
    }

    private void UpdatePlayerProgress()
    {
        if (ProgressManager.Instance != null)
        {
            for (int i = 0; i <= 3; i++)
            {
                ProgressManager.Instance.SetChapterCompletedOnly(i, true);
            }

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
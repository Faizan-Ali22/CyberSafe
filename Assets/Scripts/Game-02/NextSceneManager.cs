using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NextSceneManager : MonoBehaviour
{
    private void Start()
    {
        UpdateProgressManager();
    }

    private void UpdateProgressManager()
    {
        if (ProgressManager.Instance != null)
        {
            // Set Chapter Completed for indices 0-3
            for (int i = 0; i <= 3; i++)
            {
                ProgressManager.Instance.SetChapterCompletedOnly(i, true);
            }

            // Set Chapter Unlocked for indices 0-4
            for (int i = 0; i <= 4; i++)
            {
                ProgressManager.Instance.SetChapterUnlocked(i, true);
            }

            Debug.Log("✅ ProgressManager updated: Chapters 0-3 completed, Chapters 0-4 unlocked");
        }
        else
        {
            Debug.LogWarning("⚠️ ProgressManager.Instance is null!");
        }
    }

    public void LoadNextScene()
    {
        SceneManager.LoadSceneAsync(sceneBuildIndex: 18);
    }

    void Update()
    {
        
    }
}

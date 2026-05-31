using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class CutSceneToDDOS : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] public string sceneName = "DDOS";
    
    // Optional: Use scene index instead of name
    [SerializeField] private bool useSceneIndex = true;
    [SerializeField] public int sceneIndex= 21;

    void OnEnable()
    {
        MarkChapterProgress();

        if (useSceneIndex)
        {
            StartCoroutine(LoadSceneAsync(sceneIndex));
        }
        else
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }
    }

    private void MarkChapterProgress()
    {
        if (ProgressManager.Instance == null)
        {
            Debug.LogWarning("CutSceneChapter2: ProgressManager instance not found.");
            return;
        }

        // Chapter Completed: Element 0, Element 1
        ProgressManager.Instance.SetChapterCompletedOnly(0, true);
        ProgressManager.Instance.SetChapterCompletedOnly(1, true);
        ProgressManager.Instance.SetChapterCompletedOnly(2, true);
        ProgressManager.Instance.SetChapterCompletedOnly(3, true);
        ProgressManager.Instance.SetChapterCompletedOnly(4, true);
        // Chapter Unlocked: Element 0, Element 1, Element 2
        ProgressManager.Instance.SetChapterUnlocked(0, true);
        ProgressManager.Instance.SetChapterUnlocked(1, true);
        ProgressManager.Instance.SetChapterUnlocked(2, true);
        ProgressManager.Instance.SetChapterUnlocked(3, true);
        ProgressManager.Instance.SetChapterUnlocked(4, true);
        ProgressManager.Instance.SetChapterUnlocked(5, true);
    }
    
    private IEnumerator LoadSceneAsync(string name)
    {
        yield return SceneManager.LoadSceneAsync(name);
    }
    
    private IEnumerator LoadSceneAsync(int index)
    {
        yield return SceneManager.LoadSceneAsync(index);
    }
}





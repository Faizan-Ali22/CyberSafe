using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class CutSceneDeaddrop : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] public string sceneName = "Game";
    
    // Optional: Use scene index instead of name
    [SerializeField] private bool useSceneIndex = true;
    [SerializeField] public int sceneIndex= 19;

    void Start()
    {
        MarkChapterProgress();

        if (useSceneIndex)
        {
            StartCoroutine(LoadSceneAsyncRoutine(sceneIndex));
        }
        else
        {
            StartCoroutine(LoadSceneAsyncRoutine(sceneName));
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
        // Chapter Unlocked: Element 0, Element 1, Element 2
        ProgressManager.Instance.SetChapterUnlocked(0, true);
        ProgressManager.Instance.SetChapterUnlocked(1, true);
        ProgressManager.Instance.SetChapterUnlocked(2, true);
        ProgressManager.Instance.SetChapterUnlocked(3, true);
        ProgressManager.Instance.SetChapterUnlocked(4, true);
    }
    
    private IEnumerator LoadSceneAsyncRoutine(string name)
    {
        yield return SceneManager.LoadSceneAsync(name);
    }
    
    private IEnumerator LoadSceneAsyncRoutine(int index)
    {
        // Wait a frame so we exit the Timeline/OnEnable evaluation step first
        yield return null; 
        yield return SceneManager.LoadSceneAsync(index);
    }
}

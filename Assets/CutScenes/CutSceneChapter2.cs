using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
public class CutSceneChapter2 : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] public string sceneName = "RedLinksCutscene";
    
    // Optional: Use scene index instead of name
    [SerializeField] private bool useSceneIndex = true;
    [SerializeField] public int sceneIndex= 13;

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

        // Chapter Unlocked: Element 0, Element 1, Element 2
        ProgressManager.Instance.SetChapterUnlocked(0, true);
        ProgressManager.Instance.SetChapterUnlocked(1, true);
        ProgressManager.Instance.SetChapterUnlocked(2, true);
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
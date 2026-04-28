using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CutSceneChapter0 : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] public string sceneName = "Chapter-0 Before Change";
    
    // Optional: Use scene index instead of name
    [SerializeField] private bool useSceneIndex = true;
    [SerializeField] public int sceneIndex= 3;

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
            Debug.LogWarning("CutSceneChapter0: ProgressManager instance not found.");
            return;
        }

        // Chapter Completed: Element 0, Element 1

        // Chapter Unlocked: Element 0, Element 1, Element 2
        ProgressManager.Instance.SetChapterUnlocked(0, true);
 
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

using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
public class CutSceneChapterC : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] public string sceneName = "ChapterC";
    
    // Optional: Use scene index instead of name
    [SerializeField] private bool useSceneIndex = true;
    [SerializeField] public int sceneIndex= 6;

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
            Debug.LogWarning("CutSceneChapterC: ProgressManager instance not found.");
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

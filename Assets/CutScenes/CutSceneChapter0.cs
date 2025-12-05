using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CutSceneChapter0 : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] public string sceneName = "Chapter-0";
    
    // Optional: Use scene index instead of name
    [SerializeField] private bool useSceneIndex = false;
    [SerializeField] private int sceneIndex = 0;

    void OnEnable()
    {
        if (useSceneIndex)
        {
            StartCoroutine(LoadSceneAsync(sceneIndex));
        }
        else
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }
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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class CutSceneChapter0 : MonoBehaviour
{
      void OnEnable()
    {
        StartCoroutine(LoadSceneAsync("MainMenu"));
    }
    
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName);
    }
}

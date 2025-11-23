using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class CutSceneMainMenu : MonoBehaviour
{
     void OnEnable()
    {
        StartCoroutine(LoadSceneAsync("Chapter-0"));
    }
    
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName);
    }
}

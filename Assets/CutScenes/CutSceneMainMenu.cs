using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class CutSceneMainMenu : MonoBehaviour
{
     void OnEnable()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
    // IEnumerator LoadMainMenu()
    // {
        
        
    // }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;   
public class RedLinksMinigame : MonoBehaviour
{
   public void OnSuccess_SaveNPCAndReturn()
    {
        // increment counter (persistent)
        if (GameProgressManager.Instance != null)
            GameProgressManager.Instance.IncrementSaved();

        // return to main scene - replace "MainScene" with your scene name
        SceneManager.LoadScene("School");
    }

    public void OnFail_AllowRetry()
    {
        // show hint, reduce firewall health, etc
    }
}

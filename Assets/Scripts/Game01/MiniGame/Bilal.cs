using UnityEngine;
using UnityEngine.UI; // Add this for Button
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Bilal : MonoBehaviour
{
    [Header("Screens")]
    public GameObject LockScreen;
    public GameObject LockScreenMsg;
    public GameObject OpenMsg;
    public GameObject RightChoice;
    public GameObject WrongChoice;
    public GameObject Flash;
    public GameObject Hacked;
    [Header("Buttons")]
    public Button activationButton;
    public Button OpenButton;
    public Button Legit;
    public Button Phishing;
    public Button givePhoneBackButton;
    
    private NPCInteraction npcInteraction;

    [Header("Camera References")]
    public Camera introCamera;
    public Vector3 returnPosition;

    void Start()
    {
        LockScreen.SetActive(true);
        LockScreenMsg.SetActive(false);
        OpenMsg.SetActive(false);
        RightChoice.SetActive(false);
        WrongChoice.SetActive(false);
        Flash.SetActive(false);
        Hacked.SetActive(false);
        
        if (activationButton != null && OpenButton !=null && Legit !=null && Phishing !=null) 
        {
            activationButton.onClick.AddListener(OnTap);
            OpenButton.onClick.AddListener(OnOpen);
            Legit.onClick.AddListener(OnLegit);
            Phishing.onClick.AddListener(OnPhishing);
        }
        else
        {
            Debug.LogWarning("Button reference not set in Bilal script!");
        }
        
        if (givePhoneBackButton != null)
        {
            givePhoneBackButton.onClick.AddListener(OnGivePhoneBack);
        }
        
        npcInteraction = FindFirstObjectByType<NPCInteraction>();
    }

    public void OnTap()
    {
        LockScreen.SetActive(false);
        LockScreenMsg.SetActive(true);
    }
    
    public void OnOpen()
    {
        LockScreenMsg.SetActive(false);
        OpenMsg.SetActive(true);
    }
    
    public void OnLegit()
    {
        OpenMsg.SetActive(false);
        WrongChoice.SetActive(true);
        StartCoroutine(ShowFlashAfterDelay());
    }

    private IEnumerator ShowFlashAfterDelay()
    {
        yield return new WaitForSecondsRealtime(2);
        WrongChoice.SetActive(false);
        Flash.SetActive(true);
        StartCoroutine(ShowHackedAfterDelay());
    }
    
    private IEnumerator ShowHackedAfterDelay()
    {
        yield return new WaitForSecondsRealtime(2);
        Hacked.SetActive(true);
    }
    
    public void OnPhishing()
    {
        OpenMsg.SetActive(false);
        StartCoroutine(ShowRightChoiceAfterDelay());
    }
    
    private IEnumerator ShowRightChoiceAfterDelay()
    {
        yield return new WaitForSecondsRealtime(0.01f);
        RightChoice.SetActive(true);
    }
    
    public void OnGivePhoneBack()
    {
        // Disable NPC interaction and increment save count
        if (npcInteraction != null)
        {
            npcInteraction.DisableInteraction();
            GameProgressManager.Instance.IncrementSaved();
        }

        // Save player state BEFORE loading scene
        PlayerPrefs.SetInt("RedLinksScene", 1);
        PlayerPrefs.Save();
        
        // Load Game-01 scene
        SceneManager.LoadScene("Game-02");
    }

    void OnDestroy()
    {
        if (activationButton != null && OpenButton !=null && Legit !=null && Phishing !=null)
        {
            activationButton.onClick.RemoveListener(OnTap);
            OpenButton.onClick.RemoveListener(OnOpen);
            Legit.onClick.RemoveListener(OnLegit);
            Phishing.onClick.RemoveListener(OnPhishing);
        }
        if (givePhoneBackButton != null)
        {
            givePhoneBackButton.onClick.RemoveListener(OnGivePhoneBack);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Bilal : MonoBehaviour
{
   [Header("Controller Reference")]
    public StudentInteraction studentController; // DRAG YOUR STUDENT OBJECT HERE

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
    public Button LegitButton;
    public Button PhishingButton;
    public Button givePhoneBackButton;

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioClip notificationSound;

    void Start()
    {
        SetAllScreensInactive();
        LockScreen.SetActive(true);

        // Buttons
        if (activationButton != null) activationButton.onClick.AddListener(OnTap);
        if (OpenButton != null) OpenButton.onClick.AddListener(OnOpen);
        if (LegitButton != null) LegitButton.onClick.AddListener(OnLegit);
        if (PhishingButton != null) PhishingButton.onClick.AddListener(OnPhishing);
        if (givePhoneBackButton != null) givePhoneBackButton.onClick.AddListener(OnGivePhoneBack);

        StartCoroutine(AutoTransitionFromLockScreen());
    }

    IEnumerator AutoTransitionFromLockScreen()
    {
        yield return new WaitForSeconds(5.0f);
        if (sfxSource != null && notificationSound != null)
            sfxSource.PlayOneShot(notificationSound);
        yield return new WaitForSeconds(0.5f);
        LockScreen.SetActive(false);
        LockScreenMsg.SetActive(true);
    }

    public void OnTap() { LockScreen.SetActive(false); LockScreenMsg.SetActive(true); }
    public void OnOpen() { LockScreenMsg.SetActive(false); OpenMsg.SetActive(true); }

    public void OnLegit() 
    { 
        OpenMsg.SetActive(false); 
        WrongChoice.SetActive(true); 
        StartCoroutine(WrongChoiceSequence()); 
    }

    IEnumerator WrongChoiceSequence()
    {
        yield return new WaitForSeconds(2f);
        WrongChoice.SetActive(false);
        Flash.SetActive(true);
        yield return new WaitForSeconds(2f);
        Flash.SetActive(false);
        Hacked.SetActive(true);
    }

    public void OnPhishing() { OpenMsg.SetActive(false); RightChoice.SetActive(true); }

    public void OnGivePhoneBack()
    {
        // THIS IS THE KEY LINK BACK TO THE MAIN GAME
        if (studentController != null)
        {
            studentController.CloseMinigameAndFinish();
        }
        else
        {
            Debug.LogError("Bilal.cs: You forgot to assign the Student Interaction script in the Inspector!");
        }
    }

    private void SetAllScreensInactive()
    {
        LockScreen.SetActive(false); LockScreenMsg.SetActive(false); OpenMsg.SetActive(false);
        RightChoice.SetActive(false); WrongChoice.SetActive(false); Flash.SetActive(false); Hacked.SetActive(false);
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
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
    public Button LegitButton;
    public Button PhishingButton;
    public Button givePhoneBackButton;

    [Header("Audio")]
    public AudioSource sfxSource;           // assign an AudioSource in Inspector
    public AudioClip notificationSound;     // assign the "ting" or any SFX

    private NPCInteraction npcInteraction;

    void Start()
    {
        // ✅ Initialize all screens
        SetAllScreensInactive();
        LockScreen.SetActive(true);

        // ✅ Hook up button events
        if (activationButton != null) activationButton.onClick.AddListener(OnTap);
        if (OpenButton != null) OpenButton.onClick.AddListener(OnOpen);
        if (LegitButton != null) LegitButton.onClick.AddListener(OnLegit);
        if (PhishingButton != null) PhishingButton.onClick.AddListener(OnPhishing);
        if (givePhoneBackButton != null) givePhoneBackButton.onClick.AddListener(OnGivePhoneBack);

        // ✅ Get NPC interaction reference if exists
        npcInteraction = FindFirstObjectByType<NPCInteraction>();

        // ✅ Start intro auto-transition sequence
        StartCoroutine(AutoTransitionFromLockScreen());
    }

    // --------------------------------------------------------------------
    // 🔹 Automatic transition from LockScreen → LockScreenMsg
    IEnumerator AutoTransitionFromLockScreen()
    {
        // Wait 2 seconds on the first screen
        yield return new WaitForSeconds(5.0f);

        // Play notification sound (optional)
        if (sfxSource != null && notificationSound != null)
            sfxSource.PlayOneShot(notificationSound);

        // Wait a small delay for the sound to play
        yield return new WaitForSeconds(0.5f);

        // Switch screens
        LockScreen.SetActive(false);
        LockScreenMsg.SetActive(true);
    }

    // --------------------------------------------------------------------
    // 🔹 Button-driven transitions

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

    public void OnPhishing()
    {
        OpenMsg.SetActive(false);
        RightChoice.SetActive(true);
    }

    public void OnGivePhoneBack()
    {
        // Logic for returning to main scene
        // You can re-enable NPC interaction or increment progress here
        // GameProgressManager.Instance.IncrementSaved();
        // SceneManager.LoadScene("Game-01");
    }

    // --------------------------------------------------------------------
    // 🔹 Helpers

    private void SetAllScreensInactive()
    {
        LockScreen.SetActive(false);
        LockScreenMsg.SetActive(false);
        OpenMsg.SetActive(false);
        RightChoice.SetActive(false);
        WrongChoice.SetActive(false);
        Flash.SetActive(false);
        Hacked.SetActive(false);
    }

    private void OnDestroy()
    {
        if (activationButton != null) activationButton.onClick.RemoveListener(OnTap);
        if (OpenButton != null) OpenButton.onClick.RemoveListener(OnOpen);
        if (LegitButton != null) LegitButton.onClick.RemoveListener(OnLegit);
        if (PhishingButton != null) PhishingButton.onClick.RemoveListener(OnPhishing);
        if (givePhoneBackButton != null) givePhoneBackButton.onClick.RemoveListener(OnGivePhoneBack);
    }
}

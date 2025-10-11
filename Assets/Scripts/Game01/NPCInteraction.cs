using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class DialogueLine
{
    public enum Speaker { Player, NPC }
    public Speaker speaker;
    [TextArea] public string text;
    public float duration = 2.0f;
}
public class NPCInteraction : MonoBehaviour
{
   [Header("UI / Audio")]
    public GameObject tapToInteractUI;   // world-space canvas child (assign)
    public AudioClip notificationClip;   // the "ting" sound
    public AudioSource audioSource;      // assign or auto-get

    [Header("Cutscene")]
    public Camera cutsceneCamera;        // specific camera for this interaction
    public Camera mainFollowCamera;      // assign your normal follow camera
    public Animator npcAnimator;         // NPC animator (has 'isTalking' bool)
    public Animator playerAnimator;      // main player animator
    public DialogueLine[] dialogueSequence;

    [Header("Red Links")]
    public GameObject redLinksLogoUI;    // overlay UI to show logo (disable initially)
    public string redLinksSceneName = "RedLinksScene"; // scene to load next

    bool playerInRange = false;
    PlayerManager playerManager;

    void Start()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (tapToInteractUI != null) tapToInteractUI.SetActive(false);
        if (cutsceneCamera != null) cutsceneCamera.gameObject.SetActive(false);
        if (redLinksLogoUI != null) redLinksLogoUI.SetActive(false);

        playerManager = FindFirstObjectByType<PlayerManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (tapToInteractUI != null) tapToInteractUI.SetActive(true);
            StartCoroutine(PlayNotificationTwice());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (tapToInteractUI != null) tapToInteractUI.SetActive(false);
        }
    }

    IEnumerator PlayNotificationTwice()
    {
        if (audioSource != null && notificationClip != null)
        {
            audioSource.PlayOneShot(notificationClip);
            yield return new WaitForSeconds(0.25f);
            audioSource.PlayOneShot(notificationClip);
        }
    }

    // Hook this method to the UI button OnClick
    public void OnTapInteract()
    {
        if (!playerInRange) return;
        if (tapToInteractUI != null) tapToInteractUI.SetActive(false);
        StartCoroutine(PlayInteractionCutscene());
    }

    IEnumerator PlayInteractionCutscene()
    {
        // 1) Enter cutscene mode
        if (playerManager != null) playerManager.isInCutscene = true;

        // 2) Switch cameras
        if (mainFollowCamera != null) mainFollowCamera.gameObject.SetActive(false);
        if (cutsceneCamera != null) cutsceneCamera.gameObject.SetActive(true);

        // 3) Freeze player physics briefly
        Rigidbody playerRb = playerManager ? playerManager.GetComponent<Rigidbody>() : null;
        if (playerRb != null) { playerRb.linearVelocity = Vector3.zero; playerRb.angularVelocity = Vector3.zero; }

        // 4) Play dialogue sequence
        for (int i = 0; i < dialogueSequence.Length; i++)
        {
            DialogueLine ln = dialogueSequence[i];
            if (ln.speaker == DialogueLine.Speaker.Player)
            {
                if (playerAnimator != null) playerAnimator.SetBool("isTalking", true);
                if (npcAnimator != null) npcAnimator.SetBool("isTalking", false);
            }
            else
            {
                if (npcAnimator != null) npcAnimator.SetBool("isTalking", true);
                if (playerAnimator != null) playerAnimator.SetBool("isTalking", false);
            }

            DialogueManager.Instance.ShowSubtitle(ln.text);
            yield return new WaitForSeconds(ln.duration);

            // stop talk animation so transitions can happen
            if (playerAnimator != null) playerAnimator.SetBool("isTalking", false);
            if (npcAnimator != null) npcAnimator.SetBool("isTalking", false);
            DialogueManager.Instance.HideSubtitle();

            // small buffer between lines
            yield return new WaitForSeconds(0.15f);
        }

        // 5) End cutscene: show Red Links logo & then go to minigame scene
        if (cutsceneCamera != null) cutsceneCamera.gameObject.SetActive(false);
        if (redLinksLogoUI != null)
        {
            redLinksLogoUI.SetActive(true);
            // wait the 5 seconds (logo duration)
            yield return new WaitForSeconds(5f);
            redLinksLogoUI.SetActive(false);
        }

        // 6) Wait a few seconds, then load next mini-game scene (Red Links)
        yield return new WaitForSeconds(10f); // adjust delay as needed
        if (!string.IsNullOrEmpty(redLinksSceneName))
            SceneManager.LoadScene(redLinksSceneName);
           
        else
        {
            // fallback: return control
            if (mainFollowCamera != null) mainFollowCamera.gameObject.SetActive(true);
            if (playerManager != null) playerManager.isInCutscene = false;
        }
    }
}

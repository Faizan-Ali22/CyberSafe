using UnityEngine;
using System.Collections;

[System.Serializable]
public class CutsceneDialogue
{
    public enum Speaker { Ayan, Zara, Ahmed }
    public Speaker speaker;
    [TextArea] public string text;
    public float duration = 2.5f;
}

[DisallowMultipleComponent]
public class CutsceneController : MonoBehaviour
{
     [Header("Cameras")]
    public Camera cutsceneCamera;
    public Camera mainCamera;

    [Header("Animators")]
    public Animator ayanAnimator;
    public Animator zaraAnimator;
    public Animator ahmedAnimator;

    [Header("Dialogue")]
    public CutsceneDialogue[] dialogueLines;

    [Header("UI")]
    public GameObject missionUI;

    [Header("Player")]
    public GameObject player;                      // Assign your player GameObject
    public MonoBehaviour movementScript;           // Drag the movement script (PlayerMovement, etc.)

    private bool cutscenePlaying = false;

    public void BeginCutscene()
    {
        if (!cutscenePlaying)
            StartCoroutine(PlayCutsceneSequence());
    }

    public IEnumerator PlayCutsceneSequence()
    {
        cutscenePlaying = true;

        // 🎬 1. Disable only the movement script
        if (movementScript != null)
            movementScript.enabled = false;

        // 🎥 2. Switch to cutscene camera
        cutsceneCamera.gameObject.SetActive(true);
        mainCamera.gameObject.SetActive(false);

        // 🗣️ 3. Dialogue sequence
        foreach (var line in dialogueLines)
        {
            Animator activeAnimator = null;
            switch (line.speaker)
            {
                case CutsceneDialogue.Speaker.Ayan: activeAnimator = ayanAnimator; break;
                case CutsceneDialogue.Speaker.Zara: activeAnimator = zaraAnimator; break;
                case CutsceneDialogue.Speaker.Ahmed: activeAnimator = ahmedAnimator; break;
            }

            if (activeAnimator != null) activeAnimator.SetBool("isTalking", true);
            DialogueManager.Instance.ShowSubtitle(line.text);

            yield return new WaitForSeconds(line.duration);

            if (activeAnimator != null) activeAnimator.SetBool("isTalking", false);
            DialogueManager.Instance.HideSubtitle();

            yield return new WaitForSeconds(0.3f);
        }

        // 🎬 4. Return to main camera
        cutsceneCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);

        // 🎯 5. Show mission UI
        if (missionUI != null)
            missionUI.SetActive(true);

        // 🧍 6. Re-enable player movement
        if (movementScript != null)
            movementScript.enabled = true;

        cutscenePlaying = false;
        Debug.Log("✅ Cutscene complete → Player control restored.");
    }
}

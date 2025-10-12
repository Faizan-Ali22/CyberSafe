// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.Rendering;
// using UnityEngine.Rendering.Universal;
// using TMPro;
// using System.Collections;

// public class IntroManager : MonoBehaviour
// {
//     [Header("UI Elements")]
//     public Image blackPanel;
//     public TMP_Text chapterText;
//     public Image logoImage;

//     [Header("Camera Control")]
//     public Animator introCameraAnim;
//     public Camera introCamera;
//     public Camera cutsceneCamera;
//     public Camera mainCamera;
//     public float cameraBlendDuration = 2f;

//     [Header("Timing (in frames @30 fps)")]
//     public int staticFrames = 30;
//     public int totalFrames = 240;
//     public float fadeDuration = 1f;

//     [Header("Post-Processing")]
//     public Volume postProcessVolume; // assign your URP volume
//     private DepthOfField dof;

//     [Header("Audio")]
//     public AudioSource backgroundMusic;
//     public float fadeAudioDuration = 2f;

//     [Header("References")]
//     public CutsceneController cutsceneController;

//     private float FrameToTime(int frames) => frames / 30f;

//     void Start()
//     {
//         if (postProcessVolume != null)
//             postProcessVolume.profile.TryGet(out dof);
//         StartCoroutine(PlayIntroSequence());
//     }

//     IEnumerator PlayIntroSequence()
//     {
//         // --- INITIAL SETUP ---
//         introCamera.gameObject.SetActive(true);
//         cutsceneCamera.gameObject.SetActive(false);
//         mainCamera.gameObject.SetActive(false);

//         if (blackPanel != null)
//             blackPanel.color = new Color(0, 0, 0, 1);

//         chapterText.alpha = 0;
//         logoImage.color = new Color(1, 1, 1, 0);

//         if (introCameraAnim != null)
//             introCameraAnim.SetTrigger("Play");

//         // --- 1️⃣ Typewriter effect + logo fade ---
//         yield return StartCoroutine(TypeText("Chapter 01", 0.05f));
//         yield return StartCoroutine(FadeInLogo());
//         yield return new WaitForSeconds(FrameToTime(staticFrames));

//         // --- 2️⃣ Fade out intro visuals ---
//         yield return StartCoroutine(FadeOutIntro());

//         // --- 3️⃣ Prepare DOF blur & sound ---
//         if (dof != null)
//         {
//             dof.active = true;
//             dof.focusDistance.value = 2f;
//         }

//         if (backgroundMusic != null)
//             StartCoroutine(FadeAudio(backgroundMusic, 1f, 0.25f, fadeAudioDuration));

//         // --- 4️⃣ Wait until end of intro camera animation ---
//         float remain = FrameToTime(totalFrames - staticFrames);
//         yield return new WaitForSeconds(remain - cameraBlendDuration);

//         // --- 5️⃣ Smooth camera blend ---
//         yield return StartCoroutine(BlendCameras(introCamera, cutsceneCamera, cameraBlendDuration));

//         // --- 6️⃣ DOF & audio reset ---
//         if (dof != null)
//             StartCoroutine(FocusPull(dof, 2f, 10f, 1.5f));

//         if (backgroundMusic != null)
//             StartCoroutine(FadeAudio(backgroundMusic, 0.25f, 1f, fadeAudioDuration));

//         // --- 7️⃣ Start the cutscene dialogue ---
//         yield return new WaitForSeconds(1f);
//         if (cutsceneController != null)
//             cutsceneController.BeginCutscene();
//     }

//     IEnumerator TypeText(string text, float delay)
//     {
//         chapterText.text = "";
//         chapterText.alpha = 1;
//         foreach (char c in text)
//         {
//             chapterText.text += c;
//             yield return new WaitForSeconds(delay);
//         }
//     }

//     IEnumerator FadeInLogo()
//     {
//         float t = 0;
//         while (t < fadeDuration)
//         {
//             float a = Mathf.Lerp(0, 1, t / fadeDuration);
//             logoImage.color = new Color(1, 1, 1, a);
//             t += Time.deltaTime;
//             yield return null;
//         }
//     }

//     IEnumerator FadeOutIntro()
//     {
//         float t = 0;
//         while (t < fadeDuration)
//         {
//             float a = Mathf.Lerp(1, 0, t / fadeDuration);
//             blackPanel.color = new Color(0, 0, 0, a);
//             chapterText.alpha = 1 - a;
//             logoImage.color = new Color(1, 1, 1, 1 - a);
//             t += Time.deltaTime;
//             yield return null;
//         }
//     }

//     IEnumerator BlendCameras(Camera fromCam, Camera toCam, float duration)
//     {
//         toCam.gameObject.SetActive(true);

//         Vector3 startPos = fromCam.transform.position;
//         Quaternion startRot = fromCam.transform.rotation;
//         Vector3 endPos = toCam.transform.position;
//         Quaternion endRot = toCam.transform.rotation;

//         float t = 0;
//         while (t < duration)
//         {
//             float blend = t / duration;
//             fromCam.transform.position = Vector3.Lerp(startPos, endPos, blend);
//             fromCam.transform.rotation = Quaternion.Slerp(startRot, endRot, blend);
//             t += Time.deltaTime;
//             yield return null;
//         }

//         fromCam.gameObject.SetActive(false);
//     }

//     IEnumerator FocusPull(DepthOfField dof, float from, float to, float duration)
//     {
//         float t = 0;
//         while (t < duration)
//         {
//             dof.focusDistance.value = Mathf.Lerp(from, to, t / duration);
//             t += Time.deltaTime;
//             yield return null;
//         }
//     }

//     IEnumerator FadeAudio(AudioSource source, float from, float to, float duration)
//     {
//         float t = 0;
//         float startVol = source.volume * from;
//         float targetVol = source.volume * to;

//         while (t < duration)
//         {
//             source.volume = Mathf.Lerp(startVol, targetVol, t / duration);
//             t += Time.deltaTime;
//             yield return null;
//         }

//         source.volume = targetVol;
//     }
// }
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;
using System.Collections;

public class IntroManager : MonoBehaviour
{
    [Header("UI")]
    public CanvasGroup introPanel;
    public TMP_Text chapterText;
    public Image logoImage;

    [Header("Camera")]
    public Animator introCameraAnim;
    public Camera introCamera;
    public Camera cutsceneCamera;
    public Camera mainCamera;

    [Header("Timing")]
    public float typewriterDelay = 0.07f;
    public float chapterHold = 1f;
    public float logoFadeIn = 1.2f;
    public float logoHold = 1.5f;
    public float logoFadeOut = 1f;
    public float cameraAnimLength = 8f;
    public float transitionDelay = 1f;

    [Header("Next Step")]
    public CutsceneController cutsceneController;

    IEnumerator Start()
    {
        // --- Ensure visibility setup ---
        introCamera.gameObject.SetActive(true);
        cutsceneCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(false);

        introPanel.gameObject.SetActive(true);
        introPanel.alpha = 1f;
        introPanel.interactable = false;
        introPanel.blocksRaycasts = false;

        chapterText.text = "";
        logoImage.color = new Color(1, 1, 1, 0);

        // 🟩 1. Typewriter for “Chapter 01”
        string text = "Chapter 01";
        foreach (char c in text)
        {
            chapterText.text += c;
            yield return new WaitForSeconds(typewriterDelay);
        }
        yield return new WaitForSeconds(chapterHold);

        // 🟩 2. Fade in logo
        yield return StartCoroutine(FadeImage(logoImage, 0f, 1f, logoFadeIn));
        yield return new WaitForSeconds(logoHold);

        // 🟩 3. Fade out both
        yield return StartCoroutine(FadeImage(logoImage, 1f, 0f, logoFadeOut));
        yield return StartCoroutine(FadeText(chapterText, 1f, 0f, 0.8f));

        // Keep intro panel enabled until animation finishes
        introPanel.alpha = 0f;

        // 🟩 4. Trigger intro camera animation *only now*
        if (introCameraAnim != null)
        {
            introCameraAnim.ResetTrigger("Play");
            introCameraAnim.SetTrigger("Play");
        }

        // Wait until camera animation fully finishes
        yield return new WaitForSeconds(cameraAnimLength);

        // 🟩 5. Fade to black before switching camera
        yield return StartCoroutine(FadeCanvas(introPanel, 0f, 1f, 0.5f));

        // 🟩 6. Switch to cutscene camera
        introCamera.gameObject.SetActive(false);
        cutsceneCamera.gameObject.SetActive(true);

        // 🟩 7. Fade out black and start dialogue
        yield return StartCoroutine(FadeCanvas(introPanel, 1f, 0f, 0.5f));
       if (introPanel.gameObject != this.gameObject)
             introPanel.gameObject.SetActive(false);

        yield return new WaitForSeconds(transitionDelay);

        // 🟩 8. Begin cutscene dialogue
        if (cutsceneController != null)
        {
            yield return StartCoroutine(cutsceneController.PlayCutsceneSequence());
        }

        // 🟩 9. When done, show main camera
        cutsceneCamera.gameObject.SetActive(false);
        mainCamera.gameObject.SetActive(true);
    }

    IEnumerator FadeCanvas(CanvasGroup cg, float from, float to, float dur)
    {
        float t = 0;
        while (t < dur)
        {
            cg.alpha = Mathf.Lerp(from, to, t / dur);
            t += Time.deltaTime;
            yield return null;
        }
        cg.alpha = to;
    }

    IEnumerator FadeImage(Image img, float from, float to, float dur)
    {
        float t = 0;
        Color c = img.color;
        while (t < dur)
        {
            c.a = Mathf.Lerp(from, to, t / dur);
            img.color = c;
            t += Time.deltaTime;
            yield return null;
        }
        c.a = to;
        img.color = c;
    }

    IEnumerator FadeText(TMP_Text txt, float from, float to, float dur)
    {
        float t = 0;
        Color c = txt.color;
        while (t < dur)
        {
            c.a = Mathf.Lerp(from, to, t / dur);
            txt.color = c;
            t += Time.deltaTime;
            yield return null;
        }
        c.a = to;
        txt.color = c;
    }
    
}

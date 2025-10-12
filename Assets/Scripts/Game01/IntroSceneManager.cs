using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class IntroSceneManager : MonoBehaviour
{
    [Header("UI")]
    public CanvasGroup introPanel;      // black panel (CanvasGroup)
    public TMP_Text chapterText;
    public Image logoImage;

    [Header("Timing")]
    public float typeDelay = 0.06f;     // typewriter speed
    public float holdAfterText = 0.8f;
    public float logoFadeIn = 1.0f;
    public float logoHold = 1.4f;
    public float logoFadeOut = 0.8f;
    public float fadeToBlackBeforeLoad = 0.6f;

    [Header("Scene")]
    public string mainSceneName = "MainScene"; // exact name in Build Settings

    [Header("Debug")]
    public bool debugLogs = true;

    IEnumerator Start()
    {
        if (debugLogs) Debug.Log("[Intro] Starting intro sequence...");

        introPanel.alpha = 1f;
        introPanel.interactable = false;
        introPanel.blocksRaycasts = false;

        chapterText.text = "";
        logoImage.color = new Color(1, 1, 1, 0);

        // 1️⃣ Typewriter “Chapter 01”
        string full = "Chapter 01";
        foreach (char c in full)
        {
            chapterText.text += c;
            yield return new WaitForSeconds(typeDelay);
        }
        yield return new WaitForSeconds(holdAfterText);

        // 2️⃣ Fade in logo
        yield return StartCoroutine(FadeImage(logoImage, 0f, 1f, logoFadeIn));
        yield return new WaitForSeconds(logoHold);

        // 3️⃣ Start loading main scene async (invisible background)
        AsyncOperation op = SceneManager.LoadSceneAsync(mainSceneName);
        op.allowSceneActivation = false;

        if (debugLogs) Debug.Log("[Intro] Loading next scene in background...");

        // fade out logo + text during loading
        yield return StartCoroutine(FadeImage(logoImage, 1f, 0f, logoFadeOut));
        yield return StartCoroutine(FadeText(chapterText, 1f, 0f, logoFadeOut));

        // wait for background load to reach ~90%
        while (op.progress < 0.9f)
        {
            yield return null;
        }

        if (debugLogs) Debug.Log("[Intro] Scene preloaded. Fading to black...");

        // 4️⃣ Fade panel fully black (like cinematic fade-out)
        yield return StartCoroutine(FadeCanvas(introPanel, 0f, 1f, fadeToBlackBeforeLoad));

        yield return new WaitForSeconds(0.2f);

        // 5️⃣ Activate scene
        op.allowSceneActivation = true;
    }

    // 🔧 Fade Helpers
    IEnumerator FadeImage(Image img, float from, float to, float dur)
    {
        float t = 0f;
        Color col = img.color;
        col.a = from;
        img.color = col;
        while (t < dur)
        {
            t += Time.deltaTime;
            col.a = Mathf.Lerp(from, to, t / dur);
            img.color = col;
            yield return null;
        }
        col.a = to;
        img.color = col;
    }

    IEnumerator FadeText(TMP_Text txt, float from, float to, float dur)
    {
        float t = 0f;
        Color c = txt.color;
        c.a = from;
        txt.color = c;
        while (t < dur)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, t / dur);
            txt.color = c;
            yield return null;
        }
        c.a = to;
        txt.color = c;
    }

    IEnumerator FadeCanvas(CanvasGroup cg, float from, float to, float dur)
    {
        float t = 0f;
        cg.alpha = from;
        while (t < dur)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, t / dur);
            yield return null;
        }
        cg.alpha = to;
    }
}

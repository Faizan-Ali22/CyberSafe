using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

// Controls the HUD bar + all screen panels
// Attach to: GameManager

public class HUDManager : MonoBehaviour
{
    [Header("HUD References")]
    public TMP_Text      waveTimeText;
    public TMP_Text      loadPctText;
    public TMP_Text      frustPctText;
    public RectTransform loadBarFill;
    public RectTransform frustBarFill;
    public RectTransform loadBarBG;
    public RectTransform frustBarBG;

    [Header("Screen Panels")]
    public GameObject startScreen;
    public GameObject lostScreen;
    public GameObject wonScreen;
    public GameObject instructionOverlay;

    [Header("Lost Screen")]
    public TMP_Text failReasonText;

    [Header("Won Screen")]
    public TMP_Text finalLoadText;
    public TMP_Text finalFrustText;

    // Colors
    static readonly Color COL_LOAD_SAFE  = new Color(0.063f, 0.725f, 0.506f); // #10B981
    static readonly Color COL_LOAD_CRIT  = new Color(0.937f, 0.267f, 0.267f); // #EF4444
    static readonly Color COL_FRUST      = new Color(0.961f, 0.620f, 0.043f); // #F59E0B
    static readonly Color COL_TEXT_WHITE = Color.white;
    static readonly Color COL_TEXT_CRIT  = new Color(0.937f, 0.267f, 0.267f); // #EF4444
    static readonly Color COL_TEXT_AMBER = new Color(0.961f, 0.620f, 0.043f); // #F59E0B

    void Start()
    {
        GameController.Instance.OnStateChanged += OnStateChanged;
        GameController.Instance.OnTick         += UpdateHUD;

        // Show start screen on launch
        ShowOnlyScreen(startScreen);
        instructionOverlay.SetActive(false);
    }

    // ── Screen switching ──────────────────────────────────────────
    void OnStateChanged(GameController.GameState state)
    {
        switch (state)
        {
            case GameController.GameState.START:
                ShowOnlyScreen(startScreen);
                instructionOverlay.SetActive(false);
                break;

            case GameController.GameState.PLAYING:
                ShowOnlyScreen(null);
                instructionOverlay.SetActive(true);
                StartCoroutine(FadeOutInstruction());
                UpdateHUD();
                break;

            case GameController.GameState.LOST:
                ShowOnlyScreen(lostScreen);
                SetLostText();
                instructionOverlay.SetActive(false);
                break;

            case GameController.GameState.WON:
                ShowOnlyScreen(wonScreen);
                SetWonText();
                instructionOverlay.SetActive(false);
                break;
        }
    }

    // Show one screen, hide the rest
    void ShowOnlyScreen(GameObject target)
    {
        startScreen.SetActive(startScreen == target);
        lostScreen .SetActive(lostScreen  == target);
        wonScreen  .SetActive(wonScreen   == target);
    }

    // ── Instruction overlay fade out ──────────────────────────────
    IEnumerator FadeOutInstruction()
    {
        // Wait 3 seconds then fade out
        yield return new WaitForSeconds(3f);

        CanvasGroup cg = instructionOverlay.GetComponent<CanvasGroup>();
        if (cg == null) cg = instructionOverlay.AddComponent<CanvasGroup>();

        float elapsed = 0f;
        float duration = 1f;
        float startAlpha = 0.3f;

        while (elapsed < duration)
        {
            elapsed  += Time.deltaTime;
            cg.alpha  = Mathf.Lerp(startAlpha, 0f, elapsed / duration);
            yield return null;
        }

        instructionOverlay.SetActive(false);
    }

    // ── HUD update (called every second + on load/frust change) ──
    void UpdateHUD()
    {
        float load  = GameController.Instance.Load;
        float frust = GameController.Instance.Frustration;
        int   wave  = GameController.Instance.Wave;
        float time  = GameController.Instance.TimeLeft;

        // Wave + time label
        waveTimeText.text = $"WAVE {wave}/3 | T-{Mathf.CeilToInt(time)}s";

        // Load percentage text
        loadPctText.text  = $"{Mathf.FloorToInt(load)}%";
        loadPctText.color = load > 80f ? COL_TEXT_CRIT : COL_TEXT_WHITE;

        // Frustration percentage text
        frustPctText.text  = $"{Mathf.FloorToInt(frust)}%";
        frustPctText.color = frust > 80f ? COL_TEXT_AMBER : COL_TEXT_WHITE;

        // Bar fills
        SetBarWidth(loadBarFill,  loadBarBG,  load  / 100f);
        SetBarWidth(frustBarFill, frustBarBG, frust / 100f);

        // Load bar color
        Image loadFillImage = loadBarFill.GetComponent<Image>();
        if (loadFillImage != null)
            loadFillImage.color = load > 80f ? COL_LOAD_CRIT : COL_LOAD_SAFE;
    }

    // Drive bar fill width as a fraction of its background width
    void SetBarWidth(RectTransform fill, RectTransform bg, float fraction)
    {
        float maxWidth = bg.rect.width;
        fill.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            maxWidth * Mathf.Clamp01(fraction)
        );
    }

    // ── Lost screen text ──────────────────────────────────────────
    void SetLostText()
    {
        string reason = GameController.Instance.LostReason;
        failReasonText.text = reason == "SERVER_CRASH"
            ? "Load exceeded 100%. The server melted down under the malicious requests."
            : "Frustration exceeded 100%. You over-blocked and locked out the real students.";
    }

    // ── Won screen text ───────────────────────────────────────────
    void SetWonText()
    {
        finalLoadText.text  = $"{Mathf.FloorToInt(GameController.Instance.Load)}%";
        finalFrustText.text = $"{Mathf.FloorToInt(GameController.Instance.Frustration)}%";
    }

    void OnDestroy()
    {
        if (GameController.Instance != null)
        {
            GameController.Instance.OnStateChanged -= OnStateChanged;
            GameController.Instance.OnTick         -= UpdateHUD;
        }
    }
}
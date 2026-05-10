using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

// Handles floating rule text popups that appear when a wall is deployed


public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    [Header("References")]
    public RectTransform popupContainer; // assign UIScreens or Canvas RectTransform
    public TMP_FontAsset fontAsset;      // assign JetBrainsMono Bold SDF

    // Colors
    static readonly Color COL_CYAN_TEXT   = new Color(0.220f, 0.741f, 0.973f); // #38BDF8
    static readonly Color COL_CYAN_BG     = new Color(0.059f, 0.090f, 0.165f, 0.90f); // #0F172A 90%
    static readonly Color COL_CYAN_BORDER = new Color(0.220f, 0.741f, 0.973f, 0.80f); // #38BDF8 80%

    static readonly Color COL_AMBER_TEXT   = new Color(0.961f, 0.620f, 0.043f); // #F59E0B
    static readonly Color COL_AMBER_BG     = new Color(0.059f, 0.090f, 0.165f, 0.90f);
    static readonly Color COL_AMBER_BORDER = new Color(0.961f, 0.620f, 0.043f, 0.80f);

    // Track active popups so we can clear on reset
    private List<GameObject> _activePopups = new List<GameObject>();

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        GameController.Instance.OnStateChanged += OnStateChanged;
    }

    void OnStateChanged(GameController.GameState state)
    {
        if (state == GameController.GameState.PLAYING)
            ClearAll();
    }

    // ── Main public method ────────────────────────────────────────
    // pos     = RenderTexture pixel position (from FirewallManager)
    // text    = rule text or "TIMEOUT (UX DROP)"
    // isAmber = true for timeout popups, false for rule popups
    public void ShowPopup(Vector2 rtPos, string text, bool isAmber = false)
    {
        // Convert RenderTexture pixel pos → Canvas local pos
        Vector2 canvasPos = RTPixelToCanvasPos(rtPos);

        // Offset upward so popup appears above the wall midpoint
        canvasPos.y += 40f;

        // Choose colors
        Color textCol   = isAmber ? COL_AMBER_TEXT   : COL_CYAN_TEXT;
        Color bgCol     = isAmber ? COL_AMBER_BG     : COL_CYAN_BG;
        Color borderCol = isAmber ? COL_AMBER_BORDER : COL_CYAN_BORDER;

        // Build popup GameObject
        GameObject popup = BuildPopup(text, textCol, bgCol, borderCol);
        popup.transform.SetParent(popupContainer, false);

        RectTransform rt = popup.GetComponent<RectTransform>();
        rt.anchoredPosition = canvasPos;

        _activePopups.Add(popup);

        // Animate and destroy after lifetime
        StartCoroutine(AnimatePopup(popup, rt));
    }

    // ── Build the popup UI object ─────────────────────────────────
    GameObject BuildPopup(string text, Color textCol, Color bgCol, Color borderCol)
    {
        // Root object — holds background Image
        GameObject root = new GameObject("Popup");
        root.AddComponent<RectTransform>();

        // Background Image
        Image bg = root.AddComponent<Image>();
        bg.color = bgCol;
        bg.raycastTarget = false;

        // Outline to simulate border
        Outline outline = root.AddComponent<Outline>();
        outline.effectColor    = borderCol;
        outline.effectDistance = new Vector2(1f, -1f);

        // TMP Text child
        GameObject textObj = new GameObject("PopupText");
        textObj.transform.SetParent(root.transform, false);

        TMP_Text tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text         = text;
        tmp.color        = textCol;
        tmp.fontSize     = 16f;
        tmp.fontStyle    = FontStyles.Bold;
        tmp.alignment    = TextAlignmentOptions.Center;
        tmp.raycastTarget = false;

        if (fontAsset != null)
            tmp.font = fontAsset;

        // Size the root to fit text with padding
        RectTransform rootRT = root.GetComponent<RectTransform>();
        rootRT.pivot         = new Vector2(0.5f, 0.5f);

        // Estimate width from character count
        float estimatedWidth = text.Length * 9.5f + 18f;
        rootRT.sizeDelta = new Vector2(estimatedWidth, 32f);

        // TMP fills root
        RectTransform textRT = textObj.GetComponent<RectTransform>();
        textRT.anchorMin    = Vector2.zero;
        textRT.anchorMax    = Vector2.one;
        textRT.offsetMin    = new Vector2(6f,  2f);
        textRT.offsetMax    = new Vector2(-6f, -2f);

        // Add CanvasGroup for alpha control
        root.AddComponent<CanvasGroup>();

        return root;
    }

    // ── Animate: rise upward + fade in + fade out ─────────────────
    IEnumerator AnimatePopup(GameObject popup, RectTransform rt)
    {
        if (popup == null) yield break;

        CanvasGroup cg      = popup.GetComponent<CanvasGroup>();
        Vector2     startPos = rt.anchoredPosition;
        float       life     = 1.5f; // seconds (matches React 90 frames / 60fps)
        float       elapsed  = 0f;

        while (elapsed < life)
        {
            if (popup == null) yield break;

            elapsed += Time.deltaTime;

            // Rise upward 30px over lifetime (0.5px per frame at 60fps)
            rt.anchoredPosition = startPos + Vector2.up * (elapsed * 20f);

            // Fade in fast (first 0.3s), hold, fade out (last 0.3s)
            float alpha;
            if      (elapsed < 0.3f)        alpha = elapsed / 0.3f;
            else if (elapsed > life - 0.3f) alpha = (life - elapsed) / 0.3f;
            else                            alpha = 1f;

            cg.alpha = Mathf.Clamp01(alpha);

            yield return null;
        }

        _activePopups.Remove(popup);
        Destroy(popup);
    }

    // ── Convert RenderTexture pixel pos → Canvas anchored pos ─────
    // RenderTexture is 1920x1080, origin top-left
    // Canvas is 1920x1080 Screen Space Overlay, origin center
    Vector2 RTPixelToCanvasPos(Vector2 rtPos)
    {
        // rt origin is top-left, canvas origin is center
        float x = rtPos.x - 960f; // shift from [0,1920] to [-960, 960]
        float y = -(rtPos.y - 540f); // flip Y and shift from [0,1080] to [-540, 540]
        return new Vector2(x, y);
    }

    // ── Clear all popups on game reset ────────────────────────────
    public void ClearAll()
    {
        StopAllCoroutines();

        foreach (var p in _activePopups)
            if (p != null) Destroy(p);

        _activePopups.Clear();
    }

    void OnDestroy()
    {
        if (GameController.Instance != null)
            GameController.Instance.OnStateChanged -= OnStateChanged;
    }
}
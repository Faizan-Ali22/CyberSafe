using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[DefaultExecutionOrder(-200)]
[RequireComponent(typeof(RansomwareLogic))]
public class RansomwareUI : MonoBehaviour
{
    [Header("Auto-Setup")]
    [SerializeField] private Canvas targetCanvas;

    private RansomwareLogic logic;
    private RectTransform rootRect;
    private RectTransform fileLayer;
    private RectTransform trapLayer;
    private Image waveFill;
    private RectTransform driveRect;
    private TextMeshProUGUI securedCountText;
    private RectTransform driveListContainer;
    private GameObject startOverlay;
    private GameObject endOverlay;
    private TextMeshProUGUI endTitleText;
    private TextMeshProUGUI endSubtitleText;
    private TextMeshProUGUI endRankText;
    private TextMeshProUGUI endStatsText;
    private Button startButton;
    private Button retryButton;
    private RectTransform dragGhostRect;
    private TextMeshProUGUI dragGhostIcon;
    private TextMeshProUGUI dragGhostLabel;

    private void Awake()
    {
        logic = GetComponent<RansomwareLogic>();

        if (targetCanvas == null)
        {
            targetCanvas = FindFirstObjectByType<Canvas>();
            if (targetCanvas == null)
            {
                CreateCanvas();
            }
        }

        EnsureEventSystem();
        BuildUI();
        AssignReferences();
    }

    private void CreateCanvas()
    {
        GameObject canvasObj = new GameObject("RansomwareCanvas");
        targetCanvas = canvasObj.AddComponent<Canvas>();
        targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        targetCanvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        canvasObj.AddComponent<GraphicRaycaster>();
    }

    private void EnsureEventSystem()
    {
        var existing = FindFirstObjectByType<EventSystem>();
        if (existing == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM
            eventSystem.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
#else
            eventSystem.AddComponent<StandaloneInputModule>();
#endif
        }
#if ENABLE_INPUT_SYSTEM
        else
        {
            var oldModule = existing.GetComponent<StandaloneInputModule>();
            var newModule = existing.GetComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            if (newModule == null && oldModule != null)
            {
                DestroyImmediate(oldModule);
                existing.gameObject.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            }
        }
#endif
    }

    private void BuildUI()
    {
        GameObject root = CreatePanel("RansomwareRoot", targetCanvas.transform, new Color(0.12f, 0.16f, 0.23f, 1f));
        rootRect = root.GetComponent<RectTransform>();
        StretchToFullScreen(rootRect);

        fileLayer = CreateLayer("FileLayer", root.transform);
        trapLayer = CreateLayer("TrapLayer", root.transform);

        GameObject wave = CreatePanel("Wave", root.transform, new Color(0.4f, 0.05f, 0.08f, 0.6f));
        waveFill = wave.GetComponent<Image>();
        RectTransform waveRect = wave.GetComponent<RectTransform>();
        waveRect.anchorMin = new Vector2(0, 0);
        waveRect.anchorMax = new Vector2(0, 1);
        waveRect.pivot = new Vector2(0, 0.5f);
        waveRect.sizeDelta = new Vector2(0, 0);
        waveFill.raycastTarget = false;

        GameObject drivePanel = CreatePanel("DrivePanel", root.transform, new Color(0.08f, 0.1f, 0.15f, 0.95f));
        driveRect = drivePanel.GetComponent<RectTransform>();
        driveRect.anchorMin = new Vector2(0.75f, 0);
        driveRect.anchorMax = new Vector2(1, 1);
        driveRect.offsetMin = Vector2.zero;
        driveRect.offsetMax = Vector2.zero;

        TextMeshProUGUI driveIcon = CreateText("DriveIcon", drivePanel.transform, "💾", 48, TextAlignmentOptions.Center);
        driveIcon.rectTransform.anchoredPosition = new Vector2(0, 360);

        TextMeshProUGUI driveLabel = CreateText("DriveLabel", drivePanel.transform, "SAFE\nBACKUP", 18, TextAlignmentOptions.Center);
        driveLabel.rectTransform.anchoredPosition = new Vector2(0, 300);
        driveLabel.color = new Color(0.7f, 0.9f, 0.7f);

        securedCountText = CreateText("SecuredCount", drivePanel.transform, "0 SECURED", 14, TextAlignmentOptions.Center);
        securedCountText.rectTransform.anchoredPosition = new Vector2(0, 240);
        securedCountText.color = new Color(0.7f, 0.7f, 0.8f);

        GameObject scrollObj = new GameObject("DriveScroll");
        scrollObj.transform.SetParent(drivePanel.transform, false);
        RectTransform scrollRect = scrollObj.AddComponent<RectTransform>();
        scrollRect.anchorMin = new Vector2(0, 0);
        scrollRect.anchorMax = new Vector2(1, 1);
        scrollRect.offsetMin = new Vector2(20, 20);
        scrollRect.offsetMax = new Vector2(-20, -260);

        Image viewportImage = scrollObj.AddComponent<Image>();
        viewportImage.color = new Color(0f, 0f, 0f, 0.15f);
        Mask mask = scrollObj.AddComponent<Mask>();
        mask.showMaskGraphic = false;
        ScrollRect scroll = scrollObj.AddComponent<ScrollRect>();
        scroll.viewport = scrollRect;
        scroll.horizontal = false;
        scroll.vertical = true;

        GameObject contentObj = new GameObject("Content");
        contentObj.transform.SetParent(scrollObj.transform, false);
        RectTransform contentRect = contentObj.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 1);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.pivot = new Vector2(0.5f, 1);
        contentRect.sizeDelta = new Vector2(0, 0);
        var layout = contentObj.AddComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;
        layout.spacing = 6;
        contentObj.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scroll.content = contentRect;
        driveListContainer = contentRect;

        startOverlay = CreateOverlay("StartOverlay", root.transform, new Color(0f, 0f, 0f, 0.85f));
        TextMeshProUGUI startTitle = CreateText("StartTitle", startOverlay.transform, "RANSOMWARE SIMULATOR", 36, TextAlignmentOptions.Center);
        startTitle.color = new Color(1f, 0.3f, 0.3f);
        startTitle.rectTransform.anchoredPosition = new Vector2(0, 200);

        TextMeshProUGUI startBody = CreateText("StartBody", startOverlay.transform,
            "A destructive encryption wave is sweeping across your system.\nRescue your files before they are corrupted!", 18, TextAlignmentOptions.Center);
        startBody.rectTransform.anchoredPosition = new Vector2(0, 120);
        startBody.color = new Color(0.85f, 0.85f, 0.9f);

        TextMeshProUGUI startHints = CreateText("StartHints", startOverlay.transform,
            "• The spreading red tide encrypts instantly.\n• Drag files onto the SAFE BACKUP drive.\n• Important files glow yellow.\n• Ignore pop-up traps.", 16, TextAlignmentOptions.Center);
        startHints.rectTransform.anchoredPosition = new Vector2(0, 20);
        startHints.color = new Color(0.8f, 0.8f, 0.85f);

        startButton = CreateButton("StartButton", startOverlay.transform, "START RECOVERY OPERATION", 20);
        startButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -120);
        startButton.GetComponent<Image>().color = new Color(0.7f, 0.1f, 0.1f);

        endOverlay = CreateOverlay("EndOverlay", root.transform, new Color(0f, 0f, 0f, 0.9f));
        endOverlay.SetActive(false);
        endTitleText = CreateText("EndTitle", endOverlay.transform, "ENCRYPTION COMPLETE", 32, TextAlignmentOptions.Center);
        endTitleText.rectTransform.anchoredPosition = new Vector2(0, 200);
        endTitleText.color = Color.white;

        endSubtitleText = CreateText("EndSubtitle", endOverlay.transform, "", 16, TextAlignmentOptions.Center);
        endSubtitleText.rectTransform.anchoredPosition = new Vector2(0, 140);
        endSubtitleText.color = new Color(0.9f, 0.9f, 0.9f);

        endRankText = CreateText("EndRank", endOverlay.transform, "", 22, TextAlignmentOptions.Center);
        endRankText.rectTransform.anchoredPosition = new Vector2(0, 60);

        endStatsText = CreateText("EndStats", endOverlay.transform, "", 18, TextAlignmentOptions.Center);
        endStatsText.rectTransform.anchoredPosition = new Vector2(0, -20);
        endStatsText.color = new Color(0.85f, 0.85f, 0.9f);

        retryButton = CreateButton("RetryButton", endOverlay.transform, "REBOOT SYSTEM & TRY AGAIN", 18);
        retryButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -140);
        retryButton.GetComponent<Image>().color = new Color(0.9f, 0.9f, 0.9f);

        GameObject dragGhost = CreatePanel("DragGhost", root.transform, new Color(0f, 0f, 0f, 0f));
        Image dragGhostImage = dragGhost.GetComponent<Image>();
        dragGhostImage.raycastTarget = false;
        dragGhostRect = dragGhost.GetComponent<RectTransform>();
        dragGhostRect.sizeDelta = new Vector2(120, 120);
        dragGhostIcon = CreateText("DragGhostIcon", dragGhostRect, "📄", 32, TextAlignmentOptions.Center);
        dragGhostIcon.rectTransform.anchoredPosition = new Vector2(0, 20);
        dragGhostLabel = CreateText("DragGhostLabel", dragGhostRect, "", 12, TextAlignmentOptions.Center);
        dragGhostLabel.rectTransform.anchoredPosition = new Vector2(0, -30);
        dragGhostLabel.color = Color.white;
    }

    private void AssignReferences()
    {
        if (logic == null)
        {
            return;
        }

        logic.AssignUIReferences(
            rootRect,
            fileLayer,
            trapLayer,
            waveFill,
            driveRect,
            securedCountText,
            driveListContainer,
            startOverlay,
            endOverlay,
            endTitleText,
            endSubtitleText,
            endRankText,
            endStatsText,
            startButton,
            retryButton,
            dragGhostRect,
            dragGhostIcon,
            dragGhostLabel
        );
    }

    private GameObject CreatePanel(string name, Transform parent, Color color)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.localScale = Vector3.one;
        Image img = obj.AddComponent<Image>();
        img.color = color;
        return obj;
    }

    private RectTransform CreateLayer(string name, Transform parent)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        return rect;
    }

    private GameObject CreateOverlay(string name, Transform parent, Color color)
    {
        GameObject obj = CreatePanel(name, parent, color);
        RectTransform rect = obj.GetComponent<RectTransform>();
        StretchToFullScreen(rect);
        return obj;
    }

    private TextMeshProUGUI CreateText(string name, Transform parent, string text, int size, TextAlignmentOptions alignment)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(900, 80);
        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.alignment = alignment;
        tmp.enableAutoSizing = false;
        tmp.raycastTarget = false;
        tmp.color = Color.white;
        return tmp;
    }

    private Button CreateButton(string name, Transform parent, string label, int fontSize)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(420, 60);
        Image img = obj.AddComponent<Image>();
        img.color = new Color(0.2f, 0.4f, 0.6f);
        Button button = obj.AddComponent<Button>();

        TextMeshProUGUI text = CreateText("Text", obj.transform, label, fontSize, TextAlignmentOptions.Center);
        text.rectTransform.anchorMin = Vector2.zero;
        text.rectTransform.anchorMax = Vector2.one;
        text.rectTransform.sizeDelta = Vector2.zero;
        text.color = Color.white;
        return button;
    }

    private void StretchToFullScreen(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}

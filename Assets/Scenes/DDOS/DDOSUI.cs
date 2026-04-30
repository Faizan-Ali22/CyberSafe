using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[DefaultExecutionOrder(-200)]
[RequireComponent(typeof(DDOSLogic))]
public class DDOSUI : MonoBehaviour
{
    [Header("Auto-Setup")]
    [SerializeField] private Canvas targetCanvas;

    private DDOSLogic logic;
    private RectTransform gameArea;
    private RectTransform packetLayer;
    private RectTransform trailLayer;
    private RectTransform particleLayer;
    private RectTransform floatingTextLayer;
    private TextMeshProUGUI waveText;
    private TextMeshProUGUI scoreText;
    private TextMeshProUGUI loadPercentText;
    private Image loadFill;
    private TextMeshProUGUI frustrationPercentText;
    private Image frustrationFill;
    private GameObject tutorialOverlay;
    private CanvasGroup tutorialCanvasGroup;
    private TextMeshProUGUI tutorialTitleText;
    private TextMeshProUGUI tutorialBodyText;
    private Button tutorialStartButton;
    private GameObject stateOverlay;
    private GameObject startPanel;
    private GameObject lostPanel;
    private GameObject wonPanel;
    private TextMeshProUGUI lostReasonText;
    private TextMeshProUGUI finalScoreText;
    private TextMeshProUGUI finalWaveText;
    private TextMeshProUGUI wonScoreText;
    private Button startTutorialButton;
    private Button startGameButton;
    private Button retryButton;
    private Button playAgainButton;

    private void Awake()
    {
        logic = GetComponent<DDOSLogic>();

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
        GameObject canvasObj = new GameObject("DDOSCanvas");
        targetCanvas = canvasObj.AddComponent<Canvas>();
        targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        targetCanvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
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
        GameObject root = CreatePanel("DDOSRoot", targetCanvas.transform, new Color(0.02f, 0.04f, 0.08f, 1f));
        RectTransform rootRect = root.GetComponent<RectTransform>();
        StretchToFullScreen(rootRect);

        GameObject header = CreatePanel("Header", root.transform, new Color(0.05f, 0.09f, 0.15f, 1f));
        RectTransform headerRect = header.GetComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0, 1);
        headerRect.anchorMax = new Vector2(1, 1);
        headerRect.pivot = new Vector2(0.5f, 1f);
        headerRect.sizeDelta = new Vector2(0, 160);

        TextMeshProUGUI title = CreateText("Title", header.transform, "DEFCON: TACTICAL", 20, TextAlignmentOptions.Left);
        title.rectTransform.anchoredPosition = new Vector2(20, -20);
        title.color = new Color(0.3f, 0.9f, 0.9f);

        waveText = CreateText("WaveText", header.transform, "WAVE 1/3", 12, TextAlignmentOptions.Right);
        waveText.rectTransform.anchoredPosition = new Vector2(-30, -20);
        waveText.color = new Color(0.5f, 0.8f, 0.9f);

        scoreText = CreateText("ScoreText", header.transform, "00000", 24, TextAlignmentOptions.Right);
        scoreText.rectTransform.anchoredPosition = new Vector2(-30, -50);
        scoreText.color = new Color(0.3f, 1f, 1f);

        GameObject loadBar = CreatePanel("LoadBar", header.transform, new Color(0.1f, 0.1f, 0.16f, 1f));
        RectTransform loadRect = loadBar.GetComponent<RectTransform>();
        loadRect.sizeDelta = new Vector2(380, 20);
        loadRect.anchoredPosition = new Vector2(0, -90);

        TextMeshProUGUI loadLabel = CreateText("LoadLabel", header.transform, "SERVER LOAD", 10, TextAlignmentOptions.Left);
        loadLabel.rectTransform.anchoredPosition = new Vector2(20, -80);
        loadLabel.color = new Color(0.3f, 0.8f, 0.9f);

        loadPercentText = CreateText("LoadPercent", header.transform, "0%", 10, TextAlignmentOptions.Right);
        loadPercentText.rectTransform.anchoredPosition = new Vector2(-30, -80);
        loadPercentText.color = new Color(0.8f, 0.8f, 0.8f);

        loadFill = CreatePanel("LoadFill", loadBar.transform, new Color(0.94f, 0.27f, 0.27f, 1f)).GetComponent<Image>();
        RectTransform loadFillRect = loadFill.GetComponent<RectTransform>();
        loadFillRect.anchorMin = new Vector2(0, 0);
        loadFillRect.anchorMax = new Vector2(0, 1);
        loadFillRect.pivot = new Vector2(0, 0.5f);
        loadFillRect.sizeDelta = new Vector2(0, 0);
        loadFill.type = Image.Type.Filled;
        loadFill.fillMethod = Image.FillMethod.Horizontal;
        loadFill.fillAmount = 0;

        GameObject frustrationBar = CreatePanel("FrustrationBar", header.transform, new Color(0.1f, 0.1f, 0.16f, 1f));
        RectTransform frustrationRect = frustrationBar.GetComponent<RectTransform>();
        frustrationRect.sizeDelta = new Vector2(380, 20);
        frustrationRect.anchoredPosition = new Vector2(0, -125);

        TextMeshProUGUI frustrationLabel = CreateText("FrustrationLabel", header.transform, "USER FRUSTRATION", 10, TextAlignmentOptions.Left);
        frustrationLabel.rectTransform.anchoredPosition = new Vector2(20, -115);
        frustrationLabel.color = new Color(0.9f, 0.6f, 0.2f);

        frustrationPercentText = CreateText("FrustrationPercent", header.transform, "0%", 10, TextAlignmentOptions.Right);
        frustrationPercentText.rectTransform.anchoredPosition = new Vector2(-30, -115);
        frustrationPercentText.color = new Color(0.8f, 0.8f, 0.8f);

        frustrationFill = CreatePanel("FrustrationFill", frustrationBar.transform, new Color(0.96f, 0.65f, 0.15f, 1f)).GetComponent<Image>();
        RectTransform frustrationFillRect = frustrationFill.GetComponent<RectTransform>();
        frustrationFillRect.anchorMin = new Vector2(0, 0);
        frustrationFillRect.anchorMax = new Vector2(0, 1);
        frustrationFillRect.pivot = new Vector2(0, 0.5f);
        frustrationFillRect.sizeDelta = new Vector2(0, 0);
        frustrationFill.type = Image.Type.Filled;
        frustrationFill.fillMethod = Image.FillMethod.Horizontal;
        frustrationFill.fillAmount = 0;

        GameObject footer = CreatePanel("Footer", root.transform, new Color(0.05f, 0.09f, 0.15f, 1f));
        RectTransform footerRect = footer.GetComponent<RectTransform>();
        footerRect.anchorMin = new Vector2(0, 0);
        footerRect.anchorMax = new Vector2(1, 0);
        footerRect.pivot = new Vector2(0.5f, 0);
        footerRect.sizeDelta = new Vector2(0, 70);

        TextMeshProUGUI dropLegend = CreateText("DropLegend", footer.transform, "⬆ DROP (RED)", 10, TextAlignmentOptions.Left);
        dropLegend.rectTransform.anchoredPosition = new Vector2(20, 20);
        dropLegend.color = new Color(0.94f, 0.27f, 0.27f);

        TextMeshProUGUI tapLegend = CreateText("TapLegend", footer.transform, "TAP: INSPECT (YELLOW)", 10, TextAlignmentOptions.Center);
        tapLegend.rectTransform.anchoredPosition = new Vector2(0, 20);
        tapLegend.color = new Color(0.92f, 0.7f, 0.17f);

        TextMeshProUGUI routeLegend = CreateText("RouteLegend", footer.transform, "⬇ ROUTE (GREEN)", 10, TextAlignmentOptions.Right);
        routeLegend.rectTransform.anchoredPosition = new Vector2(-20, 20);
        routeLegend.color = new Color(0.06f, 0.73f, 0.5f);

        GameObject areaObj = CreatePanel("GameArea", root.transform, new Color(0.03f, 0.06f, 0.12f, 1f));
        gameArea = areaObj.GetComponent<RectTransform>();
        gameArea.anchorMin = new Vector2(0, 0);
        gameArea.anchorMax = new Vector2(1, 1);
        gameArea.offsetMin = new Vector2(0, footerRect.sizeDelta.y);
        gameArea.offsetMax = new Vector2(0, -headerRect.sizeDelta.y);
        gameArea.pivot = new Vector2(0, 1);

        var inputCatcher = areaObj.AddComponent<DDOSInputCatcher>();
        inputCatcher.Initialize(logic);

        trailLayer = CreateLayer("TrailLayer", gameArea);
        packetLayer = CreateLayer("PacketLayer", gameArea);
        particleLayer = CreateLayer("ParticleLayer", gameArea);
        floatingTextLayer = CreateLayer("FloatingTextLayer", gameArea);

        tutorialOverlay = CreatePanel("TutorialOverlay", gameArea.transform, new Color(0.05f, 0.09f, 0.15f, 0.95f));
        RectTransform tutorialRect = tutorialOverlay.GetComponent<RectTransform>();
        tutorialRect.anchorMin = new Vector2(0.5f, 1);
        tutorialRect.anchorMax = new Vector2(0.5f, 1);
        tutorialRect.pivot = new Vector2(0.5f, 1);
        tutorialRect.sizeDelta = new Vector2(620, 220);
        tutorialRect.anchoredPosition = new Vector2(0, -20);

        tutorialCanvasGroup = tutorialOverlay.AddComponent<CanvasGroup>();
        tutorialTitleText = CreateText("TutorialTitle", tutorialOverlay.transform, "TRAINING STEP 1", 14, TextAlignmentOptions.Center);
        tutorialTitleText.rectTransform.anchoredPosition = new Vector2(0, -20);
        tutorialBodyText = CreateText("TutorialBody", tutorialOverlay.transform, "", 12, TextAlignmentOptions.Center);
        tutorialBodyText.rectTransform.anchoredPosition = new Vector2(0, -80);
        tutorialBodyText.color = new Color(0.85f, 0.95f, 1f);

        tutorialStartButton = CreateButton("TutorialStartButton", tutorialOverlay.transform, "START MISSION", 14);
        tutorialStartButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -150);
        tutorialStartButton.gameObject.SetActive(false);

        stateOverlay = CreateOverlay("StateOverlay", root.transform, new Color(0f, 0f, 0f, 0.85f));

        startPanel = CreatePanel("StartPanel", stateOverlay.transform, new Color(0.05f, 0.09f, 0.15f, 1f));
        RectTransform startRect = startPanel.GetComponent<RectTransform>();
        startRect.sizeDelta = new Vector2(620, 520);

        TextMeshProUGUI startTitle = CreateText("StartTitle", startPanel.transform, "GESTURE TRIAGE", 22, TextAlignmentOptions.Center);
        startTitle.rectTransform.anchoredPosition = new Vector2(0, 200);
        startTitle.color = Color.white;

        TextMeshProUGUI startDesc = CreateText("StartDesc", startPanel.transform, "Survive 3 waves of cyber attacks using touch gestures.", 12, TextAlignmentOptions.Center);
        startDesc.rectTransform.anchoredPosition = new Vector2(0, 150);
        startDesc.color = new Color(0.7f, 0.9f, 1f);

        startTutorialButton = CreateButton("StartTutorialButton", startPanel.transform, "PLAY TUTORIAL", 14);
        startTutorialButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -30);
        startTutorialButton.GetComponent<Image>().color = new Color(0.1f, 0.2f, 0.3f);

        startGameButton = CreateButton("StartGameButton", startPanel.transform, "START WAVE 1", 14);
        startGameButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -110);
        startGameButton.GetComponent<Image>().color = new Color(0.1f, 0.6f, 0.7f);

        lostPanel = CreatePanel("LostPanel", stateOverlay.transform, new Color(0.05f, 0.09f, 0.15f, 1f));
        RectTransform lostRect = lostPanel.GetComponent<RectTransform>();
        lostRect.sizeDelta = new Vector2(560, 460);

        TextMeshProUGUI lostTitle = CreateText("LostTitle", lostPanel.transform, "SYSTEM FAILURE", 20, TextAlignmentOptions.Center);
        lostTitle.rectTransform.anchoredPosition = new Vector2(0, 170);
        lostTitle.color = new Color(0.94f, 0.27f, 0.27f);

        lostReasonText = CreateText("LostReason", lostPanel.transform, "", 12, TextAlignmentOptions.Center);
        lostReasonText.rectTransform.anchoredPosition = new Vector2(0, 120);
        lostReasonText.color = new Color(0.7f, 0.7f, 0.7f);

        finalScoreText = CreateText("FinalScore", lostPanel.transform, "0", 36, TextAlignmentOptions.Center);
        finalScoreText.rectTransform.anchoredPosition = new Vector2(0, 40);
        finalScoreText.color = new Color(0.3f, 0.9f, 1f);

        finalWaveText = CreateText("FinalWave", lostPanel.transform, "", 12, TextAlignmentOptions.Center);
        finalWaveText.rectTransform.anchoredPosition = new Vector2(0, 0);
        finalWaveText.color = new Color(0.6f, 0.7f, 0.8f);

        retryButton = CreateButton("RetryButton", lostPanel.transform, "RETRY DEPLOYMENT", 14);
        retryButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -80);
        retryButton.GetComponent<Image>().color = new Color(0.1f, 0.6f, 0.7f);

        wonPanel = CreatePanel("WonPanel", stateOverlay.transform, new Color(0.05f, 0.09f, 0.15f, 1f));
        RectTransform wonRect = wonPanel.GetComponent<RectTransform>();
        wonRect.sizeDelta = new Vector2(560, 420);

        TextMeshProUGUI wonTitle = CreateText("WonTitle", wonPanel.transform, "ATTACK THWARTED", 20, TextAlignmentOptions.Center);
        wonTitle.rectTransform.anchoredPosition = new Vector2(0, 140);
        wonTitle.color = new Color(0.1f, 0.9f, 0.6f);

        TextMeshProUGUI wonDesc = CreateText("WonDesc", wonPanel.transform, "You survived all waves and secured the infrastructure.", 12, TextAlignmentOptions.Center);
        wonDesc.rectTransform.anchoredPosition = new Vector2(0, 95);
        wonDesc.color = new Color(0.7f, 0.8f, 0.9f);

        wonScoreText = CreateText("WonScore", wonPanel.transform, "0", 36, TextAlignmentOptions.Center);
        wonScoreText.rectTransform.anchoredPosition = new Vector2(0, 20);
        wonScoreText.color = new Color(0.3f, 0.9f, 1f);

        playAgainButton = CreateButton("PlayAgainButton", wonPanel.transform, "PLAY AGAIN", 14);
        playAgainButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -60);
        playAgainButton.GetComponent<Image>().color = new Color(0.1f, 0.7f, 0.5f);

        startPanel.SetActive(true);
        lostPanel.SetActive(false);
        wonPanel.SetActive(false);
    }

    private void AssignReferences()
    {
        if (logic == null)
        {
            return;
        }

        logic.AssignUIReferences(
            gameArea,
            packetLayer,
            trailLayer,
            particleLayer,
            floatingTextLayer,
            waveText,
            scoreText,
            loadPercentText,
            loadFill,
            frustrationPercentText,
            frustrationFill,
            tutorialOverlay,
            tutorialCanvasGroup,
            tutorialTitleText,
            tutorialBodyText,
            tutorialStartButton,
            stateOverlay,
            startPanel,
            lostPanel,
            wonPanel,
            lostReasonText,
            finalScoreText,
            finalWaveText,
            wonScoreText,
            startTutorialButton,
            startGameButton,
            retryButton,
            playAgainButton
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
        rect.sizeDelta = new Vector2(500, 60);
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
        rect.sizeDelta = new Vector2(360, 60);
        Image img = obj.AddComponent<Image>();
        img.color = new Color(0.2f, 0.4f, 0.6f);
        Button button = obj.AddComponent<Button>();

        TextMeshProUGUI text = CreateText("Text", obj.transform, label, fontSize, TextAlignmentOptions.Center);
        text.rectTransform.anchorMin = Vector2.zero;
        text.rectTransform.anchorMax = Vector2.one;
        text.rectTransform.sizeDelta = Vector2.zero;
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

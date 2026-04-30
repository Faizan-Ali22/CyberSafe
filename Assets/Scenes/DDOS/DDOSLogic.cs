using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

public class DDOSLogic : MonoBehaviour
{
    private enum GameState
    {
        Start,
        Playing,
        Tutorial,
        Won,
        Lost
    }

    private enum FailReason
    {
        None,
        ServerCrash,
        UserRevolt
    }

    private enum PacketState
    {
        Falling,
        Inspecting,
        Dying
    }

    private class Packet
    {
        public int Id;
        public int Lane;
        public float X;
        public float Y;
        public string Type;
        public string HiddenType;
        public PacketState State;
        public float Timer;
        public float Speed;
        public RectTransform Rect;
        public Image Image;
        public RectTransform ScanLine;
    }

    private class Particle
    {
        public RectTransform Rect;
        public Image Image;
        public Vector2 Velocity;
        public float Life;
    }

    private class SwipeTrail
    {
        public RectTransform Rect;
        public Image Image;
        public float Life;
    }

    private class FloatingText
    {
        public RectTransform Rect;
        public TextMeshProUGUI Text;
        public float Life;
    }

    [Header("UI References - AUTO-ASSIGNED")]
    [SerializeField] private RectTransform gameArea;
    [SerializeField] private RectTransform packetLayer;
    [SerializeField] private RectTransform trailLayer;
    [SerializeField] private RectTransform particleLayer;
    [SerializeField] private RectTransform floatingTextLayer;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI loadPercentText;
    [SerializeField] private Image loadFill;
    [SerializeField] private TextMeshProUGUI frustrationPercentText;
    [SerializeField] private Image frustrationFill;
    [SerializeField] private GameObject tutorialOverlay;
    [SerializeField] private CanvasGroup tutorialCanvasGroup;
    [SerializeField] private TextMeshProUGUI tutorialTitleText;
    [SerializeField] private TextMeshProUGUI tutorialBodyText;
    [SerializeField] private Button tutorialStartButton;
    [SerializeField] private GameObject stateOverlay;
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject lostPanel;
    [SerializeField] private GameObject wonPanel;
    [SerializeField] private TextMeshProUGUI lostReasonText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI finalWaveText;
    [SerializeField] private TextMeshProUGUI wonScoreText;
    [SerializeField] private Button startTutorialButton;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button playAgainButton;

    private readonly List<Packet> packets = new List<Packet>();
    private readonly List<Particle> particles = new List<Particle>();
    private readonly List<SwipeTrail> swipeTrails = new List<SwipeTrail>();
    private readonly List<FloatingText> floatingTexts = new List<FloatingText>();
    private readonly Dictionary<string, Sprite> pixelSprites = new Dictionary<string, Sprite>();

    private GameState state = GameState.Start;
    private FailReason failReason = FailReason.None;
    private int lanes = 4;
    private float laneWidth;
    private float gameWidth;
    private float gameHeight;
    private int score;
    private int load;
    private int frustration;
    private int wave = 1;
    private int frame;
    private int tutorialStep;
    private float tutorialWait;
    private Vector2 touchStart;
    private bool touchActive;
    private Vector2 lastAreaSize;
    private int pixelScale = 4;
    private Canvas rootCanvas;
    private int packetIdCounter;

    private const int TutorialLegitLaneIndex = 1;
    private const int TutorialBotLaneIndex = 2;
    private const int TutorialMaskedLaneIndex = 1;

    private static readonly int[,] BotPixels =
    {
        { 1, 0, 1, 0, 1 },
        { 0, 1, 1, 1, 0 },
        { 1, 1, 0, 1, 1 },
        { 1, 1, 1, 1, 1 },
        { 1, 0, 0, 0, 1 }
    };

    private static readonly int[,] LegitPixels =
    {
        { 0, 0, 1, 0, 0 },
        { 0, 1, 1, 1, 0 },
        { 0, 1, 1, 1, 0 },
        { 1, 1, 0, 1, 1 },
        { 1, 0, 0, 0, 1 }
    };

    private static readonly int[,] MaskedPixels =
    {
        { 1, 1, 1, 1, 1 },
        { 1, 0, 1, 0, 1 },
        { 1, 0, 0, 1, 1 },
        { 1, 0, 1, 0, 1 },
        { 1, 1, 1, 1, 1 }
    };

    public void AssignUIReferences(
        RectTransform area,
        RectTransform packetParent,
        RectTransform trailParent,
        RectTransform particleParent,
        RectTransform floatingParent,
        TextMeshProUGUI waveLabel,
        TextMeshProUGUI scoreLabel,
        TextMeshProUGUI loadLabel,
        Image loadBar,
        TextMeshProUGUI frustrationLabel,
        Image frustrationBar,
        GameObject tutorialPanel,
        CanvasGroup tutorialGroup,
        TextMeshProUGUI tutorialTitle,
        TextMeshProUGUI tutorialBody,
        Button tutorialButton,
        GameObject overlay,
        GameObject startView,
        GameObject lostView,
        GameObject wonView,
        TextMeshProUGUI lostReasonLabel,
        TextMeshProUGUI finalScoreLabel,
        TextMeshProUGUI finalWaveLabel,
        TextMeshProUGUI wonScoreLabel,
        Button tutorialStart,
        Button startGame,
        Button retry,
        Button playAgain)
    {
        gameArea = area;
        packetLayer = packetParent;
        trailLayer = trailParent;
        particleLayer = particleParent;
        floatingTextLayer = floatingParent;
        waveText = waveLabel;
        scoreText = scoreLabel;
        loadPercentText = loadLabel;
        loadFill = loadBar;
        frustrationPercentText = frustrationLabel;
        frustrationFill = frustrationBar;
        tutorialOverlay = tutorialPanel;
        tutorialCanvasGroup = tutorialGroup;
        tutorialTitleText = tutorialTitle;
        tutorialBodyText = tutorialBody;
        tutorialStartButton = tutorialButton;
        stateOverlay = overlay;
        startPanel = startView;
        lostPanel = lostView;
        wonPanel = wonView;
        lostReasonText = lostReasonLabel;
        finalScoreText = finalScoreLabel;
        finalWaveText = finalWaveLabel;
        wonScoreText = wonScoreLabel;
        startTutorialButton = tutorialStart;
        startGameButton = startGame;
        retryButton = retry;
        playAgainButton = playAgain;

        rootCanvas = gameArea != null ? gameArea.GetComponentInParent<Canvas>() : null;
    }

    private void Start()
    {
        BuildSprites();
        SetupButtons();
        SetState(GameState.Start, FailReason.None);
        UpdateMetricsUI();
        UpdateTutorialUI();
    }

    private void Update()
    {
        UpdateLayoutIfNeeded();

        if (state == GameState.Playing || state == GameState.Tutorial)
        {
            float deltaMultiplier = Time.deltaTime * 60f;
            UpdateGame(deltaMultiplier);
            UpdateVisuals(deltaMultiplier);
        }
    }

    private void SetupButtons()
    {
        if (startTutorialButton != null)
        {
            startTutorialButton.onClick.AddListener(StartTutorial);
        }
        if (startGameButton != null)
        {
            startGameButton.onClick.AddListener(StartGame);
        }
        if (retryButton != null)
        {
            retryButton.onClick.AddListener(StartGame);
        }
        if (playAgainButton != null)
        {
            playAgainButton.onClick.AddListener(StartGame);
        }
        if (tutorialStartButton != null)
        {
            tutorialStartButton.onClick.AddListener(StartGame);
        }
    }

    private void BuildSprites()
    {
        pixelSprites["bot"] = CreateSprite(BotPixels);
        pixelSprites["legit"] = CreateSprite(LegitPixels);
        pixelSprites["masked"] = CreateSprite(MaskedPixels);
    }

    private Sprite CreateSprite(int[,] pixels)
    {
        int size = pixels.GetLength(0);
        Texture2D texture = new Texture2D(size, size, TextureFormat.ARGB32, false);
        texture.filterMode = FilterMode.Point;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                bool on = pixels[y, x] == 1;
                texture.SetPixel(x, size - 1 - y, on ? Color.white : Color.clear);
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 1f);
    }

    private void StartTutorial()
    {
        SetState(GameState.Tutorial, FailReason.None);
        tutorialStep = 1;
        tutorialWait = 0;
        ResetGame();
        UpdateMetricsUI();
        UpdateTutorialUI();
    }

    private void StartGame()
    {
        state = GameState.Playing;
        failReason = FailReason.None;
        ResetGame();
        UpdateMetricsUI();
        UpdateTutorialUI();
        SetState(GameState.Playing, FailReason.None);
    }

    private void ResetGame()
    {
        ClearPackets();
        ClearParticles();
        ClearTrails();
        ClearFloatingTexts();

        score = 0;
        load = 0;
        frustration = 0;
        wave = 1;
        frame = 0;
        tutorialWait = 0;
        touchActive = false;
        packetIdCounter = 0;
    }

    private void UpdateLayoutIfNeeded()
    {
        if (gameArea == null)
        {
            return;
        }

        Vector2 size = gameArea.rect.size;
        if (size != lastAreaSize)
        {
            lastAreaSize = size;
            gameWidth = size.x;
            gameHeight = size.y;
            laneWidth = gameWidth / lanes;
            pixelScale = Mathf.Max(4, Mathf.FloorToInt(laneWidth / 20f));
            UpdatePacketSizes();
        }
    }

    private void UpdatePacketSizes()
    {
        float size = pixelScale * 5f;
        foreach (var packet in packets)
        {
            if (packet.Rect != null)
            {
                packet.Rect.sizeDelta = new Vector2(size, size);
            }
            if (packet.ScanLine != null)
            {
                packet.ScanLine.sizeDelta = new Vector2(size, 3);
            }
        }
    }

    private void UpdateGame(float deltaMultiplier)
    {
        frame++;

        if (state == GameState.Tutorial)
        {
            if (tutorialWait > 0)
            {
                tutorialWait -= deltaMultiplier;
                return;
            }
            if (packets.Count == 0)
            {
                if (tutorialStep == 1) SpawnPacket(TutorialLegitLaneIndex, "legit");
                if (tutorialStep == 2) SpawnPacket(TutorialBotLaneIndex, "bot");
                if (tutorialStep == 3) SpawnPacket(TutorialMaskedLaneIndex, "masked");
            }
        }

        if (state == GameState.Playing)
        {
            if (frame % 600 == 0)
            {
                wave++;
                UpdateMetricsUI();
            }

            int spawnRate = Mathf.Max(60, 180 - (wave * 25));
            if (frame % spawnRate == 0)
            {
                int lane = Random.Range(0, lanes);
                float randomValue = Random.value;
                string type = "legit";
                if (wave == 1)
                {
                    type = randomValue > 0.5f ? "bot" : "legit";
                }
                else if (wave == 2)
                {
                    type = randomValue < 0.25f ? "masked" : (randomValue > 0.5f ? "bot" : "legit");
                }
                else
                {
                    type = randomValue < 0.4f ? "masked" : (randomValue > 0.6f ? "bot" : "legit");
                }

                SpawnPacket(lane, type);
            }
        }

        for (int i = packets.Count - 1; i >= 0; i--)
        {
            Packet packet = packets[i];
            if (packet.State == PacketState.Dying)
            {
                packet.Timer -= Time.deltaTime;
                if (packet.Timer <= 0f)
                {
                    RemovePacket(packet);
                }
                continue;
            }

            if (packet.State == PacketState.Inspecting)
            {
                packet.Timer -= deltaMultiplier;
                if (packet.Timer <= 0f)
                {
                    packet.Type = packet.HiddenType;
                    packet.State = PacketState.Falling;
                    SpawnParticles(packet.X, packet.Y, new Color(0.23f, 0.51f, 0.96f), 8);
                    if (state == GameState.Tutorial && tutorialStep == 3)
                    {
                        tutorialWait = 60;
                        AdvanceTutorial();
                        packet.Type = "legit";
                    }
                }
                continue;
            }

            packet.Y += packet.Speed * deltaMultiplier;

            if (packet.Y > gameHeight - 60f)
            {
                string actualNature = packet.Type == "masked" ? packet.HiddenType : packet.Type;
                if (actualNature == "bot")
                {
                    load += 10;
                    SpawnParticles(packet.X, packet.Y, new Color(0.94f, 0.27f, 0.27f), 10);
                    AddFloatingText(packet.X, packet.Y - 20f, "MISSED BOT! +10 Load", new Color(0.94f, 0.27f, 0.27f));
                }
                else
                {
                    frustration += 10;
                    SpawnParticles(packet.X, packet.Y, new Color(0.96f, 0.65f, 0.15f), 10);
                    AddFloatingText(packet.X, packet.Y - 20f, "MISSED USER! +10 Frustration", new Color(0.96f, 0.65f, 0.15f));
                }

                if (state == GameState.Tutorial)
                {
                    packet.Y = -20f;
                    if (!string.IsNullOrEmpty(packet.HiddenType))
                    {
                        packet.Type = "masked";
                    }
                }
                else
                {
                    RemovePacket(packet);
                }

                UpdateMetricsUI();
            }
        }

        for (int i = floatingTexts.Count - 1; i >= 0; i--)
        {
            var text = floatingTexts[i];
            text.Rect.anchoredPosition += new Vector2(0, 0.5f * deltaMultiplier);
            text.Life -= deltaMultiplier;
            if (text.Life <= 0f)
            {
                Destroy(text.Rect.gameObject);
                floatingTexts.RemoveAt(i);
            }
            else
            {
                Color color = text.Text.color;
                color.a = Mathf.Clamp01(text.Life / 60f);
                text.Text.color = color;
            }
        }

        for (int i = swipeTrails.Count - 1; i >= 0; i--)
        {
            var trail = swipeTrails[i];
            trail.Life -= deltaMultiplier;
            if (trail.Life <= 0f)
            {
                Destroy(trail.Rect.gameObject);
                swipeTrails.RemoveAt(i);
            }
            else
            {
                Color color = trail.Image.color;
                color.a = Mathf.Clamp01(trail.Life / 15f);
                trail.Image.color = color;
            }
        }

        for (int i = particles.Count - 1; i >= 0; i--)
        {
            var particle = particles[i];
            particle.Rect.anchoredPosition += particle.Velocity * deltaMultiplier;
            particle.Life -= 0.05f * deltaMultiplier;
            if (particle.Life <= 0f)
            {
                Destroy(particle.Rect.gameObject);
                particles.RemoveAt(i);
            }
            else
            {
                Color color = particle.Image.color;
                color.a = Mathf.Clamp01(particle.Life);
                particle.Image.color = color;
            }
        }

        if (state == GameState.Playing)
        {
            if (load >= 100 || frustration >= 100)
            {
                state = GameState.Lost;
                failReason = load >= 100 ? FailReason.ServerCrash : FailReason.UserRevolt;
                SetState(GameState.Lost, failReason);
            }
            else if (wave > 3)
            {
                state = GameState.Won;
                SetState(GameState.Won, FailReason.None);
            }
        }
    }

    private void UpdateVisuals(float deltaMultiplier)
    {
        foreach (var packet in packets)
        {
            if (packet.Rect == null)
            {
                continue;
            }

            packet.Rect.anchoredPosition = new Vector2(packet.X, -packet.Y);

            Color baseColor = GetPacketColor(packet.Type);
            if (packet.State == PacketState.Inspecting)
            {
                bool blink = frame % 10 < 5;
                packet.Image.color = blink ? new Color(0.23f, 0.51f, 0.96f) : baseColor;
            }
            else
            {
                packet.Image.color = baseColor;
            }

            if (packet.ScanLine != null)
            {
                if (packet.State == PacketState.Inspecting)
                {
                    packet.ScanLine.gameObject.SetActive(true);
                    float scanY = packet.Y - 20f + ((45f - packet.Timer) / 45f) * 40f;
                    float localOffset = scanY - packet.Y;
                    packet.ScanLine.anchoredPosition = new Vector2(0, -localOffset);
                }
                else
                {
                    packet.ScanLine.gameObject.SetActive(false);
                }
            }
        }

        UpdateMetricsUI();
        UpdateTutorialUI();
    }

    private void SpawnPacket(int lane, string type)
    {
        string hiddenType = null;
        if (type == "masked")
        {
            hiddenType = Random.value > 0.5f ? "bot" : "legit";
        }

        Packet packet = new Packet
        {
            Id = ++packetIdCounter,
            Lane = lane,
            X = (lane * laneWidth) + (laneWidth / 2f),
            Y = -20f,
            Type = type,
            HiddenType = hiddenType,
            State = PacketState.Falling,
            Timer = 0f,
            Speed = 0.5f + (wave * 0.15f)
        };

        packet.Rect = CreatePacketVisual(packet, type);
        packets.Add(packet);
    }

    private RectTransform CreatePacketVisual(Packet packet, string type)
    {
        GameObject obj = new GameObject($"Packet_{packet.Id}");
        obj.transform.SetParent(packetLayer, false);
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(pixelScale * 5f, pixelScale * 5f);

        Image image = obj.AddComponent<Image>();
        image.sprite = pixelSprites[type];
        image.color = GetPacketColor(type);
        image.preserveAspect = true;

        GameObject scanObj = new GameObject("ScanLine");
        scanObj.transform.SetParent(obj.transform, false);
        RectTransform scanRect = scanObj.AddComponent<RectTransform>();
        scanRect.anchorMin = new Vector2(0.5f, 0.5f);
        scanRect.anchorMax = new Vector2(0.5f, 0.5f);
        scanRect.pivot = new Vector2(0.5f, 0.5f);
        scanRect.sizeDelta = new Vector2(pixelScale * 5f, 3f);
        Image scanImage = scanObj.AddComponent<Image>();
        scanImage.color = Color.white;
        scanObj.SetActive(false);

        packet.Image = image;
        packet.ScanLine = scanRect;
        return rect;
    }

    private Color GetPacketColor(string type)
    {
        if (type == "bot")
        {
            return new Color(0.94f, 0.27f, 0.27f);
        }
        if (type == "legit")
        {
            return new Color(0.06f, 0.73f, 0.5f);
        }
        return new Color(0.92f, 0.7f, 0.17f);
    }

    private void RemovePacket(Packet packet)
    {
        packets.Remove(packet);
        if (packet.Rect != null)
        {
            Destroy(packet.Rect.gameObject);
        }
    }

    public void HandlePointerDown(PointerEventData eventData)
    {
        if (state != GameState.Playing && state != GameState.Tutorial)
        {
            return;
        }
        if (laneWidth <= 0f)
        {
            return;
        }

        if (!ScreenToGameCoords(eventData.position, out var gamePos))
        {
            return;
        }

        touchStart = gamePos;
        touchActive = true;
    }

    public void HandlePointerUp(PointerEventData eventData)
    {
        if (!touchActive)
        {
            return;
        }
        if (laneWidth <= 0f)
        {
            return;
        }

        touchActive = false;

        if (!ScreenToGameCoords(eventData.position, out var gamePos))
        {
            return;
        }

        float deltaYDown = gamePos.y - touchStart.y;
        float deltaX = gamePos.x - touchStart.x;
        int lane = Mathf.FloorToInt(touchStart.x / laneWidth);
        if (lane < 0 || lane >= lanes)
        {
            return;
        }

        if (Mathf.Abs(deltaYDown) > 30f && Mathf.Abs(deltaYDown) > Mathf.Abs(deltaX))
        {
            if (deltaYDown < 0)
            {
                HandleGesture(lane, "DROP", touchStart.y, gamePos.y);
            }
            else
            {
                HandleGesture(lane, "ROUTE", touchStart.y, gamePos.y);
            }
        }
        else if (Mathf.Abs(deltaYDown) < 20f && Mathf.Abs(deltaX) < 20f)
        {
            HandleGesture(lane, "INSPECT", touchStart.y, touchStart.y);
        }
    }

    private bool ScreenToGameCoords(Vector2 screenPos, out Vector2 gamePos)
    {
        gamePos = Vector2.zero;
        if (gameArea == null)
        {
            return false;
        }

        Camera camera = GetUiCamera();
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(gameArea, screenPos, camera, out var local))
        {
            return false;
        }

        gamePos = new Vector2(local.x, -local.y);
        return true;
    }

    private void HandleGesture(int laneIndex, string gestureType, float startY, float endY)
    {
        if (state != GameState.Playing && state != GameState.Tutorial)
        {
            return;
        }

        Packet target = null;
        float bestY = float.MinValue;

        foreach (var packet in packets)
        {
            if (packet.Lane != laneIndex || packet.State == PacketState.Dying)
            {
                continue;
            }
            if (packet.Y > bestY)
            {
                bestY = packet.Y;
                target = packet;
            }
        }

        float x = (laneIndex * laneWidth) + (laneWidth / 2f);
        Color color = new Color(0.23f, 0.51f, 0.96f);
        if (gestureType == "DROP")
        {
            color = new Color(0.94f, 0.27f, 0.27f);
        }
        if (gestureType == "ROUTE")
        {
            color = new Color(0.06f, 0.73f, 0.5f);
        }

        AddSwipeTrail(x, startY, endY, color);

        if (target != null)
        {
            ResolveHit(target, gestureType);
        }
    }

    private void ResolveHit(Packet packet, string action)
    {
        if (action == "INSPECT")
        {
            if (packet.Type == "masked")
            {
                packet.State = PacketState.Inspecting;
                packet.Timer = 45f;
                SpawnParticles(packet.X, packet.Y, new Color(0.23f, 0.51f, 0.96f), 8);
            }
            return;
        }

        bool isCorrect = false;
        string actualNature = packet.Type == "masked" ? packet.HiddenType : packet.Type;

        if (action == "DROP" && actualNature == "bot")
        {
            isCorrect = true;
        }
        if (action == "ROUTE" && actualNature == "legit")
        {
            isCorrect = true;
        }

        if (isCorrect)
        {
            score += 150;
            load = Mathf.Max(0, load - 5);
            SpawnParticles(packet.X, packet.Y, action == "DROP" ? new Color(0.94f, 0.27f, 0.27f) : new Color(0.06f, 0.73f, 0.5f), 12);
            AddFloatingText(packet.X, packet.Y, "GOOD!", new Color(0.06f, 0.73f, 0.5f));

            if (state == GameState.Tutorial)
            {
                if (tutorialStep == 1 && actualNature == "legit") AdvanceTutorial();
                if (tutorialStep == 2 && actualNature == "bot") AdvanceTutorial();
                if (tutorialStep == 5 && actualNature == "legit") AdvanceTutorial();
            }
        }
        else
        {
            if (action == "DROP" && actualNature == "legit")
            {
                frustration += 15;
                SpawnParticles(packet.X, packet.Y, new Color(0.96f, 0.65f, 0.15f), 15);
                AddFloatingText(packet.X, packet.Y, "BLOCKED REAL USER! +15 Frustration", new Color(0.96f, 0.65f, 0.15f));
            }
            if (action == "ROUTE" && actualNature == "bot")
            {
                load += 15;
                SpawnParticles(packet.X, packet.Y, new Color(0.94f, 0.27f, 0.27f), 15);
                AddFloatingText(packet.X, packet.Y, "LET BOT IN! +15 Load", new Color(0.94f, 0.27f, 0.27f));
            }
            if (packet.Type == "masked" && action != "INSPECT")
            {
                SpawnParticles(packet.X, packet.Y, new Color(0.92f, 0.7f, 0.17f), 15);
                AddFloatingText(packet.X, packet.Y + 15f, "DIDN'T INSPECT FIRST!", new Color(0.92f, 0.7f, 0.17f));
            }
        }

        packet.State = PacketState.Dying;
        packet.Timer = 0.1f;
        UpdateMetricsUI();
    }

    private void AdvanceTutorial()
    {
        tutorialWait = 60f;
        tutorialStep++;
        UpdateTutorialUI();
    }

    private void AddSwipeTrail(float x, float startY, float endY, Color color)
    {
        GameObject obj = new GameObject("SwipeTrail");
        obj.transform.SetParent(trailLayer, false);
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0.5f, 0.5f);
        float height = Mathf.Abs(endY - startY);
        rect.sizeDelta = new Vector2(6f, Mathf.Max(height, 6f));
        rect.anchoredPosition = new Vector2(x, -(startY + endY) / 2f);
        Image img = obj.AddComponent<Image>();
        img.color = color;

        swipeTrails.Add(new SwipeTrail { Rect = rect, Image = img, Life = 15f });
    }

    private void SpawnParticles(float x, float y, Color color, int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject obj = new GameObject("Particle");
            obj.transform.SetParent(particleLayer, false);
            RectTransform rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(6f, 6f);
            rect.anchoredPosition = new Vector2(x, -y);
            Image img = obj.AddComponent<Image>();
            img.color = color;

            particles.Add(new Particle
            {
                Rect = rect,
                Image = img,
                Velocity = new Vector2((Random.value - 0.5f) * 8f, (Random.value - 0.5f) * 8f),
                Life = 1f
            });
        }
    }

    private void AddFloatingText(float x, float y, string text, Color color)
    {
        GameObject obj = new GameObject("FloatingText");
        obj.transform.SetParent(floatingTextLayer, false);
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(300, 30);
        rect.anchoredPosition = new Vector2(x, -y);
        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 12;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = color;
        tmp.raycastTarget = false;

        floatingTexts.Add(new FloatingText { Rect = rect, Text = tmp, Life = 60f });
    }

    private void UpdateMetricsUI()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString("D5");
        }

        if (waveText != null)
        {
            waveText.text = $"WAVE {Mathf.Min(wave, 3)}/3";
        }

        if (loadPercentText != null)
        {
            loadPercentText.text = $"{load}%";
            loadPercentText.color = load > 80 ? new Color(0.94f, 0.27f, 0.27f) : new Color(0.7f, 0.7f, 0.7f);
        }

        if (loadFill != null)
        {
            loadFill.fillAmount = Mathf.Clamp01(load / 100f);
        }

        if (frustrationPercentText != null)
        {
            frustrationPercentText.text = $"{frustration}%";
            frustrationPercentText.color = frustration > 80 ? new Color(0.96f, 0.65f, 0.15f) : new Color(0.7f, 0.7f, 0.7f);
        }

        if (frustrationFill != null)
        {
            frustrationFill.fillAmount = Mathf.Clamp01(frustration / 100f);
        }
    }

    private void UpdateTutorialUI()
    {
        if (tutorialOverlay == null)
        {
            return;
        }

        tutorialOverlay.SetActive(state == GameState.Tutorial);

        if (state != GameState.Tutorial)
        {
            return;
        }

        if (tutorialTitleText != null)
        {
            tutorialTitleText.text = $"TRAINING STEP {tutorialStep}";
        }

        if (tutorialBodyText != null)
        {
            tutorialBodyText.text = GetTutorialBody(tutorialStep);
        }

        bool showButton = tutorialStep >= 6;
        if (tutorialStartButton != null)
        {
            tutorialStartButton.gameObject.SetActive(showButton);
        }

        if (tutorialCanvasGroup != null)
        {
            tutorialCanvasGroup.blocksRaycasts = showButton;
            tutorialCanvasGroup.interactable = showButton;
        }
    }

    private string GetTutorialBody(int step)
    {
        switch (step)
        {
            case 1:
                return $"A Legit User (Green) is in Lane {TutorialLegitLaneIndex + 1}.\nSWIPE DOWN to ROUTE them.";
            case 2:
                return $"A Bot (Red) is in Lane {TutorialBotLaneIndex + 1}.\nSWIPE UP to DROP them.";
            case 3:
                return "A Masked Packet (Yellow) approaches!\nTAP IT FIRST to reveal its color.";
            case 4:
                return "Wait for the blue scan to finish...";
            case 5:
                return "It was a Legit User (Green)!\nSWIPE DOWN now to ROUTE it.";
            default:
                return "Training Complete. Defend the network for 3 waves!";
        }
    }

    private void SetState(GameState newState, FailReason reason)
    {
        state = newState;
        failReason = reason;

        if (stateOverlay != null)
        {
            stateOverlay.SetActive(state != GameState.Playing && state != GameState.Tutorial);
        }

        if (startPanel != null)
        {
            startPanel.SetActive(state == GameState.Start);
        }
        if (lostPanel != null)
        {
            lostPanel.SetActive(state == GameState.Lost);
        }
        if (wonPanel != null)
        {
            wonPanel.SetActive(state == GameState.Won);
        }

        if (state == GameState.Lost && lostReasonText != null)
        {
            lostReasonText.text = reason == FailReason.ServerCrash
                ? "Too many bots bypassed triage. Server crashed."
                : "You dropped too many legitimate users. Massive UX revolt.";
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = score.ToString();
        }
        if (finalWaveText != null)
        {
            finalWaveText.text = $"SURVIVED TO WAVE {Mathf.Min(wave, 3)}/3";
        }
        if (wonScoreText != null)
        {
            wonScoreText.text = score.ToString();
        }
    }

    private void ClearPackets()
    {
        foreach (var packet in packets)
        {
            if (packet.Rect != null)
            {
                Destroy(packet.Rect.gameObject);
            }
        }
        packets.Clear();
    }

    private void ClearParticles()
    {
        foreach (var particle in particles)
        {
            if (particle.Rect != null)
            {
                Destroy(particle.Rect.gameObject);
            }
        }
        particles.Clear();
    }

    private void ClearTrails()
    {
        foreach (var trail in swipeTrails)
        {
            if (trail.Rect != null)
            {
                Destroy(trail.Rect.gameObject);
            }
        }
        swipeTrails.Clear();
    }

    private void ClearFloatingTexts()
    {
        foreach (var text in floatingTexts)
        {
            if (text.Rect != null)
            {
                Destroy(text.Rect.gameObject);
            }
        }
        floatingTexts.Clear();
    }

    private Camera GetUiCamera()
    {
        if (rootCanvas == null)
        {
            return null;
        }
        return rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera;
    }
}

internal class DDOSInputCatcher : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private DDOSLogic logic;

    public void Initialize(DDOSLogic owner)
    {
        logic = owner;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (logic != null)
        {
            logic.HandlePointerDown(eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (logic != null)
        {
            logic.HandlePointerUp(eventData);
        }
    }
}

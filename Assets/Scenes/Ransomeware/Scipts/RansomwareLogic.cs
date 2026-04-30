using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

public class RansomwareLogic : MonoBehaviour
{
    private enum GameState
    {
        Idle,
        Playing,
        GameOverWave,
        GameOverTrap
    }

    [Serializable]
    private class FileState
    {
        public int Id;
        public string Name;
        public string Type;
        public bool Important;
        public float X;
        public float Y;
        public bool Saved;
        public bool Encrypted;
    }

    private class FileView
    {
        public FileState Data;
        public RectTransform Root;
        public CanvasGroup CanvasGroup;
        public TextMeshProUGUI IconText;
        public TextMeshProUGUI LabelText;
        public Image LabelBackground;
    }

    private class TrapView
    {
        public GameObject Root;
        public Coroutine LifetimeRoutine;
    }

    private static readonly FileState[] InitialFiles =
    {
        new FileState { Id = 1, Name = "Thesis_Final.docx", Type = "doc", Important = true, X = 15, Y = 20 },
        new FileState { Id = 2, Name = "Family_Photos.zip", Type = "zip", Important = true, X = 28, Y = 35 },
        new FileState { Id = 3, Name = "Lab_Data_2026.csv", Type = "xls", Important = true, X = 18, Y = 55 },
        new FileState { Id = 4, Name = "Passports.pdf", Type = "doc", Important = true, X = 42, Y = 25 },
        new FileState { Id = 5, Name = "Meme_Collection", Type = "folder", Important = false, X = 25, Y = 75 },
        new FileState { Id = 6, Name = "Game_Backup.7z", Type = "zip", Important = false, X = 50, Y = 65 },
        new FileState { Id = 7, Name = "Random_Notes.txt", Type = "doc", Important = false, X = 10, Y = 85 },
        new FileState { Id = 8, Name = "Project_Pitch.pptx", Type = "doc", Important = true, X = 55, Y = 15 },
        new FileState { Id = 9, Name = "Funny_Cats.mp4", Type = "video", Important = false, X = 35, Y = 50 },
        new FileState { Id = 10, Name = "Tax_Returns.pdf", Type = "doc", Important = true, X = 60, Y = 80 },
        new FileState { Id = 11, Name = "Source_Code.zip", Type = "zip", Important = true, X = 12, Y = 38 },
        new FileState { Id = 12, Name = "Wallpapers", Type = "folder", Important = false, X = 40, Y = 85 },
        new FileState { Id = 13, Name = "Budget.xlsx", Type = "xls", Important = true, X = 65, Y = 45 },
        new FileState { Id = 14, Name = "Movie_Draft.mp4", Type = "video", Important = false, X = 8, Y = 65 },
        new FileState { Id = 15, Name = "Secret_Keys.txt", Type = "doc", Important = true, X = 32, Y = 15 }
    };

    [Header("UI References - AUTO-ASSIGNED")]
    [SerializeField] private RectTransform containerRect;
    [SerializeField] private RectTransform fileLayer;
    [SerializeField] private RectTransform trapLayer;
    [SerializeField] private Image waveFill;
    [SerializeField] private RectTransform driveRect;
    [SerializeField] private TextMeshProUGUI securedCountText;
    [SerializeField] private RectTransform driveListContainer;
    [SerializeField] private GameObject startOverlay;
    [SerializeField] private GameObject endOverlay;
    [SerializeField] private TextMeshProUGUI endTitleText;
    [SerializeField] private TextMeshProUGUI endSubtitleText;
    [SerializeField] private TextMeshProUGUI endRankText;
    [SerializeField] private TextMeshProUGUI endStatsText;
    [SerializeField] private Button startButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private RectTransform dragGhostRect;
    [SerializeField] private TextMeshProUGUI dragGhostIconText;
    [SerializeField] private TextMeshProUGUI dragGhostLabelText;

    private readonly List<FileState> files = new List<FileState>();
    private readonly Dictionary<int, FileView> fileViews = new Dictionary<int, FileView>();
    private readonly List<TrapView> traps = new List<TrapView>();
    private readonly List<GameObject> savedFileLabels = new List<GameObject>();
    private GameState gameState = GameState.Idle;
    private int? draggingId;
    private Vector2 dragScreenPosition;
    private float wavePercent;
    private Coroutine trapCoroutine;
    private Vector2 lastContainerSize;
    private Canvas rootCanvas;

    private const float WaveSpeedPercentPerSecond = 4f;
    private const float TrapSpawnInterval = 2f;
    private const float DriveHitMargin = 20f;

    public void AssignUIReferences(
        RectTransform container,
        RectTransform fileParent,
        RectTransform trapParent,
        Image waveImage,
        RectTransform drivePanel,
        TextMeshProUGUI securedCount,
        RectTransform driveList,
        GameObject startPanel,
        GameObject endPanel,
        TextMeshProUGUI endTitle,
        TextMeshProUGUI endSubtitle,
        TextMeshProUGUI endRank,
        TextMeshProUGUI endStats,
        Button startGameButton,
        Button retryGameButton,
        RectTransform dragGhost,
        TextMeshProUGUI dragGhostIcon,
        TextMeshProUGUI dragGhostLabel)
    {
        containerRect = container;
        fileLayer = fileParent;
        trapLayer = trapParent;
        waveFill = waveImage;
        driveRect = drivePanel;
        securedCountText = securedCount;
        driveListContainer = driveList;
        startOverlay = startPanel;
        endOverlay = endPanel;
        endTitleText = endTitle;
        endSubtitleText = endSubtitle;
        endRankText = endRank;
        endStatsText = endStats;
        startButton = startGameButton;
        retryButton = retryGameButton;
        dragGhostRect = dragGhost;
        dragGhostIconText = dragGhostIcon;
        dragGhostLabelText = dragGhostLabel;

        rootCanvas = containerRect != null ? containerRect.GetComponentInParent<Canvas>() : null;
    }

    private void Start()
    {
        SetupButtons();
        ShowStartScreen();
        if (dragGhostRect != null)
        {
            dragGhostRect.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (containerRect != null && containerRect.rect.size != lastContainerSize)
        {
            lastContainerSize = containerRect.rect.size;
            UpdateAllFilePositions();
            UpdateWaveVisual();
        }

        if (gameState != GameState.Playing)
        {
            return;
        }

        wavePercent += WaveSpeedPercentPerSecond * Time.deltaTime;
        if (wavePercent >= 100f)
        {
            wavePercent = 100f;
            EndGame(GameState.GameOverWave);
        }

        UpdateWaveVisual();
        EncryptFilesByWave();
        UpdateDragGhost();
    }

    private void SetupButtons()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGame);
        }
        if (retryButton != null)
        {
            retryButton.onClick.AddListener(StartGame);
        }
    }

    private void ShowStartScreen()
    {
        gameState = GameState.Idle;
        if (startOverlay != null)
        {
            startOverlay.SetActive(true);
        }
        if (endOverlay != null)
        {
            endOverlay.SetActive(false);
        }
    }

    private void StartGame()
    {
        gameState = GameState.Playing;
        wavePercent = 0f;
        draggingId = null;
        ClearTraps();
        BuildFiles();
        RefreshSavedList();
        UpdateDriveCount();
        UpdateWaveVisual();

        if (startOverlay != null)
        {
            startOverlay.SetActive(false);
        }
        if (endOverlay != null)
        {
            endOverlay.SetActive(false);
        }

        if (trapCoroutine != null)
        {
            StopCoroutine(trapCoroutine);
        }
        trapCoroutine = StartCoroutine(SpawnTraps());
    }

    private void EndGame(GameState endState)
    {
        if (gameState != GameState.Playing)
        {
            return;
        }

        gameState = endState;
        if (trapCoroutine != null)
        {
            StopCoroutine(trapCoroutine);
            trapCoroutine = null;
        }
        draggingId = null;
        if (dragGhostRect != null)
        {
            dragGhostRect.gameObject.SetActive(false);
        }
        UpdateEndScreen(endState);
        ClearTraps();
    }

    private void BuildFiles()
    {
        foreach (var view in fileViews.Values)
        {
            if (view.Root != null)
            {
                Destroy(view.Root.gameObject);
            }
        }

        fileViews.Clear();
        files.Clear();

        foreach (var template in InitialFiles)
        {
            files.Add(CloneFile(template));
        }

        foreach (var file in files)
        {
            CreateFileView(file);
        }

        UpdateAllFilePositions();
    }

    private static FileState CloneFile(FileState template)
    {
        return new FileState
        {
            Id = template.Id,
            Name = template.Name,
            Type = template.Type,
            Important = template.Important,
            X = template.X,
            Y = template.Y,
            Saved = false,
            Encrypted = false
        };
    }

    private void CreateFileView(FileState file)
    {
        GameObject root = new GameObject($"File_{file.Id}");
        root.transform.SetParent(fileLayer, false);
        RectTransform rect = root.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(120, 120);

        CanvasGroup canvasGroup = root.AddComponent<CanvasGroup>();
        Image background = root.AddComponent<Image>();
        background.color = new Color(1f, 1f, 1f, 0.05f);

        TextMeshProUGUI iconText = CreateText("Icon", root.transform, "", 36, TextAlignmentOptions.Center);
        iconText.rectTransform.sizeDelta = new Vector2(120, 60);
        iconText.rectTransform.anchoredPosition = new Vector2(0, 20);

        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(root.transform, false);
        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.sizeDelta = new Vector2(130, 36);
        labelRect.anchoredPosition = new Vector2(0, -40);
        Image labelBackground = labelObj.AddComponent<Image>();
        labelBackground.color = new Color(0.1f, 0.1f, 0.15f, 0.9f);

        TextMeshProUGUI labelText = CreateText("LabelText", labelObj.transform, file.Name, 14, TextAlignmentOptions.Center);
        labelText.rectTransform.sizeDelta = labelRect.sizeDelta;
        labelText.rectTransform.anchoredPosition = Vector2.zero;
        labelText.overflowMode = TextOverflowModes.Ellipsis;

        var item = root.AddComponent<RansomwareFileItem>();
        item.Initialize(this, file.Id);

        FileView view = new FileView
        {
            Data = file,
            Root = rect,
            CanvasGroup = canvasGroup,
            IconText = iconText,
            LabelText = labelText,
            LabelBackground = labelBackground
        };

        fileViews[file.Id] = view;
        UpdateFileView(file);
    }

    private void UpdateAllFilePositions()
    {
        if (containerRect == null)
        {
            return;
        }

        foreach (var file in files)
        {
            if (fileViews.TryGetValue(file.Id, out var view))
            {
                SetFilePosition(file, view);
            }
        }
    }

    private void SetFilePosition(FileState file, FileView view)
    {
        if (containerRect == null || view == null)
        {
            return;
        }

        Rect rect = containerRect.rect;
        float x = (file.X / 100f) * rect.width - rect.width / 2f;
        float y = rect.height / 2f - (file.Y / 100f) * rect.height;
        view.Root.anchoredPosition = new Vector2(x, y);
    }

    private void UpdateFileView(FileState file)
    {
        if (!fileViews.TryGetValue(file.Id, out var view))
        {
            return;
        }

        view.Root.gameObject.SetActive(!file.Saved);
        view.IconText.text = GetIcon(file.Type, file.Encrypted);
        view.LabelText.text = file.Name;

        if (file.Encrypted)
        {
            view.LabelText.fontStyle = FontStyles.Strikethrough;
            view.LabelText.color = new Color(1f, 0.6f, 0.6f);
            view.LabelBackground.color = new Color(0.4f, 0.1f, 0.1f, 0.9f);
        }
        else
        {
            view.LabelText.fontStyle = FontStyles.Normal;
            view.LabelText.color = file.Important ? new Color(1f, 0.95f, 0.8f) : new Color(0.85f, 0.85f, 0.85f);
            view.LabelBackground.color = file.Important
                ? new Color(0.7f, 0.55f, 0.1f, 0.85f)
                : new Color(0.1f, 0.1f, 0.15f, 0.9f);
        }
    }

    private string GetIcon(string type, bool encrypted)
    {
        if (encrypted)
        {
            return "🔒";
        }
        switch (type)
        {
            case "doc":
                return "📄";
            case "zip":
                return "🗜️";
            case "xls":
                return "📊";
            case "img":
                return "🖼️";
            case "video":
                return "🎞️";
            case "folder":
                return "📁";
            default:
                return "📄";
        }
    }

    private void EncryptFilesByWave()
    {
        bool changed = false;
        foreach (var file in files)
        {
            if (file.Saved || file.Encrypted)
            {
                continue;
            }
            if (file.X < wavePercent && draggingId != file.Id)
            {
                file.Encrypted = true;
                UpdateFileView(file);
                changed = true;
            }
        }

        if (changed)
        {
            UpdateDriveCount();
        }
    }

    private void UpdateWaveVisual()
    {
        if (waveFill == null || containerRect == null)
        {
            return;
        }

        float width = containerRect.rect.width * (wavePercent / 100f);
        waveFill.rectTransform.sizeDelta = new Vector2(width, 0);
    }

    private void UpdateDriveCount()
    {
        if (securedCountText == null)
        {
            return;
        }

        int savedCount = 0;
        foreach (var file in files)
        {
            if (file.Saved)
            {
                savedCount++;
            }
        }

        securedCountText.text = $"{savedCount} SECURED";
    }

    private void RefreshSavedList()
    {
        foreach (var item in savedFileLabels)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }
        savedFileLabels.Clear();

        if (driveListContainer == null)
        {
            return;
        }

        foreach (var file in files)
        {
            if (!file.Saved)
            {
                continue;
            }

            GameObject item = new GameObject($"Saved_{file.Id}");
            item.transform.SetParent(driveListContainer, false);
            RectTransform rect = item.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, 28);
            Image bg = item.AddComponent<Image>();
            bg.color = new Color(0.1f, 0.3f, 0.2f, 0.8f);

            TextMeshProUGUI text = CreateText("Text", item.transform, $"✓ {file.Name}", 12, TextAlignmentOptions.Left);
            text.rectTransform.anchorMin = new Vector2(0, 0);
            text.rectTransform.anchorMax = new Vector2(1, 1);
            text.rectTransform.sizeDelta = new Vector2(-10, 0);
            text.rectTransform.anchoredPosition = new Vector2(5, 0);
            text.color = new Color(0.6f, 1f, 0.7f);

            savedFileLabels.Add(item);
        }
    }

    private IEnumerator SpawnTraps()
    {
        while (gameState == GameState.Playing)
        {
            yield return new WaitForSeconds(TrapSpawnInterval);
            if (UnityEngine.Random.value < 0.35f)
            {
                CreateTrap();
            }
        }
    }

    private void CreateTrap()
    {
        if (trapLayer == null || containerRect == null)
        {
            return;
        }

        float xPercent = 20f + UnityEngine.Random.value * 50f;
        float yPercent = 20f + UnityEngine.Random.value * 60f;

        GameObject trapRoot = new GameObject("Trap");
        trapRoot.transform.SetParent(trapLayer, false);
        RectTransform rect = trapRoot.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(320, 200);

        Image bg = trapRoot.AddComponent<Image>();
        bg.color = new Color(0.93f, 0.91f, 0.85f, 1f);

        SetAnchoredPercentPosition(rect, xPercent, yPercent);

        GameObject header = new GameObject("Header");
        header.transform.SetParent(trapRoot.transform, false);
        RectTransform headerRect = header.AddComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0, 1);
        headerRect.anchorMax = new Vector2(1, 1);
        headerRect.pivot = new Vector2(0.5f, 1);
        headerRect.sizeDelta = new Vector2(0, 32);
        headerRect.anchoredPosition = Vector2.zero;
        Image headerImg = header.AddComponent<Image>();
        headerImg.color = new Color(0.7f, 0.1f, 0.1f, 1f);

        TextMeshProUGUI headerText = CreateText("HeaderText", header.transform, "System Warning", 14, TextAlignmentOptions.Left);
        headerText.rectTransform.anchorMin = new Vector2(0, 0);
        headerText.rectTransform.anchorMax = new Vector2(1, 1);
        headerText.rectTransform.sizeDelta = new Vector2(-40, 0);
        headerText.rectTransform.anchoredPosition = new Vector2(10, 0);
        headerText.color = Color.white;

        Button closeButton = CreateButton("Close", header.transform, "✕", 14);
        RectTransform closeRect = closeButton.GetComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(1, 0.5f);
        closeRect.anchorMax = new Vector2(1, 0.5f);
        closeRect.pivot = new Vector2(1, 0.5f);
        closeRect.sizeDelta = new Vector2(30, 24);
        closeRect.anchoredPosition = new Vector2(-6, 0);
        closeButton.onClick.AddListener(() => RemoveTrap(trapRoot));

        TextMeshProUGUI bodyText = CreateText("BodyText", trapRoot.transform,
            "Critical files detected. Encryption inevitable without immediate payment.", 12, TextAlignmentOptions.Center);
        bodyText.rectTransform.sizeDelta = new Vector2(280, 60);
        bodyText.rectTransform.anchoredPosition = new Vector2(0, -30);
        bodyText.color = Color.black;

        Button payButton = CreateButton("PayButton", trapRoot.transform, "Decrypt Now — Pay 50,000 PKR", 12);
        RectTransform payRect = payButton.GetComponent<RectTransform>();
        payRect.sizeDelta = new Vector2(280, 40);
        payRect.anchoredPosition = new Vector2(0, -80);
        payButton.onClick.AddListener(() => EndGame(GameState.GameOverTrap));

        TrapView trapView = new TrapView { Root = trapRoot };
        trapView.LifetimeRoutine = StartCoroutine(RemoveTrapAfter(trapView, 3f + UnityEngine.Random.value * 2.5f));
        traps.Add(trapView);
    }

    private void RemoveTrap(GameObject trapRoot)
    {
        TrapView target = traps.Find(t => t.Root == trapRoot);
        if (target != null)
        {
            if (target.LifetimeRoutine != null)
            {
                StopCoroutine(target.LifetimeRoutine);
            }
            traps.Remove(target);
        }

        if (trapRoot != null)
        {
            Destroy(trapRoot);
        }
    }

    private IEnumerator RemoveTrapAfter(TrapView trap, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (trap != null && trap.Root != null)
        {
            RemoveTrap(trap.Root);
        }
    }

    private void ClearTraps()
    {
        foreach (var trap in traps)
        {
            if (trap.LifetimeRoutine != null)
            {
                StopCoroutine(trap.LifetimeRoutine);
            }
            if (trap.Root != null)
            {
                Destroy(trap.Root);
            }
        }
        traps.Clear();
    }

    public void BeginDrag(int fileId, PointerEventData eventData)
    {
        if (gameState != GameState.Playing)
        {
            return;
        }

        FileState file = files.Find(f => f.Id == fileId);
        if (file == null || file.Saved || file.Encrypted)
        {
            return;
        }

        if (wavePercent >= file.X)
        {
            return;
        }

        draggingId = fileId;
        dragScreenPosition = eventData.position;
        if (fileViews.TryGetValue(fileId, out var view))
        {
            view.CanvasGroup.alpha = 0.2f;
        }
        UpdateDragGhostContents(file);
        UpdateDragGhost();
    }

    public void UpdateDrag(PointerEventData eventData)
    {
        if (draggingId == null || gameState != GameState.Playing)
        {
            return;
        }

        dragScreenPosition = eventData.position;
        UpdateDragGhost();
    }

    public void EndDrag(PointerEventData eventData)
    {
        if (draggingId == null || gameState != GameState.Playing)
        {
            return;
        }

        int fileId = draggingId.Value;
        draggingId = null;
        dragScreenPosition = eventData.position;

        if (fileViews.TryGetValue(fileId, out var view))
        {
            view.CanvasGroup.alpha = 1f;
        }

        FileState file = files.Find(f => f.Id == fileId);
        if (file == null)
        {
            return;
        }

        bool saved = TrySaveFile(file, dragScreenPosition);
        if (!saved)
        {
            UpdateFilePositionFromScreen(file, dragScreenPosition);
        }

        if (dragGhostRect != null)
        {
            dragGhostRect.gameObject.SetActive(false);
        }
    }

    private void UpdateDragGhostContents(FileState file)
    {
        if (dragGhostRect == null)
        {
            return;
        }

        dragGhostRect.gameObject.SetActive(true);
        if (dragGhostIconText != null)
        {
            dragGhostIconText.text = GetIcon(file.Type, false);
        }
        if (dragGhostLabelText != null)
        {
            dragGhostLabelText.text = file.Name;
        }
    }

    private void UpdateDragGhost()
    {
        if (dragGhostRect == null || draggingId == null)
        {
            return;
        }

        Camera camera = GetUiCamera();
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(containerRect, dragScreenPosition, camera, out var local))
        {
            dragGhostRect.anchoredPosition = local;
        }
    }

    private bool TrySaveFile(FileState file, Vector2 screenPosition)
    {
        if (driveRect == null)
        {
            return false;
        }

        Camera camera = GetUiCamera();
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(driveRect, screenPosition, camera, out var local))
        {
            return false;
        }

        Vector2 size = driveRect.rect.size;
        if (local.x >= -size.x / 2f - DriveHitMargin &&
            local.x <= size.x / 2f + DriveHitMargin &&
            local.y >= -size.y / 2f - DriveHitMargin &&
            local.y <= size.y / 2f + DriveHitMargin)
        {
            file.Saved = true;
            UpdateFileView(file);
            RefreshSavedList();
            UpdateDriveCount();
            return true;
        }

        return false;
    }

    private void UpdateFilePositionFromScreen(FileState file, Vector2 screenPosition)
    {
        if (containerRect == null)
        {
            return;
        }

        Camera camera = GetUiCamera();
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(containerRect, screenPosition, camera, out var local))
        {
            return;
        }

        Rect rect = containerRect.rect;
        float xPercent = Mathf.Clamp((local.x + rect.width / 2f) / rect.width * 100f, 2f, 98f);
        float yPercent = Mathf.Clamp((rect.height / 2f - local.y) / rect.height * 100f, 2f, 98f);
        file.X = xPercent;
        file.Y = yPercent;

        if (fileViews.TryGetValue(file.Id, out var view))
        {
            SetFilePosition(file, view);
        }
    }

    private void UpdateEndScreen(GameState endState)
    {
        if (endOverlay == null)
        {
            return;
        }

        int importantSaved = 0;
        int totalImportant = 0;
        int junkSaved = 0;
        int totalLost = 0;

        foreach (var file in files)
        {
            if (file.Important)
            {
                totalImportant++;
            }
            if (file.Saved && file.Important)
            {
                importantSaved++;
            }
            if (file.Saved && !file.Important)
            {
                junkSaved++;
            }
            if (!file.Saved)
            {
                totalLost++;
            }
        }

        string rank = "C Rank";
        Color rankColor = new Color(1f, 0.82f, 0.3f);

        if (endState == GameState.GameOverTrap)
        {
            rank = "F Rank: Scammed";
            rankColor = new Color(1f, 0.3f, 0.3f);
        }
        else if (importantSaved == totalImportant && totalImportant > 0)
        {
            rank = "S Rank: IT Security Master";
            rankColor = new Color(0.3f, 1f, 0.5f);
        }
        else if (importantSaved == 0)
        {
            rank = "F Rank: Total Data Loss";
            rankColor = new Color(1f, 0.2f, 0.2f);
        }
        else if (importantSaved > totalImportant / 2f)
        {
            rank = "B Rank: Acceptable Losses";
            rankColor = new Color(0.4f, 0.7f, 1f);
        }

        if (endTitleText != null)
        {
            endTitleText.text = endState == GameState.GameOverTrap ? "SYSTEM COMPROMISED" : "ENCRYPTION COMPLETE";
            endTitleText.color = endState == GameState.GameOverTrap ? new Color(1f, 0.3f, 0.3f) : Color.white;
        }

        if (endSubtitleText != null)
        {
            endSubtitleText.text = endState == GameState.GameOverTrap
                ? "You paid the ransom. The scammers took your money and deleted your files anyway."
                : "The ransomware wave finished. Review your recovery report below.";
        }

        if (endRankText != null)
        {
            endRankText.text = rank;
            endRankText.color = rankColor;
        }

        if (endStatsText != null)
        {
            endStatsText.text =
                $"Crucial Data Saved: {importantSaved} / {totalImportant}\n" +
                $"Junk Files Saved: {junkSaved}\n" +
                $"Lost to Ransomware: {totalLost}";
        }

        endOverlay.SetActive(true);
    }

    private TextMeshProUGUI CreateText(string name, Transform parent, string text, int size, TextAlignmentOptions alignment)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.AddComponent<RectTransform>();
        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.alignment = alignment;
        tmp.enableAutoSizing = false;
        tmp.raycastTarget = false;
        rect.sizeDelta = new Vector2(0, 0);
        return tmp;
    }

    private Button CreateButton(string name, Transform parent, string label, int fontSize)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(120, 30);
        Image img = obj.AddComponent<Image>();
        img.color = new Color(0.85f, 0.82f, 0.78f);
        Button button = obj.AddComponent<Button>();

        TextMeshProUGUI text = CreateText("Text", obj.transform, label, fontSize, TextAlignmentOptions.Center);
        text.rectTransform.anchorMin = Vector2.zero;
        text.rectTransform.anchorMax = Vector2.one;
        text.rectTransform.sizeDelta = Vector2.zero;
        text.color = Color.black;
        return button;
    }

    private void SetAnchoredPercentPosition(RectTransform rect, float xPercent, float yPercent)
    {
        if (containerRect == null)
        {
            return;
        }

        Rect container = containerRect.rect;
        float x = (xPercent / 100f) * container.width - container.width / 2f;
        float y = container.height / 2f - (yPercent / 100f) * container.height;
        rect.anchoredPosition = new Vector2(x, y);
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

internal class RansomwareFileItem : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private RansomwareLogic logic;
    private int fileId;

    public void Initialize(RansomwareLogic owner, int id)
    {
        logic = owner;
        fileId = id;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (logic != null)
        {
            logic.BeginDrag(fileId, eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (logic != null)
        {
            logic.UpdateDrag(eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (logic != null)
        {
            logic.EndDrag(eventData);
        }
    }
}

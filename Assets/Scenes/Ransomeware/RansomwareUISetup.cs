using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[DefaultExecutionOrder(-200)]
[RequireComponent(typeof(RansomwareGameManager))]
public class RansomwareUISetup : MonoBehaviour
{
    private Canvas targetCanvas;
    private RansomwareGameManager gameManager;
    private RectTransform waveRect;
    private RectTransform driveRect;
    private GameObject endScreenPanel;

    private void Awake()
    {
        gameManager = GetComponent<RansomwareGameManager>();
        SetupCanvasAndEventSystem();
        BuildGameUI();
    }

    private void SetupCanvasAndEventSystem()
    {
        // 1. Canvas
        GameObject canvasObj = new GameObject("RansomwareCanvas");
        targetCanvas = canvasObj.AddComponent<Canvas>();
        targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        
        canvasObj.AddComponent<GraphicRaycaster>();

        // 2. Background
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(canvasObj.transform, false);
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0.1f, 0.1f, 0.15f); // Dark tech background
        bgImg.rectTransform.anchorMin = Vector2.zero;
        bgImg.rectTransform.anchorMax = Vector2.one;
        bgImg.rectTransform.sizeDelta = Vector2.zero;

        // 3. Event System
        if (FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>(); // Automatically upgrades to InputSystemUIInputModule if new input system is active
        }
    }

    private void BuildGameUI()
    {
        // 1. The Wave (Anchored Left)
        GameObject waveObj = new GameObject("EncryptionWave");
        waveObj.transform.SetParent(targetCanvas.transform, false);
        Image waveImg = waveObj.AddComponent<Image>();
        waveImg.color = new Color(0.9f, 0f, 0f, 0.6f);
        waveRect = waveObj.GetComponent<RectTransform>();
        waveRect.anchorMin = new Vector2(0, 0);
        waveRect.anchorMax = new Vector2(0, 1);
        waveRect.pivot = new Vector2(0, 0.5f);
        waveRect.anchoredPosition = Vector2.zero;
        waveRect.sizeDelta = new Vector2(0, 0); // Starts at 0 width

        // 2. The Safe Drive (Anchored Right)
        GameObject driveObj = new GameObject("SafeDrive");
        driveObj.transform.SetParent(targetCanvas.transform, false);
        Image driveImg = driveObj.AddComponent<Image>();
        driveImg.color = new Color(0.1f, 0.4f, 0.2f, 0.8f);
        driveRect = driveObj.GetComponent<RectTransform>();
        driveRect.anchorMin = new Vector2(0.85f, 0);
        driveRect.anchorMax = new Vector2(1, 1);
        driveRect.pivot = new Vector2(1, 0.5f);
        driveRect.anchoredPosition = Vector2.zero;
        driveRect.sizeDelta = new Vector2(0, 0);

        TextMeshProUGUI driveText = CreateText("DriveText", driveObj.transform, "SAFE\nBACKUP\nDRIVE", 36);
        driveText.rectTransform.anchoredPosition = Vector2.zero;

        // 3. Generate Files
        GenerateFiles();

        // 4. Initialize Manager
        gameManager.Initialize(this, waveRect, driveRect);
    }

    private void GenerateFiles()
    {
        string[] names = { "Thesis_Final.docx", "Family_Photos.zip", "Lab_Data.csv", "Meme.png", "Game.exe" };
        bool[] importance = { true, true, true, false, false };

        for (int i = 0; i < 12; i++)
        {
            int rIndex = Random.Range(0, names.Length);
            GameObject fileObj = new GameObject($"File_{i}");
            fileObj.transform.SetParent(targetCanvas.transform, false);
            
            Image fileImg = fileObj.AddComponent<Image>();
            fileImg.color = importance[rIndex] ? new Color(0.8f, 0.6f, 0.1f) : new Color(0.3f, 0.4f, 0.5f);
            
            RectTransform rect = fileObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(100, 100);
            
            // Random placement in the middle/left area
            float randX = Random.Range(-800, 200);
            float randY = Random.Range(-400, 400);
            rect.anchoredPosition = new Vector2(randX, randY);

            TextMeshProUGUI text = CreateText("Label", fileObj.transform, names[rIndex], 14);
            text.rectTransform.anchoredPosition = new Vector2(0, -65);

            CanvasGroup group = fileObj.AddComponent<CanvasGroup>();
            DraggableFile draggable = fileObj.AddComponent<DraggableFile>();
            draggable.Setup(importance[rIndex], gameManager);
            
            gameManager.RegisterFile(draggable);
        }
    }

    public void SpawnPopupTrap()
    {
        if (!gameManager.isPlaying) return;

        GameObject trapObj = new GameObject("FakePopup");
        trapObj.transform.SetParent(targetCanvas.transform, false);
        Image trapImg = trapObj.AddComponent<Image>();
        trapImg.color = new Color(0.9f, 0.9f, 0.9f);
        
        RectTransform rect = trapObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(400, 200);
        rect.anchoredPosition = new Vector2(Random.Range(-400, 400), Random.Range(-300, 300));
        
        // Ensure popups appear above files
        trapObj.transform.SetAsLastSibling();

        CreateText("Title", trapObj.transform, "<color=red>SYSTEM COMPROMISED</color>", 24).rectTransform.anchoredPosition = new Vector2(0, 60);

        // Fake Button
        GameObject btnObj = new GameObject("DecryptBtn");
        btnObj.transform.SetParent(trapObj.transform, false);
        Image btnImg = btnObj.AddComponent<Image>();
        btnImg.color = new Color(0.8f, 0.2f, 0.2f);
        Button btn = btnObj.AddComponent<Button>();
        
        RectTransform btnRect = btnObj.GetComponent<RectTransform>();
        btnRect.sizeDelta = new Vector2(300, 60);
        btnRect.anchoredPosition = new Vector2(0, -20);

        CreateText("BtnText", btnObj.transform, "Decrypt Now - Pay 50k PKR", 20).rectTransform.anchoredPosition = Vector2.zero;

        btn.onClick.AddListener(() => gameManager.EndGame(true));
        
        // Auto-destroy popup after 3 seconds so screen doesn't clutter forever
        Destroy(trapObj, 3f);
    }

    public void ShowEndScreen(bool scammed, int saved, int total)
    {
        endScreenPanel = new GameObject("EndScreen");
        endScreenPanel.transform.SetParent(targetCanvas.transform, false);
        Image img = endScreenPanel.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0.9f);
        
        RectTransform rect = endScreenPanel.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;

        string resultText = scammed ? 
            "<color=red>FATAL ERROR: You paid the scammers!</color>\nThey took your money and deleted your files anyway." : 
            $"<color=green>WAVE COMPLETE</color>\nImportant Files Saved: {saved} / {total}";

        CreateText("Result", endScreenPanel.transform, resultText, 40).rectTransform.anchoredPosition = Vector2.zero;
    }

    private TextMeshProUGUI CreateText(string name, Transform parent, string content, int size)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = content;
        tmp.fontSize = size;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(400, 100);
        
        return tmp;
    }
}
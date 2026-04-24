using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

[DefaultExecutionOrder(-200)]
[RequireComponent(typeof(CyberRansomwareManager))]
public class CyberRansomwareSetup : MonoBehaviour
{
    private Canvas targetCanvas;
    private CyberRansomwareManager manager;
    
    // UI Colors based on the concept image
    private Color bgDark = new Color(0.03f, 0.05f, 0.08f);
    private Color panelDark = new Color(0.06f, 0.09f, 0.13f);
    private Color cyanAccent = new Color(0f, 0.8f, 0.7f);
    private Color redAccent = new Color(0.9f, 0.2f, 0.2f);
    private Color greenAccent = new Color(0.2f, 0.9f, 0.4f);
    private Color textGray = new Color(0.6f, 0.7f, 0.8f);

    private void Awake()
    {
        manager = GetComponent<CyberRansomwareManager>();
        CreateCanvasAndEventSystem();
        BuildInterface();
    }

    private void CreateCanvasAndEventSystem()
    {
        GameObject canvasObj = new GameObject("CyberRansomwareCanvas");
        targetCanvas = canvasObj.AddComponent<Canvas>();
        targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        
        canvasObj.AddComponent<GraphicRaycaster>();

        if (FindFirstObjectByType<EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }
    }

    private void BuildInterface()
    {
        // 1. Main Background
        GameObject bg = CreatePanel("Background", targetCanvas.transform, bgDark);
        SetRectFull(bg.GetComponent<RectTransform>());

        // 2. Top Header
        GameObject topHeader = CreatePanel("TopHeader", targetCanvas.transform, Color.clear);
        RectTransform topRect = topHeader.GetComponent<RectTransform>();
        SetRect(topRect, 0, 1, 1, 1, new Vector2(0, -100), new Vector2(0, 0));
        
        CreateText("ChapterTitle", topHeader.transform, "<color=#00ffcc>CHAPTER 5</color> - RANSOMWARE TIDE", 20, TextAlignmentOptions.Left)
            .rectTransform.anchoredPosition = new Vector2(300, 20);
        CreateText("Instructions", topHeader.transform, "<b>SAVE YOUR IMPORTANT FILES</b>\n<size=18><color=#aaaaaa>Drag files to the </color><color=#00ffcc>Safe Backup Drive</color></size>", 24, TextAlignmentOptions.Left)
            .rectTransform.anchoredPosition = new Vector2(300, -25);

        // 3. Left Sidebar
        GameObject sidebar = CreatePanel("Sidebar", targetCanvas.transform, panelDark);
        SetRect(sidebar.GetComponent<RectTransform>(), 0, 0, 0, 1, new Vector2(20, 120), new Vector2(250, -100));
        CreateText("SidebarNav", sidebar.transform, "<b>> This PC</b>\n\n★ Quick Access\n\n■ Desktop\n<color=#00ffcc>■ Documents</color>\n■ Downloads\n■ Pictures\n■ Music\n■ Videos\n\n<b>> Drives</b>\n■ Local Disk (C:)\n■ Data (D:)", 18, TextAlignmentOptions.Left)
            .rectTransform.anchoredPosition = new Vector2(120, -50);

        // 4. Right Fake Ransomware Panel
        GameObject rightPanel = CreatePanel("FakeRansomwarePanel", targetCanvas.transform, new Color(0.1f, 0.02f, 0.02f));
        SetRect(rightPanel.GetComponent<RectTransform>(), 1, 0, 1, 1, new Vector2(-350, 120), new Vector2(-20, -100));
        rightPanel.AddComponent<Outline>().effectColor = redAccent;
        
        CreateText("RansomTitle", rightPanel.transform, "<color=red>YOUR FILES ARE\nBEING ENCRYPTED!</color>", 26, TextAlignmentOptions.Center)
            .rectTransform.anchoredPosition = new Vector2(0, 300);
        CreateText("RansomDesc", rightPanel.transform, "All your files will be lost.\nPay now to recover them.", 18, TextAlignmentOptions.Center)
            .rectTransform.anchoredPosition = new Vector2(0, 200);

        Button decryptBtn = CreateButton("DecryptBtn", rightPanel.transform, "DECRYPT NOW\n<size=18>PAY 50,000 PKR</size>", new Color(0.8f, 0.1f, 0.1f));
        decryptBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 50);
        decryptBtn.GetComponent<RectTransform>().sizeDelta = new Vector2(280, 80);
        decryptBtn.onClick.AddListener(() => manager.TriggerTrap());

        CreateText("Warning", rightPanel.transform, "⚠️ WARNING\nThese are fake buttons.\nDo not click!", 16, TextAlignmentOptions.Center)
            .rectTransform.anchoredPosition = new Vector2(0, -250);

        // 5. Main File Window
        GameObject fileWindow = CreatePanel("FileWindow", targetCanvas.transform, panelDark);
        SetRect(fileWindow.GetComponent<RectTransform>(), 0, 0, 1, 1, new Vector2(270, 120), new Vector2(-370, -100));

        CreateText("Headers", fileWindow.transform, "Name                                         Type               Size               Status", 18, TextAlignmentOptions.Left)
            .rectTransform.anchoredPosition = new Vector2(600, -20);
        
        GameObject separator = CreatePanel("Separator", fileWindow.transform, new Color(0.2f, 0.3f, 0.4f));
        SetRect(separator.GetComponent<RectTransform>(), 0, 1, 1, 1, new Vector2(20, -40), new Vector2(-20, -39));

        // 6. The Wave (Inside File Window)
        GameObject waveObj = CreatePanel("RedWave", fileWindow.transform, new Color(0.9f, 0.1f, 0.1f, 0.25f));
        RectTransform waveRect = waveObj.GetComponent<RectTransform>();
        SetRect(waveRect, 0, 0, 1, 0, new Vector2(0, 0), new Vector2(0, 0)); // Starts at height 0
        
        GameObject waveLine = CreatePanel("WaveLine", waveObj.transform, redAccent);
        SetRect(waveLine.GetComponent<RectTransform>(), 0, 1, 1, 1, new Vector2(0, 0), new Vector2(0, 5));
        waveLine.AddComponent<Shadow>().effectColor = redAccent;

        // 7. Generate Files
        GenerateFiles(fileWindow.transform);

        // 8. Bottom Bar Area
        GameObject bottomBar = CreatePanel("BottomBar", targetCanvas.transform, Color.clear);
        SetRect(bottomBar.GetComponent<RectTransform>(), 0, 0, 1, 0, new Vector2(20, 20), new Vector2(-20, 100));

        // 8a. Zara Dialogue
        GameObject dialogueBox = CreatePanel("DialogueBox", bottomBar.transform, panelDark);
        SetRect(dialogueBox.GetComponent<RectTransform>(), 0, 0, 0, 1, new Vector2(0, 0), new Vector2(300, 0));
        dialogueBox.AddComponent<Outline>().effectColor = cyanAccent;
        CreateText("ZaraName", dialogueBox.transform, "<color=#00ffcc>ZARA</color>", 18, TextAlignmentOptions.Left).rectTransform.anchoredPosition = new Vector2(100, 30);
        CreateText("ZaraText", dialogueBox.transform, "That's ransomware.\nThe wave is encrypting your files.\nDrag important ones to the Safe Drive.\nIgnore any payment buttons.", 14, TextAlignmentOptions.Left)
            .rectTransform.anchoredPosition = new Vector2(150, -15);

        // 8b. Safe Drive
        GameObject safeDrive = CreatePanel("SafeDrive", bottomBar.transform, new Color(0.05f, 0.2f, 0.1f));
        SetRect(safeDrive.GetComponent<RectTransform>(), 0.5f, 0, 0.5f, 1, new Vector2(-150, 0), new Vector2(150, 0));
        safeDrive.AddComponent<Outline>().effectColor = greenAccent;
        CreateText("DriveText", safeDrive.transform, "🛡️\nSAFE BACKUP DRIVE\n<size=12>Drag files here to secure them</size>", 20, TextAlignmentOptions.Center)
            .rectTransform.anchoredPosition = Vector2.zero;

        // 8c. Progress Bar
        GameObject progressBox = CreatePanel("ProgressBox", bottomBar.transform, panelDark);
        SetRect(progressBox.GetComponent<RectTransform>(), 1, 0, 1, 1, new Vector2(-300, 0), new Vector2(0, 0));
        progressBox.AddComponent<Outline>().effectColor = cyanAccent;
        
        TextMeshProUGUI progText = CreateText("ProgressText", progressBox.transform, "FILES SECURED          <color=#00ffcc>0 / 8</color>", 18, TextAlignmentOptions.Center);
        progText.rectTransform.anchoredPosition = new Vector2(0, 10);

        // Initialize Manager
        manager.Initialize(targetCanvas, waveRect, safeDrive.GetComponent<RectTransform>(), progText);
    }

    private void GenerateFiles(Transform parent)
    {
        string[] names = { "Thesis", "Family Photos", "Lab Report", "Personal Data", "Project Files", "Financial_Records.xlsx", "ID_Card_Scan.jpg", "Semester_Results.pdf", "Passwords.txt", "Notes.txt" };
        string[] types = { "Folder", "Folder", "Folder", "Folder", "Folder", "XLSX File", "JPG File", "PDF File", "TXT File", "TXT File" };
        string[] sizes = { "—", "—", "—", "—", "—", "245 KB", "1.2 MB", "532 KB", "2 KB", "3 KB" };
        bool[] important = { true, true, true, true, true, true, true, true, false, false };

        float startY = -70;
        float rowHeight = 65;

        for (int i = 0; i < names.Length; i++)
        {
            GameObject row = CreatePanel($"FileRow_{i}", parent, Color.clear);
            RectTransform rect = row.GetComponent<RectTransform>();
            SetRect(rect, 0, 1, 1, 1, new Vector2(20, startY - rowHeight), new Vector2(-20, startY));
            
            // Background hover effect placeholder
            Image bg = row.GetComponent<Image>();
            bg.color = new Color(1, 1, 1, 0);

            CreateText("Name", row.transform, $"📁 {names[i]}", 18, TextAlignmentOptions.Left).rectTransform.anchoredPosition = new Vector2(200, 0);
            CreateText("Type", row.transform, types[i], 16, TextAlignmentOptions.Left).rectTransform.anchoredPosition = new Vector2(480, 0);
            CreateText("Size", row.transform, sizes[i], 16, TextAlignmentOptions.Left).rectTransform.anchoredPosition = new Vector2(650, 0);
            TextMeshProUGUI status = CreateText("Status", row.transform, "<color=#aaaaaa>AT RISK</color>", 16, TextAlignmentOptions.Left);
            status.rectTransform.anchoredPosition = new Vector2(850, 0);

            CyberFile fileComponent = row.AddComponent<CyberFile>();
            fileComponent.Setup(names[i], important[i], bg, status, manager);
            manager.RegisterFile(fileComponent);

            startY -= rowHeight;
        }
    }

    // --- Utility Builders ---

    private GameObject CreatePanel(string name, Transform parent, Color color)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        Image img = obj.AddComponent<Image>();
        img.color = color;
        return obj;
    }

    private TextMeshProUGUI CreateText(string name, Transform parent, string text, int fontSize, TextAlignmentOptions alignment)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = alignment;
        tmp.color = Color.white;
        tmp.enableWordWrapping = false;
        
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(800, 100);
        return tmp;
    }

    private Button CreateButton(string name, Transform parent, string text, Color color)
    {
        GameObject obj = CreatePanel(name, parent, color);
        Button btn = obj.AddComponent<Button>();
        obj.AddComponent<Outline>().effectColor = redAccent;
        
        CreateText("Text", obj.transform, text, 22, TextAlignmentOptions.Center).rectTransform.anchoredPosition = Vector2.zero;
        return btn;
    }

    private void SetRect(RectTransform rect, float minX, float minY, float maxX, float maxY, Vector2 offsetMin, Vector2 offsetMax)
    {
        rect.anchorMin = new Vector2(minX, minY);
        rect.anchorMax = new Vector2(maxX, maxY);
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
    }

    private void SetRectFull(RectTransform rect)
    {
        SetRect(rect, 0, 0, 1, 1, Vector2.zero, Vector2.zero);
    }
}
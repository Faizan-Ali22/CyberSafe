using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static UnityEngine.UI.CanvasScaler;
using static UnityEngine.UIElements.VisualElement;
using static UnityEngine.UIElements.TextElement;    
using static UnityEngine.UIElements.TextField;
using static UnityEngine.UIElements.VisualTreeAsset;    
using static UnityEngine.UIElements.Label;  
using static UnityEngine.UIElements.Image;
using static UnityEngine.UIElements.ScrollView;
using static UnityEngine.UIElements.ListView;
using static UnityEngine.UIElements.Toggle;
using static UnityEngine.UIElements.Slider;
using static UnityEngine.UIElements.ProgressBar;
using static UnityEngine.UIElements.Foldout;
using static UnityEngine.UIElements.RadioButton;
using static UnityEngine.UIElements.RadioButtonGroup;
using Image = UnityEngine.UI.Image;
using Button = UnityEngine.UI.Button;
   

[RequireComponent(typeof(PasswordStrengthChecker))]
public class PasswordUISetup : MonoBehaviour
{
 [Header("Auto-Setup")]
    [SerializeField] private Canvas targetCanvas;
    
    private PasswordStrengthChecker checker;
    private GameObject avatarPanel;
    private GameObject passwordPanel;
    private List<GameObject> avatarButtons = new List<GameObject>();
    
    private void Awake()
    {
        checker = GetComponent<PasswordStrengthChecker>();
        
        if (targetCanvas == null)
        {
            targetCanvas = FindFirstObjectByType<Canvas>();
            if (targetCanvas == null)
            {
                CreateCanvas();
            }
        }
        
        SetupCompleteUI();
    }
    
    private void CreateCanvas()
    {
        GameObject canvasObj = new GameObject("PasswordCheckerCanvas");
        targetCanvas = canvasObj.AddComponent<Canvas>();
        targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Add EventSystem if not present
        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        
        Debug.Log("✅ Canvas created with Unity 6 optimizations");
    }
    
    private void SetupCompleteUI()
    {
        CreateAvatarSelectionPanel();
        CreatePasswordCheckerPanel();
    }
    
    private void CreateAvatarSelectionPanel()
    {
        // Main avatar panel
        avatarPanel = CreateUIPanel("AvatarSelectionPanel", targetCanvas.transform);
        RectTransform panelRect = avatarPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        avatarPanel.GetComponent<Image>().color = new Color(0.05f, 0.05f, 0.1f, 1f);
        
        // Title
        TextMeshProUGUI title = CreateText("Title", avatarPanel.transform,
            "<b>🔐 Select Account to Set Password</b>", 36);
        PositionElement(title.rectTransform, 0, 300, 1000, 80);
        title.alignment = TextAlignmentOptions.Center;
        title.color = new Color(0.3f, 0.8f, 1f);
        
        // Get custom names from PasswordStrengthChecker
        string[] names = checker.GetAvatarNames();
        string[] emojis = { "👩", "👨", "🧑", "👩‍💼", "🧑‍💻" };
        Color[] colors = {
            new Color(1f, 0.4f, 0.4f),
            new Color(0.4f, 0.6f, 1f),
            new Color(0.8627452f, 0.1921569f, 0.1960784f),
            new Color(1f, 0.8f, 0.4f),
            new Color(0.8f, 0.4f, 1f)
        };
        
        float startX = -400f;
        float spacing = 200f;
        
        for (int i = 0; i < 5; i++)
        {
            GameObject avatarBtn = CreateAvatarButton(
                $"Avatar_{i}", 
                avatarPanel.transform, 
                names[i], 
                emojis[i], 
                colors[i],
                i
            );
            
            float xPos = startX + (i * spacing);
            PositionElement(avatarBtn.GetComponent<RectTransform>(), xPos, 50, 180, 250);
            avatarButtons.Add(avatarBtn);
        }
    }
    
    private GameObject CreateAvatarButton(string name, Transform parent, string userName, string emoji, Color color, int index)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);
        RectTransform rect = btnObj.AddComponent<RectTransform>();
        Image img = btnObj.AddComponent<Image>();
        Button btn = btnObj.AddComponent<Button>();
        
        img.color = color;
        
        // Avatar circle
        GameObject circle = new GameObject("Circle");
        circle.transform.SetParent(btnObj.transform, false);
        RectTransform circleRect = circle.AddComponent<RectTransform>();
        circleRect.sizeDelta = new Vector2(120, 120);
        circleRect.anchoredPosition = new Vector2(0, 30);
        Image circleImg = circle.AddComponent<Image>();
        circleImg.color = Color.white;
        circleImg.raycastTarget = false;
        
        // Emoji
        TextMeshProUGUI emojiText = CreateText("Emoji", circle.transform, emoji, 60);
        emojiText.rectTransform.sizeDelta = new Vector2(120, 120);
        emojiText.rectTransform.anchoredPosition = Vector2.zero;
        emojiText.alignment = TextAlignmentOptions.Center;
        emojiText.raycastTarget = false;
        
        // Name
        TextMeshProUGUI nameText = CreateText("Name", btnObj.transform, userName, 22);
        nameText.rectTransform.sizeDelta = new Vector2(180, 40);
        nameText.rectTransform.anchoredPosition = new Vector2(0, -50);
        nameText.alignment = TextAlignmentOptions.Center;
        nameText.fontStyle = FontStyles.Bold;
        nameText.color = Color.white;
        nameText.raycastTarget = false;
        
        // Status indicator (password set or not)
        TextMeshProUGUI statusText = CreateText("Status", btnObj.transform, "No Password", 16);
        statusText.rectTransform.sizeDelta = new Vector2(180, 30);
        statusText.rectTransform.anchoredPosition = new Vector2(0, -90);
        statusText.alignment = TextAlignmentOptions.Center;
        statusText.color = new Color(0.7f, 0.7f, 0.7f);
        statusText.raycastTarget = false;
        
        // Button click handler
        int accountIndex = index;
        btn.onClick.AddListener(() => OnAvatarClicked(accountIndex));
        
        // Button colors
        ColorBlock colors = btn.colors;
        colors.normalColor = color;
        colors.highlightedColor = color * 1.2f;
        colors.pressedColor = color * 0.8f;
        colors.selectedColor = color * 1.1f;
        btn.colors = colors;
        
        return btnObj;
    }
    
    private void OnAvatarClicked(int index)
    {
        Debug.Log($"Avatar {index} clicked!");
        checker.SelectAccount(index);
    }
    
    private void Update()
    {
        // Update avatar button status indicators
        if (avatarPanel != null && avatarPanel.activeSelf)
        {
            for (int i = 0; i < avatarButtons.Count; i++)
            {
                Transform statusTransform = avatarButtons[i].transform.Find("Status");
                if (statusTransform != null)
                {
                    TextMeshProUGUI statusText = statusTransform.GetComponent<TextMeshProUGUI>();
                    if (statusText != null)
                    {
                        if (checker.IsPasswordSet(i))
                        {
                            statusText.text = "<color=green>●</color> Password Set";
                            statusText.color = Color.white;
                        }
                        else
                        {
                            statusText.text = "No Password";
                            statusText.color = new Color(0.7f, 0.7f, 0.7f);
                        }
                    }
                }
            }
        }
    }
    
    private void CreatePasswordCheckerPanel()
    {
        // Main password panel
        passwordPanel = CreateUIPanel("PasswordCheckerPanel", targetCanvas.transform);
        RectTransform panelRect = passwordPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(850, 850);
        
        Image panelImg = passwordPanel.GetComponent<Image>();
        panelImg.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);
        panelImg.raycastTarget = false;
        
        passwordPanel.SetActive(false);
        
        // Back button
        GameObject backBtn = CreateButton("BackButton", passwordPanel.transform, "← Back");
        PositionElement(backBtn.GetComponent<RectTransform>(), -330, 390, 120, 50);
        backBtn.GetComponent<Button>().onClick.AddListener(() => checker.ShowAvatarSelection());
        backBtn.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.4f);
        
        // Current account name
        TextMeshProUGUI accountName = CreateText("AccountName", passwordPanel.transform,
            "Setting password for: <b>User</b>", 22);
        PositionElement(accountName.rectTransform, 0, 390, 600, 50);
        accountName.alignment = TextAlignmentOptions.Center;
        accountName.color = new Color(0.3f, 0.8f, 1f);
        accountName.raycastTarget = false;
        
        // Title
        TextMeshProUGUI title = CreateText("Title", passwordPanel.transform,
            "<b>🔐 Password Strength Analyzer</b>", 28);
        PositionElement(title.rectTransform, 0, 330, 800, 60);
        title.alignment = TextAlignmentOptions.Center;
        title.color = new Color(0.3f, 0.8f, 1f);
        title.raycastTarget = false;
        
        // Password Display Box
        GameObject displayBox = CreateUIPanel("DisplayBox", passwordPanel.transform);
        PositionElement(displayBox.GetComponent<RectTransform>(), 0, 240, 800, 80);
        displayBox.GetComponent<Image>().color = new Color(0.15f, 0.15f, 0.2f);
        displayBox.GetComponent<Image>().raycastTarget = false;
        
        TextMeshProUGUI passwordDisplay = CreateText("PasswordDisplay", displayBox.transform,
            "Start typing your password...", 24);
        PositionElement(passwordDisplay.rectTransform, -30, 0, 720, 60);
        passwordDisplay.alignment = TextAlignmentOptions.Center;
        passwordDisplay.color = Color.white;
        passwordDisplay.raycastTarget = false;
        passwordDisplay.textWrappingMode = TextWrappingModes.Normal;
        passwordDisplay.overflowMode = TextOverflowModes.Ellipsis;
        
        // Toggle Visibility Button
        GameObject toggleButton = CreateButton("ToggleButton", displayBox.transform, "👁️ Show");
        PositionElement(toggleButton.GetComponent<RectTransform>(), 330, 0, 100, 60);
        Button toggleBtnComponent = toggleButton.GetComponent<Button>();
        toggleBtnComponent.onClick.AddListener(() => checker.TogglePasswordVisibility());
        toggleBtnComponent.GetComponent<Image>().color = new Color(0.2f, 0.4f, 0.6f);
        
        TextMeshProUGUI toggleBtnText = toggleButton.GetComponentInChildren<TextMeshProUGUI>();
        toggleBtnText.fontSize = 16;
        toggleBtnText.alignment = TextAlignmentOptions.Center;
        toggleBtnText.color = Color.white;
        toggleBtnText.fontStyle = FontStyles.Bold;
        
        // Character Count
        TextMeshProUGUI charCount = CreateText("CharacterCount", passwordPanel.transform,
            "Characters: 0", 18);
        PositionElement(charCount.rectTransform, 0, 180, 800, 30);
        charCount.alignment = TextAlignmentOptions.Center;
        charCount.color = new Color(0.7f, 0.7f, 0.7f);
        charCount.raycastTarget = false;
        
        // Strength Bar Background
        GameObject barBg = CreateUIPanel("StrengthBarBG", passwordPanel.transform);
        PositionElement(barBg.GetComponent<RectTransform>(), 0, 130, 800, 30);
        barBg.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.25f);
        barBg.GetComponent<Image>().raycastTarget = false;
        
        // Strength Bar Fill
        GameObject barFill = CreateUIPanel("StrengthBarFill", barBg.transform);
        RectTransform barFillRect = barFill.GetComponent<RectTransform>();
        barFillRect.anchorMin = new Vector2(0, 0);
        barFillRect.anchorMax = new Vector2(0, 1);
        barFillRect.pivot = new Vector2(0, 0.5f);
        barFillRect.anchoredPosition = Vector2.zero;
        barFillRect.sizeDelta = new Vector2(800, 0);
        
        Image barFillImg = barFill.GetComponent<Image>();
        barFillImg.color = new Color(1f, 0.2f, 0.2f);
        barFillImg.type = Image.Type.Filled;
        barFillImg.fillMethod = Image.FillMethod.Horizontal;
        barFillImg.fillAmount = 0;
        barFillImg.raycastTarget = false;
        
        // Strength Text
        TextMeshProUGUI strengthText = CreateText("StrengthText", passwordPanel.transform,
            "Strength: None\nScore: 0/100", 24);
        PositionElement(strengthText.rectTransform, 0, 70, 800, 60);
        strengthText.alignment = TextAlignmentOptions.Center;
        strengthText.fontStyle = FontStyles.Bold;
        strengthText.raycastTarget = false;
        
        // Crack Time Display
        GameObject crackTimeBox = CreateUIPanel("CrackTimeBox", passwordPanel.transform);
        PositionElement(crackTimeBox.GetComponent<RectTransform>(), 0, -10, 800, 90);
        crackTimeBox.GetComponent<Image>().color = new Color(0.12f, 0.12f, 0.18f);
        crackTimeBox.GetComponent<Image>().raycastTarget = false;
        
        TextMeshProUGUI crackTimeText = CreateText("CrackTimeText", crackTimeBox.transform, "", 20);
        PositionElement(crackTimeText.rectTransform, 0, 0, 780, 80);
        crackTimeText.alignment = TextAlignmentOptions.Center;
        crackTimeText.color = Color.white;
        crackTimeText.raycastTarget = false;
        crackTimeText.enableAutoSizing = false;
        
        // Hints Panel
        GameObject hintsPanel = CreateUIPanel("HintsPanel", passwordPanel.transform);
        PositionElement(hintsPanel.GetComponent<RectTransform>(), 0, -132, 800, 180);
        hintsPanel.GetComponent<Image>().color = new Color(0.15f, 0.15f, 0.2f);
        hintsPanel.GetComponent<Image>().raycastTarget = false;
        
        TextMeshProUGUI hintsText = CreateText("HintsText", hintsPanel.transform,
            "💡 Tips for a Strong Password:\n" +
            "• Use at least 12 characters\n" +
            "• Mix uppercase and lowercase letters\n" +
            "• Include numbers and special characters\n" +
            "• Avoid common words and patterns", 16);
        PositionElement(hintsText.rectTransform, 0, 0, 780, 160);
        hintsText.alignment = TextAlignmentOptions.TopLeft;
        hintsText.color = new Color(0.9f, 0.9f, 0.9f);
        hintsText.raycastTarget = false;
        
        // Set Password Button (BIG GREEN BUTTON!)
        GameObject setPassBtn = CreateButton("SetPasswordButton", passwordPanel.transform, "✓ Set Password");
        PositionElement(setPassBtn.GetComponent<RectTransform>(), 0, -193, 400, 70);
        Button setPassBtnComponent = setPassBtn.GetComponent<Button>();
        setPassBtnComponent.GetComponent<Image>().color = new Color(0.2f, 0.8f, 0.3f);
        
        TextMeshProUGUI setPassBtnText = setPassBtn.GetComponentInChildren<TextMeshProUGUI>();
        setPassBtnText.fontSize = 26;
        setPassBtnText.fontStyle = FontStyles.Bold;
        setPassBtnText.color = Color.white;
        
        // Add highlight effect to button
        ColorBlock setBtnColors = setPassBtnComponent.colors;
        setBtnColors.normalColor = new Color(0.2f, 0.8f, 0.3f);
        setBtnColors.highlightedColor = new Color(0.3f, 1f, 0.4f);
        setBtnColors.pressedColor = new Color(0.15f, 0.6f, 0.25f);
        setBtnColors.selectedColor = new Color(0.25f, 0.9f, 0.35f);
        setPassBtnComponent.colors = setBtnColors;
        
        // Connect to PasswordStrengthChecker
        if (checker != null)
        {
            var type = typeof(PasswordStrengthChecker);
            var bindingFlags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            
            type.GetField("passwordDisplayText", bindingFlags)?.SetValue(checker, passwordDisplay);
            type.GetField("strengthText", bindingFlags)?.SetValue(checker, strengthText);
            type.GetField("hintsText", bindingFlags)?.SetValue(checker, hintsText);
            type.GetField("characterCountText", bindingFlags)?.SetValue(checker, charCount);
            type.GetField("strengthBar", bindingFlags)?.SetValue(checker, barFillImg);
            type.GetField("crackTimeText", bindingFlags)?.SetValue(checker, crackTimeText);
            type.GetField("toggleVisibilityButton", bindingFlags)?.SetValue(checker, toggleBtnComponent);
            type.GetField("setPasswordButton", bindingFlags)?.SetValue(checker, setPassBtnComponent);
            type.GetField("currentAccountNameText", bindingFlags)?.SetValue(checker, accountName);
            type.GetField("avatarSelectionPanel", bindingFlags)?.SetValue(checker, avatarPanel);
            type.GetField("passwordCheckerPanel", bindingFlags)?.SetValue(checker, passwordPanel);
        }
        
        Canvas.ForceUpdateCanvases();
        
        Debug.Log("✅ Password Strength Checker UI Setup Complete!");
        Debug.Log("✅ 5 Avatar accounts created!");
        Debug.Log("✅ Password visibility toggle added!");
        Debug.Log("✅ Crack time calculator enabled!");
        Debug.Log("✅ Set Password button added!");
    }
    
    private GameObject CreateUIPanel(string name, Transform parent)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.AddComponent<RectTransform>();
        Image img = obj.AddComponent<Image>();
        
        if (img.material == null)
        {
            img.material = Canvas.GetDefaultCanvasMaterial();
        }
        
        return obj;
    }
    
    private GameObject CreateButton(string name, Transform parent, string buttonText)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.AddComponent<RectTransform>();
        Image img = obj.AddComponent<Image>();
        Button btn = obj.AddComponent<Button>();
        
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(obj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = buttonText;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.fontSize = 18;
        
        ColorBlock colors = btn.colors;
        colors.normalColor = new Color(0.2f, 0.4f, 0.6f);
        colors.highlightedColor = new Color(0.3f, 0.5f, 0.7f);
        colors.pressedColor = new Color(0.15f, 0.35f, 0.55f);
        colors.selectedColor = new Color(0.25f, 0.45f, 0.65f);
        btn.colors = colors;
        
        return obj;
    }
    
    private TextMeshProUGUI CreateText(string name, Transform parent, string text, int fontSize)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.AddComponent<RectTransform>();
        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.enableAutoSizing = false;
        tmp.overflowMode = TextOverflowModes.Overflow;
        tmp.parseCtrlCharacters = true;
        tmp.isOrthographic = true;
        tmp.enableWordWrapping = true;
        tmp.raycastTarget = false;
        
        return tmp;
    }
    
    private void PositionElement(RectTransform rect, float x, float y, float width, float height)
    {
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector2(x, y);
        rect.sizeDelta = new Vector2(width, height);
    }
}
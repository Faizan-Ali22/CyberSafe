using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;
using Button = UnityEngine.UI.Button;

/// <summary>
/// Password UI Setup - Unity 6 + Android Compatible
/// Professional 175 IQ implementation with proper reference assignment
/// </summary>
[DefaultExecutionOrder(-200)]
[RequireComponent(typeof(PasswordStrengthChecker))]
public class PasswordUISetup : MonoBehaviour
{
    [Header("Auto-Setup")]
    [SerializeField] private Canvas targetCanvas;
    
    private PasswordStrengthChecker checker;
    private GameObject avatarPanel;
    private GameObject passwordPanel;
    private List<GameObject> avatarButtons = new List<GameObject>();
    
    // Store references to assign
    private TextMeshProUGUI passwordDisplayText;
    private TextMeshProUGUI strengthText;
    private TextMeshProUGUI hintsText;
    private TextMeshProUGUI characterCountText;
    private TextMeshProUGUI crackTimeText;
    private TextMeshProUGUI currentAccountNameText;
    private Image strengthBar;
    private Button toggleVisibilityButton;
    private Button setPasswordButton;
    private TMP_InputField passwordInputField;
    
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

        EnsureEventSystem();
        SetupCompleteUI();
        AssignReferencesToChecker();
    }
    
    private void CreateCanvas()
    {
        GameObject canvasObj = new GameObject("PasswordCheckerCanvas");
        targetCanvas = canvasObj.AddComponent<Canvas>();
        targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        targetCanvas.sortingOrder = 100;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        Debug.Log("✅ Canvas created - Android compatible");
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
            Debug.Log("✅ EventSystem created with InputSystemUIInputModule");
#else
            eventSystem.AddComponent<StandaloneInputModule>();
            Debug.Log("✅ EventSystem created with StandaloneInputModule");
#endif
            return;
        }

#if ENABLE_INPUT_SYSTEM
        var oldModule = existing.GetComponent<StandaloneInputModule>();
        var newModule = existing.GetComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        if (newModule == null && oldModule != null)
        {
            DestroyImmediate(oldModule);
            existing.gameObject.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            Debug.Log("✅ Switched to InputSystemUIInputModule");
        }
#endif
    }
    
    private void SetupCompleteUI()
    {
        CreateAvatarSelectionPanel();
        CreatePasswordCheckerPanel();
    }
    
    /// <summary>
    /// CRITICAL: Assign all references to PasswordStrengthChecker after UI creation
    /// </summary>
    private void AssignReferencesToChecker()
    {
        if (checker != null)
        {
            checker.AssignUIReferences(
                passwordDisplayText,
                strengthText,
                hintsText,
                characterCountText,
                crackTimeText,
                currentAccountNameText,
                strengthBar,
                toggleVisibilityButton,
                setPasswordButton,
                avatarPanel,
                passwordPanel,
                passwordInputField
            );
            
            Debug.Log("✅ All UI references assigned to PasswordStrengthChecker!");
        }
        else
        {
            Debug.LogError("❌ PasswordStrengthChecker component not found!");
        }
    }
    
    private void CreateAvatarSelectionPanel()
    {
        avatarPanel = CreateUIPanel("AvatarSelectionPanel", targetCanvas.transform);
        RectTransform panelRect = avatarPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;
        avatarPanel.GetComponent<Image>().color = new Color(0.05f, 0.05f, 0.1f, 1f);
        
        TextMeshProUGUI title = CreateText("Title", avatarPanel.transform,
            "<b>🔐 Select Account to Set Password</b>", 36);
        PositionElement(title.rectTransform, 0, 300, 1000, 80);
        title.alignment = TextAlignmentOptions.Center;
        title.color = new Color(0.3f, 0.8f, 1f);
        
        string[] names = checker.GetAvatarNames();
        Sprite[] customImages = checker.GetAvatarImages();
        string[] emojis = { "👩", "👨", "🧑", "👩‍💼", "🧑‍💻" };
        Color[] colors = {
            new Color(1f, 0.4f, 0.4f),
            new Color(0.4f, 0.6f, 1f),
            new Color(0.4f, 1f, 0.6f),
            new Color(1f, 0.8f, 0.4f),
            new Color(0.8f, 0.4f, 1f)
        };
        
        float startX = -400f;
        float spacing = 200f;
        
        for (int i = 0; i < 5; i++)
        {
            // Use custom image if provided, otherwise use emoji
            Sprite avatarSprite = (customImages != null && i < customImages.Length) ? customImages[i] : null;
            string avatarEmoji = (avatarSprite == null) ? emojis[i] : "";
            
            GameObject avatarBtn = CreateAvatarButton(
                $"Avatar_{i}", 
                avatarPanel.transform, 
                names[i], 
                avatarEmoji,
                avatarSprite,
                colors[i],
                i
            );
            
            float xPos = startX + (i * spacing);
            PositionElement(avatarBtn.GetComponent<RectTransform>(), xPos, 50, 180, 250);
            avatarButtons.Add(avatarBtn);
        }
    }
    
    private GameObject CreateAvatarButton(string name, Transform parent, string userName, string emoji, Sprite customImage, Color color, int index)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);
        RectTransform rect = btnObj.AddComponent<RectTransform>();
        Image img = btnObj.AddComponent<Image>();
        Button btn = btnObj.AddComponent<Button>();
        
        img.color = color;
        
        // Avatar circle/square container
        GameObject circle = new GameObject("AvatarContainer");
        circle.transform.SetParent(btnObj.transform, false);
        RectTransform circleRect = circle.AddComponent<RectTransform>();
        circleRect.sizeDelta = new Vector2(120, 120);
        circleRect.anchoredPosition = new Vector2(0, 30);
        Image circleImg = circle.AddComponent<Image>();
        circleImg.raycastTarget = false;
        
        // If custom image provided, use it; otherwise use white background for emoji
        if (customImage != null)
        {
            circleImg.sprite = customImage;
            circleImg.color = Color.white;
            circleImg.preserveAspect = true;
            
            Debug.Log($"✅ Using custom image for avatar {index}: {userName}");
        }
        else
        {
            // White background for emoji
            circleImg.color = Color.white;
            
            // Create emoji text
            TextMeshProUGUI emojiText = CreateText("Emoji", circle.transform, emoji, 60);
            emojiText.rectTransform.sizeDelta = new Vector2(120, 120);
            emojiText.rectTransform.anchoredPosition = Vector2.zero;
            emojiText.alignment = TextAlignmentOptions.Center;
            emojiText.raycastTarget = false;
        }
        
        TextMeshProUGUI nameText = CreateText("Name", btnObj.transform, userName, 22);
        nameText.rectTransform.sizeDelta = new Vector2(180, 40);
        nameText.rectTransform.anchoredPosition = new Vector2(0, -50);
        nameText.alignment = TextAlignmentOptions.Center;
        nameText.fontStyle = FontStyles.Bold;
        nameText.color = Color.white;
        nameText.raycastTarget = false;
        
        TextMeshProUGUI statusText = CreateText("Status", btnObj.transform, "No Password", 16);
        statusText.rectTransform.sizeDelta = new Vector2(180, 30);
        statusText.rectTransform.anchoredPosition = new Vector2(0, -90);
        statusText.alignment = TextAlignmentOptions.Center;
        statusText.color = new Color(0.7f, 0.7f, 0.7f);
        statusText.raycastTarget = false;
        
        int accountIndex = index;
        btn.onClick.AddListener(() => OnAvatarClicked(accountIndex));
        
        ColorBlock colorsBlock = btn.colors;
        colorsBlock.normalColor = color;
        colorsBlock.highlightedColor = color * 1.2f;
        colorsBlock.pressedColor = color * 0.8f;
        colorsBlock.selectedColor = color * 1.1f;
        btn.colors = colorsBlock;
        
        return btnObj;
    }
    
    private void OnAvatarClicked(int index)
    {
        Debug.Log($"🎯 Avatar {index} clicked: {checker.GetAvatarNames()[index]}");
        checker.SelectAccount(index);
    }
    
    private void Update()
    {
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
        
        // Account name
        currentAccountNameText = CreateText("AccountName", passwordPanel.transform,
            "Setting password for: <b>User</b>", 22);
        PositionElement(currentAccountNameText.rectTransform, 0, 390, 600, 50);
        currentAccountNameText.alignment = TextAlignmentOptions.Center;
        currentAccountNameText.color = new Color(0.3f, 0.8f, 1f);
        currentAccountNameText.raycastTarget = false;
        
        // Title
        TextMeshProUGUI title = CreateText("Title", passwordPanel.transform,
            "<b>🔐 Password Strength Analyzer</b>", 28);
        PositionElement(title.rectTransform, 0, 330, 800, 60);
        title.alignment = TextAlignmentOptions.Center;
        title.color = new Color(0.3f, 0.8f, 1f);
        title.raycastTarget = false;
        
        // Input field box
        GameObject displayBox = CreateUIPanel("DisplayBox", passwordPanel.transform);
        PositionElement(displayBox.GetComponent<RectTransform>(), 0, 240, 800, 80);
        displayBox.GetComponent<Image>().color = new Color(0.15f, 0.15f, 0.2f);
        
        // TMP_InputField for Android keyboard
        passwordInputField = CreateTMPInputField("PasswordInput", displayBox.transform);
        PositionElement(passwordInputField.GetComponent<RectTransform>(), -30, 0, 720, 60);
        
        // Password display (shows masked version)
        passwordDisplayText = CreateText("PasswordDisplay", displayBox.transform,
            "Start typing your password...", 24);
        PositionElement(passwordDisplayText.rectTransform, -30, 0, 720, 60);
        passwordDisplayText.alignment = TextAlignmentOptions.Center;
        passwordDisplayText.color = Color.white;
        passwordDisplayText.raycastTarget = false;
        passwordDisplayText.overflowMode = TextOverflowModes.Ellipsis;
        
        // Toggle button
        GameObject toggleButton = CreateButton("ToggleButton", displayBox.transform, "👁️ Show");
        PositionElement(toggleButton.GetComponent<RectTransform>(), 330, 0, 100, 60);
        toggleVisibilityButton = toggleButton.GetComponent<Button>();
        toggleVisibilityButton.GetComponent<Image>().color = new Color(0.2f, 0.4f, 0.6f);
        
        TextMeshProUGUI toggleBtnText = toggleButton.GetComponentInChildren<TextMeshProUGUI>();
        toggleBtnText.fontSize = 16;
        toggleBtnText.alignment = TextAlignmentOptions.Center;
        toggleBtnText.color = Color.white;
        toggleBtnText.fontStyle = FontStyles.Bold;
        
        // Character count
        characterCountText = CreateText("CharacterCount", passwordPanel.transform,
            "Characters: 0", 18);
        PositionElement(characterCountText.rectTransform, 0, 180, 800, 30);
        characterCountText.alignment = TextAlignmentOptions.Center;
        characterCountText.color = new Color(0.7f, 0.7f, 0.7f);
        characterCountText.raycastTarget = false;
        
        // Strength bar background
        GameObject barBg = CreateUIPanel("StrengthBarBG", passwordPanel.transform);
        PositionElement(barBg.GetComponent<RectTransform>(), 0, 130, 800, 30);
        barBg.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.25f);
        barBg.GetComponent<Image>().raycastTarget = false;
        
        // Strength bar fill
        GameObject barFill = CreateUIPanel("StrengthBarFill", barBg.transform);
        RectTransform barFillRect = barFill.GetComponent<RectTransform>();
        barFillRect.anchorMin = new Vector2(0, 0);
        barFillRect.anchorMax = new Vector2(0, 1);
        barFillRect.pivot = new Vector2(0, 0.5f);
        barFillRect.anchoredPosition = Vector2.zero;
        barFillRect.sizeDelta = new Vector2(800, 0);
        
        strengthBar = barFill.GetComponent<Image>();
        strengthBar.color = new Color(1f, 0.2f, 0.2f);
        strengthBar.type = Image.Type.Filled;
        strengthBar.fillMethod = Image.FillMethod.Horizontal;
        strengthBar.fillAmount = 0;
        strengthBar.raycastTarget = false;
        
        // Strength text
        strengthText = CreateText("StrengthText", passwordPanel.transform,
            "Strength: None\nScore: 0/100", 24);
        PositionElement(strengthText.rectTransform, 0, 70, 800, 60);
        strengthText.alignment = TextAlignmentOptions.Center;
        strengthText.fontStyle = FontStyles.Bold;
        strengthText.raycastTarget = false;
        
        // Crack time box
        GameObject crackTimeBox = CreateUIPanel("CrackTimeBox", passwordPanel.transform);
        PositionElement(crackTimeBox.GetComponent<RectTransform>(), 0, -10, 800, 90);
        crackTimeBox.GetComponent<Image>().color = new Color(0.12f, 0.12f, 0.18f);
        crackTimeBox.GetComponent<Image>().raycastTarget = false;
        
        crackTimeText = CreateText("CrackTimeText", crackTimeBox.transform, "", 20);
        PositionElement(crackTimeText.rectTransform, 0, 0, 780, 80);
        crackTimeText.alignment = TextAlignmentOptions.Center;
        crackTimeText.color = Color.white;
        crackTimeText.raycastTarget = false;
        crackTimeText.enableAutoSizing = false;
        
        // Hints panel
        GameObject hintsPanel = CreateUIPanel("HintsPanel", passwordPanel.transform);
        PositionElement(hintsPanel.GetComponent<RectTransform>(), 0, -132, 800, 180);
        hintsPanel.GetComponent<Image>().color = new Color(0.15f, 0.15f, 0.2f);
        hintsPanel.GetComponent<Image>().raycastTarget = false;
        
        hintsText = CreateText("HintsText", hintsPanel.transform,
            "💡 Tips for a Strong Password:\n" +
            "• Use at least 12 characters\n" +
            "• Mix uppercase and lowercase letters\n" +
            "• Include numbers and special characters\n" +
            "• Avoid common words and patterns", 16);
        PositionElement(hintsText.rectTransform, 0, 0, 780, 160);
        hintsText.alignment = TextAlignmentOptions.TopLeft;
        hintsText.color = new Color(0.9f, 0.9f, 0.9f);
        hintsText.raycastTarget = false;
        
        // Set password button
        GameObject setPassBtn = CreateButton("SetPasswordButton", passwordPanel.transform, "✓ Set Password");
        PositionElement(setPassBtn.GetComponent<RectTransform>(), 0, -293, 400, 70);
        setPasswordButton = setPassBtn.GetComponent<Button>();
        setPasswordButton.GetComponent<Image>().color = new Color(0.2f, 0.8f, 0.3f);
        
        TextMeshProUGUI setPassBtnText = setPassBtn.GetComponentInChildren<TextMeshProUGUI>();
        setPassBtnText.fontSize = 26;
        setPassBtnText.fontStyle = FontStyles.Bold;
        setPassBtnText.color = Color.white;
        
        ColorBlock setBtnColors = setPasswordButton.colors;
        setBtnColors.normalColor = new Color(0.2f, 0.8f, 0.3f);
        setBtnColors.highlightedColor = new Color(0.3f, 1f, 0.4f);
        setBtnColors.pressedColor = new Color(0.15f, 0.6f, 0.25f);
        setBtnColors.selectedColor = new Color(0.25f, 0.9f, 0.35f);
        setPasswordButton.colors = setBtnColors;
        
        Canvas.ForceUpdateCanvases();
        
        Debug.Log("✅ Password UI Complete - Android keyboard enabled!");
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

    private TMP_InputField CreateTMPInputField(string name, Transform parent)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.AddComponent<RectTransform>();

        TMP_InputField input = obj.AddComponent<TMP_InputField>();
        
        // CRITICAL: Android keyboard configuration
        input.lineType = TMP_InputField.LineType.SingleLine;
        input.contentType = TMP_InputField.ContentType.Password;
        input.inputType = TMP_InputField.InputType.Standard;
        input.keyboardType = TouchScreenKeyboardType.Default;
        input.characterLimit = 128;
        input.caretWidth = 2;
        input.caretColor = new Color(0.2f, 1f, 0.3f, 1f);
        input.customCaretColor = true;
        input.selectionColor = new Color(0.2f, 0.5f, 1f, 0.3f);
        input.richText = false;
        
        // Text component
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(obj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.offsetMin = new Vector2(10, 5);
        textRect.offsetMax = new Vector2(-10, -5);

        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.fontSize = 24;
        text.color = Color.white;
        text.enableAutoSizing = false;
        text.overflowMode = TextOverflowModes.Ellipsis;
        text.alignment = TextAlignmentOptions.Center;
        text.raycastTarget = false;

        // Placeholder
        GameObject placeholderObj = new GameObject("Placeholder");
        placeholderObj.transform.SetParent(obj.transform, false);
        RectTransform placeholderRect = placeholderObj.AddComponent<RectTransform>();
        placeholderRect.anchorMin = Vector2.zero;
        placeholderRect.anchorMax = Vector2.one;
        placeholderRect.sizeDelta = Vector2.zero;
        placeholderRect.offsetMin = new Vector2(10, 5);
        placeholderRect.offsetMax = new Vector2(-10, -5);

        TextMeshProUGUI placeholder = placeholderObj.AddComponent<TextMeshProUGUI>();
        placeholder.text = "Tap to enter password...";
        placeholder.fontSize = 24;
        placeholder.color = new Color(1f, 1f, 1f, 0.3f);
        placeholder.enableAutoSizing = false;
        placeholder.alignment = TextAlignmentOptions.Center;
        placeholder.raycastTarget = false;

        input.textViewport = rect;
        input.textComponent = text;
        input.placeholder = placeholder;

        Debug.Log("✅ TMP_InputField created for Android keyboard");
        
        return input;
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
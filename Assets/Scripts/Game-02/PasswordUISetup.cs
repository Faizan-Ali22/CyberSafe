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
  [Header("Auto-Setup (Will find/create automatically)")]
    [SerializeField] private Canvas targetCanvas;
    
    [Header("Unity 6 Settings")]
    [SerializeField] private bool useCanvasScalerUIToolkit = false;
    
    private void Awake()
    {
        // Find or create canvas
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
        
        // Add CanvasScaler with Unity 6 optimizations
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        Debug.Log("✅ Canvas created with Unity 6 optimizations");
    }
    
    private void SetupCompleteUI()
    {
        // Create main panel
        GameObject panel = CreateUIPanel("MainPanel", targetCanvas.transform);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(850, 750);
        
        Image panelImg = panel.GetComponent<Image>();
        panelImg.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);
        panelImg.raycastTarget = false;
        
        // Title
        TextMeshProUGUI title = CreateText("Title", panel.transform, 
            "<b>🔐 Password Strength Analyzer</b>", 32);
        PositionElement(title.rectTransform, 0, 330, 800, 60);
        title.alignment = TextAlignmentOptions.Center;
        title.color = new Color(0.3f, 0.8f, 1f);
        title.raycastTarget = false;
        
        // Password Display Box
        GameObject displayBox = CreateUIPanel("DisplayBox", panel.transform);
        PositionElement(displayBox.GetComponent<RectTransform>(), 0, 240, 800, 80);
        displayBox.GetComponent<Image>().color = new Color(0.15f, 0.15f, 0.2f);
        displayBox.GetComponent<Image>().raycastTarget = false;
        
        TextMeshProUGUI passwordDisplay = CreateText("PasswordDisplay", displayBox.transform,
            "Start typing your password...", 24);
        PositionElement(passwordDisplay.rectTransform, -30, 0, 720, 60);
        passwordDisplay.alignment = TextAlignmentOptions.Center;
        passwordDisplay.color = Color.white;
        passwordDisplay.raycastTarget = false;
        passwordDisplay.enableWordWrapping = true;
        passwordDisplay.overflowMode = TextOverflowModes.Ellipsis;
        
        // Toggle Visibility Button
        GameObject toggleButton = CreateButton("ToggleButton", displayBox.transform, "👁️ Show");
        PositionElement(toggleButton.GetComponent<RectTransform>(), 330, 0, 100, 60);
        Button toggleBtnComponent = toggleButton.GetComponent<Button>();
        toggleBtnComponent.GetComponent<Image>().color = new Color(0.2f, 0.4f, 0.6f);
        
        TextMeshProUGUI toggleBtnText = toggleButton.GetComponentInChildren<TextMeshProUGUI>();
        toggleBtnText.fontSize = 16;
        toggleBtnText.alignment = TextAlignmentOptions.Center;
        toggleBtnText.color = Color.white;
        toggleBtnText.fontStyle = FontStyles.Bold;
        
        // Character Count
        TextMeshProUGUI charCount = CreateText("CharacterCount", panel.transform,
            "Characters: 0", 18);
        PositionElement(charCount.rectTransform, 0, 180, 800, 30);
        charCount.alignment = TextAlignmentOptions.Center;
        charCount.color = new Color(0.7f, 0.7f, 0.7f);
        charCount.raycastTarget = false;
        
        // Strength Bar Background
        GameObject barBg = CreateUIPanel("StrengthBarBG", panel.transform);
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
        TextMeshProUGUI strengthText = CreateText("StrengthText", panel.transform,
            "Strength: None\nScore: 0/100", 24);
        PositionElement(strengthText.rectTransform, 0, 70, 800, 60);
        strengthText.alignment = TextAlignmentOptions.Center;
        strengthText.fontStyle = FontStyles.Bold;
        strengthText.raycastTarget = false;
        
        // Crack Time Display (NEW!)
        GameObject crackTimeBox = CreateUIPanel("CrackTimeBox", panel.transform);
        PositionElement(crackTimeBox.GetComponent<RectTransform>(), 0, -10, 800, 90);
        crackTimeBox.GetComponent<Image>().color = new Color(0.12f, 0.12f, 0.18f);
        crackTimeBox.GetComponent<Image>().raycastTarget = false;
        
        TextMeshProUGUI crackTimeText = CreateText("CrackTimeText", crackTimeBox.transform,
            "", 20);
        PositionElement(crackTimeText.rectTransform, 0, 0, 780, 80);
        crackTimeText.alignment = TextAlignmentOptions.Center;
        crackTimeText.color = Color.white;
        crackTimeText.raycastTarget = false;
        crackTimeText.enableAutoSizing = false;
        
        // Hints Panel
        GameObject hintsPanel = CreateUIPanel("HintsPanel", panel.transform);
        PositionElement(hintsPanel.GetComponent<RectTransform>(), 0, -180, 800, 200);
        hintsPanel.GetComponent<Image>().color = new Color(0.15f, 0.15f, 0.2f);
        hintsPanel.GetComponent<Image>().raycastTarget = false;
        
        TextMeshProUGUI hintsText = CreateText("HintsText", hintsPanel.transform,
            "💡 Tips for a Strong Password:\n" +
            "• Use at least 12 characters\n" +
            "• Mix uppercase and lowercase letters\n" +
            "• Include numbers and special characters\n" +
            "• Avoid common words and patterns", 16);
        PositionElement(hintsText.rectTransform, 0, 0, 780, 180);
        hintsText.alignment = TextAlignmentOptions.TopLeft;
        hintsText.color = new Color(0.9f, 0.9f, 0.9f);
        hintsText.raycastTarget = false;
        
        // Connect to PasswordStrengthChecker using Unity 6 compatible method
        PasswordStrengthChecker checker = GetComponent<PasswordStrengthChecker>();
        if (checker != null)
        {
            // Use reflection to set private fields (works in Unity 6)
            var type = typeof(PasswordStrengthChecker);
            var bindingFlags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            
            type.GetField("passwordDisplayText", bindingFlags)?.SetValue(checker, passwordDisplay);
            type.GetField("strengthText", bindingFlags)?.SetValue(checker, strengthText);
            type.GetField("hintsText", bindingFlags)?.SetValue(checker, hintsText);
            type.GetField("characterCountText", bindingFlags)?.SetValue(checker, charCount);
            type.GetField("strengthBar", bindingFlags)?.SetValue(checker, barFillImg);
            type.GetField("crackTimeText", bindingFlags)?.SetValue(checker, crackTimeText);
            type.GetField("toggleVisibilityButton", bindingFlags)?.SetValue(checker, toggleBtnComponent);
        }
        
        // Unity 6: Force canvas update
        Canvas.ForceUpdateCanvases();
        
        Debug.Log("✅ Password Strength Checker UI Setup Complete! (Unity 6 Compatible)");
        Debug.Log("✅ Password visibility toggle button added!");
        Debug.Log("✅ Crack time calculator enabled!");
    }
    
    private GameObject CreateUIPanel(string name, Transform parent)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        RectTransform rect = obj.AddComponent<RectTransform>();
        Image img = obj.AddComponent<Image>();
        
        // Unity 6: Set default material if needed
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
        
        // Create text child for button
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
        
        // Button color transition
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
        
        // Unity 6: Enable vertex data optimizations
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


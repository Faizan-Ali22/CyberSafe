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
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
public class MainMenu : MonoBehaviour
{
    //    [Header("Background Settings")]
    //     [SerializeField] private RawImage backgroundImage;
    //     [SerializeField] private Texture backgroundTexture;
    //     [SerializeField] private float backgroundPanSpeed = 2f;
    //     [SerializeField] private Color backgroundTint = new Color(0.1f, 0.3f, 0.4f, 0.5f);

    //     [Header("Terminal Settings")]
    //     [SerializeField] private Color terminalGreen = new Color(0.2f, 1f, 0.3f);
    //     [SerializeField] private Color terminalHighlight = new Color(0.4f, 1f, 0.5f);
    //     [SerializeField] private float typewriterSpeed = 0.05f;
    //     [SerializeField] private float cursorBlinkSpeed = 0.5f;

    //     [Header("Audio (Optional)")]
    //     [SerializeField] private AudioClip keyboardTypingSound;
    //     [SerializeField] private AudioClip glitchSound;
    //     [SerializeField] private AudioClip selectSound;
    //     [SerializeField] private AudioSource audioSource;

    //     [Header("Scene Names")]
    //     [SerializeField] private string newGameSceneName = "GameScene";
    //     [SerializeField] private string resumeSceneName = "GameScene";

    //     // UI References
    //     private Canvas canvas;
    //     private GameObject terminalPanel;
    //     private TextMeshProUGUI titleText;
    //     private List<MenuButton> menuButtons = new List<MenuButton>();
    //     private TextMeshProUGUI systemLogText;
    //     private TextMeshProUGUI cursorText;

    //     private int selectedIndex = 0;
    //     private bool isTyping = false;
    //     private float backgroundOffset = 0f;

    //     // Menu items
    //     private string[] menuItems = {
    //         "> CONNECT TO SERVER",
    //         "> RESUME SESSION", 
    //         "> ACCESS TRAINING MODULES",
    //         "> SETTINGS",
    //         "> EXIT TERMINAL"
    //     };

    //     private string[] menuDescriptions = {
    //         "New Game",
    //         "Continue",
    //         "Mini-games",
    //         "Settings",
    //         "Exit"
    //     };

    //     private void Awake()
    //     {
    //         SetupCanvas();
    //         SetupBackground();
    //         SetupUI();
    //         StartCoroutine(InitializeTerminal());
    //     }

    //     private void Update()
    //     {
    //         HandleInput();
    //         AnimateBackground();

    //         // Glitch effects randomly
    //         if (Random.value < 0.001f && audioSource != null && glitchSound != null)
    //         {
    //             audioSource.PlayOneShot(glitchSound, 0.1f);
    //             StartCoroutine(GlitchEffect());
    //         }
    //     }

    //     private void SetupCanvas()
    //     {
    //         GameObject canvasObj = new GameObject("MainMenuCanvas");
    //         canvas = canvasObj.AddComponent<Canvas>();
    //         canvas.renderMode = RenderMode.ScreenSpaceOverlay;

    //         CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
    //         scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
    //         scaler.referenceResolution = new Vector2(1920, 1080);
    //         scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
    //         scaler.matchWidthOrHeight = 0.5f;

    //         canvasObj.AddComponent<GraphicRaycaster>();

    //         if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
    //         {
    //             GameObject eventSystem = new GameObject("EventSystem");
    //             eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
    //             eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
    //         }
    //     }

    //     private void SetupBackground()
    //     {
    //         GameObject bgObj = new GameObject("Background");
    //         bgObj.transform.SetParent(canvas.transform, false);

    //         RectTransform bgRect = bgObj.AddComponent<RectTransform>();
    //         bgRect.anchorMin = Vector2.zero;
    //         bgRect.anchorMax = Vector2.one;
    //         bgRect.sizeDelta = Vector2.zero;

    //         backgroundImage = bgObj.AddComponent<RawImage>();

    //         if (backgroundTexture != null)
    //         {
    //             backgroundImage.texture = backgroundTexture;
    //             backgroundImage.color = backgroundTint;
    //         }
    //         else
    //         {
    //             // Default dark background with grid pattern
    //             backgroundImage.color = new Color(0.05f, 0.05f, 0.1f, 1f);
    //         }

    //         // Add overlay for effect
    //         GameObject overlayObj = new GameObject("Overlay");
    //         overlayObj.transform.SetParent(bgObj.transform, false);

    //         RectTransform overlayRect = overlayObj.AddComponent<RectTransform>();
    //         overlayRect.anchorMin = Vector2.zero;
    //         overlayRect.anchorMax = Vector2.one;
    //         overlayRect.sizeDelta = Vector2.zero;

    //         Image overlay = overlayObj.AddComponent<Image>();
    //         overlay.color = new Color(0f, 0.1f, 0.2f, 0.7f);
    //         overlay.raycastTarget = false;
    //     }

    //     private void SetupUI()
    //     {
    //         // Main terminal panel
    //         terminalPanel = new GameObject("TerminalPanel");
    //         terminalPanel.transform.SetParent(canvas.transform, false);

    //         RectTransform panelRect = terminalPanel.AddComponent<RectTransform>();
    //         panelRect.anchorMin = new Vector2(0.5f, 0.5f);
    //         panelRect.anchorMax = new Vector2(0.5f, 0.5f);
    //         panelRect.pivot = new Vector2(0.5f, 0.5f);
    //         panelRect.sizeDelta = new Vector2(1200, 800);
    //         panelRect.anchoredPosition = Vector2.zero;

    //         Image panelImg = terminalPanel.AddComponent<Image>();
    //         panelImg.color = new Color(0.05f, 0.05f, 0.1f, 0.85f);
    //         panelImg.raycastTarget = false;

    //         // Scanline effect


    //         // Title
    //         CreateTitle();

    //         // System log (top right corner)
    //         CreateSystemLog();

    //         // Menu items
    //         CreateMenuItems();

    //         // Cursor
    //         CreateCursor();

    //         // Border glow effect
    //         CreateBorderGlow();
    //     }


    //     public IEnumerator AnimateScanlines(RectTransform rect)
    //     {
    //         float offset = 0f;
    //         while (true)
    //         {
    //             offset += Time.deltaTime * 50f;
    //             if (offset > 1080f) offset = 0f;
    //             rect.anchoredPosition = new Vector2(0, offset);
    //             yield return null;
    //         }
    //     }

    //     private void CreateTitle()
    //     {
    //         GameObject titleObj = new GameObject("Title");
    //         titleObj.transform.SetParent(terminalPanel.transform, false);

    //         RectTransform rect = titleObj.AddComponent<RectTransform>();
    //         rect.anchorMin = new Vector2(0.5f, 1f);
    //         rect.anchorMax = new Vector2(0.5f, 1f);
    //         rect.pivot = new Vector2(0.5f, 1f);
    //         rect.sizeDelta = new Vector2(1100, 100);
    //         rect.anchoredPosition = new Vector2(0, -50);

    //         titleText = titleObj.AddComponent<TextMeshProUGUI>();
    //         titleText.text = "";
    //         titleText.fontSize = 48;
    //         titleText.fontStyle = FontStyles.Bold;
    //         titleText.color = terminalGreen;
    //         titleText.alignment = TextAlignmentOptions.Center;
    //         titleText.raycastTarget = false;

    //         // Add glitch effect
    //         StartCoroutine(RandomGlitch(titleText));
    //     }

    //     private void CreateSystemLog()
    //     {
    //         GameObject logObj = new GameObject("SystemLog");
    //         logObj.transform.SetParent(terminalPanel.transform, false);

    //         RectTransform rect = logObj.AddComponent<RectTransform>();
    //         rect.anchorMin = new Vector2(1f, 1f);
    //         rect.anchorMax = new Vector2(1f, 1f);
    //         rect.pivot = new Vector2(1f, 1f);
    //         rect.sizeDelta = new Vector2(350, 150);
    //         rect.anchoredPosition = new Vector2(-20, -20);

    //         systemLogText = logObj.AddComponent<TextMeshProUGUI>();
    //         systemLogText.fontSize = 12;
    //         systemLogText.color = new Color(0.5f, 0.8f, 0.5f, 0.6f);
    //         systemLogText.alignment = TextAlignmentOptions.TopRight;
    //         systemLogText.raycastTarget = false;
    //         systemLogText.enableWordWrapping = true;

    //         StartCoroutine(UpdateSystemLog());
    //     }

    //     private void CreateMenuItems()
    //     {
    //         float startY = 100f;
    //         float spacing = 80f;

    //         for (int i = 0; i < menuItems.Length; i++)
    //         {
    //             MenuButton btn = CreateMenuButton(i, startY - (i * spacing));
    //             menuButtons.Add(btn);
    //         }
    //     }

    //     private MenuButton CreateMenuButton(int index, float yPos)
    //     {
    //         GameObject btnObj = new GameObject($"MenuItem_{index}");
    //         btnObj.transform.SetParent(terminalPanel.transform, false);

    //         RectTransform rect = btnObj.AddComponent<RectTransform>();
    //         rect.anchorMin = new Vector2(0.5f, 0.5f);
    //         rect.anchorMax = new Vector2(0.5f, 0.5f);
    //         rect.pivot = new Vector2(0.5f, 0.5f);
    //         rect.sizeDelta = new Vector2(800, 60);
    //         rect.anchoredPosition = new Vector2(0, yPos);

    //         TextMeshProUGUI text = btnObj.AddComponent<TextMeshProUGUI>();
    //         text.fontSize = 32;
    //         text.color = terminalGreen;
    //         text.alignment = TextAlignmentOptions.Left;
    //         text.raycastTarget = false;

    //         // Description text
    //         GameObject descObj = new GameObject("Description");
    //         descObj.transform.SetParent(btnObj.transform, false);

    //         RectTransform descRect = descObj.AddComponent<RectTransform>();
    //         descRect.anchorMin = new Vector2(1f, 0.5f);
    //         descRect.anchorMax = new Vector2(1f, 0.5f);
    //         descRect.pivot = new Vector2(0f, 0.5f);
    //         descRect.sizeDelta = new Vector2(200, 40);
    //         descRect.anchoredPosition = new Vector2(20, 0);

    //         TextMeshProUGUI descText = descObj.AddComponent<TextMeshProUGUI>();
    //         descText.text = $"// {menuDescriptions[index]}";
    //         descText.fontSize = 18;
    //         descText.color = new Color(0.5f, 0.8f, 0.5f, 0.6f);
    //         descText.alignment = TextAlignmentOptions.Left;
    //         descText.fontStyle = FontStyles.Italic;
    //         descText.raycastTarget = false;

    //         MenuButton menuBtn = new MenuButton
    //         {
    //             textComponent = text,
    //             descriptionText = descText,
    //             originalText = menuItems[index],
    //             index = index
    //         };

    //         return menuBtn;
    //     }

    //     private void CreateCursor()
    //     {
    //         GameObject cursorObj = new GameObject("Cursor");
    //         cursorObj.transform.SetParent(terminalPanel.transform, false);

    //         RectTransform rect = cursorObj.AddComponent<RectTransform>();
    //         rect.anchorMin = new Vector2(0.5f, 0.5f);
    //         rect.anchorMax = new Vector2(0.5f, 0.5f);
    //         rect.pivot = new Vector2(0f, 0.5f);
    //         rect.sizeDelta = new Vector2(30, 50);
    //         rect.anchoredPosition = new Vector2(-420, 100);

    //         cursorText = cursorObj.AddComponent<TextMeshProUGUI>();
    //         cursorText.text = "█";
    //         cursorText.fontSize = 40;
    //         cursorText.color = terminalHighlight;
    //         cursorText.alignment = TextAlignmentOptions.Left;
    //         cursorText.raycastTarget = false;

    //         StartCoroutine(BlinkCursor());
    //     }

    //     private void CreateBorderGlow()
    //     {
    //         GameObject borderObj = new GameObject("BorderGlow");
    //         borderObj.transform.SetParent(terminalPanel.transform, false);

    //         RectTransform rect = borderObj.AddComponent<RectTransform>();
    //         rect.anchorMin = Vector2.zero;
    //         rect.anchorMax = Vector2.one;
    //         rect.sizeDelta = new Vector2(10, 10);

    //         Image border = borderObj.AddComponent<Image>();
    //         border.color = new Color(0, 0, 0, 0);
    //         border.raycastTarget = false;

    //         // Create glow outline
    //         Outline outline = borderObj.AddComponent<Outline>();
    //         outline.effectColor = terminalGreen;
    //         outline.effectDistance = new Vector2(2, 2);

    //         StartCoroutine(PulseBorder(outline));
    //     }

    //     private IEnumerator InitializeTerminal()
    //     {
    //         yield return new WaitForSeconds(0.5f);

    //         // Type title
    //         yield return StartCoroutine(TypeText(titleText, "═══ SECURITY TERMINAL v2.4.7 ═══"));

    //         yield return new WaitForSeconds(0.3f);

    //         // Type menu items in loop forever
    //         StartCoroutine(TypeMenuItemsLoop());
    //     }

    //     private IEnumerator TypeMenuItemsLoop()
    //     {
    //         while (true)
    //         {
    //             for (int i = 0; i < menuButtons.Count; i++)
    //             {
    //                 isTyping = true;
    //                 yield return StartCoroutine(TypeText(menuButtons[i].textComponent, menuButtons[i].originalText));
    //                 yield return new WaitForSeconds(0.2f);
    //             }

    //             isTyping = false;
    //             yield return new WaitForSeconds(3f);

    //             // Clear all text
    //             foreach (var btn in menuButtons)
    //             {
    //                 btn.textComponent.text = "";
    //             }

    //             yield return new WaitForSeconds(0.5f);
    //         }
    //     }

    //     private IEnumerator TypeText(TextMeshProUGUI textComponent, string fullText)
    //     {
    //         textComponent.text = "";

    //         foreach (char c in fullText)
    //         {
    //             textComponent.text += c;

    //             if (audioSource != null && keyboardTypingSound != null && Random.value < 0.3f)
    //             {
    //                 audioSource.PlayOneShot(keyboardTypingSound, 0.1f);
    //             }

    //             yield return new WaitForSeconds(typewriterSpeed);
    //         }
    //     }

    //     private IEnumerator BlinkCursor()
    //     {
    //         while (true)
    //         {
    //             cursorText.enabled = !cursorText.enabled;
    //             yield return new WaitForSeconds(cursorBlinkSpeed);
    //         }
    //     }

    //     private IEnumerator UpdateSystemLog()
    //     {
    //         string[] logs = {
    //             "[SYSTEM] Connection established...",
    //             "[INFO] Loading neural interface...",
    //             "[WARN] Anomaly detected in sector 7",
    //             "[ERROR] NULL entity signature found",
    //             "[SYSTEM] Security protocols active",
    //             "[INFO] Firewall status: ENGAGED",
    //             "[WARN] Unauthorized access attempt",
    //             "[SYSTEM] Monitoring data streams..."
    //         };

    //         while (true)
    //         {
    //             List<string> recentLogs = new List<string>();

    //             for (int i = 0; i < 5; i++)
    //             {
    //                 string log = $"[{System.DateTime.Now:HH:mm:ss}] {logs[Random.Range(0, logs.Length)]}";
    //                 recentLogs.Insert(0, log);

    //                 if (recentLogs.Count > 5)
    //                     recentLogs.RemoveAt(recentLogs.Count - 1);

    //                 systemLogText.text = string.Join("\n", recentLogs);
    //                 yield return new WaitForSeconds(Random.Range(2f, 5f));
    //             }
    //         }
    //     }

    //     private IEnumerator RandomGlitch(TextMeshProUGUI text)
    //     {
    //         while (true)
    //         {
    //             yield return new WaitForSeconds(Random.Range(5f, 15f));

    //             string original = text.text;
    //             for (int i = 0; i < 3; i++)
    //             {
    //                 text.text = GetGlitchedText(original);
    //                 yield return new WaitForSeconds(0.05f);
    //             }
    //             text.text = original;
    //         }
    //     }

    //     private string GetGlitchedText(string original)
    //     {
    //         char[] glitchChars = { '█', '▓', '▒', '░', '╬', '╣', '║', '╗', '╝', '┐', '└' };
    //         char[] chars = original.ToCharArray();

    //         for (int i = 0; i < chars.Length; i++)
    //         {
    //             if (Random.value < 0.1f)
    //             {
    //                 chars[i] = glitchChars[Random.Range(0, glitchChars.Length)];
    //             }
    //         }

    //         return new string(chars);
    //     }

    //     private IEnumerator GlitchEffect()
    //     {
    //         Color original = terminalPanel.GetComponent<Image>().color;
    //         terminalPanel.GetComponent<Image>().color = new Color(1f, 0f, 0f, 0.1f);
    //         yield return new WaitForSeconds(0.05f);
    //         terminalPanel.GetComponent<Image>().color = original;
    //     }

    //     private IEnumerator PulseBorder(Outline outline)
    //     {
    //         while (true)
    //         {
    //             float alpha = Mathf.PingPong(Time.time * 0.5f, 1f) * 0.5f + 0.3f;
    //             outline.effectColor = new Color(terminalGreen.r, terminalGreen.g, terminalGreen.b, alpha);
    //             yield return null;
    //         }
    //     }

    //     private void AnimateBackground()
    //     {
    //         if (backgroundImage != null && backgroundImage.texture != null)
    //         {
    //             backgroundOffset += Time.deltaTime * backgroundPanSpeed * 0.001f;
    //             backgroundImage.uvRect = new Rect(backgroundOffset, 0, 1, 1);
    //         }
    //     }

    //     private void HandleInput()
    //     {
    //         if (isTyping) return;

    //         bool upPressed = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
    //         bool downPressed = Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);
    //         bool enterPressed = Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space);

    //         if (upPressed)
    //         {
    //             selectedIndex--;
    //             if (selectedIndex < 0) selectedIndex = menuButtons.Count - 1;
    //             UpdateSelection();
    //             PlaySelectSound();
    //         }
    //         else if (downPressed)
    //         {
    //             selectedIndex++;
    //             if (selectedIndex >= menuButtons.Count) selectedIndex = 0;
    //             UpdateSelection();
    //             PlaySelectSound();
    //         }
    //         else if (enterPressed)
    //         {
    //             ExecuteMenuAction(selectedIndex);
    //         }
    //     }

    //     private void UpdateSelection()
    //     {
    //         // Update cursor position
    //         float yPos = 100f - (selectedIndex * 80f);
    //         cursorText.rectTransform.anchoredPosition = new Vector2(-420, yPos);

    //         // Highlight selected item
    //         for (int i = 0; i < menuButtons.Count; i++)
    //         {
    //             if (i == selectedIndex)
    //             {
    //                 menuButtons[i].textComponent.color = terminalHighlight;
    //                 menuButtons[i].textComponent.fontSize = 34;
    //             }
    //             else
    //             {
    //                 menuButtons[i].textComponent.color = terminalGreen;
    //                 menuButtons[i].textComponent.fontSize = 32;
    //             }
    //         }
    //     }

    //     private void ExecuteMenuAction(int index)
    //     {
    //         PlaySelectSound();

    //         switch (index)
    //         {
    //             case 0: // New Game
    //                 StartCoroutine(LoadSceneWithTransition(newGameSceneName));
    //                 break;
    //             case 1: // Resume
    //                 StartCoroutine(LoadSceneWithTransition(resumeSceneName));
    //                 break;
    //             case 2: // Mini-games
    //                 Debug.Log("Mini-games selected - Add your scene here");
    //                 break;
    //             case 3: // Settings
    //                 Debug.Log("Settings selected - Add your settings menu here");
    //                 break;
    //             case 4: // Exit
    //                 StartCoroutine(ExitGame());
    //                 break;
    //         }
    //     }

    //     private IEnumerator LoadSceneWithTransition(string sceneName)
    //     {
    //         // Terminal shutdown effect
    //         titleText.text = "> ESTABLISHING CONNECTION...";
    //         yield return new WaitForSeconds(1f);

    //         SceneManager.LoadScene(sceneName);
    //     }

    //     private IEnumerator ExitGame()
    //     {
    //         titleText.text = "> TERMINATING SESSION...";
    //         yield return new WaitForSeconds(1f);

    //         #if UNITY_EDITOR
    //         UnityEditor.EditorApplication.isPlaying = false;
    //         #else
    //         Application.Quit();
    //         #endif
    //     }

    //     private void PlaySelectSound()
    //     {
    //         if (audioSource != null && selectSound != null)
    //         {
    //             audioSource.PlayOneShot(selectSound, 0.3f);
    //         }
    //     }

    //     private class MenuButton
    //     {
    //         public TextMeshProUGUI textComponent;
    //         public TextMeshProUGUI descriptionText;
    //         public string originalText;
    //         public int index;
    //     }
[Header("🎨 UI References - ASSIGN YOUR UI ELEMENTS")]
    [Tooltip("Title text at the top")]
    [SerializeField] private TextMeshProUGUI titleText;
    
    [Tooltip("System log text (top right)")]
    [SerializeField] private TextMeshProUGUI systemLogText;
    
    [Tooltip("Cursor symbol (blinking █)")]
    [SerializeField] private TextMeshProUGUI cursorText;
    
    [Tooltip("Background image for pan effect")]
    [SerializeField] private RawImage backgroundImage;
    
    [Tooltip("Scanlines object (optional)")]
    [SerializeField] private RectTransform scanlinesRect;
    
    [Tooltip("Border glow outline component")]
    [SerializeField] private Outline borderOutline;
    
    [Tooltip("Terminal panel for glitch effect")]
    [SerializeField] private Image terminalPanelImage;
    
    [Header("📝 Menu Items - ASSIGN IN ORDER")]
    [Tooltip("Drag menu TextMeshPro items here in order: Item0, Item1, Item2, etc.")]
    [SerializeField] private TextMeshProUGUI[] menuItemTexts = new TextMeshProUGUI[5];
    
    [Header("⚙️ Effect Settings")]
    [SerializeField] private float typewriterSpeed = 0.05f;
    [SerializeField] private float cursorBlinkSpeed = 0.5f;
    [SerializeField] private float backgroundPanSpeed = 2f;
    [SerializeField] private Color terminalGreen = new Color(0.2f, 1f, 0.3f);
    [SerializeField] private Color terminalHighlight = new Color(0.4f, 1f, 0.5f);
    
    [Header("🔊 Audio (Optional)")]
    [SerializeField] private AudioClip keyboardTypingSound;
    [SerializeField] private AudioClip glitchSound;
    [SerializeField] private AudioSource audioSource;
    
    [Header("🎮 Scene Configuration")]
    [Tooltip("Scene to load for Menu Item 0 (New Game)")]
    [SerializeField] private string newGameSceneName = "GameScene";
    
    [Tooltip("Scene to load for Menu Item 1 (Continue)")]
    [SerializeField] private string continueSceneName = "GameScene";
    
    [Tooltip("Scene to load for Menu Item 2 (Mini-games)")]
    [SerializeField] private string minigamesSceneName = "PasswordGame";
    
    [Tooltip("Scene to load for Menu Item 3 (Settings)")]
    [SerializeField] private string settingsSceneName = "Settings";
    
    // Private variables
    private string[] menuItemOriginalTexts;
    private float backgroundOffset = 0f;
    private bool isTyping = false;
    private int selectedIndex = 0;
    
    private void Start()
    {
        // Setup audio source if needed
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        
        // Store original menu text
        menuItemOriginalTexts = new string[menuItemTexts.Length];
        for (int i = 0; i < menuItemTexts.Length; i++)
        {
            if (menuItemTexts[i] != null)
            {
                menuItemOriginalTexts[i] = menuItemTexts[i].text;
            }
        }
        
        // Start effects
        StartCoroutine(InitializeEffects());
        
        Debug.Log("✅ Main Menu Effects Initialized!");
        Debug.Log("💡 Use Arrow Keys + Enter to navigate");
    }
    
    private void Update()
    {
        HandleInput();
        AnimateBackground();
        RandomGlitchTrigger();
    }
    
    private IEnumerator InitializeEffects()
    {
        yield return new WaitForSeconds(0.5f);
        
        // Type title if assigned
        if (titleText != null)
        {
            yield return StartCoroutine(TypeText(titleText, titleText.text));
        }
        
        yield return new WaitForSeconds(0.3f);
        
        // Start continuous effects
        StartCoroutine(TypeMenuItemsLoop());
        
        if (cursorText != null)
            StartCoroutine(BlinkCursor());
        
        if (systemLogText != null)
            StartCoroutine(UpdateSystemLog());
        
        if (scanlinesRect != null)
            StartCoroutine(AnimateScanlines());
        
        if (borderOutline != null)
            StartCoroutine(PulseBorder());
        
        if (titleText != null)
            StartCoroutine(RandomGlitch(titleText));
    }
    
    #region Typewriter Effect
    
    private IEnumerator TypeMenuItemsLoop()
    {
        while (true)
        {
            // Type all menu items
            for (int i = 0; i < menuItemTexts.Length; i++)
            {
                if (menuItemTexts[i] != null && !string.IsNullOrEmpty(menuItemOriginalTexts[i]))
                {
                    isTyping = true;
                    yield return StartCoroutine(TypeText(menuItemTexts[i], menuItemOriginalTexts[i]));
                    yield return new WaitForSeconds(0.2f);
                }
            }
            
            isTyping = false;
            yield return new WaitForSeconds(3f);
            
            // Clear all menu text
            foreach (var menuText in menuItemTexts)
            {
                if (menuText != null)
                {
                    menuText.text = "";
                }
            }
            
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    private IEnumerator TypeText(TextMeshProUGUI textComponent, string fullText)
    {
        textComponent.text = "";
        
        foreach (char c in fullText)
        {
            textComponent.text += c;
            
            // Play typing sound occasionally
            if (audioSource != null && keyboardTypingSound != null && Random.value < 0.3f)
            {
                audioSource.PlayOneShot(keyboardTypingSound, 0.1f);
            }
            
            yield return new WaitForSeconds(typewriterSpeed);
        }
    }
    
    #endregion
    
    #region Cursor Effects
    
    private IEnumerator BlinkCursor()
    {
        while (true)
        {
            cursorText.enabled = !cursorText.enabled;
            yield return new WaitForSeconds(cursorBlinkSpeed);
        }
    }
    
    private void UpdateCursorPosition()
    {
        if (cursorText == null) return;
        
        // Calculate Y position based on selected menu item
        // Adjust these values to match your menu layout
        float baseY = 100f;
        float spacing = 80f;
        float yPos = baseY - (selectedIndex * spacing);
        
        RectTransform cursorRect = cursorText.GetComponent<RectTransform>();
        if (cursorRect != null)
        {
            Vector2 currentPos = cursorRect.anchoredPosition;
            cursorRect.anchoredPosition = new Vector2(currentPos.x, yPos);
        }
    }
    
    #endregion
    
    #region System Log
    
    private IEnumerator UpdateSystemLog()
    {
        string[] logs = {
            "[SYSTEM] Connection established...",
            "[INFO] Loading neural interface...",
            "[WARN] Anomaly detected in sector 7",
            "[ERROR] NULL entity signature found",
            "[SYSTEM] Security protocols active",
            "[INFO] Firewall status: ENGAGED",
            "[WARN] Unauthorized access attempt",
            "[SYSTEM] Monitoring data streams...",
            "[INFO] Decrypting encrypted files...",
            "[WARN] Suspicious activity detected",
            "[SYSTEM] Backup systems online"
        };
        
        List<string> recentLogs = new List<string>();
        
        while (true)
        {
            string log = $"[{System.DateTime.Now:HH:mm:ss}] {logs[Random.Range(0, logs.Length)]}";
            recentLogs.Insert(0, log);
            
            if (recentLogs.Count > 5)
                recentLogs.RemoveAt(recentLogs.Count - 1);
            
            systemLogText.text = string.Join("\n", recentLogs);
            
            yield return new WaitForSeconds(Random.Range(2f, 5f));
        }
    }
    
    #endregion
    
    #region Visual Effects
    
    private IEnumerator AnimateScanlines()
    {
        float offset = 0f;
        while (true)
        {
            offset += Time.deltaTime * 50f;
            if (offset > 1080f) offset = 0f;
            scanlinesRect.anchoredPosition = new Vector2(0, offset);
            yield return null;
        }
    }
    
    private IEnumerator PulseBorder()
    {
        while (true)
        {
            float alpha = Mathf.PingPong(Time.time * 0.5f, 1f) * 0.5f + 0.3f;
            borderOutline.effectColor = new Color(terminalGreen.r, terminalGreen.g, terminalGreen.b, alpha);
            yield return null;
        }
    }
    
    private void AnimateBackground()
    {
        if (backgroundImage != null && backgroundImage.texture != null)
        {
            backgroundOffset += Time.deltaTime * backgroundPanSpeed * 0.001f;
            backgroundImage.uvRect = new Rect(backgroundOffset, 0, 1, 1);
        }
    }
    
    #endregion
    
    #region Glitch Effects
    
    private void RandomGlitchTrigger()
    {
        if (Random.value < 0.001f && audioSource != null && glitchSound != null)
        {
            audioSource.PlayOneShot(glitchSound, 0.1f);
            StartCoroutine(GlitchEffect());
        }
    }
    
    private IEnumerator RandomGlitch(TextMeshProUGUI text)
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(5f, 15f));
            
            string original = text.text;
            
            // Glitch 3 times rapidly
            for (int i = 0; i < 3; i++)
            {
                text.text = GetGlitchedText(original);
                yield return new WaitForSeconds(0.05f);
            }
            
            text.text = original;
        }
    }
    
    private string GetGlitchedText(string original)
    {
        char[] glitchChars = { '█', '▓', '▒', '░', '╬', '╣', '║', '╗', '╝', '┐', '└', '≡', '∞', '¤' };
        char[] chars = original.ToCharArray();
        
        for (int i = 0; i < chars.Length; i++)
        {
            if (Random.value < 0.1f)
            {
                chars[i] = glitchChars[Random.Range(0, glitchChars.Length)];
            }
        }
        
        return new string(chars);
    }
    
    private IEnumerator GlitchEffect()
    {
        if (terminalPanelImage == null) yield break;
        
        Color original = terminalPanelImage.color;
        terminalPanelImage.color = new Color(1f, 0f, 0f, 0.1f);
        yield return new WaitForSeconds(0.05f);
        terminalPanelImage.color = original;
    }
    
    #endregion
    
    #region Input & Navigation
    
    private void HandleInput()
    {
        if (isTyping) return;
        
        bool upPressed = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
        bool downPressed = Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S);
        bool enterPressed = Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space);
        
        if (upPressed)
        {
            selectedIndex--;
            if (selectedIndex < 0) selectedIndex = menuItemTexts.Length - 1;
            UpdateSelection();
        }
        else if (downPressed)
        {
            selectedIndex++;
            if (selectedIndex >= menuItemTexts.Length) selectedIndex = 0;
            UpdateSelection();
        }
        else if (enterPressed)
        {
            ExecuteMenuAction(selectedIndex);
        }
    }
    
    private void UpdateSelection()
    {
        UpdateCursorPosition();
        
        // Highlight selected item
        for (int i = 0; i < menuItemTexts.Length; i++)
        {
            if (menuItemTexts[i] != null)
            {
                if (i == selectedIndex)
                {
                    menuItemTexts[i].color = terminalHighlight;
                    menuItemTexts[i].fontSize = menuItemTexts[i].fontSize + 2;
                }
                else
                {
                    menuItemTexts[i].color = terminalGreen;
                    menuItemTexts[i].fontSize = menuItemTexts[i].fontSize - 2;
                }
            }
        }
        
        // Play select sound
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }
    
    #endregion
    
    #region Menu Actions
    
    private void ExecuteMenuAction(int index)
    {
        string sceneName = "";
        
        switch (index)
        {
            case 0: // New Game
                sceneName = newGameSceneName;
                break;
            case 1: // Continue
                sceneName = continueSceneName;
                break;
            case 2: // Mini-games
                sceneName = minigamesSceneName;
                break;
            case 3: // Settings
                sceneName = settingsSceneName;
                break;
            case 4: // Exit
                StartCoroutine(ExitGame());
                return;
        }
        
        if (!string.IsNullOrEmpty(sceneName))
        {
            StartCoroutine(LoadSceneWithTransition(sceneName));
        }
        else
        {
            Debug.LogWarning($"⚠️ No scene configured for menu index {index}");
        }
    }
    
    private IEnumerator LoadSceneWithTransition(string sceneName)
    {
        if (titleText != null)
        {
            titleText.text = "> ESTABLISHING CONNECTION...";
        }
        
        yield return new WaitForSeconds(1f);
        
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"❌ Scene '{sceneName}' not found! Add it to Build Settings.");
            if (titleText != null)
            {
                titleText.text = $"> ERROR: Scene not found";
            }
        }
    }
    
    private IEnumerator ExitGame()
    {
        if (titleText != null)
        {
            titleText.text = "> TERMINATING SESSION...";
        }
        
        yield return new WaitForSeconds(1f);
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    #endregion
    
    #region Public Methods (Optional - for buttons)
    
    /// <summary>
    /// Call this from UI Button onClick event
    /// </summary>
    public void OnNewGameClicked()
    {
        ExecuteMenuAction(0);
    }
    
    /// <summary>
    /// Call this from UI Button onClick event
    /// </summary>
    public void OnContinueClicked()
    {
        ExecuteMenuAction(1);
    }
    
    /// <summary>
    /// Call this from UI Button onClick event
    /// </summary>
    public void OnMinigamesClicked()
    {
        ExecuteMenuAction(2);
    }
    
    /// <summary>
    /// Call this from UI Button onClick event
    /// </summary>
    public void OnSettingsClicked()
    {
        ExecuteMenuAction(3);
    }
    
    /// <summary>
    /// Call this from UI Button onClick event
    /// </summary>
    public void OnExitClicked()
    {
        ExecuteMenuAction(4);
    }
    
    #endregion
}

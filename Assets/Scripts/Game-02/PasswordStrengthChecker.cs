using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using static System.Math;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Text;
public class PasswordStrengthChecker : MonoBehaviour
{
   [Header("Avatar Names (Customize Here!)")]
    [SerializeField] private string[] avatarNames = { "Alice", "Bob", "Charlie", "Diana", "Eve" };
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI passwordDisplayText;
    [SerializeField] private TextMeshProUGUI strengthText;
    [SerializeField] private TextMeshProUGUI hintsText;
    [SerializeField] private TextMeshProUGUI characterCountText;
    [SerializeField] private TextMeshProUGUI crackTimeText;
    [SerializeField] private TextMeshProUGUI currentAccountNameText;
    
    [Header("Visual Feedback")]
    [SerializeField] private UnityEngine.UI.Image strengthBar;
    [SerializeField] private Color weakColor = new Color(1f, 0.2f, 0.2f);
    [SerializeField] private Color mediumColor = new Color(1f, 0.8f, 0f);
    [SerializeField] private Color strongColor = new Color(0.2f, 1f, 0.2f);
    [SerializeField] private Color veryStrongColor = new Color(0f, 0.8f, 0f);
    
    [Header("Password Display Settings")]
    [SerializeField] private bool showPassword = false;
    [SerializeField] private UnityEngine.UI.Button toggleVisibilityButton;
    [SerializeField] private UnityEngine.UI.Button setPasswordButton;
    
    [Header("Panel References")]
    [SerializeField] private GameObject avatarSelectionPanel;
    [SerializeField] private GameObject passwordCheckerPanel;
    
    private StringBuilder currentPassword = new StringBuilder();
    private const int MAX_PASSWORD_LENGTH = 128;
    private bool useNewInputSystem = false;
    
    private int currentAccountIndex = -1;
    private Dictionary<int, string> accountPasswords = new Dictionary<int, string>();
    
    // Crack time calculation constants
    private const long ATTEMPTS_PER_SECOND = 1000000000; // 1 billion attempts per second (modern GPU)
    
    private void Start()
    {
        DetectInputSystem();
        ShowAvatarSelection();
        
        // Setup set password button
        if (setPasswordButton != null)
        {
            setPasswordButton.onClick.AddListener(OnSetPasswordClicked);
        }
    }
    
    private void DetectInputSystem()
    {
        #if ENABLE_INPUT_SYSTEM
        useNewInputSystem = true;
        Debug.Log("✅ Using New Input System (Unity 6)");
        #else
        useNewInputSystem = false;
        Debug.Log("✅ Using Legacy Input Manager");
        #endif
    }
    
    private void Update()
    {
        if (passwordCheckerPanel != null && passwordCheckerPanel.activeSelf)
        {
            HandleKeyboardInput();
        }
    }
    
    public void ShowAvatarSelection()
    {
        if (avatarSelectionPanel != null)
            avatarSelectionPanel.SetActive(true);
        
        if (passwordCheckerPanel != null)
            passwordCheckerPanel.SetActive(false);
    }
    
    public void SelectAccount(int accountIndex)
    {
        currentAccountIndex = accountIndex;
        currentPassword.Clear();
        
        if (avatarSelectionPanel != null)
            avatarSelectionPanel.SetActive(false);
        
        if (passwordCheckerPanel != null)
            passwordCheckerPanel.SetActive(true);
        
        InitializeUI();
        UpdateDisplay();
        
        // Update account name display with custom names
        if (currentAccountNameText != null && accountIndex >= 0 && accountIndex < avatarNames.Length)
        {
            currentAccountNameText.text = $"Setting password for: <b>{avatarNames[accountIndex]}</b>";
        }
    }
    
    public string[] GetAvatarNames()
    {
        return avatarNames;
    }
    
    private void OnSetPasswordClicked()
    {
        if (currentPassword.Length == 0)
        {
            Debug.Log("Cannot set empty password!");
            return;
        }
        
        // Save the password for this account
        accountPasswords[currentAccountIndex] = currentPassword.ToString();
        
        Debug.Log($"✅ Password set for account {currentAccountIndex}!");
        
        // Show confirmation animation
        if (setPasswordButton != null)
        {
            TextMeshProUGUI btnText = setPasswordButton.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null)
            {
                btnText.text = "✅ Password Set!";
            }
        }
        
        // Return to avatar selection after short delay
        StartCoroutine(ReturnToAvatarSelection());
    }
    
    private IEnumerator ReturnToAvatarSelection()
    {
        yield return new WaitForSeconds(1.5f);
        
        // Reset button text
        if (setPasswordButton != null)
        {
            TextMeshProUGUI btnText = setPasswordButton.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null)
            {
                btnText.text = "Set Password";
            }
        }
        
        ShowAvatarSelection();
    }
    
    public bool IsPasswordSet(int accountIndex)
    {
        return accountPasswords.ContainsKey(accountIndex);
    }
    
    private void InitializeUI()
    {
        if (passwordDisplayText != null)
            passwordDisplayText.text = "Start typing your password...";
        
        if (characterCountText != null)
            characterCountText.text = "Characters: 0";
        
        if (crackTimeText != null)
            crackTimeText.text = "";
    }
    
    public void TogglePasswordVisibility()
    {
        showPassword = !showPassword;
        UpdateDisplay();
        
        if (toggleVisibilityButton != null)
        {
            TextMeshProUGUI buttonText = toggleVisibilityButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = showPassword ? "🙈 Hide" : "👁️ Show";
            }
        }
        
        Debug.Log($"Password visibility: {(showPassword ? "VISIBLE" : "HIDDEN")}");
    }
    
    private void HandleKeyboardInput()
    {
        #if ENABLE_INPUT_SYSTEM
        HandleNewInputSystem();
        #else
        HandleLegacyInputSystem();
        #endif
    }
    
    #if ENABLE_INPUT_SYSTEM
    private void HandleNewInputSystem()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;
        
        if (keyboard.backspaceKey.wasPressedThisFrame)
        {
            if (currentPassword.Length > 0)
            {
                currentPassword.Length--;
                UpdateDisplay();
            }
            return;
        }
        
        string textInput = GetNewInputSystemCharacters(keyboard);
        
        foreach (char c in textInput)
        {
            if (!char.IsControl(c) && currentPassword.Length < MAX_PASSWORD_LENGTH)
            {
                currentPassword.Append(c);
                UpdateDisplay();
            }
        }
    }
    
    private string GetNewInputSystemCharacters(Keyboard keyboard)
    {
        StringBuilder chars = new StringBuilder();
        
        if (keyboard.aKey.wasPressedThisFrame) chars.Append(keyboard.shiftKey.isPressed ? 'A' : 'a');
        if (keyboard.bKey.wasPressedThisFrame) chars.Append(keyboard.shiftKey.isPressed ? 'B' : 'b');
        if (keyboard.cKey.wasPressedThisFrame) chars.Append(keyboard.shiftKey.isPressed ? 'C' : 'c');
        if (keyboard.dKey.wasPressedThisFrame) chars.Append(keyboard.shiftKey.isPressed ? 'D' : 'd');
        if (keyboard.eKey.wasPressedThisFrame) chars.Append(keyboard.shiftKey.isPressed ? 'E' : 'e');
        if (keyboard.fKey.wasPressedThisFrame) chars.Append(keyboard.shiftKey.isPressed ? 'F' : 'f');
        if (keyboard.gKey.wasPressedThisFrame) chars.Append(keyboard.shiftKey.isPressed ? 'G' : 'g');
        if (keyboard.hKey.wasPressedThisFrame) chars.Append(keyboard.shiftKey.isPressed ? 'H' : 'h');
        if (keyboard.iKey.wasPressedThisFrame) chars.Append(keyboard.shiftKey.isPressed ? 'I' : 'i');
        if (keyboard.jKey.wasPressedThisFrame) chars.Append(keyboard.shiftKey.isPressed ? 'J' : 'j');
        if (keyboard.kKey.wasPressedThisFrame) chars.Append(keyboard.shiftKey.isPressed ? 'K' : 'k');
        if (keyboard.lKey.wasPressedThisFrame) chars.Append(keyboard.shiftKey.isPressed ? 'L' : 'l');
        if (keyboard.mKey.wasPressedThisFrame) chars.Append(keyboard.shiftKey.isPressed ? 'M' : 'm');
        if (keyboard.nKey.wasPressedThisFrame) chars.Append(keyboard.shiftKey.isPressed ? 'N' : 'n');
        if (keyboard.oKey.wasPressedThisFrame) chars.Append(keyboard.shiftKey.isPressed ? 'O' : 'o');
        if (keyboard.pKey.wasPressedThisFrame) chars.Append(keyboard.shiftKey.isPressed ? 'P' : 'p');
        if (keyboard.qKey.wasPressedThisFrame) chars.Append(keyboard.shiftKey.isPressed ? 'Q' : 'q');
        if (keyboard.rKey.wasPressedThisFrame) chars.Append(keyboard.shiftKey.isPressed ? 'R' : 'r');
        if (keyboard.sKey.wasPressedThisFrame) chars.Append(keyboard.shiftKey.isPressed ? 'S' : 's');
        if (keyboard.tKey.wasPressedThisFrame) chars.Append(keyboard.shiftKey.isPressed ? 'T' : 't');
        if (keyboard.uKey.wasPressedThisFrame) chars.Append(keyboard.shiftKey.isPressed ? 'U' : 'u');
        if (keyboard.vKey.wasPressedThisFrame) chars.Append(keyboard.shiftKey.isPressed ? 'V' : 'v');
        if (keyboard.wKey.wasPressedThisFrame) chars.Append(keyboard.shiftKey.isPressed ? 'W' : 'w');
        if (keyboard.xKey.wasPressedThisFrame) chars.Append(keyboard.shiftKey.isPressed ? 'X' : 'x');
        if (keyboard.yKey.wasPressedThisFrame) chars.Append(keyboard.shiftKey.isPressed ? 'Y' : 'y');
        if (keyboard.zKey.wasPressedThisFrame) chars.Append(keyboard.shiftKey.isPressed ? 'Z' : 'z');
        
        bool shift = keyboard.shiftKey.isPressed;
        if (keyboard.digit1Key.wasPressedThisFrame) chars.Append(shift ? '!' : '1');
        if (keyboard.digit2Key.wasPressedThisFrame) chars.Append(shift ? '@' : '2');
        if (keyboard.digit3Key.wasPressedThisFrame) chars.Append(shift ? '#' : '3');
        if (keyboard.digit4Key.wasPressedThisFrame) chars.Append(shift ? '$' : '4');
        if (keyboard.digit5Key.wasPressedThisFrame) chars.Append(shift ? '%' : '5');
        if (keyboard.digit6Key.wasPressedThisFrame) chars.Append(shift ? '^' : '6');
        if (keyboard.digit7Key.wasPressedThisFrame) chars.Append(shift ? '&' : '7');
        if (keyboard.digit8Key.wasPressedThisFrame) chars.Append(shift ? '*' : '8');
        if (keyboard.digit9Key.wasPressedThisFrame) chars.Append(shift ? '(' : '9');
        if (keyboard.digit0Key.wasPressedThisFrame) chars.Append(shift ? ')' : '0');
        
        if (keyboard.minusKey.wasPressedThisFrame) chars.Append(shift ? '_' : '-');
        if (keyboard.equalsKey.wasPressedThisFrame) chars.Append(shift ? '+' : '=');
        if (keyboard.leftBracketKey.wasPressedThisFrame) chars.Append(shift ? '{' : '[');
        if (keyboard.rightBracketKey.wasPressedThisFrame) chars.Append(shift ? '}' : ']');
        if (keyboard.backslashKey.wasPressedThisFrame) chars.Append(shift ? '|' : '\\');
        if (keyboard.semicolonKey.wasPressedThisFrame) chars.Append(shift ? ':' : ';');
        if (keyboard.quoteKey.wasPressedThisFrame) chars.Append(shift ? '"' : '\'');
        if (keyboard.commaKey.wasPressedThisFrame) chars.Append(shift ? '<' : ',');
        if (keyboard.periodKey.wasPressedThisFrame) chars.Append(shift ? '>' : '.');
        if (keyboard.slashKey.wasPressedThisFrame) chars.Append(shift ? '?' : '/');
        if (keyboard.spaceKey.wasPressedThisFrame) chars.Append(' ');
        
        return chars.ToString();
    }
    #endif
    
    private void HandleLegacyInputSystem()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (currentPassword.Length > 0)
            {
                currentPassword.Length--;
                UpdateDisplay();
            }
            return;
        }
        
        string input = Input.inputString;
        foreach (char c in input)
        {
            if (!char.IsControl(c) && currentPassword.Length < MAX_PASSWORD_LENGTH)
            {
                currentPassword.Append(c);
                UpdateDisplay();
            }
        }
    }
    
    private void UpdateDisplay()
    {
        string password = currentPassword.ToString();
        
        if (passwordDisplayText != null)
        {
            if (password.Length == 0)
                passwordDisplayText.text = "Start typing your password...";
            else
                passwordDisplayText.text = showPassword ? password : new string('●', password.Length);
        }
        
        if (characterCountText != null)
            characterCountText.text = $"Characters: {password.Length}";
        
        PasswordStrength strength = CalculateStrength(password);
        UpdateStrengthDisplay(strength);
        
        if (crackTimeText != null)
        {
            string crackTime = CalculateCrackTime(password, strength);
            crackTimeText.text = crackTime;
        }
        
        string hints = GenerateHints(password, strength);
        if (hintsText != null)
            hintsText.text = hints;
    }
    
    private string CalculateCrackTime(string password, PasswordStrength strength)
    {
        if (password.Length == 0)
            return "";
        
        int poolSize = 0;
        if (strength.HasLower) poolSize += 26;
        if (strength.HasUpper) poolSize += 26;
        if (strength.HasDigit) poolSize += 10;
        if (strength.HasSpecial) poolSize += 32;
        
        if (poolSize == 0) poolSize = 26;
        
        double combinations = Math.Pow(poolSize, password.Length);
        double secondsToCrack = combinations / ATTEMPTS_PER_SECOND;
        
        string timeString = FormatCrackTime(secondsToCrack);
        string emoji = GetCrackTimeEmoji(secondsToCrack);
        
        return $"{emoji} <b>Time to Crack:</b> {timeString}\n<size=14><color=#888888>(At 1 billion guesses/second)</color></size>";
    }
    
    private string FormatCrackTime(double seconds)
    {
        if (seconds < 1)
            return "<color=red>Instantly</color>";
        
        if (seconds < 60)
            return $"<color=red>{seconds:F1} seconds</color>";
        
        double minutes = seconds / 60;
        if (minutes < 60)
            return $"<color=orange>{minutes:F1} minutes</color>";
        
        double hours = minutes / 60;
        if (hours < 24)
            return $"<color=yellow>{hours:F1} hours</color>";
        
        double days = hours / 24;
        if (days < 30)
            return $"<color=yellow>{days:F1} days</color>";
        
        double months = days / 30;
        if (months < 12)
            return $"<color=#90EE90>{months:F1} months</color>";
        
        double years = days / 365;
        if (years < 1000)
            return $"<color=green>{years:F1} years</color>";
        
        if (years < 1000000)
        {
            double thousands = years / 1000;
            return $"<color=green>{thousands:F1} thousand years</color>";
        }
        
        if (years < 1000000000)
        {
            double millions = years / 1000000;
            return $"<color=green>{millions:F1} million years</color>";
        }
        
        double billions = years / 1000000000;
        return $"<color=green>{billions:F1} billion years</color>";
    }
    
    private string GetCrackTimeEmoji(double seconds)
    {
        if (seconds < 60) return "💀";
        if (seconds < 3600) return "⚠️";
        if (seconds < 86400) return "⏰";
        if (seconds < 2592000) return "📅";
        if (seconds < 31536000) return "🗓️";
        return "🔒";
    }
    
    private PasswordStrength CalculateStrength(string password)
    {
        if (password.Length == 0)
            return new PasswordStrength { Score = 0, Level = "None" };
        
        int score = 0;
        bool hasLower = false, hasUpper = false, hasDigit = false, hasSpecial = false;
        
        foreach (char c in password)
        {
            if (char.IsLower(c)) hasLower = true;
            else if (char.IsUpper(c)) hasUpper = true;
            else if (char.IsDigit(c)) hasDigit = true;
            else if (!char.IsLetterOrDigit(c)) hasSpecial = true;
        }
        
        if (password.Length >= 8) score += 20;
        if (password.Length >= 12) score += 15;
        if (password.Length >= 16) score += 15;
        
        if (hasLower) score += 10;
        if (hasUpper) score += 15;
        if (hasDigit) score += 15;
        if (hasSpecial) score += 20;
        
        if (hasLower && hasUpper && hasDigit && hasSpecial)
            score += 10;
        
        string level;
        if (score < 30) level = "Very Weak";
        else if (score < 50) level = "Weak";
        else if (score < 70) level = "Medium";
        else if (score < 85) level = "Strong";
        else level = "Very Strong";
        
        return new PasswordStrength 
        { 
            Score = score, 
            Level = level,
            HasLower = hasLower,
            HasUpper = hasUpper,
            HasDigit = hasDigit,
            HasSpecial = hasSpecial,
            Length = password.Length
        };
    }
    
    private void UpdateStrengthDisplay(PasswordStrength strength)
    {
        if (strengthText != null)
        {
            strengthText.text = $"Strength: {strength.Level}\nScore: {strength.Score}/100";
            
            Color textColor = weakColor;
            switch (strength.Level)
            {
                case "Very Weak": textColor = weakColor; break;
                case "Weak": textColor = new Color(1f, 0.5f, 0f); break;
                case "Medium": textColor = mediumColor; break;
                case "Strong": textColor = strongColor; break;
                case "Very Strong": textColor = veryStrongColor; break;
            }
            strengthText.color = textColor;
        }
        
        if (strengthBar != null)
        {
            strengthBar.fillAmount = strength.Score / 100f;
            strengthBar.color = strengthText != null ? strengthText.color : weakColor;
        }
    }
    
    private string GenerateHints(string password, PasswordStrength strength)
    {
        if (password.Length == 0)
            return "💡 <b>Tips for a Strong Password:</b>\n" +
                   "• Use at least 12 characters\n" +
                   "• Mix uppercase and lowercase letters\n" +
                   "• Include numbers and special characters\n" +
                   "• Avoid common words and patterns";
        
        StringBuilder hints = new StringBuilder();
        hints.AppendLine("<b>💡 Improvement Suggestions:</b>");
        
        bool needsImprovement = false;
        
        if (strength.Length < 8)
        {
            hints.AppendLine($"❌ Add {8 - strength.Length} more characters (minimum 8)");
            needsImprovement = true;
        }
        else if (strength.Length < 12)
        {
            hints.AppendLine($"⚠️ Add {12 - strength.Length} more characters (recommended 12+)");
            needsImprovement = true;
        }
        else
        {
            hints.AppendLine("✓ Length is good");
        }
        
        if (!strength.HasLower)
        {
            hints.AppendLine("❌ Add lowercase letters (a-z)");
            needsImprovement = true;
        }
        else
        {
            hints.AppendLine("✓ Has lowercase letters");
        }
        
        if (!strength.HasUpper)
        {
            hints.AppendLine("❌ Add uppercase letters (A-Z)");
            needsImprovement = true;
        }
        else
        {
            hints.AppendLine("✓ Has uppercase letters");
        }
        
        if (!strength.HasDigit)
        {
            hints.AppendLine("❌ Add numbers (0-9)");
            needsImprovement = true;
        }
        else
        {
            hints.AppendLine("✓ Has numbers");
        }
        
        if (!strength.HasSpecial)
        {
            hints.AppendLine("❌ Add special characters (!@#$%^&*)");
            needsImprovement = true;
        }
        else
        {
            hints.AppendLine("✓ Has special characters");
        }
        
        if (!needsImprovement && strength.Score >= 85)
        {
            hints.AppendLine("\n<color=green>🎉 Excellent password!</color>");
        }
        
        return hints.ToString();
    }
    
    private struct PasswordStrength
    {
        public int Score;
        public string Level;
        public bool HasLower;
        public bool HasUpper;
        public bool HasDigit;
        public bool HasSpecial;
        public int Length;
    }
}
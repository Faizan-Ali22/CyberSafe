using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Text;

/// <summary>
/// Password Strength Checker - Unity 6 + Android Compatible
/// Professional 175 IQ implementation with mobile keyboard support
/// </summary>
public class PasswordStrengthChecker : MonoBehaviour
{
    [Header("Avatar Names (Customize Here!)")]
    [SerializeField] private string[] avatarNames = { "Alice", "Bob", "Charlie", "Diana", "Eve" };
    
    [Header("Avatar Images (Optional - Drag Images Here!)")]
    [Tooltip("Leave empty to use default emojis. Drag Sprite/Texture2D for custom images.")]
    [SerializeField] private Sprite[] avatarImages = new Sprite[5];
    
    [Header("UI References - AUTO-ASSIGNED")]
    [SerializeField] private TextMeshProUGUI passwordDisplayText;
    [SerializeField] private TextMeshProUGUI strengthText;
    [SerializeField] private TextMeshProUGUI hintsText;
    [SerializeField] private TextMeshProUGUI characterCountText;
    [SerializeField] private TextMeshProUGUI crackTimeText;
    [SerializeField] private TextMeshProUGUI currentAccountNameText;
    [SerializeField] private TMP_InputField passwordInputField;
    
    [Header("Visual Feedback")]
    [SerializeField] private Image strengthBar;
    [SerializeField] private Color weakColor = new Color(1f, 0.2f, 0.2f);
    [SerializeField] private Color mediumColor = new Color(1f, 0.8f, 0f);
    [SerializeField] private Color strongColor = new Color(0.2f, 1f, 0.2f);
    [SerializeField] private Color veryStrongColor = new Color(0f, 0.8f, 0f);
    
    [Header("Password Display Settings")]
    [SerializeField] private bool showPassword = false;
    [SerializeField] private Button toggleVisibilityButton;
    [SerializeField] private Button setPasswordButton;
    
    [Header("Panel References")]
    [SerializeField] private GameObject avatarSelectionPanel;
    [SerializeField] private GameObject passwordCheckerPanel;
    
    private StringBuilder currentPassword = new StringBuilder();
    private const int MAX_PASSWORD_LENGTH = 128;
    private int currentAccountIndex = -1;
    private Dictionary<int, string> accountPasswords = new Dictionary<int, string>();
    private const long ATTEMPTS_PER_SECOND = 1000000000;
    
    private void Start()
    {
        ShowAvatarSelection();
        SetupButtons();
        SetupInputField();
        
        Debug.Log("✅ Password Checker Ready - Android Compatible!");
    }
    
    private void SetupButtons()
    {
        if (setPasswordButton != null)
        {
            setPasswordButton.onClick.AddListener(OnSetPasswordClicked);
        }
        
        if (toggleVisibilityButton != null)
        {
            toggleVisibilityButton.onClick.AddListener(TogglePasswordVisibility);
        }
    }
    
    private void SetupInputField()
    {
        if (passwordInputField != null)
        {
            // Configure for Android keyboard
            passwordInputField.contentType = TMP_InputField.ContentType.Standard;
            passwordInputField.inputType = TMP_InputField.InputType.Standard;
            passwordInputField.keyboardType = TouchScreenKeyboardType.Default;
            passwordInputField.characterLimit = MAX_PASSWORD_LENGTH;
            
            // Listen to input changes
            passwordInputField.onValueChanged.AddListener(OnPasswordInputChanged);
            passwordInputField.onEndEdit.AddListener(OnPasswordInputEnd);
            
            Debug.Log("✅ InputField configured for Android keyboard");
        }
    }
    
    /// <summary>
    /// Called by PasswordUISetup to assign UI references
    /// </summary>
    public void AssignUIReferences(
        TextMeshProUGUI passwordDisplay,
        TextMeshProUGUI strength,
        TextMeshProUGUI hints,
        TextMeshProUGUI charCount,
        TextMeshProUGUI crackTime,
        TextMeshProUGUI accountName,
        Image bar,
        Button toggleBtn,
        Button setBtn,
        GameObject avatarPanel,
        GameObject passwordPanel,
        TMP_InputField inputField)
    {
        passwordDisplayText = passwordDisplay;
        strengthText = strength;
        hintsText = hints;
        characterCountText = charCount;
        crackTimeText = crackTime;
        currentAccountNameText = accountName;
        strengthBar = bar;
        toggleVisibilityButton = toggleBtn;
        setPasswordButton = setBtn;
        avatarSelectionPanel = avatarPanel;
        passwordCheckerPanel = passwordPanel;
        passwordInputField = inputField;
        
        Debug.Log("✅ All UI references assigned successfully!");
    }
    
    private void OnPasswordInputChanged(string input)
    {
        // Update password from input field
        currentPassword.Clear();
        currentPassword.Append(input);
        UpdateDisplay();
    }
    
    private void OnPasswordInputEnd(string input)
    {
        // Final update when editing ends
        currentPassword.Clear();
        currentPassword.Append(input);
        UpdateDisplay();
    }
    
    public void ShowAvatarSelection()
    {
        if (avatarSelectionPanel != null)
            avatarSelectionPanel.SetActive(true);
        
        if (passwordCheckerPanel != null)
            passwordCheckerPanel.SetActive(false);
        
        // Clear input field
        if (passwordInputField != null)
        {
            passwordInputField.text = "";
        }
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
        
        // Focus input field for Android keyboard
        if (passwordInputField != null)
        {
            StartCoroutine(FocusInputFieldDelayed());
        }
        
        if (currentAccountNameText != null && accountIndex >= 0 && accountIndex < avatarNames.Length)
        {
            currentAccountNameText.text = $"Setting password for: <b>{avatarNames[accountIndex]}</b>";
        }
    }
    
    private IEnumerator FocusInputFieldDelayed()
    {
        yield return new WaitForSeconds(0.1f);
        passwordInputField.ActivateInputField();
        passwordInputField.Select();
    }
    
    public string[] GetAvatarNames()
    {
        return avatarNames;
    }
    
    public Sprite[] GetAvatarImages()
    {
        return avatarImages;
    }
    
    private void OnSetPasswordClicked()
    {
        if (currentPassword.Length == 0)
        {
            Debug.Log("❌ Cannot set empty password!");
            return;
        }
        
        accountPasswords[currentAccountIndex] = currentPassword.ToString();
        
        Debug.Log($"✅ Password set for account {currentAccountIndex}: {avatarNames[currentAccountIndex]}");
        
        if (setPasswordButton != null)
        {
            TextMeshProUGUI btnText = setPasswordButton.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null)
            {
                btnText.text = "✅ Password Set!";
            }
        }
        
        StartCoroutine(ReturnToAvatarSelection());
    }
    
    private IEnumerator ReturnToAvatarSelection()
    {
        yield return new WaitForSeconds(1.5f);
        
        if (setPasswordButton != null)
        {
            TextMeshProUGUI btnText = setPasswordButton.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null)
            {
                btnText.text = "✓ Set Password";
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
        
        if (passwordInputField != null)
        {
            passwordInputField.text = "";
        }
    }
    
    public void TogglePasswordVisibility()
    {
        showPassword = !showPassword;
        
        if (passwordInputField != null)
        {
            passwordInputField.contentType = showPassword ? 
                TMP_InputField.ContentType.Standard : 
                TMP_InputField.ContentType.Password;
            passwordInputField.ForceLabelUpdate();
        }
        
        UpdateDisplay();
        
        if (toggleVisibilityButton != null)
        {
            TextMeshProUGUI buttonText = toggleVisibilityButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = showPassword ? "🙈 Hide" : "👁️ Show";
            }
        }
        
        Debug.Log($"👁️ Password visibility: {(showPassword ? "VISIBLE" : "HIDDEN")}");
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
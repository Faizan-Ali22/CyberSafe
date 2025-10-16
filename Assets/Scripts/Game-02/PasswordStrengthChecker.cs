using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Text;
public class PasswordStrengthChecker : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI passwordDisplayText;
    [SerializeField] private TextMeshProUGUI strengthText;
    [SerializeField] private TextMeshProUGUI hintsText;
    [SerializeField] private TextMeshProUGUI characterCountText;
    
    [Header("Visual Feedback")]
    [SerializeField] private UnityEngine.UI.Image strengthBar;
    [SerializeField] private Color weakColor = new Color(1f, 0.2f, 0.2f);
    [SerializeField] private Color mediumColor = new Color(1f, 0.8f, 0f);
    [SerializeField] private Color strongColor = new Color(0.2f, 1f, 0.2f);
    [SerializeField] private Color veryStrongColor = new Color(0f, 0.8f, 0f);
    
    private StringBuilder currentPassword = new StringBuilder();
    private const int MAX_PASSWORD_LENGTH = 128;
    private bool useNewInputSystem = false;
    
    private void Start()
    {
        InitializeUI();
        UpdateDisplay();
        DetectInputSystem();
    }
    
    private void DetectInputSystem()
    {
        // Check if new Input System is available
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
        HandleKeyboardInput();
    }
    
    private void InitializeUI()
    {
        if (passwordDisplayText != null)
            passwordDisplayText.text = "Start typing your password...";
        
        if (characterCountText != null)
            characterCountText.text = "Characters: 0";
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
        
        // Handle backspace with new Input System
        if (keyboard.backspaceKey.wasPressedThisFrame)
        {
            if (currentPassword.Length > 0)
            {
                currentPassword.Length--;
                UpdateDisplay();
            }
            return;
        }
        
        // Handle text input with new Input System
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
        
        // Check letter keys
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
        
        // Numbers
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
        
        // Special characters
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
        // Handle backspace with legacy Input
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (currentPassword.Length > 0)
            {
                currentPassword.Length--;
                UpdateDisplay();
            }
            return;
        }
        
        // Handle character input with legacy Input
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
        
        // Update masked password display
        if (passwordDisplayText != null)
        {
            if (password.Length == 0)
                passwordDisplayText.text = "Start typing your password...";
            else
                passwordDisplayText.text = new string('●', password.Length);
        }
        
        // Update character count
        if (characterCountText != null)
            characterCountText.text = $"Characters: {password.Length}";
        
        // Calculate and display strength
        PasswordStrength strength = CalculateStrength(password);
        UpdateStrengthDisplay(strength);
        
        // Generate and display hints
        string hints = GenerateHints(password, strength);
        if (hintsText != null)
            hintsText.text = hints;
    }
    
    private PasswordStrength CalculateStrength(string password)
    {
        if (password.Length == 0)
            return new PasswordStrength { Score = 0, Level = "None" };
        
        int score = 0;
        bool hasLower = false, hasUpper = false, hasDigit = false, hasSpecial = false;
        
        // Analyze characters
        foreach (char c in password)
        {
            if (char.IsLower(c)) hasLower = true;
            else if (char.IsUpper(c)) hasUpper = true;
            else if (char.IsDigit(c)) hasDigit = true;
            else if (!char.IsLetterOrDigit(c)) hasSpecial = true;
        }
        
        // Length scoring
        if (password.Length >= 8) score += 20;
        if (password.Length >= 12) score += 15;
        if (password.Length >= 16) score += 15;
        
        // Complexity scoring
        if (hasLower) score += 10;
        if (hasUpper) score += 15;
        if (hasDigit) score += 15;
        if (hasSpecial) score += 20;
        
        // Bonus for using all character types
        if (hasLower && hasUpper && hasDigit && hasSpecial)
            score += 10;
        
        // Determine strength level
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
            
            // Color coding
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
        
        // Update strength bar
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

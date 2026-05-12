using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Text;
using System;

public class PasswordStrengthChecker : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip typingClip;
    [SerializeField] private AudioSource typingAudioSource;

    [Header("Avatar Names")]
    [SerializeField] private string[] avatarNames = { "Bilal", "Ali", "Saad", "Ayesha", "Maham" };

    [Header("Avatar Images (Optional)")]
    [Tooltip("Leave empty slots to use default emoji text instead.")]
    [SerializeField] private Sprite[] avatarImages = new Sprite[5];

    // ── Drag each Avatar card's Status TMP text here (5 slots) ──
    [Header("Avatar Status Labels (one per avatar card)")]
    [SerializeField] private TextMeshProUGUI[] avatarStatusLabels = new TextMeshProUGUI[5];

    // ── Password Checker Panel UI ────────────────────────────────
    [Header("Password Checker Panel — Drag refs from Inspector")]
    [SerializeField] private TextMeshProUGUI strengthText;
    [SerializeField] private TextMeshProUGUI hintsText;
    [SerializeField] private TextMeshProUGUI characterCountText;
    [SerializeField] private TextMeshProUGUI crackTimeText;
    [SerializeField] private TextMeshProUGUI currentAccountNameText;
    [SerializeField] private TMP_InputField  passwordInputField;
    [SerializeField] private Image           strengthBar;
    [SerializeField] private Button          toggleVisibilityButton;
    [SerializeField] private Button          setPasswordButton;

    [Header("Strength Colors")]
    [SerializeField] private Color weakColor      = new Color(1f, 0.2f, 0.2f);
    [SerializeField] private Color mediumColor    = new Color(1f, 0.8f, 0f);
    [SerializeField] private Color strongColor    = new Color(0.2f, 1f, 0.2f);
    [SerializeField] private Color veryStrongColor = new Color(0f, 0.8f, 0f);

    [Header("Panel References")]
    [SerializeField] private GameObject avatarSelectionPanel;
    [SerializeField] private GameObject passwordCheckerPanel;

    [Header("Password Visibility")]
    [SerializeField] private bool showPassword = false;

    // ── Private state ────────────────────────────────────────────
    private StringBuilder currentPassword    = new StringBuilder();
    private const int     MAX_PASSWORD_LENGTH = 128;
    private int           currentAccountIndex = -1;
    private Dictionary<int, string> accountPasswords = new Dictionary<int, string>();
    private const long    ATTEMPTS_PER_SECOND = 1_000_000_000;

    // ── Lifecycle ────────────────────────────────────────────────
    private void Start()
    {
        ShowAvatarSelection();
        SetupButtons();
        SetupInputField();
        Debug.Log("✅ PasswordStrengthChecker ready — Inspector-wired mode.");
    }

    private void SetupButtons()
    {
        if (setPasswordButton != null)
            setPasswordButton.onClick.AddListener(OnSetPasswordClicked);

        if (toggleVisibilityButton != null)
            toggleVisibilityButton.onClick.AddListener(TogglePasswordVisibility);
    }

    private void SetupInputField()
    {
        if (passwordInputField == null) return;

        // Android keyboard config
        passwordInputField.contentType    = TMP_InputField.ContentType.Standard;
        passwordInputField.inputType      = TMP_InputField.InputType.Standard;
        passwordInputField.keyboardType   = TouchScreenKeyboardType.Default;
        passwordInputField.characterLimit = MAX_PASSWORD_LENGTH;

        passwordInputField.onValueChanged.AddListener(OnPasswordInputChanged);
        passwordInputField.onEndEdit.AddListener(OnPasswordInputEnd);
    }

    // ── Public API ───────────────────────────────────────────────
    public string[] GetAvatarNames()  => avatarNames;
    public Sprite[] GetAvatarImages() => avatarImages;

    public bool IsPasswordSet(int index)
        => accountPasswords.ContainsKey(index);

    public void ClearPasswords()
    {
        accountPasswords.Clear();
        RefreshAllStatusLabels();
    }

    // ── Navigation ───────────────────────────────────────────────
    public void ShowAvatarSelection()
    {
        if (avatarSelectionPanel  != null) avatarSelectionPanel.SetActive(true);
        if (passwordCheckerPanel  != null) passwordCheckerPanel.SetActive(false);
        if (passwordInputField    != null) passwordInputField.text = "";
    }

    public void SelectAccount(int index)
    {
        currentAccountIndex = index;
        currentPassword.Clear();

        if (avatarSelectionPanel != null) avatarSelectionPanel.SetActive(false);
        if (passwordCheckerPanel != null) passwordCheckerPanel.SetActive(true);

        InitializeUI();
        UpdateDisplay();

        if (currentAccountNameText != null && index >= 0 && index < avatarNames.Length)
            currentAccountNameText.text =
                $"Setting password for: <b>{avatarNames[index]}</b>";

        if (passwordInputField != null)
        {
            passwordInputField.text = "";
            passwordInputField.ActivateInputField();
            passwordInputField.Select();
        }
    }

    // ── Input Handlers ───────────────────────────────────────────
    private void OnPasswordInputChanged(string input)
    {
        currentPassword.Clear();
        currentPassword.Append(input);
        UpdateDisplay();

        if (typingAudioSource != null && typingClip != null)
            typingAudioSource.PlayOneShot(typingClip);
    }

    private void OnPasswordInputEnd(string input)
    {
        currentPassword.Clear();
        currentPassword.Append(input);
        UpdateDisplay();
    }

    // ── Set Password ─────────────────────────────────────────────
    private void OnSetPasswordClicked()
    {
        if (currentPassword.Length == 0)
        {
            Debug.Log("❌ Cannot set empty password!");
            return;
        }

        accountPasswords[currentAccountIndex] = currentPassword.ToString();
        Debug.Log($"✅ Password set for: {avatarNames[currentAccountIndex]}");

        // Update button text temporarily
        if (setPasswordButton != null)
        {
            var txt = setPasswordButton.GetComponentInChildren<TextMeshProUGUI>();
            if (txt) txt.text = "✅ Password Set!";
        }

        // Update this avatar's status label immediately
        UpdateAvatarStatusLabel(currentAccountIndex, locked: true);

        StartCoroutine(ReturnToAvatarSelection());
    }

    private IEnumerator ReturnToAvatarSelection()
    {
        yield return new WaitForSeconds(1.5f);

        // Reset button text
        if (setPasswordButton != null)
        {
            var txt = setPasswordButton.GetComponentInChildren<TextMeshProUGUI>();
            if (txt) txt.text = "✓ Set Password";
        }

        ShowAvatarSelection();
    }

    public void TogglePasswordVisibility()
    {
        showPassword = !showPassword;

        if (passwordInputField != null)
        {
            passwordInputField.contentType = showPassword
                ? TMP_InputField.ContentType.Standard
                : TMP_InputField.ContentType.Password;
            passwordInputField.ForceLabelUpdate();
        }

        UpdateDisplay();

        if (toggleVisibilityButton != null)
        {
            var txt = toggleVisibilityButton.GetComponentInChildren<TextMeshProUGUI>();
            if (txt) txt.text = showPassword ? "🙈 Hide" : "👁️ Show";
        }
    }

    // ── Avatar Status Labels ─────────────────────────────────────
    // Call this to refresh all 5 status labels on the avatar panel
    private void RefreshAllStatusLabels()
    {
        for (int i = 0; i < 5; i++)
            UpdateAvatarStatusLabel(i, accountPasswords.ContainsKey(i));
    }

    private void UpdateAvatarStatusLabel(int index, bool locked)
    {
        if (avatarStatusLabels == null || index >= avatarStatusLabels.Length) return;
        var label = avatarStatusLabels[index];
        if (label == null) return;

        if (locked)
        {
            label.text  = "🔒 Password Set";
            label.color = new Color(0.4f, 1f, 0.6f); // green
        }
        else
        {
            label.text  = "No Password";
            label.color = new Color(0.7f, 0.7f, 0.7f); // grey
        }
    }

    // ── Display Update ───────────────────────────────────────────
    private void InitializeUI()
    {
        if (characterCountText != null) characterCountText.text = "Characters: 0";
        if (crackTimeText      != null) crackTimeText.text      = "";
        if (strengthText       != null) strengthText.text       = "Strength:  None\nScore: 0/100";
        if (strengthBar        != null) strengthBar.fillAmount  = 0f;
        if (hintsText          != null)
            hintsText.text = "💡 <b>Tips for a Strong Password:</b>\n" +
                             "• Use at least 12 characters\n" +
                             "• Mix uppercase and lowercase letters\n" +
                             "• Include numbers and special characters\n" +
                             "• Avoid common words and patterns";
    }

    private void UpdateDisplay()
    {
        string pwd = currentPassword.ToString();

        if (characterCountText != null)
            characterCountText.text = $"Characters: {pwd.Length}";

        PasswordStrength strength = CalculateStrength(pwd);
        UpdateStrengthDisplay(strength);

        if (hintsText    != null) hintsText.text    = GenerateHints(pwd, strength);
        if (crackTimeText != null) crackTimeText.text = CalculateCrackTime(pwd, strength);
    }

    private void UpdateStrengthDisplay(PasswordStrength s)
    {
        Color c = s.Level switch
        {
            "Very Weak"   => weakColor,
            "Weak"        => new Color(1f, 0.5f, 0f),
            "Medium"      => mediumColor,
            "Strong"      => strongColor,
            "Very Strong" => veryStrongColor,
            _             => weakColor
        };

        if (strengthText != null)
        {
            strengthText.text  = $"Strength:  {s.Level}\nScore: {s.Score}/100";
            strengthText.color = c;
        }
        if (strengthBar != null)
        {
            strengthBar.fillAmount = s.Score / 100f;
            strengthBar.color      = c;
        }
    }

    // ── Strength Calculation ─────────────────────────────────────
    private PasswordStrength CalculateStrength(string pwd)
    {
        if (pwd.Length == 0)
            return new PasswordStrength { Score = 0, Level = "None" };

        int  score = 0;
        bool hasLower = false, hasUpper = false, hasDigit = false, hasSpecial = false;

        foreach (char c in pwd)
        {
            if      (char.IsLower(c))          hasLower   = true;
            else if (char.IsUpper(c))          hasUpper   = true;
            else if (char.IsDigit(c))          hasDigit   = true;
            else if (!char.IsLetterOrDigit(c)) hasSpecial = true;
        }

        if (pwd.Length >= 8)  score += 20;
        if (pwd.Length >= 12) score += 15;
        if (pwd.Length >= 16) score += 15;
        if (hasLower)   score += 10;
        if (hasUpper)   score += 15;
        if (hasDigit)   score += 15;
        if (hasSpecial) score += 20;
        if (hasLower && hasUpper && hasDigit && hasSpecial) score += 10;

        string level = score < 30 ? "Very Weak"  :
                       score < 50 ? "Weak"        :
                       score < 70 ? "Medium"      :
                       score < 85 ? "Strong"      : "Very Strong";

        return new PasswordStrength {
            Score = score, Level = level,
            HasLower = hasLower, HasUpper = hasUpper,
            HasDigit = hasDigit, HasSpecial = hasSpecial,
            Length = pwd.Length
        };
    }

    // ── Hints ────────────────────────────────────────────────────
    private string GenerateHints(string pwd, PasswordStrength s)
    {
        if (pwd.Length == 0)
            return "💡 <b>Tips for a Strong Password:</b>\n" +
                   "• Use at least 12 characters\n" +
                   "• Mix uppercase and lowercase letters\n" +
                   "• Include numbers and special characters\n" +
                   "• Avoid common words and patterns";

        var sb = new StringBuilder("<b>💡 Improvement Suggestions:</b>\n");

        if      (s.Length < 8)  sb.AppendLine($"❌ Add {8  - s.Length} more chars (min 8)");
        else if (s.Length < 12) sb.AppendLine($"⚠️ Add {12 - s.Length} more chars (rec 12+)");
        else                     sb.AppendLine("✓ Length is good");

        sb.AppendLine(!s.HasLower   ? "❌ Add lowercase letters (a-z)"   : "✓ Has lowercase");
        sb.AppendLine(!s.HasUpper   ? "❌ Add uppercase letters (A-Z)"   : "✓ Has uppercase");
        sb.AppendLine(!s.HasDigit   ? "❌ Add numbers (0-9)"             : "✓ Has numbers");
        sb.AppendLine(!s.HasSpecial ? "❌ Add special chars (!@#$%^&*)"  : "✓ Has special chars");

        if (s.Score >= 85)
            sb.AppendLine("\n<color=green>🎉 Excellent password!</color>");

        return sb.ToString();
    }

    // ── Crack Time ───────────────────────────────────────────────
    private string CalculateCrackTime(string pwd, PasswordStrength s)
    {
        if (pwd.Length == 0) return "";

        int pool = 0;
        if (s.HasLower)   pool += 26;
        if (s.HasUpper)   pool += 26;
        if (s.HasDigit)   pool += 10;
        if (s.HasSpecial) pool += 32;
        if (pool == 0)    pool  = 26;

        double combos  = Math.Pow(pool, pwd.Length);
        double seconds = combos / ATTEMPTS_PER_SECOND;
        string emoji   = GetCrackEmoji(seconds);

        return $"{emoji} <b>Time to Crack:</b> {FormatTime(seconds)}\n" +
               "<size=14><color=#888888>(At 1 billion guesses/second)</color></size>";
    }

    private string FormatTime(double sec)
    {
        if (sec < 1)            return "<color=red>Instantly</color>";
        if (sec < 60)           return $"<color=red>{sec:F1} seconds</color>";
        double m = sec / 60;
        if (m  < 60)            return $"<color=orange>{m:F1} minutes</color>";
        double h = m / 60;
        if (h  < 24)            return $"<color=yellow>{h:F1} hours</color>";
        double d = h / 24;
        if (d  < 30)            return $"<color=yellow>{d:F1} days</color>";
        double mo = d / 30;
        if (mo < 12)            return $"<color=#90EE90>{mo:F1} months</color>";
        double y = d / 365;
        if (y  < 1_000)         return $"<color=green>{y:F1} years</color>";
        if (y  < 1_000_000)     return $"<color=green>{y/1_000:F1} thousand years</color>";
        if (y  < 1_000_000_000) return $"<color=green>{y/1_000_000:F1} million years</color>";
        return $"<color=green>{y/1_000_000_000:F1} billion years</color>";
    }

    private string GetCrackEmoji(double sec)
    {
        if (sec < 60)        return "💀";
        if (sec < 3_600)     return "⚠️";
        if (sec < 86_400)    return "⏰";
        if (sec < 2_592_000) return "📅";
        if (sec < 31_536_000)return "🗓️";
        return "🔒";
    }

    // ── Data ─────────────────────────────────────────────────────
    private struct PasswordStrength
    {
        public int    Score, Length;
        public string Level;
        public bool   HasLower, HasUpper, HasDigit, HasSpecial;
    }
}
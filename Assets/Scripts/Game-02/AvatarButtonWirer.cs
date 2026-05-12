using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AvatarButtonWirer : MonoBehaviour
{
    [SerializeField] private int avatarIndex = 0;
    [SerializeField] private PasswordStrengthChecker checker;

    private void Awake()
    {
        // Auto-find checker if not assigned, including if it's currently disabled
        if (checker == null)
            checker = FindFirstObjectByType<PasswordStrengthChecker>(FindObjectsInactive.Include);

        var btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (checker == null) return;
        if (checker.IsPasswordSet(avatarIndex)) return; // already locked
        checker.SelectAccount(avatarIndex);
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SafeDrive : MonoBehaviour
{
    [Header("References")]
public Image driveImage;
public Image iconImage;
public TMP_Text driveLabel;
public TMP_Text savedCount;
public Image[] borders; // BorderLeft, BorderTop, BorderBottom
public Shadow driveGlow;
 
private RectTransform rt;
private int saveCount = 0;
 
// Idle colors
static readonly Color32 BG_IDLE = new Color32(15,23,42,76);
static readonly Color32 ICON_IDLE = new Color32(100,116,139,255);
static readonly Color32 LABEL_IDLE = new Color32(71,85,105,255);
static readonly Color32 BORDER_IDLE = new Color32(51,65,85,180);
// Active colors
static readonly Color32 BG_ACTIVE = new Color32(20,83,45,76);
static readonly Color32 ICON_ACTIVE = new Color32(74,222,128,255);
static readonly Color32 LABEL_ACTIVE = new Color32(74,222,128,255);
static readonly Color32 BORDER_ACTIVE = new Color32(74,222,128,255);

void Awake() { rt = GetComponent<RectTransform>(); }
public void SetHighlight(bool active) {
driveImage.color = active ? BG_ACTIVE : BG_IDLE;
iconImage.color = active ? ICON_ACTIVE : ICON_IDLE;
driveLabel.color = active ? LABEL_ACTIVE : LABEL_IDLE;
if (driveGlow) driveGlow.enabled = active;
foreach (var b in borders)
b.color = active ? BORDER_ACTIVE : BORDER_IDLE;
}
public bool IsPointerOver(Vector2 screenPos) {
return RectTransformUtility.RectangleContainsScreenPoint(rt, screenPos, null);
}
public void RegisterSave() {
saveCount++;
savedCount.text = $"{saveCount} saved";
}
public void ResetDrive() {
saveCount = 0;
savedCount.text = "0 saved";
SetHighlight(false);
}
}
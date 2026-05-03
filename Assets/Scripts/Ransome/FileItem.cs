using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public enum FileType { Doc, Zip, Spreadsheet, Image, Video, Folder }
public class FileItem : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler 
{
   [Header("UI References")]
public Image iconImage;
public TMP_Text nameLabel;
public Image labelBadge;
public Outline importantOutline;
public Shadow importantGlow;
public CanvasGroup canvasGroup;
 
[Header("Icon Sprites — assign in Prefab")]
public Sprite sprDoc, sprZip, sprSheet, sprImage, sprVideo, sprFolder, sprLock;
 
// Runtime state
public bool IsSaved { get; private set; }
public bool IsEncrypted { get; private set; }
public bool IsImportant { get; private set; }
private FileData data;
private RectTransform rt;
private Canvas rootCanvas;
private GameObject dragClone; // assigned from DragManager
 
// Colors
static readonly Color32 COL_BADGE_NORMAL = new Color32(30,41,59,204);
static readonly Color32 COL_BADGE_IMPORTANT = new Color32(133,77,14,128);
static readonly Color32 COL_BADGE_ENCRYPTED = new Color32(69,10,10,230);
static readonly Color32 COL_TEXT_NORMAL = new Color32(226,232,240,255);
static readonly Color32 COL_TEXT_ENCRYPTED = new Color32(252,165,165,255);
static readonly Color32 COL_ICON_DOC = new Color32(96,165,250,255);
static readonly Color32 COL_ICON_ZIP = new Color32(234,179,8,255);
static readonly Color32 COL_ICON_SHEET = new Color32(34,197,94,255);
static readonly Color32 COL_ICON_VIDEO = new Color32(251,146,60,255);
static readonly Color32 COL_ICON_FOLDER = new Color32(253,224,71,255);
static readonly Color32 COL_ICON_LOCK = new Color32(239,68,68,255);
 
public void Initialize(FileData d) {
data = d;
IsImportant = d.isImportant;
rt = GetComponent<RectTransform>();
rootCanvas = GetComponentInParent<Canvas>();
nameLabel.text = d.fileName;
SetIcon(d.fileType);
ApplyBadgeStyle();
if (importantOutline) importantOutline.enabled = IsImportant;
if (importantGlow) importantGlow.enabled = IsImportant;
GameManager.Instance.OnWaveUpdated += CheckEncryption;
}
 
void SetIcon(FileType t) {
switch(t) {
case FileType.Doc: iconImage.sprite = sprDoc; iconImage.color = COL_ICON_DOC; break;
case FileType.Zip: iconImage.sprite = sprZip; iconImage.color = COL_ICON_ZIP; break;
case FileType.Spreadsheet: iconImage.sprite = sprSheet; iconImage.color = COL_ICON_SHEET;
break;
case FileType.Image: iconImage.sprite = sprImage; iconImage.color = COL_ICON_DOC; break;
case FileType.Video: iconImage.sprite = sprVideo; iconImage.color = COL_ICON_VIDEO; break;
case FileType.Folder: iconImage.sprite = sprFolder; iconImage.color = COL_ICON_FOLDER;
break;
}
}
 
void ApplyBadgeStyle() {
if (IsEncrypted) {
labelBadge.color = COL_BADGE_ENCRYPTED;
nameLabel.color = COL_TEXT_ENCRYPTED;
nameLabel.fontStyle = FontStyles.Strikethrough;
iconImage.sprite = sprLock;
iconImage.color = COL_ICON_LOCK;
} else if (IsImportant) {
labelBadge.color = COL_BADGE_IMPORTANT;
nameLabel.color = COL_TEXT_NORMAL;
nameLabel.fontStyle = FontStyles.Bold;
} else {
labelBadge.color = COL_BADGE_NORMAL;
nameLabel.color = COL_TEXT_NORMAL;
nameLabel.fontStyle = FontStyles.Normal;
}
}
 
void CheckEncryption(float waveX) {
if (IsSaved || IsEncrypted) return;
RectTransform parentRt = rt.parent as RectTransform;
float w = parentRt.rect.width;
float myXPercent = (rt.anchoredPosition.x + w * 0.5f) / w * 100f;
if (myXPercent < waveX) Encrypt();
}
 
public void Encrypt() {
if (IsEncrypted) return;
IsEncrypted = true;
canvasGroup.blocksRaycasts = false;
ApplyBadgeStyle();
if (GameManager.Instance != null)
GameManager.Instance.RegisterFileLost(IsImportant);
}
 
public void Save() {
IsSaved = true;
if (GameManager.Instance != null)
GameManager.Instance.RegisterFileSaved(IsImportant);
GameManager.Instance.OnWaveUpdated -= CheckEncryption;
gameObject.SetActive(false);
}
public void OnPointerDown(PointerEventData e) {
if (IsEncrypted || IsSaved) return;
if (GameManager.Instance.State != GameManager.GameState.Playing) return;
canvasGroup.alpha = 0.25f;
DragManager.Instance.BeginDrag(this, e.position);
}
public void OnDrag(PointerEventData e) {
DragManager.Instance.UpdateDrag(e.position);
}
public void OnPointerUp(PointerEventData e) {
canvasGroup.alpha = 1f;
DragManager.Instance.EndDrag(e.position, this);
}
void OnDestroy() {
if (GameManager.Instance != null)
GameManager.Instance.OnWaveUpdated -= CheckEncryption;
}
}
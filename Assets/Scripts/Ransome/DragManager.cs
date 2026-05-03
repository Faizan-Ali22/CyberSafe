using UnityEngine;
using UnityEngine.UI;
public class DragManager : MonoBehaviour{
    private static DragManager instance;

    [Header("References")]
public RectTransform dragClone; // the ghost drag object
public SafeDrive safeDrive;
public RectTransform canvasRect;
 
private FileItem currentItem;

    public static DragManager Instance { get => instance; set => instance = value; }

    void Awake() => Instance = this;
 
public void BeginDrag(FileItem item, Vector2 screenPos) {
currentItem = item;
dragClone.gameObject.SetActive(true);
UpdateDrag(screenPos);
safeDrive.SetHighlight(true);
}
 
public void UpdateDrag(Vector2 screenPos) {
if (dragClone.gameObject.activeSelf)
dragClone.position = screenPos;
}
 
public void EndDrag(Vector2 screenPos, FileItem item) {
dragClone.gameObject.SetActive(false);
safeDrive.SetHighlight(false);
if (currentItem == null) return;
 
if (safeDrive.IsPointerOver(screenPos)) {
item.Save();
safeDrive.RegisterSave();
} else {
// Move file to dropped position (throw ahead of wave)
Vector2 local;
RectTransformUtility.ScreenPointToLocalPointInRectangle(
item.GetComponent<RectTransform>().parent as RectTransform,
screenPos, null, out local);
item.GetComponent<RectTransform>().anchoredPosition = local;
}
currentItem = null;
}
}
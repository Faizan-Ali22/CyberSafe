using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class HUDManager : MonoBehaviour
{
    [Header("HUD")]
public TMP_Text waveLabel;
public Image loadBarFill;
public TMP_Text loadPercent;
public Image frustBarFill;
public TMP_Text frustPercent;
 
[Header("Screens")]
public GameObject startScreen;
public GameObject lostScreen;
public GameObject wonScreen;
public TMP_Text failReason;
public TMP_Text finalLoad;
public TMP_Text finalFrust;
 
[Header("Instruct Overlay")]
public CanvasGroup instructOverlay;
private bool firstDragMade = false;
private float bounceTimer = 0f;
private RectTransform instructRT;
 
static readonly Color COL_EMERALD = new Color(0.063f,0.725f,0.506f); // #10B981
static readonly Color COL_RED = new Color(0.937f,0.267f,0.267f); // #EF4444
static readonly Color COL_AMBER = new Color(0.961f,0.620f,0.043f); // #F59E0B
 
void Start() {
GameController.Instance.OnTick += OnTick;
GameController.Instance.OnEnd += OnEnd;
instructRT = instructOverlay.GetComponent<RectTransform>();
ShowStart();
}
 
void Update() {
// Animate instruction bounce (simulate animate-bounce)
if (!firstDragMade && instructOverlay.alpha > 0f) {
bounceTimer += Time.deltaTime;
float offsetY = Mathf.Abs(Mathf.Sin(bounceTimer * 3f)) * -12f;
instructRT.anchoredPosition = new Vector2(0f, 60f + offsetY);
}
}
 
void OnTick(float time, float load, float frust, int wave) {
waveLabel.text = $"WAVE {wave}/3 | T-{Mathf.CeilToInt(time)}s";
 
// Load bar
float loadPct = load / 100f;
loadBarFill.rectTransform.anchorMax = new Vector2(loadPct, 1f);
loadBarFill.color = load > 80f ? COL_RED : COL_EMERALD;
loadPercent.text = $"{Mathf.FloorToInt(load)}%";
loadPercent.color = load > 80f ? COL_RED : Color.white;
 
// Frust bar
float frustPct = frust / 100f;
frustBarFill.rectTransform.anchorMax = new Vector2(frustPct, 1f);
frustPercent.text = $"{Mathf.FloorToInt(frust)}%";
frustPercent.color = frust > 80f ? COL_AMBER : Color.white;
}
 
void OnEnd(GameController.State result, string reason) {
if (result == GameController.State.LOST) ShowLost(reason);
else ShowWon();
}
 
public void NotifyFirstDrag() {
if (firstDragMade) return;
firstDragMade = true;
StartCoroutine(FadeOutOverlay());
}
 
System.Collections.IEnumerator FadeOutOverlay() {
while (instructOverlay.alpha > 0f) {
instructOverlay.alpha -= Time.deltaTime * 2f;
yield return null;
}
}
 
void ShowStart() {
startScreen.SetActive(true);
lostScreen.SetActive(false);
wonScreen.SetActive(false);
}
 
void ShowLost(string reason) {
lostScreen.SetActive(true);
startScreen.SetActive(false);
wonScreen.SetActive(false);
failReason.text = reason == "SERVER_CRASH"
? "Load exceeded 100%. The server melted down under the malicious requests."
: "Frustration exceeded 100%. You over-blocked and locked out the real students.";
// Pulse AlertIcon — find it and start coroutine
Transform alertIcon = lostScreen.transform.Find("AlertIcon");
if (alertIcon != null) StartCoroutine(PulseIcon(alertIcon.GetComponent<RectTransform>()));
}   
void ShowWon() {
wonScreen.SetActive(true);
startScreen.SetActive(false);
lostScreen.SetActive(false);
finalLoad.text = $"{Mathf.FloorToInt(GameController.Instance.Load)}%";
finalFrust.text = $"{Mathf.FloorToInt(GameController.Instance.Frustration)}%";
}
System.Collections.IEnumerator PulseIcon(RectTransform rt) {
if (rt == null) yield break;
while (lostScreen.activeSelf) {
float t = 0f;
while (t < 0.5f) { t += Time.deltaTime;
rt.localScale = Vector3.Lerp(Vector3.one, Vector3.one*1.1f, t/0.5f);
yield return null; }
while (t > 0f) { t -= Time.deltaTime;
rt.localScale = Vector3.Lerp(Vector3.one, Vector3.one*1.1f, t/0.5f);
yield return null; }
}
}
void OnDestroy() {
if (GameController.Instance) {
GameController.Instance.OnTick -= OnTick;
GameController.Instance.OnEnd -= OnEnd;
}
}
}

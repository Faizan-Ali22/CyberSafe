using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UIManager : MonoBehaviour
{
    [Header("Screens")]
public GameObject startScreen;
public GameObject gameOverScreen;
 
[Header("GameOver UI")]
public TMP_Text rankText;
public TMP_Text importantSavedText;
public TMP_Text junkSavedText;
public TMP_Text lostText;
public Image headerIcon;
public TMP_Text headlineText;
public Sprite sprDrive;
public Sprite sprAlert;
 
static readonly Color32 COL_S = new Color32(74,222,128,255);
static readonly Color32 COL_B = new Color32(96,165,250,255);
static readonly Color32 COL_C = new Color32(250,204,21,255);
static readonly Color32 COL_F = new Color32(220,38,38,255);
 
void Start() {
GameManager.Instance.OnGameStateChanged += OnStateChange;
ShowStart();
}
 
void OnStateChange(GameManager.GameState s) {
startScreen.SetActive(false);
gameOverScreen.SetActive(false);
if (s == GameManager.GameState.Idle) ShowStart();
if (s == GameManager.GameState.GameOverWave) ShowGameOver(false);
if (s == GameManager.GameState.GameOverTrap) ShowGameOver(true);
}
 
void ShowStart() => startScreen.SetActive(true);
 
void ShowGameOver(bool wasTrapped) {
gameOverScreen.SetActive(true);
var gm = GameManager.Instance;
int totalImp = gm != null ? gm.TotalImportant : 0;
int savedImp = gm != null ? gm.SavedImportant : 0;
int savedJunk = gm != null ? gm.SavedJunk : 0;
int totalLost = gm != null ? gm.TotalLost : 0;
importantSavedText.text = $"{savedImp} / {totalImp}";
importantSavedText.color = savedImp == totalImp ? COL_S : COL_C;

junkSavedText.text = savedJunk.ToString();
lostText.text = totalLost.ToString();
 
if (wasTrapped) {
rankText.text = "F Rank: Scammed";
rankText.color = COL_F;
headlineText.text = "SYSTEM COMPROMISED";
headlineText.color = COL_F;
headerIcon.sprite = sprAlert;
headerIcon.color = COL_F;
} else if (savedImp == totalImp && totalImp > 0) {
rankText.text = "S Rank: IT Security Master"; rankText.color = COL_S;
headlineText.text = "ENCRYPTION COMPLETE"; headlineText.color = Color.white;
headerIcon.sprite = sprDrive; headerIcon.color = new Color32(59,130,246,255);
} else if (savedImp == 0) {
rankText.text = "F Rank: Total Data Loss"; rankText.color = COL_F;
headlineText.text = "ENCRYPTION COMPLETE"; headlineText.color = Color.white;
headerIcon.sprite = sprDrive; headerIcon.color = new Color32(59,130,246,255);
} else if (savedImp > totalImp / 2) {
rankText.text = "B Rank: Acceptable Losses"; rankText.color = COL_B;
headlineText.text = "ENCRYPTION COMPLETE"; headlineText.color = Color.white;
headerIcon.sprite = sprDrive; headerIcon.color = new Color32(59,130,246,255);
} else {
rankText.text = "C Rank"; rankText.color = COL_C;
headlineText.text = "ENCRYPTION COMPLETE"; headlineText.color = Color.white;
headerIcon.sprite = sprDrive; headerIcon.color = new Color32(59,130,246,255);
}
}
 
void OnDestroy() {
if (GameManager.Instance != null)
GameManager.Instance.OnGameStateChanged -= OnStateChange;
}
}
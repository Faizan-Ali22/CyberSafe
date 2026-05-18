using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState { Idle, Playing, GameOverWave, GameOverTrap }
public static GameManager Instance;
 
[Header("Wave Settings")]
public float waveSpeed = 3.0f; // percent of screen per second

[Header("Wave Progression")]
public int totalWaves = 3;
public int[] filesPerWave = new int[] { 5, 8, 11 };
public int[] trapsPerWave = new int[] { 2, 4, 5 };
public float waveSpeedIncrement = 3.0f;

[Header("Win Conditions")]
[Tooltip("Percentage of important files that must be saved to win (e.g., 0.8 = 80%)")]
[Range(0f, 1f)]
public float requiredSavePercentage = 0.7f;
public GameState State { get; private set; } = GameState.Idle;
public float WavePercent { get; private set; }
public int CurrentWave { get; private set; } = 1;
public bool IsRunActive { get; private set; }

public int TotalImportant { get; private set; }
public int SavedImportant { get; private set; }
public int SavedJunk { get; private set; }
public int TotalLost { get; private set; }
 
public event System.Action<GameState> OnGameStateChanged;
public event System.Action<float> OnWaveUpdated;
public event System.Action<int> OnWaveStarted;

// Primary pointer (touch on device, mouse in editor)
public bool PrimaryPointerIsDown { get; private set; }
public Vector2 PrimaryPointerPosition { get; private set; }
public event System.Action<Vector2> OnPrimaryPointerDown;
public event System.Action<Vector2> OnPrimaryPointerDrag;
public event System.Action<Vector2> OnPrimaryPointerUp;

// Add this variable near your pointer variables
private int activeTouchId = -1;

float baseWaveSpeed;
 
void Awake() {
if (Instance != null) { Destroy(gameObject); return; }
Instance = this;
baseWaveSpeed = waveSpeed;
}
 
void Update() {
UpdatePrimaryPointer();

if (State != GameState.Playing) return;
WavePercent += waveSpeed * Time.deltaTime;
OnWaveUpdated?.Invoke(WavePercent);
if (WavePercent >= 100f) AdvanceWaveOrEnd();
}

void UpdatePrimaryPointer() {
    // Touch input (Android/iOS)
    if (Input.touchCount > 0) {
        foreach (Touch touch in Input.touches) {
            // Check for new touches starting
            if (touch.phase == TouchPhase.Began) {
                // Optional: Prevent starting a drag if touching a UI element
                // if (UnityEngine.EventSystems.EventSystem.current != null && 
                //     UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(touch.fingerId)) {
                //     continue; 
                // }

                // Only grab the first touch that starts and ignore others until this one ends
                if (!PrimaryPointerIsDown) {
                    activeTouchId = touch.fingerId;
                    PrimaryPointerIsDown = true;
                    PrimaryPointerPosition = touch.position;
                    OnPrimaryPointerDown?.Invoke(PrimaryPointerPosition);
                }
            }
            // Track only our actively held touch
            else if (touch.fingerId == activeTouchId) {
                PrimaryPointerPosition = touch.position;

                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) {
                    OnPrimaryPointerDrag?.Invoke(PrimaryPointerPosition);
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
                    OnPrimaryPointerUp?.Invoke(PrimaryPointerPosition);
                    PrimaryPointerIsDown = false;
                    activeTouchId = -1; // Reset active touch
                }
            }
        }
        return; // Don't process mouse if we have touches
    }

    // Mouse fallback (Editor / standalone)
    // (Your existing mouse code remains identical here)
    PrimaryPointerPosition = Input.mousePosition;
    
    if (Input.GetMouseButtonDown(0)) {
        // Optional UI check for mouse:
        // if (UnityEngine.EventSystems.EventSystem.current != null && 
        //     UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;

        PrimaryPointerIsDown = true;
        OnPrimaryPointerDown?.Invoke(PrimaryPointerPosition);
    }
    if (Input.GetMouseButton(0)) {
        if (PrimaryPointerIsDown)
            OnPrimaryPointerDrag?.Invoke(PrimaryPointerPosition);
    }
    if (Input.GetMouseButtonUp(0)) {
        if (PrimaryPointerIsDown)
            OnPrimaryPointerUp?.Invoke(PrimaryPointerPosition);
        PrimaryPointerIsDown = false;
    }
}

void AdvanceWaveOrEnd() {
if (CurrentWave < totalWaves) {
CurrentWave++;
waveSpeed = GetWaveSpeed(CurrentWave);
WavePercent = 0f;
OnWaveUpdated?.Invoke(WavePercent);
SetState(GameState.Playing);
OnWaveStarted?.Invoke(CurrentWave);
} else {
SetState(GameState.GameOverWave);
}
}

public int GetFilesForWave(int waveIndex) => GetWaveValue(filesPerWave, waveIndex);
public int GetTrapsForWave(int waveIndex) => GetWaveValue(trapsPerWave, waveIndex);
public float GetWaveSpeed(int waveIndex) => baseWaveSpeed + (waveIndex - 1) * waveSpeedIncrement;

int GetWaveValue(int[] values, int waveIndex) {
if (values == null || values.Length == 0) return 0;
int idx = Mathf.Clamp(waveIndex - 1, 0, values.Length - 1);
return values[idx];
}
    
public void StartGame() {
ResetRunStats();
CurrentWave = 1;
waveSpeed = GetWaveSpeed(CurrentWave);
WavePercent = 0f;
OnWaveUpdated?.Invoke(WavePercent);
SetState(GameState.Playing);
OnWaveStarted?.Invoke(CurrentWave);
}
    
public void SetState(GameState s) {
State = s;
if (s == GameState.Playing) IsRunActive = true;
if (s == GameState.Idle || s == GameState.GameOverWave || s == GameState.GameOverTrap)
IsRunActive = false;
OnGameStateChanged?.Invoke(s);
}

public void RegisterFileSpawned(bool isImportant) {
if (!IsRunActive) return;
if (isImportant) TotalImportant++;
}

public void RegisterFileSaved(bool isImportant) {
if (!IsRunActive) return;
if (isImportant) SavedImportant++;
else SavedJunk++;
}

public void RegisterFileLost(bool isImportant) {
if (!IsRunActive) return;
TotalLost++;
}
public bool HasPlayerWon() 
{
    // If they clicked the fake decrypt button, it's an instant loss
    if (State == GameState.GameOverTrap) return false; 
    
    // Prevent division by zero if no important files spawned
    if (TotalImportant == 0) return true; 

    // Calculate the percentage of important files saved
    float savedPercent = (float)SavedImportant / TotalImportant;
    
    // Return true if they met or exceeded the requirement
    return savedPercent >= requiredSavePercentage;
}
void ResetRunStats() {
TotalImportant = 0;
SavedImportant = 0;
SavedJunk = 0;
TotalLost = 0;
}
}

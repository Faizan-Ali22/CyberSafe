using UnityEngine;
using System.Collections;


public class GameController : MonoBehaviour
{
    public static GameController Instance;
 
public enum State { START, PLAYING, WON, LOST }
public State GameState { get; private set; } = State.START;
 
[Header("Settings")]
public float totalTime = 60f;
 
// Runtime metrics
public float TimeRemaining { get; private set; }
public float Load { get; private set; }
public float Frustration { get; private set; }
public int Wave { get; private set; } = 1;
 
public event System.Action<float, float, float, int> OnTick; // time,load,frust,wave
public event System.Action<State, string> OnEnd;
 
void Awake() {
if (Instance != null) { Destroy(gameObject); return; }
Instance = this;
}
 
public void StartGame() {
TimeRemaining = totalTime;
Load = 0; Frustration = 0; Wave = 1;
GameState = State.PLAYING;
OnTick?.Invoke(TimeRemaining, Load, Frustration, Wave);
}
 
// Called by PacketSpawner when a bot hits the server
public void AddLoad(float amount) {
Load = Mathf.Min(100f, Load + amount);
if (Load >= 100f) EndGame(State.LOST, "SERVER_CRASH");
else OnTick?.Invoke(TimeRemaining, Load, Frustration, Wave);
}
 
// Called when load decreases (legit user reaches server)
public void ReduceLoad(float amount) {
Load = Mathf.Max(0f, Load - amount);
OnTick?.Invoke(TimeRemaining, Load, Frustration, Wave);
}
 
// Called when a legit user is stuck behind a wall too long
public void AddFrustration(float amount) {
Frustration = Mathf.Min(100f, Frustration + amount);
if (Frustration >= 100f) EndGame(State.LOST, "USER_REVOLT");
else OnTick?.Invoke(TimeRemaining, Load, Frustration, Wave);
}
 
IEnumerator GameLoop() {
while (GameState == State.PLAYING) {
yield return new WaitForSeconds(1f);
TimeRemaining--;
if (TimeRemaining <= 40f && Wave < 2) { Wave = 2; }
if (TimeRemaining <= 20f && Wave < 3) { Wave = 3; }
OnTick?.Invoke(TimeRemaining, Load, Frustration, Wave);
if (TimeRemaining <= 0f) EndGame(State.WON, "");
}
}
 
public void BeginLoop() => StartCoroutine(GameLoop());
 
void EndGame(State result, string reason) {
GameState = result;
OnEnd?.Invoke(result, reason);
}
}

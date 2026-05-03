using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
public class TrapSpawner : MonoBehaviour
{
   [Header("References")]
public GameObject trapPrefab;
public RectTransform spawnParent; // TrapContainer
 
[Header("Settings")]
public float spawnInterval = 2f;
public float minLifetime = 3f;
public float maxLifetime = 5.5f;
 
void Start() {
if (GameManager.Instance != null) {
GameManager.Instance.OnGameStateChanged += OnStateChange;
GameManager.Instance.OnWaveStarted += OnWaveStarted;
}
}
 
void OnStateChange(GameManager.GameState s) {
StopAllCoroutines();
if (s != GameManager.GameState.Playing) ClearTraps();
}

void OnWaveStarted(int waveIndex) {
StopAllCoroutines();
ClearTraps();
int count = GameManager.Instance != null
? GameManager.Instance.GetTrapsForWave(waveIndex)
: 0;
if (count > 0) StartCoroutine(SpawnWave(count));
}

IEnumerator SpawnWave(int count) {
for (int i = 0; i < count; i++) {
SpawnTrap();
yield return new WaitForSeconds(spawnInterval);
}
}
 
void SpawnTrap() {
var go = Instantiate(trapPrefab, spawnParent);
var rt = go.GetComponent<RectTransform>();
float w = spawnParent.rect.width;
float h = spawnParent.rect.height;
// Spawn in the centre zone (20%-70% x, 20%-80% y)
float lx = (Random.Range(0.20f, 0.70f) * w) - w * 0.5f;
float ly = -((Random.Range(0.20f, 0.80f) * h) - h * 0.5f);
rt.anchoredPosition = new Vector2(lx, ly);
Destroy(go, Random.Range(minLifetime, maxLifetime));
}

void ClearTraps() {
if (spawnParent == null) return;
for (int i = spawnParent.childCount - 1; i >= 0; i--)
Destroy(spawnParent.GetChild(i).gameObject);
}
 
void OnDestroy() {
if (GameManager.Instance != null)
GameManager.Instance.OnGameStateChanged -= OnStateChange;
if (GameManager.Instance != null)
GameManager.Instance.OnWaveStarted -= OnWaveStarted;
}
}



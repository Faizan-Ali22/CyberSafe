using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class FileData {
public string fileName;
public FileType fileType;
public bool isImportant;
[Range(0,100)] public float xPercent;
[Range(0,100)] public float yPercent;
}
public class FileSpawner : MonoBehaviour
{
  [Header("References")]
public GameObject fileItemPrefab;
public RectTransform filesContainer;
 
[Header("File List — fill in Inspector")]
public List<FileData> allFiles;
 
private readonly List<FileItem> spawnedItems = new();
int[] shuffleBuffer;
 
void Start() {
if (GameManager.Instance != null)
GameManager.Instance.OnWaveStarted += OnWaveStarted;

int previewCount = GameManager.Instance != null
? GameManager.Instance.GetFilesForWave(1)
: allFiles.Count;
SpawnFiles(previewCount, false); // preview
}
 
void OnWaveStarted(int waveIndex) {
ClearFiles();
int count = GameManager.Instance != null
? GameManager.Instance.GetFilesForWave(waveIndex)
: allFiles.Count;
SpawnFiles(count, true);
}
 
void SpawnFiles(int count, bool registerStats) {
if (fileItemPrefab == null || filesContainer == null) return;
if (allFiles == null || allFiles.Count == 0 || count <= 0) return;

int available = allFiles.Count;
int spawnCount = Mathf.Min(count, available);
EnsureShuffleBuffer(available);
for (int i = 0; i < available; i++) shuffleBuffer[i] = i;

float w = filesContainer.rect.width;
float h = filesContainer.rect.height;
for (int i = 0; i < spawnCount; i++) {
int swap = Random.Range(i, available);
int idx = shuffleBuffer[swap];
shuffleBuffer[swap] = shuffleBuffer[i];
shuffleBuffer[i] = idx;

var data = allFiles[idx];
var go = Instantiate(fileItemPrefab, filesContainer);
var rt = go.GetComponent<RectTransform>();
rt.anchoredPosition = PercentToAnchored(data.xPercent, data.yPercent, w, h);
var item = go.GetComponent<FileItem>();
item.Initialize(data);
spawnedItems.Add(item);
if (registerStats && GameManager.Instance != null)
GameManager.Instance.RegisterFileSpawned(data.isImportant);
}
}

void EnsureShuffleBuffer(int size) {
if (shuffleBuffer == null || shuffleBuffer.Length != size)
shuffleBuffer = new int[size];
}

Vector2 PercentToAnchored(float xPct, float yPct, float w, float h) {
float x = (xPct / 100f * w) - w * 0.5f;
float y = -((yPct / 100f * h) - h * 0.5f);
return new Vector2(x, y);
}
void ClearFiles() {
foreach (var item in spawnedItems)
if (item != null) Destroy(item.gameObject);
spawnedItems.Clear();
}
 
public List<FileItem> GetAllItems() => spawnedItems;

void OnDestroy() {
if (GameManager.Instance != null)
GameManager.Instance.OnWaveStarted -= OnWaveStarted;
}
}

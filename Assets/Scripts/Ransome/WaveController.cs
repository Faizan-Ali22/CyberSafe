using UnityEngine;

public class WaveController : MonoBehaviour
{
   [Header("References")]
public RectTransform waveRect;
public RectTransform glowEdge;
public RectTransform parentRect; // Canvas RectTransform
 
void Start() {
GameManager.Instance.OnWaveUpdated += UpdateWave;
}
 
void UpdateWave(float pct) {
float totalW = parentRect.rect.width;
float waveW = (pct / 100f) * totalW;
waveRect.SetSizeWithCurrentAnchors(
RectTransform.Axis.Horizontal, waveW);
// GlowEdge stays pinned to right edge of WaveRect (via anchor)
// No position update needed if anchor is (1,0)-(1,1)
}
 
void OnDestroy() {
if (GameManager.Instance != null)
GameManager.Instance.OnWaveUpdated -= UpdateWave;
}
}

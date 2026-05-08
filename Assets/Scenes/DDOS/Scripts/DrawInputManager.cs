using UnityEngine;
using System.Collections.Generic;

public class DrawInputManager : MonoBehaviour
{
   [Header("References")]
public RectTransform gameCanvasRect;
public FirewallManager firewallManager;
public RadarDrawer radarDrawer;
 
// id → (startX, startY, currX, currY)
private Dictionary<int, Vector4> activeTouches = new Dictionary<int, Vector4>();
private bool isMouseDown = false;
private const int MOUSE_ID = -999;
 
void Update() {
if (GameController.Instance.GameState != GameController.State.PLAYING) return;
 
//  TOUCH 
for (int i = 0; i < Input.touchCount; i++) {
Touch t = Input.GetTouch(i);
Vector2 tc = TouchToCanvas(t.position);
switch (t.phase) {
case TouchPhase.Began:
activeTouches[t.fingerId] = new Vector4(tc.x, tc.y, tc.x, tc.y);
break;
case TouchPhase.Moved:
case TouchPhase.Stationary:
if (activeTouches.ContainsKey(t.fingerId)) {
var v = activeTouches[t.fingerId];
activeTouches[t.fingerId] = new Vector4(v.x, v.y, tc.x, tc.y);
}
break;
case TouchPhase.Ended:
case TouchPhase.Canceled:
if (activeTouches.ContainsKey(t.fingerId)) {
TryDeploy(activeTouches[t.fingerId]);
activeTouches.Remove(t.fingerId);
}
break;
}
}
 
//  MOUSE (Editor testing) 
Vector2 mp = TouchToCanvas(Input.mousePosition);
if (Input.GetMouseButtonDown(0)) {
isMouseDown = true;
activeTouches[MOUSE_ID] = new Vector4(mp.x, mp.y, mp.x, mp.y);
} else if (Input.GetMouseButton(0) && isMouseDown) {
if (activeTouches.ContainsKey(MOUSE_ID)) {
var v = activeTouches[MOUSE_ID];
activeTouches[MOUSE_ID] = new Vector4(v.x, v.y, mp.x, mp.y);
}
} else if (Input.GetMouseButtonUp(0) && isMouseDown) {
if (activeTouches.ContainsKey(MOUSE_ID)) {
TryDeploy(activeTouches[MOUSE_ID]);
activeTouches.Remove(MOUSE_ID);
}
isMouseDown = false;
}
 
// Pass active touches to renderer for dashed preview lines
radarDrawer.SetActiveTouches(activeTouches);
}
 
void TryDeploy(Vector4 touch) {
float dist = Mathf.Sqrt((touch.z-touch.x)*(touch.z-touch.x)+
(touch.w-touch.y)*(touch.w-touch.y));
if (dist > 30f)
firewallManager.DeployWall(touch.x, touch.y, touch.z, touch.w);
}
 
Vector2 TouchToCanvas(Vector2 screenPos) {
RectTransformUtility.ScreenPointToLocalPointInRectangle(
gameCanvasRect, screenPos, null, out Vector2 local);
float texX = local.x + gameCanvasRect.rect.width * 0.5f;
float texY = gameCanvasRect.rect.height * 0.5f - local.y;
return new Vector2(texX, texY);
}
}
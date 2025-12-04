using UnityEngine;
using UnityEditor;

public class bl_MiniMapHelperEditor : MonoBehaviour {

	[MenuItem("GameObject/UI/MiniMap/Item")]
    public static void AddItem()
    {
        GameObject go = Selection.activeGameObject;
        var m = go.AddComponent<bl_MiniMapEntity>();
        m.Target = go.transform;
    }
}
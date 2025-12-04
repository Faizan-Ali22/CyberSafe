using UnityEngine;
using Lovatto.MiniMap;

public class bl_MiniMapData : ScriptableObject
{
    public GameObject IconPrefab;
    public bl_MiniMapInputBase inputHandler;
    public bl_MiniMapIconData emptyIconData;
    public GameObject ScreenShotPrefab;
    public bl_MiniMapPlaneBase mapPlane;

    public static bl_MiniMapData _instance;
    public static bl_MiniMapData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<bl_MiniMapData>("MiniMapData") as bl_MiniMapData;
            }
            return _instance;
        }
    }
}
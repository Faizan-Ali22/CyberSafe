using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.SceneManagement;
using Lovatto.MiniMap;

[CustomEditor(typeof(bl_MiniMap))]
public class bl_MiniMapEditor : Editor
{
    AnimBool GeneralAnim;
    AnimBool ZoomAnim;
    AnimBool RotationAnim;
    AnimBool GripAnim;
    AnimBool PositionAnim;
    AnimBool AnimationsAnim;
    AnimBool DragAnim;
    AnimBool RenderAnim;
    AnimBool ReferencesAnim;
    AnimBool MarksAnim;
    SerializedProperty generalProp;
    SerializedProperty zoomProp;
    SerializedProperty positionProp;
    SerializedProperty rotationProp;
    SerializedProperty animationProp;
    SerializedProperty gripProp;
    SerializedProperty dragProp;
    SerializedProperty renderProp;
    SerializedProperty refProp;
    SerializedProperty marksProp;

    private void OnEnable()
    {
        generalProp = serializedObject.FindProperty("m_Target");
        InitAnim(ref GeneralAnim, generalProp);

        zoomProp = serializedObject.FindProperty("DefaultHeight");
        InitAnim(ref ZoomAnim, zoomProp);

        positionProp = serializedObject.FindProperty("FullMapPosition");
        InitAnim(ref PositionAnim, positionProp);

        rotationProp = serializedObject.FindProperty("mapShape");
        InitAnim(ref RotationAnim, rotationProp);

        gripProp = serializedObject.FindProperty("ShowAreaGrid");
        InitAnim(ref GripAnim, gripProp);

        animationProp = serializedObject.FindProperty("FadeOnFullScreen");
        InitAnim(ref AnimationsAnim, animationProp);

        dragProp = serializedObject.FindProperty("DragOnlyOnFullScreen");
        InitAnim(ref DragAnim, dragProp);

        renderProp = serializedObject.FindProperty("PlayerIconSprite");
        InitAnim(ref RenderAnim, renderProp);

        refProp = serializedObject.FindProperty("minimapRig");
        InitAnim(ref ReferencesAnim, refProp);

        marksProp = serializedObject.FindProperty("AllowMapMarks");
        InitAnim(ref MarksAnim, marksProp);
    }

    private void InitAnim(ref AnimBool anim, SerializedProperty prop)
    {
        anim = new AnimBool(prop.isExpanded);
        anim.valueChanged.AddListener(Repaint);
    }

    void CheckLayer(bl_MiniMap script)
    {
        string layer = LayerMask.LayerToName(script.MiniMapLayer);
        if (string.IsNullOrEmpty(layer))
        {
            CreateLayer("MiniMap");
            int layerID = LayerMask.NameToLayer("MiniMap");
            script.MiniMapLayer = layerID;
        }
    }

    public override void OnInspectorGUI()
    {
        bl_MiniMap script = (bl_MiniMap)target;
        bool allowSceneObjects = !EditorUtility.IsPersistent(target);
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();

        CheckLayer(script);
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical("window");
        EditorGUILayout.BeginVertical("box");
        if (GUILayout.Button("General Settings", EditorStyles.toolbarPopup)) { generalProp.isExpanded = !generalProp.isExpanded; GeneralAnim.target = generalProp.isExpanded; }
        if (EditorGUILayout.BeginFadeGroup(GeneralAnim.faded))
        {
            script.m_Target = EditorGUILayout.ObjectField("Target", script.m_Target, typeof(GameObject), allowSceneObjects) as GameObject;
            script.MiniMapLayer = EditorGUILayout.LayerField("MiniMap Layer", script.MiniMapLayer);
            if(script.renderType == bl_MiniMap.RenderType.RealTime)
            {
                script.useNonRenderLayer = EditorGUILayout.ToggleLeft("Use Not-Render layer", script.useNonRenderLayer, EditorStyles.toolbarButton);
                if (script.useNonRenderLayer)
                {
                    GUILayout.Space(2);
                    script.nonRenderLayer = EditorGUILayout.LayerField("Non Render Layers", script.nonRenderLayer);
                }
            }
            script.renderType = (bl_MiniMap.RenderType)EditorGUILayout.EnumPopup("Render Mode", script.renderType);
            script.canvasRenderMode = (bl_MiniMap.RenderMode)EditorGUILayout.EnumPopup("Draw Mode", script.canvasRenderMode);
            if (script.canvasRenderMode == bl_MiniMap.RenderMode.Mode2D)
            {
                script.Ortographic2D = EditorGUILayout.ToggleLeft("Orthographic", script.Ortographic2D, EditorStyles.toolbarButton);
                GUILayout.Space(2);
            }
            script.mapMode = (bl_MiniMap.MapType)EditorGUILayout.EnumPopup("Map Mode", script.mapMode);
            if (script.renderType == bl_MiniMap.RenderType.Picture)
            {
                GUILayout.Label("Map Render");
                GUILayout.BeginHorizontal();
                if (script.mapRender != null)
                {
                    GUILayout.Space(10);
                    var rrect = GUILayoutUtility.GetRect(50, 50);
                    script.mapRender.DrawOnGUI(rrect);
                }
                GUILayout.FlexibleSpace();
                script.mapRender = EditorGUILayout.ObjectField(script.mapRender, typeof(bl_MapRender), allowSceneObjects) as bl_MapRender;

                GUILayout.EndHorizontal();
                GUILayout.Space(10);
                if (script.mapBounds != null)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Render Map", GUILayout.Width(150)))
                    {
                        SetupScreenShot();
                    }
                    GUILayout.Space(5);
                    if (GUILayout.Button("Set Bounds", GUILayout.Width(75)))
                    {
                        Selection.activeTransform = script.mapBounds.BoundTransform;
                        EditorGUIUtility.PingObject(script.mapBounds.BoundTransform);
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.Space(2);
            script.isMobile = EditorGUILayout.ToggleLeft("Is Mobile", script.isMobile, EditorStyles.toolbarButton);
            script.UpdateRate = EditorGUILayout.IntSlider("Update Rate", script.UpdateRate, 1, 10);
        }
        EditorGUILayout.EndFadeGroup();
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");
        if (GUILayout.Button("Zoom Settings", EditorStyles.toolbarPopup)) { zoomProp.isExpanded = !zoomProp.isExpanded; ZoomAnim.target = zoomProp.isExpanded; }
        if (EditorGUILayout.BeginFadeGroup(ZoomAnim.faded))
        {
            EditorGUILayout.LabelField("Zoom Range", EditorStyles.label);
            EditorGUILayout.BeginHorizontal();
            script.MinZoom = EditorGUILayout.FloatField(script.MinZoom, GUILayout.Width(50));
            EditorGUILayout.MinMaxSlider(ref script.MinZoom, ref script.MaxZoom, 1, 200);
            script.MaxZoom = EditorGUILayout.FloatField(script.MaxZoom, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();
            script.DefaultHeight = EditorGUILayout.Slider("Default Zoom", script.DefaultHeight, script.MinZoom, script.MaxZoom);
            script.saveZoomInRuntime = EditorGUILayout.ToggleLeft("Save runtime zoom modifications?", script.saveZoomInRuntime, EditorStyles.toolbarButton);
            script.scrollSensitivity = EditorGUILayout.IntSlider("Zoom Steps", script.scrollSensitivity, 1, 10);
            script.IconMultiplier = EditorGUILayout.Slider("Icon Size Multiplier", script.IconMultiplier, 0.05f, 2);
            script.LerpHeight = EditorGUILayout.Slider("Zoom Speed", script.LerpHeight, 1, 20);
        }
        EditorGUILayout.EndFadeGroup();
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");
        if (GUILayout.Button("Position Settings", EditorStyles.toolbarPopup)) { positionProp.isExpanded = !positionProp.isExpanded; PositionAnim.target = positionProp.isExpanded; }
        if (EditorGUILayout.BeginFadeGroup(PositionAnim.faded))
        {
            script.lerpTrackingPosition = EditorGUILayout.ToggleLeft("Smooth Player Position Tracking?", script.lerpTrackingPosition, EditorStyles.toolbarButton);
            GUILayout.Space(2);
            script.fullScreenMode = (MiniMapFullScreenMode)EditorGUILayout.EnumPopup("Fullscreen Mode", script.fullScreenMode);
            if (script.fullScreenMode != MiniMapFullScreenMode.NoFullScreen)
            {              
                if (script.fullScreenMode == MiniMapFullScreenMode.ScreenArea)
                {
                    script.FullMapPosition = EditorGUILayout.Vector3Field("FullScreen Map Position", script.FullMapPosition);
                    script.FullMapSize = EditorGUILayout.Vector2Field("FullScreen Map Size", script.FullMapSize);
                }

                if (script.canvasRenderMode == bl_MiniMap.RenderMode.Mode3D)
                {
                    script.FullMapRotation = EditorGUILayout.Vector3Field("FullScreen Map Rotation", script.FullMapRotation);
                }

                if (script.fullScreenMode != MiniMapFullScreenMode.ScreenArea)
                {
                    script.fullScreenMargin = EditorGUILayout.Slider("Fullscreen Margin", script.fullScreenMargin, 0, 100);
                }
            }
        }
        if (script.fullScreenMode != MiniMapFullScreenMode.NoFullScreen)
        {
            if (script.fullScreenMode == MiniMapFullScreenMode.ScreenArea && positionProp.isExpanded)
            {
                if (GUILayout.Button("Catch Position"))
                {
                    script.GetFullMapSize();
                }

                if (script._isPreviewFullscreen)
                {
                    if (GUILayout.Button("Stop Fullscreen Preview"))
                    {
                        var ui = script.MiniMapUI;
                        if (ui != null)
                        {
                            ui.root.anchoredPosition = script.MiniMapPosition;
                            ui.root.sizeDelta = script.MiniMapSize;
                            ui.root.eulerAngles = script.MiniMapRotation;
                            ui.minimapMaskManager?.ChangeMaskType(false);
                        }
                        script._isPreviewFullscreen = false;
                        EditorUtility.SetDirty(script);
                    }
                }
                else
                {
                    if (GUILayout.Button("Preview Fullscreen"))
                    {
                        script.GetMiniMapSize();
                        var ui = script.MiniMapUI;
                        if (ui != null)
                        {
                            ui.root.anchoredPosition = script.FullMapPosition;
                            ui.root.sizeDelta = script.FullMapSize;
                            ui.root.eulerAngles = script.FullMapRotation;
                            ui.minimapMaskManager?.ChangeMaskType(true);
                        }
                        script._isPreviewFullscreen = true;
                        EditorUtility.SetDirty(script);
                    }
                }
            }
        }
        EditorGUILayout.EndFadeGroup();
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");
        if (GUILayout.Button("Rotation Settings", EditorStyles.toolbarPopup)) { rotationProp.isExpanded = !rotationProp.isExpanded; RotationAnim.target = rotationProp.isExpanded; }
        if (EditorGUILayout.BeginFadeGroup(RotationAnim.faded))
        {
            script.mapShape = (bl_MiniMap.MapShape)EditorGUILayout.EnumPopup("Shape", script.mapShape);
            if (script.mapShape == bl_MiniMap.MapShape.Circle)
            {
                script.CompassSize = EditorGUILayout.Slider("Circle Size", script.CompassSize, 25, 500);
            }
            script.DynamicRotation = EditorGUILayout.ToggleLeft("Rotate Map with player", script.DynamicRotation, EditorStyles.toolbarButton);
            if (script.canvasRenderMode == bl_MiniMap.RenderMode.Mode2D)
            {
                script.iconsAlwaysFacingUp = EditorGUILayout.ToggleLeft("Icons Always Facing Up", script.iconsAlwaysFacingUp, EditorStyles.toolbarButton);
            }
            else
            {
                script.iconsAlwaysFacingUp = EditorGUILayout.ToggleLeft("Icons Always Facing Up", script.iconsAlwaysFacingUp, EditorStyles.toolbarButton);
                // script.iconsAlwaysFacingUp = false;
            }
            script.SmoothRotation = EditorGUILayout.ToggleLeft("Smooth Rotation", script.SmoothRotation, EditorStyles.toolbarButton);
            if (script.SmoothRotation) { script.LerpRotation = EditorGUILayout.Slider("Rotation Lerp", script.LerpRotation, 1, 20); }
        }
        EditorGUILayout.EndFadeGroup();
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");
        if (GUILayout.Button("Grid Settings", EditorStyles.toolbarPopup)) { gripProp.isExpanded = !gripProp.isExpanded; GripAnim.target = gripProp.isExpanded; }
        if (EditorGUILayout.BeginFadeGroup(GripAnim.faded))
        {
            script.ShowAreaGrid = EditorGUILayout.ToggleLeft("Show Grid", script.ShowAreaGrid, EditorStyles.toolbarButton);
            if (script.ShowAreaGrid)
            {
                script.AreasSize = EditorGUILayout.Slider("Row Grid Size", script.AreasSize, 1, 25);
                script.gridOpacity = EditorGUILayout.Slider("Grid Opacity", script.gridOpacity, 0, 1);
            }
        }
        EditorGUILayout.EndFadeGroup();
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");
        if (GUILayout.Button("Map Pointers Settings", EditorStyles.toolbarPopup)) { marksProp.isExpanded = !marksProp.isExpanded; MarksAnim.target = marksProp.isExpanded; }
        if (EditorGUILayout.BeginFadeGroup(MarksAnim.faded))
        {
            script.AllowMapMarks = EditorGUILayout.ToggleLeft("Allow Map Pointers", script.AllowMapMarks, EditorStyles.toolbarButton);
            if (script.AllowMapMarks)
            {
                script.AllowMultipleMarks = EditorGUILayout.ToggleLeft("Allow multiples marks", script.AllowMultipleMarks, EditorStyles.toolbarButton);
                script.MapPointerPrefab = EditorGUILayout.ObjectField("Pointer Prefab", script.MapPointerPrefab, typeof(GameObject), allowSceneObjects) as GameObject;
            }
        }
        EditorGUILayout.EndFadeGroup();
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");
        if (GUILayout.Button("Drag Settings", EditorStyles.toolbarPopup)) { dragProp.isExpanded = !dragProp.isExpanded; DragAnim.target = dragProp.isExpanded; }
        if (EditorGUILayout.BeginFadeGroup(DragAnim.faded))
        {
            script.CanDragMiniMap = EditorGUILayout.ToggleLeft("Active Drag MiniMap", script.CanDragMiniMap, EditorStyles.toolbarButton);
            if (script.CanDragMiniMap)
            {
                script.DragOnlyOnFullScreen = EditorGUILayout.ToggleLeft("Only on full screen", script.DragOnlyOnFullScreen, EditorStyles.toolbarButton);
                script.ResetOffSetOnChange = EditorGUILayout.ToggleLeft("Auto reset position", script.ResetOffSetOnChange, EditorStyles.toolbarButton);
                var lw = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 100;
                EditorGUILayout.BeginHorizontal();
                Vector2 v = script.DragMovementSpeed;
                v.x = EditorGUILayout.Slider("Horizontal Speed", v.x, 0.01f, 30);
                v.y = EditorGUILayout.Slider("Vertical Speed", v.y, 0.01f, 30);
                script.DragMovementSpeed = v;
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                Vector2 v2 = script.MaxOffSetPosition;
                v2.x = EditorGUILayout.Slider("MinMax Horizontal", v2.x, 1, 2000);
                v2.y = EditorGUILayout.Slider("MinMax Vertical", v2.y, 1, 2000);
                script.MaxOffSetPosition = v2;
                EditorGUILayout.EndHorizontal();
                script.DragCursorIcon = EditorGUILayout.ObjectField("Drag cursor image", script.DragCursorIcon, typeof(Texture2D), allowSceneObjects) as Texture2D;
                EditorGUILayout.BeginHorizontal();
                Vector2 v3 = script.HotSpot;
                v3.x = EditorGUILayout.Slider("Cursor X offset", v3.x, 0.01f, 10);
                v3.y = EditorGUILayout.Slider("Cursor Y offset", v3.y, 0.01f, 10);
                script.HotSpot = v3;
                EditorGUILayout.EndHorizontal();
                EditorGUIUtility.labelWidth = lw;
            }
        }
        EditorGUILayout.EndFadeGroup();
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");
        if (GUILayout.Button("Animations Settings", EditorStyles.toolbarPopup)) { rotationProp.isExpanded = !rotationProp.isExpanded; AnimationsAnim.target = rotationProp.isExpanded; }
        if (EditorGUILayout.BeginFadeGroup(AnimationsAnim.faded))
        {
            script.FadeOnFullScreen = EditorGUILayout.ToggleLeft("Fade on full screen", script.FadeOnFullScreen, EditorStyles.toolbarButton);
            script.sizeTransitionDuration = EditorGUILayout.Slider("Resize Transition Duration", script.sizeTransitionDuration, 0.1f, 2);
            script.sizeTransitionCurve = EditorGUILayout.CurveField("Resize Transition Curve", script.sizeTransitionCurve);
        }
        EditorGUILayout.EndFadeGroup();
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");
        if (GUILayout.Button("Render Settings", EditorStyles.toolbarPopup)) { renderProp.isExpanded = !renderProp.isExpanded; RenderAnim.target = renderProp.isExpanded; }
        if (EditorGUILayout.BeginFadeGroup(RenderAnim.faded))
        {
            script.PlayerIconSprite = EditorGUILayout.ObjectField("Player Icon", script.PlayerIconSprite, typeof(Sprite), false) as Sprite;
            script.playerColor = EditorGUILayout.ColorField("Player Color", script.playerColor);
            script.overallOpacity = EditorGUILayout.Slider("MiniMap Opacity", script.overallOpacity, 0, 1);
            if (script.renderType == bl_MiniMap.RenderType.Picture)
            script.planeSaturation = EditorGUILayout.Slider("Map Saturation", script.planeSaturation, 0.2f, 2);
        }
        EditorGUILayout.EndFadeGroup();
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");
        if (GUILayout.Button("References", EditorStyles.toolbarPopup)) { refProp.isExpanded = !refProp.isExpanded; ReferencesAnim.target = refProp.isExpanded; }
        if (EditorGUILayout.BeginFadeGroup(ReferencesAnim.faded))
        {
            script.minimapRig = EditorGUILayout.ObjectField("Mini Map Rig", script.minimapRig, typeof(Transform), allowSceneObjects) as Transform;
            script.miniMapCamera = EditorGUILayout.ObjectField("Mini Map Camera", script.miniMapCamera, typeof(Camera), allowSceneObjects) as Camera;
            script.ItemPrefabSimple = EditorGUILayout.ObjectField("Icon Simple Prefab", script.ItemPrefabSimple, typeof(GameObject), allowSceneObjects) as GameObject;

            script.mapBounds = EditorGUILayout.ObjectField("Map Bounds", script.mapBounds, typeof(bl_MiniMapBounds), allowSceneObjects) as bl_MiniMapBounds;
            script.m_Canvas = EditorGUILayout.ObjectField("Canvas", script.m_Canvas, typeof(Canvas), allowSceneObjects) as Canvas;
        }
        EditorGUILayout.EndFadeGroup();
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndVertical();
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(script);

            script.OnValidate();
        }
    }

    public void CreateLayer(string name)
    {
        if (string.IsNullOrEmpty(name))
            throw new System.ArgumentNullException("name", "New layer name string is either null or empty.");

        var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        var layerProps = tagManager.FindProperty("layers");
        var propCount = layerProps.arraySize;

        SerializedProperty firstEmptyProp = null;

        for (var i = 0; i < propCount; i++)
        {
            var layerProp = layerProps.GetArrayElementAtIndex(i);

            var stringValue = layerProp.stringValue;

            if (stringValue == name) return;

            if (i < 8 || stringValue != string.Empty) continue;

            if (firstEmptyProp == null)
                firstEmptyProp = layerProp;
        }

        if (firstEmptyProp == null)
        {
            UnityEngine.Debug.LogError("Maximum limit of " + propCount + " layers exceeded. Layer \"" + name + "\" not created.");
            return;
        }

        firstEmptyProp.stringValue = name;
        tagManager.ApplyModifiedProperties();
    }

    void SetupScreenShot()
    {
        GameObject g = PrefabUtility.InstantiatePrefab(bl_MiniMapData.Instance.ScreenShotPrefab, EditorSceneManager.GetActiveScene()) as GameObject;
        g.GetComponent<bl_MiniMapRenderTool>().SetMiniMap((bl_MiniMap)target);
        Selection.activeGameObject = g;
        EditorGUIUtility.PingObject(g);
        g.transform.SetAsLastSibling();
    }
}
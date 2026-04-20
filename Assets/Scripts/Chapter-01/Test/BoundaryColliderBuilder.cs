using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class BoundaryColliderBuilder : MonoBehaviour
{
    [Header("Reference (Use one or many)")]
    [Tooltip("Optional single renderer")]
    public Renderer referenceRenderer;

    [Tooltip("Use this for multiple meshes/renderers")]
    public List<Renderer> referenceRenderers = new List<Renderer>();

    [Header("Wall Settings")]
    [Min(0.01f)] public float wallHeight = 2f;
    [Min(0.01f)] public float wallThickness = 0.5f;
    [Tooltip("Extra inward/outward padding to avoid tiny gaps")]
    public float padding = 0.05f;

    [Tooltip("Y center of collider walls. If disabled, uses reference center.y + wallHeight/2")]
    public bool useYOverride = false;
    public float yCenterOverride = 0f;

    [Header("Editor Behavior")]
    [Tooltip("If true, inspector changes auto-rebuild colliders in Edit mode")]
    public bool autoRebuildInEditor = true;

    [Tooltip("Clear previously generated child colliders before rebuild")]
    public bool clearOldFirst = true;

    [Tooltip("Show gizmo bounds in Scene view")]
    public bool showGizmos = true;

    [Tooltip("Show padded bounds (orange)")]
    public bool showPaddedGizmo = true;

    [ContextMenu("Build Boundary Colliders")]
    public void BuildBoundaryColliders()
    {
        if (!TryGetCombinedBounds(out Bounds b))
        {
            Debug.LogWarning("[BoundaryColliderBuilder] No valid renderer assigned.", this);
            return;
        }

        if (clearOldFirst) ClearChildren();

        float yCenter = useYOverride ? yCenterOverride : (b.center.y + wallHeight * 0.5f);

        float minX = b.min.x - padding;
        float maxX = b.max.x + padding;
        float minZ = b.min.z - padding;
        float maxZ = b.max.z + padding;

        float sizeX = maxX - minX;
        float sizeZ = maxZ - minZ;

        // Left wall (X-)
        CreateWall(
            "Boundary_Left",
            new Vector3(minX - wallThickness * 0.5f, yCenter, b.center.z),
            new Vector3(wallThickness, wallHeight, sizeZ + wallThickness * 2f)
        );

        // Right wall (X+)
        CreateWall(
            "Boundary_Right",
            new Vector3(maxX + wallThickness * 0.5f, yCenter, b.center.z),
            new Vector3(wallThickness, wallHeight, sizeZ + wallThickness * 2f)
        );

        // Bottom wall (Z-)
        CreateWall(
            "Boundary_Bottom",
            new Vector3(b.center.x, yCenter, minZ - wallThickness * 0.5f),
            new Vector3(sizeX + wallThickness * 2f, wallHeight, wallThickness)
        );

        // Top wall (Z+)
        CreateWall(
            "Boundary_Top",
            new Vector3(b.center.x, yCenter, maxZ + wallThickness * 0.5f),
            new Vector3(sizeX + wallThickness * 2f, wallHeight, wallThickness)
        );
    }

    [ContextMenu("Clear Boundary Colliders")]
    public void ClearChildren()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform c = transform.GetChild(i);
            if (Application.isPlaying) Destroy(c.gameObject);
            else DestroyImmediate(c.gameObject);
        }
    }

    private void CreateWall(string wallName, Vector3 worldPos, Vector3 worldSize)
    {
        GameObject go = new GameObject(wallName);
        go.transform.SetParent(transform, true);
        go.transform.position = worldPos;
        go.transform.rotation = Quaternion.identity;
        go.layer = gameObject.layer;

        BoxCollider bc = go.AddComponent<BoxCollider>();
        bc.isTrigger = false;
        bc.size = worldSize;
    }

    private bool TryGetCombinedBounds(out Bounds combined)
    {
        bool hasAny = false;
        combined = default;

        if (referenceRenderer != null)
        {
            combined = referenceRenderer.bounds;
            hasAny = true;
        }

        if (referenceRenderers != null)
        {
            for (int i = 0; i < referenceRenderers.Count; i++)
            {
                Renderer r = referenceRenderers[i];
                if (r == null) continue;

                if (!hasAny)
                {
                    combined = r.bounds;
                    hasAny = true;
                }
                else
                {
                    combined.Encapsulate(r.bounds);
                }
            }
        }

        return hasAny;
    }

    private void OnValidate()
    {
        if (!enabled) return;
        if (Application.isPlaying) return;
        if (!autoRebuildInEditor) return;

        BuildBoundaryColliders();
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;
        if (!TryGetCombinedBounds(out Bounds b)) return;

        // Base combined bounds (yellow)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(b.center, b.size);

        if (showPaddedGizmo)
        {
            float minX = b.min.x - padding;
            float maxX = b.max.x + padding;
            float minZ = b.min.z - padding;
            float maxZ = b.max.z + padding;

            Vector3 paddedCenter = new Vector3((minX + maxX) * 0.5f, b.center.y, (minZ + maxZ) * 0.5f);
            Vector3 paddedSize = new Vector3(maxX - minX, b.size.y, maxZ - minZ);

            Gizmos.color = new Color(1f, 0.6f, 0f, 1f); // orange
            Gizmos.DrawWireCube(paddedCenter, paddedSize);
        }
    }
}
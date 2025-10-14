using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor.AnimatedValues;
#endif

namespace DA_Assets.DAI
{
    public enum GroupType
    {
        Horizontal = 0,
        Vertical = 1,
        Fade = 2
    }

    public struct Group
    {
        public int InstanceId { get; set; }
        public GroupType GroupType { get; set; }
        public Action Body { get; set; }
        public GUIStyle Style { get; set; }
        public GUILayoutOption[] Options { get; set; }
        public bool Flexible { get; set; }
        public bool Scroll { get; set; }
        public int SplitterWidth { get; set; }
        public int SplitterStartPos { get; set; }

#if UNITY_EDITOR
        public AnimBool Fade { get; set; }
#endif
    }

    public class GroupData
    {
        public Vector2 ScrollPosition { get; set; } = Vector2.zero;
        public float SplitterPosition { get; set; }
        public Rect SplitterRect { get; set; }
        public bool IsDragging { get; set; }
    }
}
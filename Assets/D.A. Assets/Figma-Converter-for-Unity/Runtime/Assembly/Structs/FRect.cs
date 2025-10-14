using System;
using UnityEngine;

namespace DA_Assets.FCU
{
    [Serializable]
    public struct FRect
    {
        [SerializeField] Vector2 _position;
        [SerializeField] Vector2 _size;
        [SerializeField] RectOffsetCustom _padding;
        [SerializeField] float _angle;
        [SerializeField] float _absoluteAngle;

        public Vector2 position { get => _position; set => _position = value; }
        public Vector2 size { get => _size; set => _size = value; }
        public float angle { get => _angle; set => _angle = value; }
        public float absoluteAngle { get => _absoluteAngle; set => _absoluteAngle = value; }
        public RectOffsetCustom padding { get => _padding; set => _padding = value; }
    }
}
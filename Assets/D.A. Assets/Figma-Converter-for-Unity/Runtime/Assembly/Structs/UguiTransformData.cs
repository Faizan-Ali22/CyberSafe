using System;
using UnityEngine;

namespace DA_Assets.FCU
{
    [Serializable]
    public struct UguiTransformData
    {
        [SerializeField] Vector2 anchorMin;
        public Vector2 AnchorMin { get => anchorMin; set => anchorMin = value; }
        [SerializeField] Vector2 anchorMax;
        public Vector2 AnchorMax { get => anchorMax; set => anchorMax = value; }
        [SerializeField] Vector2 anchoredPosition;
        public Vector2 AnchoredPosition { get => anchoredPosition; set => anchoredPosition = value; }
        [SerializeField] Vector3 sizeDelta;
        public Vector3 SizeDelta { get => sizeDelta; set => sizeDelta = value; }
        [SerializeField] Quaternion localRotation;
        public Quaternion LocalRotation { get => localRotation; set => localRotation = value; }
        [SerializeField] Vector3 localPosition;
        public Vector3 LocalPosition { get => localPosition; set => localPosition = value; }
        [SerializeField] Vector3 localScale;
        public Vector3 LocalScale { get => localScale; set => localScale = value; }

        UguiTransformData(RectTransform source)
        {
            anchorMin = source.anchorMin;
            anchorMax = source.anchorMax;
            anchoredPosition = source.anchoredPosition;
            sizeDelta = source.sizeDelta;
            localRotation = source.localRotation;
            localScale = source.localScale;
            localPosition = source.localPosition;
        }

        public static UguiTransformData Create(RectTransform source)
        {
            return new UguiTransformData(source);
        }

        public void ApplyTo(RectTransform target)
        {
            target.anchorMin = anchorMin;
            target.anchorMax = anchorMax;
            target.anchoredPosition = anchoredPosition;
            target.sizeDelta = sizeDelta;
            target.localRotation = localRotation;
            target.localScale = localScale;
        }
    }
}
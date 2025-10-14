using System;
using UnityEngine;

namespace DA_Assets.DAI
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public abstract class PropertyHeader : PropertyAttribute
    {
        public GUIContent HeaderLabel { get; protected set; }

        protected PropertyHeader(GUIContent headerLabel)
        {
            HeaderLabel = headerLabel;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public abstract class CustomInspectorProperty : PropertyAttribute
    {
        public ComponentType Type { get; protected set; }
        public GUIContent Label { get; protected set; }
        public float MinValue { get; protected set; }
        public float MaxValue { get; protected set; }

        protected CustomInspectorProperty(ComponentType type, GUIContent label, float minValue = 0, float maxValue = 1)
        {
            Type = type;
            Label = label;
            MinValue = minValue;
            MaxValue = maxValue;
        }
    }

    public enum ComponentType
    {
        Toggle,
        EnumField,
        FloatField,
        Vector4Field,
        TextField,
        IntField,
        Vector2Field,
        Vector2IntField,
        ColorField,
        SliderField
    }

    [Serializable]
    public class UpdateBool
    {
        [SerializeField] bool _value;
        [SerializeField] bool _temp;

        public UpdateBool(bool value, bool temp)
        {
            _value = value;
            _temp = temp;
        }

        public bool Value { get => _value; set => _value = value; }
        public bool Temp { get => _temp; set => _temp = value; }
    }
}
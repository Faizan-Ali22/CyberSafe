using DA_Assets.DAI;
using System;
using UnityEngine;

namespace DA_Assets.FCU
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class FcuInspectorProperty : CustomInspectorProperty
    {
        public FcuInspectorProperty(
            ComponentType type,
            FcuLocKey label,
            FcuLocKey tooltip, 
            float minValue = 0,
            float maxValue = 1) : base(type, new GUIContent(GetLabelFromEnum(label), GetLabelFromEnum(tooltip)), minValue, maxValue)
        {

        }

        private static string GetLabelFromEnum(FcuLocKey value)
        {
            return value.Localize();
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public class FcuPropertyHeader : PropertyHeader
    {
        public FcuPropertyHeader(
            FcuLocKey label,
            FcuLocKey tooltip) : base(new GUIContent(GetLabelFromEnum(label), GetLabelFromEnum(tooltip)))
        {

        }

        private static string GetLabelFromEnum(FcuLocKey value)
        {
            return value.Localize();
        }
    }
}
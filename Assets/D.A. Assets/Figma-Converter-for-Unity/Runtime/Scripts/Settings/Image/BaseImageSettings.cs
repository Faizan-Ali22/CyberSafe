using DA_Assets.DAI;
using System;
using UnityEngine;

namespace DA_Assets.FCU.Model
{
    [Serializable]
    public class BaseImageSettings : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        [SerializeField] UnityEngine.UI.Image.Type type = UnityEngine.UI.Image.Type.Simple;
        [FcuInspectorProperty(ComponentType.EnumField, FcuLocKey.label_image_type, FcuLocKey.tooltip_image_type)]
        public UnityEngine.UI.Image.Type Type { get => type; set => SetValue(ref type, value); }

        [SerializeField] bool raycastTarget = true;
        [FcuInspectorProperty(ComponentType.Toggle, FcuLocKey.label_raycast_target, FcuLocKey.tooltip_raycast_target)]
        public bool RaycastTarget { get => raycastTarget; set => SetValue(ref raycastTarget, value); }

        [SerializeField] bool preserveAspect = true;
        [FcuInspectorProperty(ComponentType.Toggle, FcuLocKey.label_preserve_aspect, FcuLocKey.tooltip_preserve_aspect)]
        public bool PreserveAspect { get => preserveAspect; set => SetValue(ref preserveAspect, value); }

        [SerializeField] Vector4 raycastPadding = new Vector4(0, 0, 0, 0);
        [FcuInspectorProperty(ComponentType.Vector4Field, FcuLocKey.label_raycast_padding, FcuLocKey.tooltip_raycast_padding)]
        public Vector4 RaycastPadding { get => raycastPadding; set => SetValue(ref raycastPadding, value); }

        [SerializeField] bool maskable = true;
        [FcuInspectorProperty(ComponentType.Toggle, FcuLocKey.label_maskable, FcuLocKey.tooltip_maskable)]
        public bool Maskable { get => maskable; set => SetValue(ref maskable, value); }
    }
}
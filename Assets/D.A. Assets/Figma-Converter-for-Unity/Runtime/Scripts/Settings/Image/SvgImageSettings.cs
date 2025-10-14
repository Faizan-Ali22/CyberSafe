using DA_Assets.DAI;
using System;
using UnityEngine;

#if VECTOR_GRAPHICS_EXISTS
using Unity.VectorGraphics;
#endif

#if VECTOR_GRAPHICS_EXISTS && UNITY_EDITOR
using Unity.VectorGraphics.Editor;
#endif

namespace DA_Assets.FCU.Model
{
    [Serializable]
    [FcuPropertyHeader(FcuLocKey.label_svg_image_settings, FcuLocKey.tooltip_svg_image_settings)]
    public class SvgImageSettings : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        [SerializeField] bool raycastTarget = true;
        [FcuInspectorProperty(ComponentType.Toggle, FcuLocKey.label_raycast_target, FcuLocKey.tooltip_raycast_target)]
        public bool RaycastTarget { get => raycastTarget; set => SetValue(ref raycastTarget, value); }

        [SerializeField] bool preserveAspect = true;
        [FcuInspectorProperty(ComponentType.Toggle, FcuLocKey.label_preserve_aspect, FcuLocKey.tooltip_preserve_aspect)]
        public bool PreserveAspect { get => preserveAspect; set => SetValue(ref preserveAspect, value); }

        [SerializeField] Vector4 raycastPadding = new Vector4(0, 0, 0, 0);
        [FcuInspectorProperty(ComponentType.Vector4Field, FcuLocKey.label_raycast_padding, FcuLocKey.tooltip_raycast_padding)]
        public Vector4 RaycastPadding { get => raycastPadding; set => SetValue(ref raycastPadding, value); }
    }


    [Serializable]
    [FcuPropertyHeader(FcuLocKey.label_svg_importer_settings, FcuLocKey.tooltip_svg_importer_settings)]
    public class SVGImporterSettings : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
#if VECTOR_GRAPHICS_EXISTS && UNITY_EDITOR
        [SerializeField] SVGType svgType = SVGType.VectorSprite;
        [FcuInspectorProperty(ComponentType.EnumField, FcuLocKey.label_svg_type, FcuLocKey.tooltip_svg_type)]
        public SVGType SvgType { get => svgType; set => SetValue(ref svgType, value); }
#endif
        [SerializeField] int gradientResolution = 64;
        [FcuInspectorProperty(ComponentType.IntField, FcuLocKey.label_gradient_resolution, FcuLocKey.tooltip_gradient_resolution)]
        public int GradientResolution { get => gradientResolution; set => SetValue(ref gradientResolution, value); }

        [SerializeField] Vector2 customPivot = new Vector2(0.5f, 0.5f);
        [FcuInspectorProperty(ComponentType.Vector2Field, FcuLocKey.label_custom_pivot, FcuLocKey.tooltip_custom_pivot)]
        public Vector2 CustomPivot { get => customPivot; set => SetValue(ref customPivot, value); }

        [SerializeField] bool generatePhysicsShape = false;
        [FcuInspectorProperty(ComponentType.Toggle, FcuLocKey.label_generate_physics_shape, FcuLocKey.tooltip_generate_physics_shape)]
        public bool GeneratePhysicsShape { get => generatePhysicsShape; set => SetValue(ref generatePhysicsShape, value); }

#if UNITY_EDITOR && VECTOR_GRAPHICS_EXISTS
        [SerializeField] Unity.VectorGraphics.ViewportOptions viewportOptions = Unity.VectorGraphics.ViewportOptions.DontPreserve;
        [FcuInspectorProperty(ComponentType.EnumField, FcuLocKey.label_viewport_options, FcuLocKey.tooltip_viewport_options)]
        public ViewportOptions ViewportOptions { get => viewportOptions; set => SetValue(ref viewportOptions, value); }
#endif

        [SerializeField] float stepDistance = 1f;
        [FcuInspectorProperty(ComponentType.FloatField, FcuLocKey.label_step_distance, FcuLocKey.tooltip_step_distance)]
        public float StepDistance { get => stepDistance; set => SetValue(ref stepDistance, value); }

        [SerializeField] float samplingSteps = 3;
        [FcuInspectorProperty(ComponentType.FloatField, FcuLocKey.label_sampling_steps, FcuLocKey.tooltip_sampling_steps)]
        public float SamplingSteps { get => samplingSteps; set => SetValue(ref samplingSteps, value); }

        [SerializeField] bool advancedMode = true;
        [FcuInspectorProperty(ComponentType.Toggle, FcuLocKey.label_advanced_mode, FcuLocKey.tooltip_advanced_mode)]
        public bool AdvancedMode { get => advancedMode; set => SetValue(ref advancedMode, value); }

        [SerializeField] bool maxCordDeviationEnabled = false;
        [FcuInspectorProperty(ComponentType.Toggle, FcuLocKey.label_max_cord_deviation_enabled, FcuLocKey.tooltip_max_cord_deviation_enabled)]
        public bool MaxCordDeviationEnabled { get => maxCordDeviationEnabled; set => SetValue(ref maxCordDeviationEnabled, value); }

        [SerializeField] float maxCordDeviation = 1;
        [FcuInspectorProperty(ComponentType.FloatField, FcuLocKey.label_max_cord_deviation, FcuLocKey.tooltip_max_cord_deviation)]
        public float MaxCordDeviation { get => maxCordDeviation; set => SetValue(ref maxCordDeviation, value); }

        [SerializeField] bool maxTangentAngleEnabled = false;
        [FcuInspectorProperty(ComponentType.Toggle, FcuLocKey.label_max_tangent_angle_enabled, FcuLocKey.tooltip_max_tangent_angle_enabled)]
        public bool MaxTangentAngleEnabled { get => maxTangentAngleEnabled; set => SetValue(ref maxTangentAngleEnabled, value); }

        [SerializeField] float maxTangentAngle = 5;
        [FcuInspectorProperty(ComponentType.FloatField, FcuLocKey.label_max_tangent_angle, FcuLocKey.tooltip_max_tangent_angle)]
        public float MaxTangentAngle { get => maxTangentAngle; set => SetValue(ref maxTangentAngle, value); }
    }
}
using DA_Assets.DAI;
using System;
using UnityEngine;

namespace DA_Assets.FCU.Model
{
    [Serializable]
    [FcuPropertyHeader(FcuLocKey.label_sr_settings, FcuLocKey.tooltip_sr_settings)]
    public class SpriteRendererSettings : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        [SerializeField] bool flipX = false;
        [FcuInspectorProperty(ComponentType.Toggle, FcuLocKey.label_flip_x, FcuLocKey.tooltip_flip_x)]
        public bool FlipX { get => flipX; set => SetValue(ref flipX, value); }

        [SerializeField] bool flipY = false;
        [FcuInspectorProperty(ComponentType.Toggle, FcuLocKey.label_flip_y, FcuLocKey.tooltip_flip_y)]
        public bool FlipY { get => flipY; set => SetValue(ref flipY, value); }

        [SerializeField] SpriteMaskInteraction maskInteraction = SpriteMaskInteraction.None;
        [FcuInspectorProperty(ComponentType.EnumField, FcuLocKey.label_mask_interaction, FcuLocKey.tooltip_mask_interaction)]
        public SpriteMaskInteraction MaskInteraction { get => maskInteraction; set => SetValue(ref maskInteraction, value); }

        [SerializeField] SpriteSortPoint sortPoint = SpriteSortPoint.Center;
        [FcuInspectorProperty(ComponentType.EnumField, FcuLocKey.label_sort_point, FcuLocKey.tooltip_sort_point)]
        public SpriteSortPoint SortPoint { get => sortPoint; set => SetValue(ref sortPoint, value); }

        [SerializeField] string sortingLayer = "Default";
        [FcuInspectorProperty(ComponentType.TextField, FcuLocKey.label_sorting_layer, FcuLocKey.tooltip_sorting_layer)]
        public string SortingLayer { get => sortingLayer; set => SetValue(ref sortingLayer, value); }

        [SerializeField] int nextOrderStep = 10;
        [FcuInspectorProperty(ComponentType.IntField, FcuLocKey.label_next_order_step, FcuLocKey.tooltip_next_order_step)]
        public int NextOrderStep { get => nextOrderStep; set => SetValue(ref nextOrderStep, value); }
    }
}
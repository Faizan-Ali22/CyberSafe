using DA_Assets.FCU.Model;
using DA_Assets.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace DA_Assets.FCU.Extensions
{
    public static class TransformExtensions
    {
        public static List<Transform> GetAllParents(this Transform child)
        {
            List<Transform> parents = new List<Transform>();
            Transform currentParent = child.parent;

            while (currentParent != null)
            {
                parents.Add(currentParent);
                currentParent = currentParent.parent;
            }

            return parents;
        }

        public static float GetAngleFromMatrix(this FObject fobject)
        {
            float rotRad;
            float rotDeg;
            float a;
            float b;

            if (fobject.RelativeTransform.IsEmpty() ||
                fobject.RelativeTransform.Count < 2 ||
                fobject.RelativeTransform[1].Count < 2 ||
                fobject.RelativeTransform[1][0] == null ||
                fobject.RelativeTransform[1][1] == null)
            {
                //FcuLogger.Debug($"{nameof(GetAngleFromMatrix)} | {fobject.Data.NameHierarchy} | wrong relative transform.", FcuLogType.Error);
                return 0;
            }
            else
            {
                a = (float)fobject.RelativeTransform[1][0];
                b = (float)fobject.RelativeTransform[1][1];
                rotRad = Mathf.Atan2(a, b).Round(FcuConfig.Rounding.GetAngleFromMatrixDigits);
            }

            rotRad = -1 * rotRad;
            rotDeg = rotRad * (180f / (float)Mathf.PI);

            FcuLogger.Debug($"{nameof(GetAngleFromMatrix)} | {fobject.Data.NameHierarchy} | rotDeg: {rotDeg}\nb: {b}\na: {a}\nrotRad: {rotRad}", FcuDebugSettingsFlags.LogAngle);

            return rotDeg;
        }

        public static float GetAngleFromField(this FObject fobject)
        {
            float rotRad;
            float rotDeg;
            float a = 0f;
            float b = 0f;

            rotRad = fobject.Rotation.HasValue ? fobject.Rotation.Value.Round(FcuConfig.Rounding.GetAngleFromMatrixDigits) : 0;
            rotRad = -1 * rotRad;
            rotDeg = rotRad * (180f / (float)Mathf.PI);

            FcuLogger.Debug($"{nameof(GetAngleFromField)} | {fobject.Data.NameHierarchy} | hasValue: {fobject.Rotation.HasValue} | rotDeg: {rotDeg}\nb: {b}\na: {a}\nrotRad: {rotRad}", FcuDebugSettingsFlags.LogAngle);

            return rotDeg;
        }

        public static float GetFigmaRotationAngle(this FObject fobject)
        {
            float angle = fobject.GetAngleFromField();

            if (angle == 0)
                angle = fobject.GetAngleFromMatrix();

            return angle;
        }

        public static AnchorType GetFigmaAnchor(this FObject fobject)
        {
            string anchor = fobject.Constraints.Vertical + " " + fobject.Constraints.Horizontal;

            AnchorType anchorPreset;

            switch (anchor)
            {
                ////////////////LEFT////////////////
                case "TOP LEFT":
                    anchorPreset = AnchorType.TopLeft;
                    break;
                case "BOTTOM LEFT":
                    anchorPreset = AnchorType.BottomLeft;
                    break;
                case "TOP_BOTTOM LEFT":
                    anchorPreset = AnchorType.VertStretchLeft;
                    break;
                case "CENTER LEFT":
                    anchorPreset = AnchorType.MiddleLeft;
                    break;
                case "SCALE LEFT":
                    anchorPreset = AnchorType.VertStretchLeft;
                    break;
                ////////////////RIGHT////////////////
                case "TOP RIGHT":
                    anchorPreset = AnchorType.TopRight;
                    break;
                case "BOTTOM RIGHT":
                    anchorPreset = AnchorType.BottomRight;
                    break;
                case "TOP_BOTTOM RIGHT":
                    anchorPreset = AnchorType.VertStretchRight;
                    break;
                case "CENTER RIGHT":
                    anchorPreset = AnchorType.MiddleRight;
                    break;
                case "SCALE RIGHT":
                    anchorPreset = AnchorType.VertStretchRight;
                    break;
                ////////////////LEFT_RIGHT////////////////
                case "TOP LEFT_RIGHT":
                    anchorPreset = AnchorType.HorStretchTop;
                    break;
                case "BOTTOM LEFT_RIGHT":
                    anchorPreset = AnchorType.HorStretchBottom;
                    break;
                case "TOP_BOTTOM LEFT_RIGHT":
                    anchorPreset = AnchorType.StretchAll;
                    break;
                case "CENTER LEFT_RIGHT":
                    anchorPreset = AnchorType.HorStretchMiddle;
                    break;
                case "SCALE LEFT_RIGHT":
                    anchorPreset = AnchorType.HorStretchMiddle;
                    break;
                ////////////////CENTER////////////////
                case "TOP CENTER":
                    anchorPreset = AnchorType.TopCenter;
                    break;
                case "BOTTOM CENTER":
                    anchorPreset = AnchorType.BottomCenter;
                    break;
                case "TOP_BOTTOM CENTER":
                    anchorPreset = AnchorType.VertStretchCenter;
                    break;
                case "CENTER CENTER":
                    anchorPreset = AnchorType.MiddleCenter;
                    break;
                case "SCALE CENTER":
                    anchorPreset = AnchorType.StretchAll;
                    break;
                ////////////////SCALE////////////////
                case "TOP SCALE":
                    anchorPreset = AnchorType.HorStretchTop;
                    break;
                case "BOTTOM SCALE":
                    anchorPreset = AnchorType.HorStretchBottom;
                    break;
                case "TOP_BOTTOM SCALE":
                    anchorPreset = AnchorType.VertStretchCenter;
                    break;
                case "CENTER SCALE":
                    anchorPreset = AnchorType.StretchAll;
                    break;
                case "SCALE SCALE":
                    anchorPreset = AnchorType.StretchAll;
                    break;
                ////////////////DEFAULT////////////////
                default:
                    anchorPreset = AnchorType.MiddleCenter;
                    break;
            }

            return anchorPreset;
        }
    }
}

using DA_Assets.FCU.Model;
using DA_Assets.Extensions;
using System.Linq;

namespace DA_Assets.FCU.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsAnyEffectInChildren(this FObject fobject)
        {
            if (fobject.Effects != null && fobject.Effects.Any(effect => effect.IsVisible()))
            {
                return true;
            }

            if (fobject.Children != null)
            {
                foreach (var child in fobject.Children)
                {
                    if (child.IsAnyEffectInChildren())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool IsAnyImageOrVideoOrEmojiTypeInChildren(this FObject fobject)
        {
            if (fobject.IsAnyImageOrVideoOrEmojiType())
            {
                return true;
            }

            if (fobject.Children != null)
            {
                foreach (var child in fobject.Children)
                {
                    if (child.IsAnyImageOrVideoOrEmojiTypeInChildren())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool IsShadowType(this Effect effect) => effect.Type.ToString().Contains("SHADOW");
        public static bool IsBlurType(this Effect effect) => effect.Type.ToString().Contains("BLUR");

        public static bool IsGradientType(this Paint paint) => paint.Type.ToString().Contains("GRADIENT");

        public static bool IsAnyImageOrVideoOrEmojiType(this FObject fobject)
        {
            if (fobject.Fills.IsEmpty())
                return false;

            return fobject.Fills.Any(fill =>
                fill.IsVisible() &&
                (fill.Type == PaintType.IMAGE ||
                 fill.Type == PaintType.VIDEO ||
                 fill.Type == PaintType.EMOJI));
        }

        public static bool IsSingleImageOrVideoOrEmojiType(this FObject fobject)
        {
            bool hasImageOrVideo = fobject.IsAnyImageOrVideoOrEmojiType();

            if (!hasImageOrVideo)
                return false;

            return fobject.Fills.Count(x => x.IsVisible()) == 1;
        }

        public static bool IsAnyMask(this FObject fobject) => fobject.IsObjectMask() || fobject.IsClipMask() || fobject.IsFrameMask();
        public static bool IsFrameMask(this FObject fobject) => fobject.ContainsTag(FcuTag.Frame);
        public static bool IsClipMask(this FObject fobject) => fobject.ClipsContent.ToBoolNullFalse();
        public static bool IsObjectMask(this FObject fobject) => fobject.IsMask.ToBoolNullFalse();
        public static bool IsGenerativeType(this FObject fobject) => fobject.Data.FcuImageType == FcuImageType.Generative;
        public static bool IsDrawableType(this FObject fobject) => fobject.Data.FcuImageType == FcuImageType.Drawable;
        public static bool IsDownloadableType(this FObject fobject) => fobject.Data.FcuImageType == FcuImageType.Downloadable;
        public static bool IsMaskType(this FObject fobject) => fobject.Data.FcuImageType == FcuImageType.Mask;
    }
}

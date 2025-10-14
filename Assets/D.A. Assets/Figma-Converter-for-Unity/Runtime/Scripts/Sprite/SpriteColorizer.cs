using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace DA_Assets.FCU
{
    [Serializable]
    public class SpriteColorizer : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        public async Task ColorizeSprites(List<FObject> fobjects)
        {
            if (monoBeh.UsingSVG())
                return;

            foreach (FObject fobject in fobjects)
            {
                if (monoBeh.IsCancellationRequested(TokenType.Import))
                    return;

                // Only process objects whose image type is Downloadable.
                if (fobject.Data.FcuImageType != FcuImageType.Downloadable)
                    continue;

                if (monoBeh.UsingSpriteRenderer())
                {
                    // When using SpriteRenderer, only allow graphics that consist of a single solid color.
                    if (!fobject.Data.Graphic.HasSingleColor)
                        continue;
                }
                else if (monoBeh.IsUITK() || monoBeh.IsNova())
                {
                    // For UITK or Nova, only single-color graphics are allowed.
                    if (!fobject.Data.Graphic.HasSingleColor)
                        continue;

                    // When using UITK or Nova, allow only single-color graphics (no gradients at all).
                    if (fobject.Data.Graphic.HasSingleGradient)
                        continue;
                }
                else
                {
                    // For all other image components, allow either a single color or (conditionally) a single gradient.
                    if (!fobject.Data.Graphic.HasSingleColor && !fobject.Data.Graphic.HasSingleGradient)
                        continue;

                    // Skip coloring if downloading of supported gradients is enabled.
                    if (fobject.Data.Graphic.HasSingleGradient
                        && monoBeh.Settings.ImageSpritesSettings.DownloadOptions.HasFlag(SpriteDownloadOptions.SupportedGradients))
                        continue;
                }

                if (File.Exists(fobject.Data.SpritePath.GetFullAssetPath()) == false)
                    continue;

                byte[] rawData = File.ReadAllBytes(fobject.Data.SpritePath.GetFullAssetPath());
                Texture2D tex = new Texture2D(fobject.Data.SpriteSize.x, fobject.Data.SpriteSize.y);
                tex.LoadImage(rawData);

                tex.Colorize(Color.white);

                byte[] bytes = new byte[0];

                switch (monoBeh.Settings.ImageSpritesSettings.ImageFormat)
                {
                    case ImageFormat.PNG:
                        bytes = tex.EncodeToPNG();
                        break;
                    case ImageFormat.JPG:
                        bytes = tex.EncodeToJPG();
                        break;
                }

                File.WriteAllBytes(fobject.Data.SpritePath, bytes);

                await Task.Yield();
            }
        }
    }
}
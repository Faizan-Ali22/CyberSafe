using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Concurrent;

#if VECTOR_GRAPHICS_EXISTS && UNITY_EDITOR
using Unity.VectorGraphics.Editor;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DA_Assets.FCU
{
    [Serializable]
    public class SpriteProcessor : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        private int _errorLogSplitLimit = 50;
        ConcurrentBag<FObject> failedObjects = new ConcurrentBag<FObject>();

        public Sprite GetSprite(FObject fobject)
        {
            if (fobject.Data.SpritePath.IsEmpty())
            {
                return null;
            }

            Sprite sprite = null;
#if UNITY_EDITOR
            sprite = (Sprite)AssetDatabase.LoadAssetAtPath(fobject.Data.SpritePath, typeof(Sprite));
#endif
            return sprite;
        }

        public void SaveSprite(Texture2D texture, FObject fobject)
        {
#if UNITY_EDITOR
            string path = fobject.Data.SpritePath;

            byte[] bytes = texture.EncodeToPNG();

            System.IO.File.WriteAllBytes(path, bytes);

            AssetDatabase.Refresh();

            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;

            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
#endif
        }

#if UNITY_EDITOR
        public async Task MarkAsSprites(List<FObject> fobjects)
        {
            failedObjects = new ConcurrentBag<FObject>();
            AssetDatabase.Refresh();

            List<FObject> fobjectWithSprite = fobjects.Where(x => x.Data.SpritePath != null).ToList();

            int allCount = fobjectWithSprite.Count();
            int count = 0;

            foreach (FObject fobject in fobjectWithSprite)
            {
                if (monoBeh.IsCancellationRequested(TokenType.Import))
                    return;

                if (fobject.Data.SpritePath.IsEmpty())
                    continue;

                _ = SetImgTypeSprite(fobject, () =>
                {
                    count++;
                });
            }

            int tempCount = -1;
            while (FcuLogger.WriteLogBeforeEqual(
                ref count,
                ref allCount,
                FcuLocKey.log_mark_as_sprite.Localize(count, allCount),
                ref tempCount))
            {
                if (monoBeh.IsCancellationRequested(TokenType.Import))
                    return;

                await Task.Delay(1000);
            }

            LogFailedSprites(failedObjects);
        }

        private void LogFailedSprites(ConcurrentBag<FObject> failedObjects)
        {
            if (failedObjects.Count() > 0)
            {
                List<List<string>> comps = failedObjects.Select(x => x.Data.NameHierarchy).Split(_errorLogSplitLimit);

                foreach (List<string> comp in comps)
                {
                    string hierarchies = string.Join("\n", comp);

                    DALogger.LogError(
                        FcuLocKey.cant_load_sprites.Localize(comp.Count, hierarchies));
                }
            }
        }

        private async Task SetImgTypeSprite(FObject fobject, Action callback)
        {
            while (true)
            {
                if (monoBeh.IsCancellationRequested(TokenType.Import))
                    return;

                bool success;

                if (fobject.IsSvgExtension())
                {
                    success = SetVectorTextureSettings(fobject);
                }
                else
                {
                    success = SetRasterTextureSettings(fobject);
                }

                if (success)
                {
                    callback.Invoke();
                    break;
                }

                await Task.Delay(100);
            }
        }

        private bool SetVectorTextureSettings(FObject fobject)
        {
            try
            {
#if VECTOR_GRAPHICS_EXISTS
                SVGImporter importer = AssetImporter.GetAtPath(fobject.Data.SpritePath) as SVGImporter;
                UpdateVectorTextureSettings(importer, fobject.Data.SpritePath);
                if (IsVectorTextureSettingsCorrect(importer))
                {
                    return true;
                }
                else
                {
                    UpdateVectorTextureSettings(importer, fobject.Data.SpritePath);
                    return false;
                }
#else
                return true;
#endif
            }
            catch (Exception ex)
            {
                FcuLogger.Debug(ex);
                failedObjects.Add(fobject);
                return true;
            }
        }
#if VECTOR_GRAPHICS_EXISTS
        private bool IsVectorTextureSettingsCorrect(SVGImporter importer)
        {
            bool settingsCorrect = importer.SvgType == monoBeh.Settings.SVGImporterSettings.SvgType &&
                                   importer.SvgPixelsPerUnit == monoBeh.Settings.ImageSpritesSettings.ImageScale &&
                                   importer.GradientResolution == monoBeh.Settings.SVGImporterSettings.GradientResolution &&
                                   importer.CustomPivot == monoBeh.Settings.SVGImporterSettings.CustomPivot &&
                                   importer.GeneratePhysicsShape == monoBeh.Settings.SVGImporterSettings.GeneratePhysicsShape &&
                                   importer.ViewportOptions == monoBeh.Settings.SVGImporterSettings.ViewportOptions &&
                                   importer.StepDistance == monoBeh.Settings.SVGImporterSettings.StepDistance &&
                                   importer.SamplingStepDistance == monoBeh.Settings.SVGImporterSettings.SamplingSteps &&
                                   importer.AdvancedMode == monoBeh.Settings.SVGImporterSettings.AdvancedMode &&
                                   importer.MaxCordDeviationEnabled == monoBeh.Settings.SVGImporterSettings.MaxCordDeviationEnabled &&
                                   importer.MaxTangentAngleEnabled == monoBeh.Settings.SVGImporterSettings.MaxTangentAngleEnabled;

            return settingsCorrect;
        }

        private void UpdateVectorTextureSettings(SVGImporter importer, string spritePath)
        {
            var svgImporterSettings = monoBeh.Settings.SvgImageSettings;

            importer.SvgType = monoBeh.Settings.SVGImporterSettings.SvgType;
            importer.SvgPixelsPerUnit = monoBeh.Settings.ImageSpritesSettings.ImageScale;
            importer.GradientResolution = monoBeh.Settings.SVGImporterSettings.GradientResolution <= ushort.MaxValue ? (ushort)monoBeh.Settings.SVGImporterSettings.GradientResolution : ushort.MaxValue;
            importer.CustomPivot = monoBeh.Settings.SVGImporterSettings.CustomPivot;
            importer.GeneratePhysicsShape = monoBeh.Settings.SVGImporterSettings.GeneratePhysicsShape;
            importer.ViewportOptions = monoBeh.Settings.SVGImporterSettings.ViewportOptions;
            importer.StepDistance = monoBeh.Settings.SVGImporterSettings.StepDistance;
            importer.SamplingStepDistance = monoBeh.Settings.SVGImporterSettings.SamplingSteps;
            importer.AdvancedMode = monoBeh.Settings.SVGImporterSettings.AdvancedMode;

            importer.MaxCordDeviationEnabled = monoBeh.Settings.SVGImporterSettings.MaxCordDeviationEnabled;
            importer.MaxCordDeviation = monoBeh.Settings.SVGImporterSettings.MaxCordDeviation;

            importer.MaxTangentAngleEnabled = monoBeh.Settings.SVGImporterSettings.MaxTangentAngleEnabled;
            importer.MaxTangentAngle = monoBeh.Settings.SVGImporterSettings.MaxTangentAngle;

            SaveAsset(importer);
        }
#endif
        private void SaveAsset(AssetImporter importer)
        {
            importer.SetDirtyExt();
            importer.SaveAndReimport();
        }

        private bool SetRasterTextureSettings(FObject fobject)
        {
            try
            {
                TextureImporter importer = AssetImporter.GetAtPath(fobject.Data.SpritePath) as TextureImporter;

                SetRasterTextureSize(fobject, importer);

                if (IsRasterTextureSettingsCorrect(fobject, importer))
                {
                    return true;
                }
                else
                {
                    UpdateRasterTextureSettings(fobject, importer);
                    return false;
                }
            }
            catch (Exception ex)
            {
                FcuLogger.Debug(ex, FcuDebugSettingsFlags.LogError);
                failedObjects.Add(fobject);
                return true;
            }
        }

        private void SetRasterTextureSize(FObject fobject, TextureImporter importer)
        {
            importer.GetTextureSize(out int width, out int height);
            importer.SetMaxTextureSize(width, height);
            fobject.Data.SpriteSize = new Vector2Int(width, height);
        }

        private bool IsRasterTextureSettingsCorrect(FObject fobject, TextureImporter importer)
        {
            bool part1 = importer.isReadable == monoBeh.Settings.TextureImporterSettings.IsReadable &&
                   importer.textureType == monoBeh.Settings.TextureImporterSettings.TextureType &&
                   importer.crunchedCompression == monoBeh.Settings.TextureImporterSettings.CrunchedCompression &&
                   importer.textureCompression == monoBeh.Settings.TextureImporterSettings.TextureCompression &&
                   importer.mipmapEnabled == monoBeh.Settings.TextureImporterSettings.MipmapEnabled &&
                   importer.spriteImportMode == monoBeh.Settings.TextureImporterSettings.SpriteImportMode;

            bool perUnit;

            if (monoBeh.UsingSpriteRenderer())
            {
                perUnit = importer.spritePixelsPerUnit == monoBeh.Settings.ImageSpritesSettings.ImageScale;
            }
            else if (fobject.ContainsTag(FcuTag.Slice9))
            {
                perUnit = importer.spritePixelsPerUnit == (int)(fobject.Data.Scale * 100f);
            }
            else
            {
                perUnit = importer.spritePixelsPerUnit == monoBeh.Settings.ImageSpritesSettings.PixelsPerUnit;
            }

            return part1 && perUnit;
        }

        private void UpdateRasterTextureSettings(FObject fobject, TextureImporter importer)
        {
            importer.isReadable = monoBeh.Settings.TextureImporterSettings.IsReadable;
            importer.textureType = monoBeh.Settings.TextureImporterSettings.TextureType;
            importer.crunchedCompression = monoBeh.Settings.TextureImporterSettings.CrunchedCompression;
            importer.textureCompression = monoBeh.Settings.TextureImporterSettings.TextureCompression;
            importer.mipmapEnabled = monoBeh.Settings.TextureImporterSettings.MipmapEnabled;
            importer.spriteImportMode = monoBeh.Settings.TextureImporterSettings.SpriteImportMode;

            if (monoBeh.UsingSpriteRenderer())
            {
                importer.spritePixelsPerUnit = monoBeh.Settings.ImageSpritesSettings.ImageScale;
            }
            else if (fobject.ContainsTag(FcuTag.Slice9))
            {
                importer.spritePixelsPerUnit = (int)(fobject.Data.Scale * 100f);
            }
            else
            {
                importer.spritePixelsPerUnit = monoBeh.Settings.ImageSpritesSettings.PixelsPerUnit;
            }

            if (importer.crunchedCompression)
            {
                importer.compressionQuality = monoBeh.Settings.TextureImporterSettings.CompressionQuality;
            }

            SaveAsset(importer);
        }
#endif
    }
}
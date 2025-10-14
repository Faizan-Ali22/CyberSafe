using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Logging;
using DA_Assets.SVGMeshUnity;
using DA_Assets.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace DA_Assets.FCU
{
    [Serializable]
    public class SpriteGenerator : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        [SerializeField] RectTransform rectTransform;
        [SerializeField] MeshRenderer meshRenderer;
        [SerializeField] MeshFilter meshFilter;
        [SerializeField] Camera camera;

        private float spriteGenerationDelay = 0.25f;
        private int meshUpscaleFactor = 16;
        private int renderAntialiasing = 8;
        private float blurCoof = 10f;

        private FilterMode filterMode = FilterMode.Bilinear;
        private TextureFormat textureFormat = TextureFormat.ARGB32;
        private RenderTextureFormat renderTextureFormat = RenderTextureFormat.ARGB32;

        public async Task GenerateSprites(List<FObject> fobjects)
        {
            List<FObject> generative = fobjects.Where(x => x.Data.NeedGenerate).ToList();

            if (generative.IsEmpty())
                return;

            if (camera == null)
            {
                GameObject cameraGo = MonoBehExtensions.CreateEmptyGameObject();
                cameraGo.name = "SpriteGeneratorCamera";
                cameraGo.TryAddComponent(out camera);
                camera.orthographic = true;
                camera.backgroundColor = new Color(0, 0, 0, 0);
                camera.clearFlags = CameraClearFlags.Color;
            }

            int generatedCount = 0;

            _ = DACycles.ForEach(generative, fobject =>
            {
                FcuLogger.Debug($"GenerateSprites | {fobject.Data.NameHierarchy} | {fobject.Data.NeedGenerate}");

                try
                {
                    Texture2D fillTexture = null;
                    Texture2D strokeTexture = null;
                    Texture2D finalTexture = null;

                    FGraphic graphic = fobject.Data.Graphic;

                    if (!graphic.HasFill && graphic.HasStroke && !fobject.ContainsRoundedCorners())
                    {
                        IndividualStrokeWeights ind = fobject.IndividualStrokeWeights;

                        if (ind.IsDefault())
                        {
                            ind = new IndividualStrokeWeights
                            {
                                Left = fobject.StrokeWeight,
                                Right = fobject.StrokeWeight,
                                Top = fobject.StrokeWeight,
                                Bottom = fobject.StrokeWeight,
                            };
                        }

                        if (ind.Left != 0 && ind.Left < 1)
                        {
                            ind.Left = 1;
                        }

                        if (ind.Right != 0 && ind.Right < 1)
                        {
                            ind.Right = 1;
                        }

                        if (ind.Top != 0 && ind.Top < 1)
                        {
                            ind.Top = 1;
                        }

                        if (ind.Bottom != 0 && ind.Bottom < 1)
                        {
                            ind.Bottom = 1;
                        }

                        float coof = monoBeh.Settings.ImageSpritesSettings.ImageScale;

                        int texWidth = (int)(fobject.Size.x * coof);
                        int texHeight = (int)(fobject.Size.y * coof);

                        int leftWidth = (int)(ind.Left * coof);
                        int rightWidth = (int)(ind.Right * coof);
                        int topWidth = (int)(ind.Top * coof);
                        int bottomWidth = (int)(ind.Bottom * coof);

                        finalTexture = TextureBorderDrawer.CreateTextureWithBorder(
                            texWidth, texHeight,
                            leftWidth, rightWidth, topWidth, bottomWidth,
                            Color.white);
                    }
                    else
                    {
                        Vector2Int finalSize = default;

                        if (graphic.HasFill)
                        {
                            Vector2 fillSize = fobject.Size;
                            fillSize -= new Vector2Int(1, 0);
                            fillSize.IsSupportedRenderSize(monoBeh.Settings.ImageSpritesSettings.ImageScale, out finalSize, out Vector2Int bakeFillSize);

                            string fillPath = fobject.FillGeometry[0].Path;

                            Color textureColor;

                            if (graphic.HasFill && graphic.HasStroke)
                            {
                                textureColor = graphic.Fill.SolidPaint.Color;
                            }
                            else
                            {
                                textureColor = Color.white;
                            }

                            fillTexture = GenerateTexture(fillPath, fillSize, bakeFillSize, textureColor);
                        }

                        if (graphic.HasStroke)
                        {
                            Vector2 strokeSize = fobject.Size;
                            strokeSize += new Vector2(fobject.StrokeWeight * 2, fobject.StrokeWeight * 2);
                            strokeSize.IsSupportedRenderSize(monoBeh.Settings.ImageSpritesSettings.ImageScale, out finalSize, out Vector2Int bakeStrokeSize);

                            string strokePath = fobject.StrokeGeometry[0].Path;

                            Color textureColor;

                            if (graphic.HasFill && graphic.HasStroke)
                            {
                                textureColor = graphic.Stroke.SolidPaint.Color;
                            }
                            else
                            {
                                textureColor = Color.white;
                            }

                            strokeTexture = GenerateTexture(strokePath, strokeSize, bakeStrokeSize, textureColor);
                        }

                        FcuLogger.Debug($"GenerateSprites | {fobject.Data.NameHierarchy} | hasFills: {graphic.HasFill} | hasStrokes: {graphic.HasStroke}", FcuDebugSettingsFlags.LogDefault);

                        if (fillTexture != null && strokeTexture != null)
                        {
                            finalTexture = strokeTexture.Merge(fillTexture);
                        }
                        else if (strokeTexture != null)
                        {
                            finalTexture = strokeTexture;
                        }
                        else if (fillTexture != null)
                        {
                            finalTexture = fillTexture;
                        }

                        if (finalTexture == null)
                        {
                            throw new Exception("finalTexture is null");
                        }

                        finalTexture.Blur(monoBeh.Settings.ImageSpritesSettings.ImageScale / blurCoof);
                        finalTexture.Resize(finalSize, 0, filterMode, renderTextureFormat);
                    }

                    byte[] textureBytes = finalTexture.EncodeToPNG();
                    finalTexture.Destroy();

                    File.WriteAllBytes(fobject.Data.SpritePath, textureBytes);
                }
                catch (Exception ex)
                {
                    FcuLogger.Debug($"Can't generate '{fobject.Data.NameHierarchy}'\n{ex}", FcuDebugSettingsFlags.LogError);
                    fobject.Data.FcuImageType = FcuImageType.Drawable;
                }

                generatedCount++;
            }, spriteGenerationDelay, 1);

            while (true)
            {
                DALogger.Log(FcuLocKey.log_generating_sprites.Localize(generatedCount, generative.Count));

                if (generatedCount >= generative.Count)
                    break;

                await Task.Delay(1000);
            }

            camera.gameObject.Destroy();
        }

        public Texture2D GenerateTexture(string svgPath, Vector2 sourceSize, Vector2Int bakeResolution, Color color)
        {
            GameObject meshObject = MonoBehExtensions.CreateEmptyGameObject();
            meshObject.name = sourceSize.ToString();
            meshObject.transform.position = new Vector3(-20000, -20000, 0);

            try
            {
                GenerateMesh(meshObject, sourceSize, svgPath);
                Texture2D bakedTexture = BakeTexture(meshObject, bakeResolution, color);
                return bakedTexture;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                meshObject.Destroy();
            }
        }

        private void GenerateMesh(GameObject meshObject, Vector2 objectSize, string svgPath)
        {
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer = meshObject.AddComponent<MeshRenderer>();

#if UNITY_EDITOR
            Material spriteMaterial = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");
            meshRenderer.material = spriteMaterial;
#endif

            SVGMesh svgMesh = new SVGMesh();
            svgMesh.Init(meshUpscaleFactor);

            SVGData svgData = new SVGData();
            svgData.Path(svgPath);
            svgMesh.Fill(svgData, meshFilter);
        }

        private Texture2D BakeTexture(GameObject meshObject, Vector2Int bakeResolution, Color color)
        {
            RenderTexture renderTexture = RenderTexture.GetTemporary(bakeResolution.x, bakeResolution.y, 8, renderTextureFormat);

            renderTexture.antiAliasing = renderAntialiasing;
            renderTexture.filterMode = filterMode;

            camera.targetTexture = renderTexture;
            camera.SetToObject(meshObject);
            camera.Render();

            Texture2D texture = new Texture2D(bakeResolution.x, bakeResolution.y, textureFormat, false);
            texture.filterMode = filterMode;

            RenderTexture.active = renderTexture;

            texture.ReadPixels(new Rect(0, 0, bakeResolution.x, bakeResolution.y), 0, 0);
            texture.Apply();

            texture.Colorize(color);

            RenderTexture.ReleaseTemporary(renderTexture);
            RenderTexture.active = null;
            camera.targetTexture = null;

            return texture;
        }
    }

    public static class SpriteGeneratorExtensions
    {
        public static void SetToObject(this Camera camera, GameObject target)
        {
            target.transform.position = new Vector3((int)target.transform.position.x, (int)target.transform.position.y, (int)target.transform.position.z);

            Renderer objectRenderer = target.GetComponent<Renderer>();
            Vector3 objectSize = objectRenderer.bounds.size;
            Vector3 objectPosition = objectRenderer.bounds.center;

            camera.transform.position = new Vector3(objectPosition.x, objectPosition.y, -1);
            camera.orthographicSize = Mathf.Max(objectSize.x / (2f * camera.aspect), objectSize.y / 2f);
        }
    }

    public class TextureBorderDrawer
    {
        public static Texture2D CreateTextureWithBorder(int width, int height, int leftWidth, int rightWidth, int topWidth, int bottomWidth, Color borderColor)
        {
            Texture2D texture = new Texture2D(width, height);

            Color transparent = new Color(0, 0, 0, 0);

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    texture.SetPixel(x, y, transparent);
                }
            }

            if (leftWidth > 0)
                DrawLeftBorder(texture, leftWidth, borderColor);
            if (rightWidth > 0)
                DrawRightBorder(texture, rightWidth, borderColor);
            if (topWidth > 0)
                DrawTopBorder(texture, topWidth, borderColor);
            if (bottomWidth > 0)
                DrawBottomBorder(texture, bottomWidth, borderColor);

            texture.Apply();
            return texture;
        }

        private static void DrawLeftBorder(Texture2D texture, int borderWidth, Color borderColor)
        {
            for (int x = 0; x < borderWidth; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    texture.SetPixel(x, y, borderColor);
                }
            }
        }

        private static void DrawRightBorder(Texture2D texture, int borderWidth, Color borderColor)
        {
            for (int x = texture.width - borderWidth; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    texture.SetPixel(x, y, borderColor);
                }
            }
        }

        private static void DrawTopBorder(Texture2D texture, int borderWidth, Color borderColor)
        {
            for (int y = texture.height - borderWidth; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    texture.SetPixel(x, y, borderColor);
                }
            }
        }

        private static void DrawBottomBorder(Texture2D texture, int borderWidth, Color borderColor)
        {
            for (int y = 0; y < borderWidth; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    texture.SetPixel(x, y, borderColor);
                }
            }
        }
    }
}

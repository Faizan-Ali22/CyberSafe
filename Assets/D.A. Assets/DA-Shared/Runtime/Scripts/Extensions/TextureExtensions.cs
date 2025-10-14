using System.Reflection;
using UnityEngine;

namespace DA_Assets.Extensions
{
    public static class TextureExtensions
    {

#if UNITY_EDITOR
        /// <summary>
        /// <para><see href="https://forum.unity.com/threads/getting-original-size-of-texture-asset-in-pixels.165295/"/></para>
        /// </summary>
        public static bool GetTextureSize(this UnityEditor.TextureImporter importer, out int width, out int height)
        {
            if (importer != null)
            {
                object[] args = new object[2] { 0, 0 };
                MethodInfo mi = typeof(UnityEditor.TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
                mi.Invoke(importer, args);

                width = (int)args[0];
                height = (int)args[1];

                return true;
            }

            height = width = 0;
            return false;
        }

        /// <summary>
        /// Sets the maximum size of the texture based on its width and height.
        /// <para><see href="https://forum.unity.com/threads/getting-original-size-of-texture-asset-in-pixels.165295/"/></para>
        /// </summary>
        public static void SetMaxTextureSize(this UnityEditor.TextureImporter importer, int width, int height)
        {
            int[] maxTextureSizeValues = { 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384 };

            int max = Mathf.Max(width, height);

            int defsize = 1024; //Default size

            for (int i = 0; i < maxTextureSizeValues.Length; i++)
            {
                if (maxTextureSizeValues[i] >= max)
                {
                    defsize = maxTextureSizeValues[i];
                    break;
                }
            }

            importer.maxTextureSize = defsize;
        }
#endif

        public static Texture2D Merge(this Texture2D largeTexture, Texture2D smallTexture)
        {
            //Create a new texture with the size of the large texture
            Texture2D overlayTexture = new Texture2D(largeTexture.width, largeTexture.height, TextureFormat.RGBA32, false);

            //Copy pixels of a large texture to a new texture
            Color[] pixels = largeTexture.GetPixels();
            overlayTexture.SetPixels(pixels);

            //Determine the coordinates to center the small texture
            int startX = (largeTexture.width - smallTexture.width) / 2;
            int startY = (largeTexture.height - smallTexture.height) / 2;

            //Overlay pixels of a small texture onto a large texture
            Color[] overlayPixels = smallTexture.GetPixels();
            for (int x = 0; x < smallTexture.width; x++)
            {
                for (int y = 0; y < smallTexture.height; y++)
                {
                    int targetX = startX + x;
                    int targetY = startY + y;

                    Color overlayPixel = overlayPixels[x + y * smallTexture.width];

                    //Applying the alpha channel of a small texture to the pixels of a large texture
                    Color targetPixel = overlayTexture.GetPixel(targetX, targetY);
                    Color finalPixel = Color.Lerp(targetPixel, overlayPixel, overlayPixel.a);
                    overlayTexture.SetPixel(targetX, targetY, finalPixel);
                }
            }

            //Applying changes to the texture
            overlayTexture.Apply();

            return overlayTexture;
        }

        public static void Colorize(this Texture2D texture, Color32 color)
        {
            Color32[] pixels = texture.GetPixels32();

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = new Color32(color.r, color.g, color.b, pixels[i].a);
            }

            texture.SetPixels32(pixels);
            texture.Apply();
        }

        public static void Resize(this Texture2D texture2D, Vector2Int targetSize, int depth, FilterMode filterMode, RenderTextureFormat rtFormat)
        {
            RenderTexture rt = RenderTexture.GetTemporary(targetSize.x, targetSize.y, depth, rtFormat, RenderTextureReadWrite.Default);

            RenderTexture.active = rt;
            Graphics.Blit(texture2D, rt);

#if UNITY_2021_2_OR_NEWER
            texture2D.Reinitialize(targetSize.x, targetSize.y, texture2D.format, false);
#else
            texture2D.Resize(targetSize.x, targetSize.y, texture2D.format, false);
#endif
            texture2D.filterMode = filterMode;
            texture2D.ReadPixels(new Rect(0.0f, 0.0f, targetSize.x, targetSize.y), 0, 0);
            texture2D.Apply();

            RenderTexture.ReleaseTemporary(rt);
            RenderTexture.active = null;
        }
    }
}

using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace DA_Assets.FCU
{
    [Serializable]
    public class SpriteSlicer : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        public async Task SliceSprites(List<FObject> fobjects)
        {
            foreach (FObject fobject in fobjects)
            {
                if (monoBeh.IsCancellationRequested(TokenType.Import))
                    return;

                if (!fobject.IsSprite())
                    continue;

                if (fobject.Children.IsEmpty())
                    continue;

                if (fobject.Children.Count != 9)
                    continue;

                Sprite sprite = monoBeh.SpriteProcessor.GetSprite(fobject);

                if (sprite == null)
                    continue;

                FObject child0 = fobject.Children[0];
                FObject child1 = fobject.Children[1];
                FObject child2 = fobject.Children[2];
                FObject child3 = fobject.Children[3];
                FObject child4 = fobject.Children[4];
                FObject child5 = fobject.Children[5];
                FObject child6 = fobject.Children[6];
                FObject child7 = fobject.Children[7];
                FObject child8 = fobject.Children[8];

                float imageScale = fobject.Data.Scale;

                int left = (int)(child0.Size.x * imageScale);
                int top = (int)(child0.Size.y * imageScale);
                int right = (int)(child2.Size.x * imageScale);
                int bottom = (int)(child6.Size.y * imageScale);

                Vector4 border = new Vector4(left, bottom, right, top);

                Texture2D texture = CreateMinimal9Slice(sprite, border);
                monoBeh.SpriteProcessor.SaveSprite(texture, fobject);

                sprite = monoBeh.SpriteProcessor.GetSprite(fobject);
                monoBeh.DelegateHolder.SetSpriteRects(sprite, border);
                await Task.Yield();
            }
        }

        // Creating a new sprite without the center part
        public Texture2D CreateMinimal9Slice(Sprite sourceSprite, Vector4 borders)
        {
            // Get border sizes from 9-slice settings
            int leftBorder = (int)borders.x;
            int bottomBorder = (int)borders.y;
            int rightBorder = (int)borders.z;
            int topBorder = (int)borders.w;

            // Calculate new dimensions without the center
            int newWidth = leftBorder + rightBorder;
            int newHeight = topBorder + bottomBorder;

            // Create a new texture
            Texture2D newTexture = new Texture2D(newWidth, newHeight, sourceSprite.texture.format, false);

            // Get pixels from source texture
            Color[] sourcePixels = sourceSprite.texture.GetPixels();
            int sourceWidth = sourceSprite.texture.width;
            int sourceHeight = sourceSprite.texture.height;

            // Create array for new pixels
            Color[] newPixels = new Color[newWidth * newHeight];

            // Copy top left corner
            for (int y = 0; y < topBorder; y++)
            {
                for (int x = 0; x < leftBorder; x++)
                {
                    int sourceIndex = x + (sourceHeight - 1 - y) * sourceWidth;
                    int destIndex = x + (newHeight - 1 - y) * newWidth;
                    newPixels[destIndex] = sourcePixels[sourceIndex];
                }
            }

            // Copy top right corner
            for (int y = 0; y < topBorder; y++)
            {
                for (int x = 0; x < rightBorder; x++)
                {
                    int sourceX = sourceWidth - rightBorder + x;
                    int sourceIndex = sourceX + (sourceHeight - 1 - y) * sourceWidth;
                    int destIndex = (leftBorder + x) + (newHeight - 1 - y) * newWidth;
                    newPixels[destIndex] = sourcePixels[sourceIndex];
                }
            }

            // Copy bottom left corner
            for (int y = 0; y < bottomBorder; y++)
            {
                for (int x = 0; x < leftBorder; x++)
                {
                    int sourceIndex = x + y * sourceWidth;
                    int destIndex = x + y * newWidth;
                    newPixels[destIndex] = sourcePixels[sourceIndex];
                }
            }

            // Copy bottom right corner
            for (int y = 0; y < bottomBorder; y++)
            {
                for (int x = 0; x < rightBorder; x++)
                {
                    int sourceX = sourceWidth - rightBorder + x;
                    int sourceIndex = sourceX + y * sourceWidth;
                    int destIndex = (leftBorder + x) + y * newWidth;
                    newPixels[destIndex] = sourcePixels[sourceIndex];
                }
            }

            // Set pixels to new texture
            newTexture.SetPixels(newPixels);
            newTexture.Apply();

            return newTexture;
        }
    }
}
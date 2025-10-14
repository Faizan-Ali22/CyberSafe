using UnityEditor;
using UnityEngine;

#if U2D_SPRITE_EXISTS
using UnityEditor.U2D;
using UnityEditor.U2D.Sprites;
#endif

public class SpriteEditorUtility
{
    /// <summary>
    /// com.unity.2d.sprite@1.0.0\Documentation~\DataProvider.md
    /// </summary>
    public static void SetSpriteRects(Sprite sprite, Vector4 border)
    {
#if U2D_SPRITE_EXISTS
        SpriteDataProviderFactories factory = new SpriteDataProviderFactories();
        factory.Init();

        ISpriteEditorDataProvider dataProvider = factory.GetSpriteEditorDataProviderFromObject(sprite);
        if (dataProvider == null)
        {
            Debug.LogError("Failed to get ISpriteEditorDataProvider");
            return;
        }

        dataProvider.InitSpriteEditorDataProvider();
        SpriteRect[] spriteRects = dataProvider.GetSpriteRects();

        foreach (SpriteRect rect in spriteRects)
        {
            if (rect.spriteID == sprite.GetSpriteID())
            {
                rect.border = border;
                Debug.Log($"Updated border to: {rect.border}");
            }
        }

        dataProvider.SetSpriteRects(spriteRects);
        dataProvider.Apply();

        TextureImporter textureImporter = dataProvider.targetObject as TextureImporter;
        if (textureImporter != null)
        {
            TextureImporterSettings settings = new TextureImporterSettings();
            textureImporter.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            textureImporter.SetTextureSettings(settings);
            textureImporter.SaveAndReimport();

            Debug.Log("Asset reimported successfully");
        }
        else
        {
            Debug.LogError("Failed to reimport the asset");
        }
#endif
    }
}
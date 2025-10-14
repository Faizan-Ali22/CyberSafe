using DA_Assets.Constants;
using DA_Assets.DAI;
using DA_Assets.Extensions;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace DA_Assets.Tools
{
    internal class SpriteRemoverWindow : EditorWindow
    {
        public const string RemoveUnusedSprites = "Remove unused sprites";

        [SerializeField] string spritesPath = Path.Combine("Assets", "Sprites");
        private static Vector2 windowSize = new Vector2(500, 150);
        [SerializeField] DAInspector gui;

        [MenuItem("Tools/" + DAConstants.Publisher + "/" + nameof(DA_Assets.Tools) + ": " + RemoveUnusedSprites, false, 90)]
        public static void ShowWindow()
        {
            SpriteRemoverWindow win = GetWindow<SpriteRemoverWindow>(RemoveUnusedSprites);
            win.maxSize = windowSize;
            win.minSize = windowSize;

            win.position = new Rect(
                (Screen.currentResolution.width - windowSize.x * 2) / 2,
                (Screen.currentResolution.height - windowSize.y * 2) / 2,
                windowSize.x,
                windowSize.y);
        }

        private void OnGUI()
        {
            gui.DrawGroup(new Group
            {
                GroupType = GroupType.Vertical,
                Style = gui.ColoredStyle.TabBg2,
                Body = () =>
                {
                    gui.Label12px(new GUIContent($@"Remove sprites from the selected folder that are not used by
Image components in the current open scene."));

                    gui.Space15();

                    spritesPath = gui.FolderField(
                        new GUIContent($"Sprites Path"),
                        spritesPath,
                        new GUIContent($"…"),
                       $"Select folder");

                    gui.Space15();

                    if (gui.OutlineButton(new GUIContent("Remove"), true))
                    {
                        RemoveCurrentSceneUnusedSprites();
                    }
                }
            });
        }

        public void RemoveCurrentSceneUnusedSprites()
        {
#if UNITY_EDITOR
            Image[] images;

#if UNITY_2023_3_OR_NEWER
            images = MonoBehaviour.FindObjectsByType<Image>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#elif UNITY_2021_3_OR_NEWER
            images = MonoBehaviour.FindObjectsOfType<Image>(true);
#else
            images = MonoBehaviour.FindObjectsOfType<Image>();
#endif

            var sceneSpritePathes = images
                .Where(x => x.sprite != null)
                .Select(x => AssetDatabase.GetAssetPath(x.sprite));

            var assetSpritePathes = AssetDatabase.FindAssets($"t:{typeof(Sprite).Name}", new string[]
            {
                spritesPath
            }).Select(x => AssetDatabase.GUIDToAssetPath(x));

            var result = assetSpritePathes.Where(x1 => sceneSpritePathes.All(x2 => x2 != x1));

            foreach (var filePath in result)
            {
                File.Delete(filePath.GetFullAssetPath());
            }

            Debug.Log($"{result.Count()} sprites removed.");

            AssetDatabase.Refresh();
#endif
        }
    }
}
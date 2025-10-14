using System.Collections.Generic;
using System.Linq;

namespace DA_Assets.Extensions
{
    public static class AssetDatabaseExtensions
    {
        public static List<string> FindAssetsPathes<T>(this string path, string customType = null)
        {
            List<string> assetPathes = new List<string>();

            if (customType == null)
                customType = $"t:{typeof(T).Name}";

#if UNITY_EDITOR
            string[] guids = UnityEditor.AssetDatabase.FindAssets(customType, new string[] { path.ToRelativePath() });
            assetPathes = guids.Select(x => UnityEditor.AssetDatabase.GUIDToAssetPath(x)).ToList();
#endif
            return assetPathes;
        }
    }
}
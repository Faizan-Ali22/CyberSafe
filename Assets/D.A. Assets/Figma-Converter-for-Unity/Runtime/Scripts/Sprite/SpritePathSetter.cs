using DA_Assets.Extensions;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.DAI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace DA_Assets.FCU
{
    [Serializable]
    public class SpritePathSetter : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        public async Task SetSpritePaths(List<FObject> fobjects)
        {
#if UNITY_EDITOR
            List<FObject> canHasFile = fobjects
                .Where(x => x.IsDownloadableType() || x.IsGenerativeType())
                .ToList();

            List<FObject> noDuplicates = canHasFile
                .GroupBy(x => x.Data.Hash)
                .Select(x => x.First())
                .ToList();

            await Task.Yield();

            string filter = $"t:{typeof(Sprite).Name}";

            string[] searchInFolder = new string[]
            {
                monoBeh.Settings.ImageSpritesSettings.SpritesPath
            };

            string[] assetSpritePathes = UnityEditor.AssetDatabase
                .FindAssets(filter, searchInFolder)
                .Select(x => UnityEditor.AssetDatabase.GUIDToAssetPath(x))
                .ToArray();

            await DACycles.ForEach(noDuplicates, item =>
            {
                bool imageFileExists = GetSpritePath(item, assetSpritePathes, out string spritePath);

                SetNeedDownloadFileFlag(item, imageFileExists);
                SetNeedGenerateFlag(item, imageFileExists);

                foreach (FObject fobject in canHasFile)
                {
                    if (fobject.Data.Hash == item.Data.Hash)
                    {
                        if (imageFileExists)
                        {
                            fobject.Data.SpritePath = spritePath;
                        }
                        else
                        {
                            fobject.Data.SpritePath = GetSpritePath(item);
                        }
                    }
                }
            }, 0.01f, 500);
#endif
            await Task.Yield();
        }

        private string GetSpritePath(FObject fobject)
        {
            string spriteDir;

            if (fobject.Data.IsMutual)
                spriteDir = "Mutual";
            else
                spriteDir = fobject.Data.RootFrame.Names.FileName;

            string absoluteFramePath = Path.Combine(monoBeh.Settings.ImageSpritesSettings.SpritesPath.GetFullAssetPath(), spriteDir);
            string relativeAssetPath = Path.Combine(monoBeh.Settings.ImageSpritesSettings.SpritesPath, spriteDir, fobject.Data.Names.FileName);
            absoluteFramePath.CreateFolderIfNotExists();

            return relativeAssetPath;
        }

        private bool IsTargetExtension(FObject fobject, string spritePath)
        {
            string spriteExt = Path.GetExtension(spritePath);
            if (spriteExt.StartsWith(".") && spriteExt.Length > 1)
                spriteExt = spriteExt.Remove(0, 1);

            string targetExt = null;

            if (monoBeh.UsingSvgImage())
            {
                if (fobject.CanUseUnityImage(monoBeh))
                {
                    targetExt = ImageFormat.PNG.ToLower();
                }
            }

            if (targetExt == null)
            {
                targetExt = monoBeh.Settings.ImageSpritesSettings.ImageFormat.ToLower();
            }

            return spriteExt == targetExt;
        }

        public bool GetSpritePath(FObject fobject, string[] spritePathes, out string path)
        {
            foreach (string spritePath in spritePathes)
            {
                bool get1 = spritePath.TryParseSpriteName(out float scale1, out System.Numerics.BigInteger hash1);
                bool get2 = fobject.Data.Names.FileName.TryParseSpriteName(out float scale2, out System.Numerics.BigInteger hash2);

                bool hasData = get1 && get2;
                bool validExtension = IsTargetExtension(fobject, spritePath);
                bool sameHash = hash1 == hash2;
                //bool sameScale = scale1 == scale2;

                if (hasData && validExtension && sameHash/* && sameScale*/)
                {
                    path = spritePath;
                    return true;
                }
            }

            path = null;
            return false;
        }

        private void SetNeedDownloadFileFlag(FObject fobject, bool imageFileExists)
        {
            if (fobject.IsDownloadableType()/* || fobject.IsGenerativeType()*/)
            {
                if (monoBeh.Settings.ImageSpritesSettings.RedownloadSprites)
                {
                    fobject.Data.NeedDownload = true;
                }
                else if (imageFileExists)
                {
                    fobject.Data.NeedDownload = false;
                }
                else
                {
                    fobject.Data.NeedDownload = true;
                }
            }
            else
            {
                fobject.Data.NeedDownload = false;
            }
        }

        private void SetNeedGenerateFlag(FObject fobject, bool imageFileExists)
        {
            if (fobject.IsGenerativeType())
            {
                if (monoBeh.Settings.ImageSpritesSettings.RedownloadSprites)
                {
                    fobject.Data.NeedGenerate = true;
                }
                else if (imageFileExists)
                {
                    fobject.Data.NeedGenerate = false;
                }
                else
                {
                    fobject.Data.NeedGenerate = true;
                }
            }
            else
            {
                fobject.Data.NeedGenerate = false;
            }
        }
    }
}
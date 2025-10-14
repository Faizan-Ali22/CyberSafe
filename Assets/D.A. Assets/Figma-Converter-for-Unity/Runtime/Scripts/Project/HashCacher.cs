using DA_Assets.FCU.Model;
using DA_Assets.DAI;
using DA_Assets.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Threading.Tasks;
using DA_Assets.FCU.Extensions;

namespace DA_Assets.FCU
{
    [Serializable]
    public class HashCacher : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        public static string GetCachePath(string fileName) => Path.Combine(FcuConfig.CachePath, fileName);

        public async Task WriteCache(List<FObject> fobjects)
        {
            await Task.Run(() =>
            {
                string projectFilePath = $"{monoBeh.Settings.MainSettings.ProjectUrl.RemoveInvalidCharsFromFileName('-')}.txt";
                string fullPath = Path.Combine(GetCachePath("HashCache"), projectFilePath);

                FHash fHash = new FHash
                {
                    Hashes = fobjects.ConvertAll(fobject => new FHashObject
                    {
                        Id = $"{fobject.Data.Parent.Id}_{fobject.Data.Id}",
                        HashData = fobject.Data.HashData,
                        HashDataTree = fobject.Data.HashDataTree
                    }).ToArray()
                };

                string json = JsonUtility.ToJson(fHash);
                Path.GetDirectoryName(fullPath).CreateFolderIfNotExists();
                File.WriteAllText(fullPath, json);
            }, monoBeh.GetToken(TokenType.Import));
        }

        public async Task LoadHashCache(SyncHelper[] syncHelpers)
        {
            await Task.Run(() =>
            {
                Parallel.ForEach(syncHelpers, syncHelper =>
                {
                    string projectFilePath = $"{monoBeh.Settings.MainSettings.ProjectUrl.RemoveInvalidCharsFromFileName('-')}.txt";
                    string fullPath = Path.Combine(GetCachePath("HashCache"), projectFilePath);

                    if (File.Exists(fullPath))
                    {
                        string json = File.ReadAllText(fullPath);
                        FHash fHash = JsonUtility.FromJson<FHash>(json);

                        foreach (FHashObject hashObject in fHash.Hashes)
                        {
                            if (hashObject.Id == $"{syncHelper.Data.Parent.Id}_{syncHelper.Data.Id}")
                            {
                                syncHelper.Data.HashData = hashObject.HashData;
                                syncHelper.Data.HashDataTree = hashObject.HashDataTree;
                                break;
                            }
                        }
                    }
                });
            }, monoBeh.GetToken(TokenType.Import));        
        }

        [System.Serializable]
        private struct FHash
        {
            public FHashObject[] Hashes;
        }

        [System.Serializable]
        private struct FHashObject
        {
            public string Id;
            public string HashData;
            public string HashDataTree;
        }
    }
}
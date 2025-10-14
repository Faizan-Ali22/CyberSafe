using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Logging;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DA_Assets.FCU
{
    [Serializable]
    public class CurrentProject : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        public string GetInstanceKey(string id)
        {
            foreach (var item in figmaProject.Components)
            {
                if (item.Key == id)
                {
                    return item.Value.Key;
                }
            }

            return null;
        }

        public bool TryGetById(string id, out FObject fobject)
        {
            foreach (var item in currentPage)
            {
                if (item.Id == id)
                {
                    fobject = item;
                    return true;
                }
            }

            fobject = default;
            return false;
        }

        public bool TryGetByIndex(int index, out FObject fobject)
        {
            if (index < 0)
            {
                fobject = default;
                return false;
            }

            fobject = currentPage[index];
            return true;
        }

        public bool TryGetParent(FObject fobject, out FObject parent)
        {
            if (fobject.Data.ParentIndex < 0)
            {
                parent = default;
                return false;
            }

            parent = currentPage[fobject.Data.ParentIndex];
            return true;
        }

        public FObject GetRootFrame(FObject fobject)
        {

            if (fobject.ContainsTag(FcuTag.Frame))
            {
                return fobject;
            }

            TryGetParent(fobject, out FObject parent);

            //If the objects that are children of the "Page"
            //don’t have the "Frame" tag, this may cause errors in this place.
            /*if (parent.IsDefault())
            {
                return fobject;
            }*/

            return GetRootFrame(parent);
        }



        public bool HasLocalPrefab(SyncData fobject, out SyncHelper localPrefab)
        {
            foreach (SyncHelper lp in localPrefabs)
            {
                if (lp.Data.Hash == fobject.Hash)
                {
                    localPrefab = lp;
                    return true;
                }
            }

            foreach (SyncHelper lp in localPrefabs)
            {
                if (lp.Data.Id == fobject.Id && lp.Data.Names.FileName == fobject.Names.FileName)
                {
                    localPrefab = lp;
                    return true;
                }
            }

            localPrefab = null;
            return false;
        }

        public void LoadLocalPrefabs()
        {
            DALogger.Log(FcuLocKey.log_search_local_prefabs.Localize());
            localPrefabs = LoadAssetFromFolder<SyncHelper>(monoBeh.Settings.PrefabSettings.PrefabsPath, "t:Prefab");
            DALogger.Log(FcuLocKey.log_local_prefabs_found.Localize(localPrefabs.Count));
        }

        public List<T> LoadAssetFromFolder<T>(string fontsPath, string customType = null) where T : UnityEngine.Object
        {
            List<string> pathes = new List<string>();
            List<T> loadedAssets = new List<T>();

            if (customType == null)
                customType = $"t:{typeof(T).Name}";

            if (monoBeh.IsCancellationRequested(TokenType.Import))
                return loadedAssets;

#if UNITY_EDITOR
            string[] guids = UnityEditor.AssetDatabase.FindAssets(customType, new string[] { fontsPath.ToRelativePath() });

            foreach (string guid in guids)
            {
                if (monoBeh.IsCancellationRequested(TokenType.Import))
                    break;

                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);

                if (asset != null)
                {
                    loadedAssets.Add(asset);
                }
            }
#endif

            return loadedAssets;
        }

        public void SetRootFrames(List<FObject> fobjects)
        {
            DALogger.Log("SetRootFrames");
            for (int i = 0; i < fobjects.Count; i++)
            {
                if (fobjects[i].ContainsTag(FcuTag.Page))
                    continue;

                FObject rootFrameFObject = monoBeh.CurrentProject.GetRootFrame(fobjects[i]);
                fobjects[i].Data.RootFrame = rootFrameFObject.Data;
            }
        }

        [SerializeField] List<SyncData> lastImportedFrames = new List<SyncData>();
        public List<SyncData> LastImportedFrames { get => lastImportedFrames; set => SetValue(ref lastImportedFrames, value); }

        [SerializeField] List<SyncHelper> localPrefabs = new List<SyncHelper>();
        public List<SyncHelper> LocalPrefabs { get => localPrefabs; set => SetValue(ref localPrefabs, value); }

        [SerializeField] string projectName;
        public string ProjectName { get => projectName; set => projectName = value; }

        private FigmaProject figmaProject;
        public FigmaProject FigmaProject { get => figmaProject; set => figmaProject = value; }

        /// <summary>
        /// Contains all Figma project fobjects.
        /// </summary>
        private List<FObject> currentPage = new List<FObject>();
        public List<FObject> CurrentPage { get => currentPage; set => currentPage = value; }
    }
}
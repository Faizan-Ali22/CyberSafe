using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using DA_Assets.DAI;
using DA_Assets.Tools;
using DA_Assets.Logging;
using DA_Assets.Extensions;
using System.Threading.Tasks;
using System.Threading;

#if NOVA_UI_EXISTS
using Nova;
#endif

namespace DA_Assets.FCU
{
    [Serializable]
    public class PrefabCreator : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        private List<GameObject> toDestroy;
        [SerializeField] List<PrefabStruct> pstructs;
        Dictionary<int, SyncHelper> createdPrefabs = new Dictionary<int, SyncHelper>();
        public void CreatePrefabs()
        {
            _ = CreatePrefabsAsync();
        }

        [SerializeField] SyncHelper[] syncHelpers;

        private async Task CreatePrefabsAsync()
        {
            CancellationTokenSource cts = monoBeh.CancellationTokenController.CreateNew(TokenType.Prefab);
            CancellationToken token = cts.Token;

            try
            {
                DALogger.Log(FcuLocKey.log_start_creating_prefabs.Localize());
                await Task.Delay(100);

                bool backuped = SceneBackuper.TryBackupActiveScene();

                if (!backuped)
                {
                    DALogger.LogError(FcuLocKey.log_cant_execute_because_no_backup.Localize());
                    return;
                }

                SceneBackuper.MakeActiveSceneDirty();

                pstructs = new List<PrefabStruct>();
                toDestroy = new List<GameObject>();
                createdPrefabs = new Dictionary<int, SyncHelper>();

                syncHelpers = monoBeh.SyncHelpers.GetAllSyncHelpers();
                monoBeh.SyncHelpers.RestoreRootFrames(syncHelpers);

                SyncHelper syncHelper;
                PrefabStruct ps;

                // Create prefab structs.
                for (int i = 0; i < syncHelpers.Length; i++)
                {
                    syncHelper = syncHelpers[i];

                    if (syncHelper.IsPartOfAnyPrefab())
                        continue;

                    ps = CreatePrefabStruct(syncHelper, i);
                    ps.Name = syncHelper.name;
                    pstructs.Add(ps);
                }

                int prefabNumber = 1;
                Dictionary<int, int> hashToPrefabNumber = new Dictionary<int, int>();

                // Calculate prefab numbers.
                for (int i = 0; i < pstructs.Count; i++)
                {
                    ps = pstructs[i];

                    if (hashToPrefabNumber.TryGetValue(ps.Hash, out int existingPrefabNumber))
                    {
                        ps.PrefabNumber = existingPrefabNumber;
                    }
                    else
                    {
                        ps.PrefabNumber = prefabNumber;
                        hashToPrefabNumber[ps.Hash] = prefabNumber;
                        prefabNumber++;
                    }

                    pstructs[i] = ps;
                }

                int prefabCount = 0;

                // Create separated prefabs.
                for (int i = 0; i < pstructs.Count; i++)
                {
                    ps = pstructs[i];

                    if (!createdPrefabs.TryGetValue(ps.PrefabNumber, out var _))
                    {
                        syncHelper = syncHelpers[ps.Current];

                        if (CanBePrefab(syncHelper.Data))
                        {
                            CreatePrefab(i);
                            prefabCount++;
                        }
                    }
                }

                // Instantiate prefabs.
                for (int i = 0; i < pstructs.Count; i++)
                {
                    ps = pstructs[i];

                    if (createdPrefabs.TryGetValue(ps.PrefabNumber, out SyncHelper savedPrefab))
                    {
                        syncHelper = syncHelpers[ps.Current];

                        if (CanBePrefab(syncHelper.Data))
                        {
                            InstantiatePrefab(i, savedPrefab);
                        }
                    }
                    else
                    {
                        Debug.Log("missing prefab");
                    }
                }

                // Set parents.
                for (int i = 0; i < pstructs.Count; i++)
                {
                    ps = pstructs[i];
                    syncHelper = syncHelpers[ps.Current];
                    syncHelper.transform.SetParentEx(GetParentTransform(ps));
                }

                for (int i = 0; i < pstructs.Count; i++)
                {
                    ps = pstructs[i];
                    syncHelper = syncHelpers[ps.Current];
                    syncHelper.transform.SetSiblingIndex(ps.SiblingIndex);
                }

                // Destroy old objects.
                foreach (GameObject item in toDestroy)
                {
                    item.Destroy();
                }

                HashSet<SyncHelper> processedPrefabs = new HashSet<SyncHelper>();

                for (int i = 0; i < pstructs.Count; i++)
                {
                    ps = pstructs[i];

                    if (createdPrefabs.TryGetValue(ps.PrefabNumber, out SyncHelper savedPrefab))
                    {
                        syncHelper = syncHelpers[ps.Current];
#if UNITY_EDITOR
                        try
                        {
                            if (!processedPrefabs.Contains(savedPrefab))
                            {
                                UnityEditor.PrefabUtility.ApplyPrefabInstance(syncHelper.gameObject, UnityEditor.InteractionMode.AutomatedAction);
                                processedPrefabs.Add(savedPrefab);
                            }
                            else
                            {
                                UnityEditor.PrefabUtility.RevertPrefabInstance(syncHelper.gameObject, UnityEditor.InteractionMode.AutomatedAction);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.LogException(ex);
                        }
#endif
                    }
                }

                for (int i = 0; i < pstructs.Count; i++)
                {
                    ps = pstructs[i];
                    syncHelper = syncHelpers[ps.Current];

                    try
                    {
                        if (syncHelper.Data.Tags.Contains(FcuTag.Text))
                        {
                            syncHelper.gameObject.SetText(ps.Text);
                        }
                    }
                    catch
                    {
                        // TODO: After performing RevertPrefabInstance,
                        // some elements are missing in the syncHelpers array, which prevents further processing.
                        // A different algorithm is needed for removing duplicates during prefab creation.
                    }
                }

                await monoBeh.SyncHelpers.SetFcuToAllSyncHelpersAsync(null);
                monoBeh.SyncHelpers.RestoreRootFrames(syncHelpers);

                DALogger.Log(FcuLocKey.log_prefabs_created.Localize(prefabCount));
            }
            catch (OperationCanceledException)
            {
                Debug.Log($"The task was stopped.");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

#if UNITY_6000_0_OR_NEWER
        private static GameObject FindPartOfPrefab(Transform tr, Transform parent, Vector3 localPosition)
        {
            Transform[] transforms = MonoBehaviour.FindObjectsByType<Transform>(FindObjectsSortMode.None);

            foreach (var obj in transforms)
            {
#if UNITY_EDITOR
                bool partOfPrefab = UnityEditor.PrefabUtility.IsPartOfAnyPrefab(obj.gameObject);

                if (!partOfPrefab)
                {
                    continue;
                }
#endif
                if (obj.name == tr.name && obj.localPosition == localPosition && obj.parent.name == parent.name)
                {
                    return obj.gameObject;
                }
            }

            return null;
        }
#endif
        private static bool IsInsideAddedGameObject(GameObject gameObject)
        {
#if UNITY_EDITOR
            if (UnityEditor.PrefabUtility.IsAddedGameObjectOverride(gameObject))
                return true;

            Transform currentTransform = gameObject.transform;

            while (currentTransform != null)
            {
                bool partOfPrefab = UnityEditor.PrefabUtility.IsAddedGameObjectOverride(currentTransform.gameObject);

                if (partOfPrefab)
                {
                    return true;
                }

                currentTransform = currentTransform.parent;
            }
#endif
            return false;
        }


        private Transform GetParentTransform(PrefabStruct ps)
        {
            return ps.Parent == -1 ? monoBeh.transform : syncHelpers[ps.Parent].transform;
        }

        private PrefabStruct CreatePrefabStruct(SyncHelper syncHelper, int i)
        {
            PrefabStruct ps = new PrefabStruct();

            ps.Hash = syncHelper.Data.Hash;
            ps.Id = syncHelper.Data.Id;

            ps.Current = i;

            var parentSH = syncHelper.transform.parent.GetComponent<SyncHelper>();
            var parentIndex = Array.IndexOf(syncHelpers, parentSH);
            ps.Parent = parentIndex;

            List<SyncHelper> childs = syncHelper.GetComponentsInChildren<SyncHelper>(true).ToList();
            childs.RemoveAt(0);
            ps.Childs = childs.Select(x => Array.IndexOf(syncHelpers, x)).ToList();

            if (monoBeh.IsUGUI())
            {
                ps.UguiTransformData = UguiTransformData.Create(syncHelper.GetComponent<RectTransform>());
            }
#if NOVA_UI_EXISTS
            else if (monoBeh.IsNova())
            {
                ps.NovaTransformData = new NovaTransformData(syncHelper.GetComponent<UIBlock>());
            }
#endif
            ps.SiblingIndex = syncHelper.transform.GetSiblingIndex();
            ps.PrefabNumber = 0;

            if (syncHelper.ContainsTag(FcuTag.Text))
            {
                ps.Text = syncHelper.gameObject.GetText();
            }

            return ps;
        }

        private void CreatePrefab(int i)
        {
            PrefabStruct ps = pstructs[i];

            RemoveParent(ps);

            string prefabPath = GetPrefabPath(ps);

            if (syncHelpers[ps.Current].gameObject.SaveAsPrefabAsset(prefabPath, out SyncHelper savedPrefab, out Exception ex))
            {
                createdPrefabs.TryAddValue(ps.PrefabNumber, savedPrefab);
            }
            else
            {
                DALogger.LogException(ex);
            }

            RestoreParent(ps);
        }

        private void RestoreParent(PrefabStruct ps)
        {
            syncHelpers[ps.Current].transform.SetParentEx(GetParentTransform(ps));

            foreach (int index in ps.Childs)
            {
                syncHelpers[index].transform.SetParentEx(syncHelpers[ps.Current].transform);
            }
        }

        private void RemoveParent(PrefabStruct ps)
        {
            syncHelpers[ps.Current].transform.SetParentEx(null);

            foreach (int index in ps.Childs)
            {
                syncHelpers[index].transform.SetParentEx(null);
            }
        }

        private void InstantiatePrefab(int i, SyncHelper savedPrefab)
        {
            PrefabStruct ps = pstructs[i];

            SyncHelper syncHelper = syncHelpers[ps.Current];
            SyncHelper instantiatedPrefab = null;
#if UNITY_EDITOR
            instantiatedPrefab = (SyncHelper)UnityEditor.PrefabUtility.InstantiatePrefab(savedPrefab);
#endif
            instantiatedPrefab.name = syncHelper.name;
            instantiatedPrefab.transform.SetParentEx(GetParentTransform(ps));
            instantiatedPrefab.transform.SetSiblingIndex(ps.SiblingIndex);

            if (monoBeh.IsUGUI())
            {
                ps.UguiTransformData.ApplyTo(instantiatedPrefab.GetComponent<RectTransform>());
            }
            else if (monoBeh.IsNova())
            {
#if NOVA_UI_EXISTS
                ps.NovaTransformData.ApplyTo(instantiatedPrefab.GetComponent<UIBlock>());
#endif
            }

            instantiatedPrefab.Data.Id = ps.Id;

            syncHelpers[ps.Current] = instantiatedPrefab;
            toDestroy.Add(syncHelper.gameObject);
        }

        private string GetPrefabName(SyncHelper sh)
        {
            string prefabName;

            if (monoBeh.Settings.PrefabSettings.TextPrefabNameType != TextPrefabNameType.Figma &&
                sh.Data.Tags.Contains(FcuTag.Text))
            {
                prefabName = $"{sh.Data.Names.HumanizedTextPrefabName.Trim()} {sh.Data.Hash}";
            }
            else
            {
                prefabName = $"{sh.gameObject.name.Trim()} {sh.Data.Hash}";
            }

            return prefabName;
        }

        private string GetPrefabPath(PrefabStruct ps)
        {
            string GetSubDir(SyncData sd)
            {
                try
                {
                    if (sd.Tags.Contains(FcuTag.Text))
                        return "Texts";
                    else if (sd.Tags.Contains(FcuTag.Image))
                        return "Images";
                    else if (sd.Tags.Contains(FcuTag.Button))
                        return "Buttons";
                    else if (sd.Tags.Contains(FcuTag.InputField))
                        return "InputFields";
                    else
                        return "Other";
                }
                catch
                {
                    return "Other";
                }
            }

            SyncHelper syncHelper = syncHelpers[ps.Current];

            if (syncHelper.Data.RootFrame == null)
            {
                SyncData myRootFrame = monoBeh.SyncHelpers.GetRootFrame(syncHelper.Data);
                syncHelper.Data.RootFrame = myRootFrame;
            }

            string frameDir = Path.Combine(monoBeh.Settings.PrefabSettings.PrefabsPath, syncHelper.Data.RootFrame.Names.FileName);
            string subDir = GetSubDir(syncHelper.Data);
            string fullDir = Path.Combine(frameDir, subDir);

            fullDir.CreateFolderIfNotExists();

            string prefabName = GetPrefabName(syncHelper);
            string prefabPath = Path.Combine(frameDir, subDir, $"{prefabName}.prefab");

            string result = prefabPath.GetPathRelativeToProjectDirectory();
            return result;
        }

        private bool CanBePrefab(SyncData sd)
        {
            bool onlyCont = sd.Tags.Contains(FcuTag.Container) && sd.Tags.Count == 1;
            return !onlyCont;
        }
    }

    internal static class PrefabCreatorExtensions
    {
        internal static void SetParentEx(this Transform transform, Transform parent)
        {
            //Debug.Log($"SetParentEx | {transform} | {parent}");

            if (transform == null)
            {
                Debug.LogError("transform is null");
                return;
            }

            transform.SetParent(parent, false);
        }
    }
}

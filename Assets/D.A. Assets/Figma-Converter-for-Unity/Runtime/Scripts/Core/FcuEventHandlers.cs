using DA_Assets.DAI;
using System;

namespace DA_Assets.FCU
{
    [Serializable]
    public class FcuEventHandlers : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        public static void CreateFcu_OnClick() => AssetTools.CreateFcuOnScene();

        public void Auth_OnClick() => monoBeh.Authorizer.Auth();

        public void DownloadProject_OnClick() => monoBeh.ProjectDownloader.DownloadProject();

        public void ImportSelectedFrames_OnClick() => monoBeh.ProjectImporter.StartImport();

        public void GenerateScripts_OnClick() => monoBeh.ScriptGenerator.GenerateScripts();

        public void SerializeObjects_OnClick() => monoBeh.ScriptGenerator.Serialize();

        public void CreatePrefabs_OnClick() => monoBeh.PrefabCreator.CreatePrefabs();

        public void UpdatePrefabs_OnClick() => monoBeh.PrefabUpdater.UpdatePrefabs();

        public void DestroyLastImportedFrames_OnClick() => monoBeh.AssetTools.DestroyLastImportedFrames();

        public void DestroySyncHelpers_OnClick() => monoBeh.SyncHelpers.DestroySyncHelpers();

        public void SetFcuToSyncHelpers_OnClick() => monoBeh.SyncHelpers.SetFcuToAllSyncHelpers();

        public void OptimizeSyncHelpers_OnClick() => monoBeh.SyncHelpers.OptimizeSyncHelpers();

        public void StopImport_OnClick() => monoBeh.AssetTools.StopImport(StopImportReason.Manual);
    }
}
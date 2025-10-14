using DA_Assets.Constants;
using DA_Assets.Extensions;
using DA_Assets.Logging;
using UnityEditor;
using UnityEngine;

namespace DA_Assets.FCU
{
    internal class ContextMenuItems
    {  
        [MenuItem("Tools/" + DAConstants.Publisher + "/" + FcuConfig.ProductNameShort + ": " + FcuConfig.Create + " " + FcuConfig.ProductName, false, 0)]
        private static void CreateFcu_OnClick()
        {
            FcuEventHandlers.CreateFcu_OnClick();
        }

        [MenuItem("GameObject/Tools/" + DAConstants.Publisher + "/" + FcuConfig.ProductNameShort + ": " + FcuConfig.SetFcuToSyncHelpers, false, 10)]
        private static void SetFcuToSyncHelpers_OnClick()
        {
            if (Selection.activeGameObject.TryGetComponentSafe(out FigmaConverterUnity fcu))
            {
                fcu.EventHandlers.SetFcuToSyncHelpers_OnClick();
            }
            else
            {
                DALogger.LogError(FcuLocKey.log_component_not_selected_in_hierarchy.Localize(nameof(FigmaConverterUnity)));
            }
        }

        [MenuItem("GameObject/Tools/" + DAConstants.Publisher + "/" + FcuConfig.ProductNameShort + ": " + FcuConfig.OptimizeSyncHelpers, false, 11)]
        private static void OptimizeSyncHelpers_OnClick()
        {
            if (Selection.activeGameObject.TryGetComponentSafe(out FigmaConverterUnity fcu))
            {
                fcu.EventHandlers.OptimizeSyncHelpers_OnClick();
            }
            else
            {
                DALogger.LogError(FcuLocKey.log_component_not_selected_in_hierarchy.Localize(nameof(FigmaConverterUnity)));
            }
        }

        [MenuItem("GameObject/Tools/" + DAConstants.Publisher + "/" + FcuConfig.ProductNameShort + ": " + FcuConfig.CompareTwoObjects, false, 12)]
        private static void Compare_OnClick()
        {
            if (Selection.gameObjects.Length != 2)
            {
                DALogger.LogError(FcuLocKey.log_incorrect_selection.Localize());
                return;
            }

            GameObject go1 = Selection.gameObjects[0];
            GameObject go2 = Selection.gameObjects[1];

            bool e1 = go1.TryGetComponentSafe(out SyncHelper sh1);
            bool e2 = go2.TryGetComponentSafe(out SyncHelper sh2);

            if (e1 == false)
            {
                DALogger.LogError(FcuLocKey.log_no_sync_helper.Localize(go1.name));
                return;
            }

            if (e2 == false)
            {
                DALogger.LogError(FcuLocKey.log_no_sync_helper.Localize(go2.name));
                return;
            }

            ComparerWindow.Show(sh1, sh2);
        }

        /*[MenuItem("GameObject/Tools/" + DAConstants.Publisher + "/" + FcuConfig.ProductNameShort + ": " + FcuConfig.DestroyLastImported, false, 13)]
        private static void DestroyLastImportedFrames_OnClick()
        {
            if (Selection.activeGameObject.TryGetComponentSafe(out FigmaConverterUnity fcu))
            {
                fcu.EventHandlers.DestroyLastImportedFrames_OnClick();
            }
            else
            {
                DALogger.LogError(FcuLocKey.log_component_not_selected_in_hierarchy.Localize(nameof(FigmaConverterUnity)));
            }
        }*/

        [MenuItem("GameObject/Tools/" + DAConstants.Publisher + "/" + FcuConfig.ProductNameShort + ": " + FcuConfig.DestroySyncHelpers, false, 14)]
        private static void DestroySyncHelpers_OnClick()
        {
            if (Selection.activeGameObject.TryGetComponentSafe(out FigmaConverterUnity fcu))
            {
                fcu.EventHandlers.DestroySyncHelpers_OnClick();
            }
            else
            {
                DALogger.LogError(FcuLocKey.log_component_not_selected_in_hierarchy.Localize(nameof(FigmaConverterUnity)));
            }
        }

        //[MenuItem("GameObject/Tools/" + DAConstants.Publisher + "/" + FcuConfig.ProductNameShort + ": " + FcuConfig.UpdatePrefabs, false, 5)]
        private static void UpdatePrefabs_OnClick()
        {
            if (Selection.activeGameObject.TryGetComponentSafe(out FigmaConverterUnity fcu))
            {
                fcu.EventHandlers.UpdatePrefabs_OnClick();
            }
            else
            {
                DALogger.LogError(FcuLocKey.log_component_not_selected_in_hierarchy.Localize(nameof(FigmaConverterUnity)));
            }
        }
    }
}
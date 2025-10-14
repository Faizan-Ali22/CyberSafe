using DA_Assets.Extensions;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.DAI;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using DA_Assets.Logging;

namespace DA_Assets.FCU
{
    [Serializable]
    public class SyncHelpers : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        public void DestroySyncHelpers()
        {
            CancellationTokenSource cts = monoBeh.CancellationTokenController.CreateNew(TokenType.Import);
            _ = DestroySyncHelpersAsync(cts);
        }

        private async Task DestroySyncHelpersAsync(CancellationTokenSource cts)
        {
            SyncHelper[] syncHelpers = GetAllSyncHelpers();

            for (int i = 0; i < syncHelpers.Length; i++)
            {
                syncHelpers[i].Destroy();
                await Task.Yield();
            }

            DALogger.Log(FcuLocKey.log_current_canvas_metas_destroy.Localize(
                monoBeh.GetInstanceID(),
                syncHelpers.Length,
                nameof(SyncHelper)));
        }

        public SyncHelper[] GetAllSyncHelpers()
        {
            List<SyncHelper> childs = monoBeh.gameObject.GetComponentsInReverseOrder<SyncHelper>();
            return childs.ToArray();
        }

        public bool IsExistsOnCurrentCanvas(FObject fobject, out SyncHelper syncObject)
        {
            SyncHelper[] syncHelpers = GetAllSyncHelpers();

            foreach (SyncHelper sh in syncHelpers)
            {
                if (sh.Data.Id == fobject.Id)
                {
                    syncObject = sh;
                    return true;
                }
            }

            syncObject = null;
            return false;
        }

        /// <summary>
        /// Goes up the hierarchy until it finds RootFrame.
        /// </summary>
        public SyncData GetRootFrame(SyncData syncData)
        {
            GameObject currentGameObject = syncData.GameObject;

            while (currentGameObject != null)
            {
                SyncHelper syncHelper = currentGameObject.GetComponent<SyncHelper>();
                if (syncHelper != null && syncHelper.ContainsTag(FcuTag.Frame))
                {
                    return syncHelper.Data;
                }

                currentGameObject = currentGameObject.transform.parent?.gameObject;
            }

            return null;
        }

        /// <summary>
        /// Restore RootFrame in all SyncHelpers.
        /// </summary>
        public void RestoreRootFrames(SyncHelper[] syncHelpers)
        {
            List<SyncHelper> frames = new List<SyncHelper>();
            foreach (SyncHelper syncHelper in syncHelpers)
            {
                if (syncHelper.ContainsTag(FcuTag.Frame))
                {
                    frames.Add(syncHelper);
                }
            }

            foreach (SyncHelper frame in frames)
            {
                SetRootFrameToAllChilds(frame.gameObject, frame.Data);
            }
        }

        public void SetRootFrameToAllChilds(GameObject @object, SyncData rootFrame)
        {
            if (@object == null)
                return;

            foreach (Transform child in @object.transform)
            {
                if (child == null)
                    continue;

                if (child.TryGetComponentSafe(out SyncHelper syncObject))
                {
                    syncObject.Data.RootFrame = rootFrame;
                }

                SetRootFrameToAllChilds(child.gameObject, rootFrame);
            }
        }

        public void OptimizeSyncHelpers()
        {
            CancellationTokenSource cts = monoBeh.CancellationTokenController.CreateNew(TokenType.Import);
            _ = OptimizeSyncHelpersAsync(cts);
        }

        private async Task OptimizeSyncHelpersAsync(CancellationTokenSource cts)
        {
            int counter = 0;
            OptimizeAllChilds(monoBeh.gameObject);

            await Task.Delay(100);

            DALogger.Log(FcuLocKey.log_fcu_assigned.Localize(
                counter,
                nameof(FigmaConverterUnity),
                monoBeh.GetInstanceID()));

            void OptimizeAllChilds(GameObject @object)
            {
                if (@object == null)
                    return;

                foreach (Transform child in @object.transform)
                {
                    if (child == null)
                        continue;

                    if (child.TryGetComponentSafe(out SyncHelper syncObject))
                    {
                        counter++;
                        syncObject.Data.HashData = null;
                        syncObject.Data.HashDataTree = null;
                    }

                    OptimizeAllChilds(child.gameObject);
                }
            }
        }

        public void SetFcuToAllSyncHelpers()
        {
            CancellationTokenSource cts = monoBeh.CancellationTokenController.CreateNew(TokenType.Import);
            _ = SetFcuToAllSyncHelpersAsync(cts);
        }

        public async Task SetFcuToAllSyncHelpersAsync(CancellationTokenSource cts)
        {
            int counter = 0;
            SetFcuToAllChilds(monoBeh.gameObject, ref counter);

            await Task.Delay(100);

            DALogger.Log(FcuLocKey.log_fcu_assigned.Localize(
                counter,
                nameof(FigmaConverterUnity),
                monoBeh.GetInstanceID()));
        }

        public void SetFcuToAllChilds(GameObject @object, ref int counter)
        {
            if (@object == null)
                return;

            foreach (Transform child in @object.transform)
            {
                if (child == null)
                    continue;

                if (child.TryGetComponentSafe(out SyncHelper syncObject))
                {
                    counter++;
                    syncObject.Data.FigmaConverterUnity = monoBeh;
                }

                SetFcuToAllChilds(child.gameObject, ref counter);
            }
        }
    }
}
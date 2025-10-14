using DA_Assets.Constants;
using DA_Assets.DAI;
using DA_Assets.Extensions;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Logging;
using DA_Assets.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace DA_Assets.FCU
{
    [Serializable]
    public class ProjectImporter : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        private List<FObject> _currPage;

        private bool ValidateImportSettings(out string reason)
        {
            bool? result = null;
            reason = "null";

            if (monoBeh.IsUITK())
            {
                if (monoBeh.UsingSVG())
                {
                    reason = FcuLocKey.log_import_failed_incompatible.Localize($"{nameof(UIFramework)}.{UIFramework.UITK}", $"{nameof(ImageFormat)}.{ImageFormat.SVG}");
                    result = false;
                }
                else if (monoBeh.Settings.LocalizationSettings.LocalizationComponent == LocalizationComponent.I2Localization)
                {
                    reason = FcuLocKey.log_import_failed_incompatible.Localize($"{nameof(UIFramework)}.{UIFramework.UITK}", $"{nameof(LocalizationComponent)}.{monoBeh.Settings.LocalizationSettings.LocalizationComponent}");
                    result = false;
                }
            }
            else if (monoBeh.UsingUIBlock2D())
            {
                if (!monoBeh.IsNova())
                {
                    reason = FcuLocKey.log_import_failed_enable_required.Localize($"{nameof(UIFramework)}.{UIFramework.NOVA}", $"{nameof(ImageComponent)}.{ImageComponent.UIBlock2D}");
                    result = false;
                }
                else if (monoBeh.UsingSVG())
                {
                    reason = FcuLocKey.log_import_failed_incompatible.Localize($"{nameof(ImageComponent)}.{ImageComponent.UIBlock2D}", $"{nameof(ImageFormat)}.{ImageFormat.SVG}");
                    result = false;
                }
            }
            else if (monoBeh.UsingSvgImage())
            {
                if (!monoBeh.IsUGUI())
                {
                    reason = FcuLocKey.log_import_failed_enable_required.Localize($"{nameof(UIFramework)}.{UIFramework.UGUI}", $"{nameof(ImageComponent)}.{ImageComponent.SvgImage}");
                    result = false;
                }
                else if (!monoBeh.UsingSVG())
                {
                    reason = FcuLocKey.log_import_failed_enable_required.Localize($"{nameof(ImageFormat)}.{ImageFormat.SVG}", $"{nameof(ImageComponent)}.{ImageComponent.SvgImage}");
                    result = false;
                }
            }
            else if (!monoBeh.UsingSvgImage())
            {
                if (monoBeh.UsingSVG())
                {
                    reason = FcuLocKey.log_import_failed_unsupported.Localize($"{nameof(ImageComponent)}.{monoBeh.Settings.ImageSpritesSettings.ImageComponent}", $"{nameof(ImageFormat)}.{ImageFormat.SVG}");
                    result = false;
                }
            }

            return result.ToBoolNullTrue();
        }

        public void StartImport()
        {
            monoBeh.AssetTools.StopImport(StopImportReason.Hidden);

            if (monoBeh.IsJsonNetExists() == false)
            {
                DALogger.LogError(FcuLocKey.log_cant_find_package.Localize(DAConstants.JsonNetPackageName));
                return;
            }

            if (monoBeh.InspectorDrawer.SelectableDocument.IsProjectEmpty())
            {
                DALogger.LogError(FcuLocKey.log_project_empty.Localize());
                return;
            }

            if (!ValidateImportSettings(out string reason))
            {
                Debug.LogError(reason);
                return;
            }

            monoBeh.CancellationTokenController.CreateNew(TokenType.Import);
            _ = StartImportAsync();
        }

        private async Task StartImportAsync()
        {
            try
            {
                List<string> selectedIds = GetSelectedFrameIds();

                if (selectedIds.Count < 1)
                {
                    DALogger.Log(FcuLocKey.log_nothing_to_import.Localize());
                    return;
                }

                monoBeh.Events.OnImportStart?.Invoke(monoBeh);

                SceneBackuper.TryBackupActiveScene();

                FObject virtualPage = default;

                monoBeh.FolderCreator.CreateAll();
                monoBeh.CurrentProject.LastImportedFrames.Clear();

                _currPage = monoBeh.CurrentProject.CurrentPage;
                _currPage.Clear();

                List<FObject> nodes = await monoBeh.ProjectDownloader.DownloadAllNodes(selectedIds);

                virtualPage = new FObject
                {
                    Id = FcuConfig.PARENT_ID,
                    Name = monoBeh.CurrentProject.ProjectName,
                    Children = nodes,
                    Data = new SyncData
                    {
                        GameObject = monoBeh.gameObject,
                        RectGameObject = monoBeh.gameObject,
                        Names = new FNames
                        {
                            ObjectName = FcuTag.Page.ToString(),
                        },
                        Tags = new List<FcuTag>
                        {
                            FcuTag.Page
                        }
                    }
                };

                monoBeh.NameSetter.ClearNames();

                await monoBeh.TagSetter.SetTags(virtualPage);
                await ConvertTreeToListAsync(virtualPage, _currPage);
                await monoBeh.ImageTypeSetter.SetImageTypes(_currPage);
                await monoBeh.ImageTypeSetter.SetInsideDownloadableFlag(virtualPage);
                await monoBeh.HashGenerator.SetHashes(_currPage);
                await monoBeh.SpriteDownloader.SetScalesAndMaxSpriteSizes(_currPage);
                await monoBeh.NameSetter.SetNames(_currPage, FcuNameType.File);
                await monoBeh.NameSetter.SetNames(_currPage, FcuNameType.UssClass);

                //Setting root frames before creating game objects to use them in import algorithms.
                monoBeh.CurrentProject.SetRootFrames(_currPage);

                if (monoBeh.IsUGUI() || monoBeh.IsNova())
                {
                    monoBeh.CurrentProject.LoadLocalPrefabs();
                    await TryShowPreImportWindow();
                    monoBeh.CanvasDrawer.GameObjectDrawer.Draw(virtualPage);
                }

                await monoBeh.HashCacher.WriteCache(_currPage);

                //Setting root frames after creating game objects so that the root frames are serialized in the inspector.
                monoBeh.CurrentProject.SetRootFrames(_currPage);

                monoBeh.TagSetter.CountTags(_currPage);

                await monoBeh.DuplicateFinder.SetDuplicateFlags(_currPage);

                await monoBeh.SpritePathSetter.SetSpritePaths(_currPage);
                await monoBeh.SpriteDownloader.CreateImageChunks(_currPage);

                if (monoBeh.IsCancellationRequested(TokenType.Import))
                    return;

                await monoBeh.SpriteDownloader.DownloadSprites(_currPage);
                await monoBeh.SpriteGenerator.GenerateSprites(_currPage);
                await monoBeh.SpriteColorizer.ColorizeSprites(_currPage);

                if (monoBeh.IsCancellationRequested(TokenType.Import))
                    return;

                await monoBeh.FontDownloader.DownloadFonts(_currPage);

                if (monoBeh.IsCancellationRequested(TokenType.Import))
                    return;
#if UNITY_EDITOR
                await monoBeh.SpriteProcessor.MarkAsSprites(_currPage);
#endif
                await monoBeh.SpriteSlicer.SliceSprites(_currPage);

                if (monoBeh.IsUGUI())
                {
                    monoBeh.CanvasDrawer.AddCanvasComponent();

                    await monoBeh.TransformSetter.SetTransformPos(_currPage);
                    await monoBeh.TransformSetter.MoveUguiTransforms(_currPage);
                    await monoBeh.TransformSetter.RestoreParents(_currPage);
                    await monoBeh.TransformSetter.SetStretchAllIfNeeded(_currPage);
                    await monoBeh.TransformSetter.SetSiblingIndex(_currPage);

                    if (monoBeh.IsCancellationRequested(TokenType.Import))
                        return;

                    await monoBeh.CanvasDrawer.DrawToCanvas(_currPage);

                    if (monoBeh.UsingSpriteRenderer())
                    {
                        await monoBeh.CanvasDrawer.FixSpriteRenderers(_currPage);
                    }
                    else if (monoBeh.UsingJoshPui())
                    {
                        await monoBeh.CanvasDrawer.FixJoshPui();
                    }
                    else if (monoBeh.UsingDttPui())
                    {
                        await monoBeh.CanvasDrawer.FixDttImages(_currPage);
                    }
                }
                else if (monoBeh.IsUITK())
                {
                    await monoBeh.NameSetter.Set_UITK_Names(_currPage);

#if FCU_EXISTS && FCU_UITK_EXT_EXISTS
                    await monoBeh.UITK_Converter.Convert(virtualPage, _currPage);
#endif
                }
                else if (monoBeh.IsNova())
                {
#if NOVA_UI_EXISTS
                    monoBeh.NovaDrawer.SetupSpace();

                    monoBeh.gameObject.transform.localScale = Vector3.one;

                    await monoBeh.TransformSetter.SetTransformPos(_currPage);
                    await monoBeh.TransformSetter.RestoreParentsRect(_currPage);
                    monoBeh.TransformSetter.MoveNovaTransforms(_currPage); 

                    await monoBeh.TransformSetter.RestoreParents(_currPage);
                    await monoBeh.TransformSetter.RestoreNovaFramePositions(_currPage);

                    await monoBeh.NovaDrawer.DrawToScene(_currPage);

                    monoBeh.NovaDrawer.EnableScreenSpaceComponent();
#endif
                }

                ClearAfterImport();

                SceneBackuper.MakeActiveSceneDirty();

                if (monoBeh.IsCancellationRequested(TokenType.Import))
                    return;

                monoBeh.Events.OnImportComplete?.Invoke(monoBeh);
                monoBeh.AssetTools.ShowRateMe();
                monoBeh.AssetTools.StopImport(StopImportReason.End);
            }
            catch (TaskCanceledException ex)
            {
                Debug.LogException(ex); 
                monoBeh.AssetTools.StopImport(StopImportReason.TaskCanceled);
                return;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                monoBeh.AssetTools.StopImport(StopImportReason.Error);
                return;
            }
            finally
            {

            }
        }

        private async Task TryShowPreImportWindow()
        {
            DALogger.Log("TryShowPreImportWindow");
            SyncHelper[] syncHelpers = monoBeh.SyncHelpers.GetAllSyncHelpers();

            if (syncHelpers.Length < 1)
                return;

            //Setting root frames to existing game objects.
            monoBeh.SyncHelpers.RestoreRootFrames(syncHelpers);

            await monoBeh.HashCacher.LoadHashCache(syncHelpers);

            PreImportInput diffData = await monoBeh.PreImportDataCreator.GetPreImportData(_currPage, syncHelpers.ToList());
            PreImportOutput diffCheckResult = default;

            await monoBeh.AssetTools.ReselectFcu();
            monoBeh.DelegateHolder.ShowDifferenceChecker(diffData, _ => diffCheckResult = _);

            while (diffCheckResult.IsDefault())
            {
                if (monoBeh.IsCancellationRequested(TokenType.Import))
                    return;

                await Task.Delay(1000);
            }

            List<FObject> tempPage = new List<FObject>();

            foreach (var diffModel in diffCheckResult.ToImport)
            {
                if (monoBeh.IsCancellationRequested(TokenType.Import))
                    return;

                foreach (FObject fobject in _currPage)
                {
                    if (diffModel == fobject.Id)
                    {
                        tempPage.Add(fobject);
                    }
                }
            }

            _currPage = tempPage;

            await monoBeh.CanvasDrawer.GameObjectDrawer.DestroyMissing(diffCheckResult.ToRemove);
        }

        private List<string> GetSelectedFrameIds()
        {
            List<string> selected = monoBeh.InspectorDrawer.SelectableDocument.Childs
                .SelectMany(si => si.Childs)
                .Where(si => si.Selected)
                .Select(si => si.Id)
                .ToList();

            return selected;
        }

        public async Task ConvertTreeToListAsync(FObject parent, List<FObject> fobjects)
        {
            DALogger.Log("ConvertTreeToListAsync");
            await Task.Run(() => ConvertTreeToList(parent, fobjects), monoBeh.GetToken(TokenType.Import));
        }

        private void ConvertTreeToList(FObject parent, List<FObject> fobjects, int depth = 0)
        {
            foreach (FObject child in parent.Children)
            {
                if (child.Data.IsEmpty || child.ContainsTag(FcuTag.Ignore))
                {
                    child.SetFlagToAllChilds(x => x.Data.IsEmpty = true);
                    continue;
                }

                child.Data.HierarchyLevel = depth + 1;

                int parentIndex = fobjects.IndexOf(parent);
                child.Data.ParentIndex = parentIndex;

                fobjects.Add(child);

                if (!child.Data.Parent.ContainsTag(FcuTag.Page))
                {
                    fobjects[child.Data.ParentIndex].Data.ChildIndexes.Add(fobjects.Count - 1);
                }

                if (child.Data.ForceImage)
                {
                    child.SetFlagToAllChilds(x => x.Data.IsEmpty = true);
                    continue;
                }

                if (child.Children.IsEmpty())
                    continue;

                ConvertTreeToList(child, fobjects, depth + 1);
            }
        }

        private void ClearAfterImport()
        {
            monoBeh.CanvasDrawer.GameObjectDrawer.ClearTempRectFrames();

            if (!monoBeh.IsDebug())
            {
                SyncHelper[] syncHelpers = monoBeh.SyncHelpers.GetAllSyncHelpers();

                Parallel.ForEach(syncHelpers, syncHelper =>
                {
                    ObjectCleaner.ClearByAttribute<ClearAttribute>(syncHelper.Data);
                });

                monoBeh.ImageTypeSetter.ClearAllIds();
                monoBeh.CanvasDrawer.ButtonDrawer.Buttons.Clear();
            }
        }
    }
}
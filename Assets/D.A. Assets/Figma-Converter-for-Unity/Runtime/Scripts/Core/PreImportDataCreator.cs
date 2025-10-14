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
using UnityEngine.UI;

namespace DA_Assets.FCU
{
    [Serializable]
    public class PreImportDataCreator : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        public const string TO_IMPORT_MENU_ID = "1000023990958793079";
        public const string TO_REMOVE_MENU_ID = "1000082127942688996";

        internal async Task<PreImportInput> GetPreImportData(List<FObject> fobjects, List<SyncHelper> syncHelpers)
        {
            SelectableObject<DiffInfo> toImport = await GetToImport(fobjects, syncHelpers);
            toImport.SetAllSelected(true);

            SelectableObject<SyncData> toRemove = await GetToRemove(fobjects, syncHelpers);
            toRemove.SetAllSelected(false);

            DALogger.Log($"GetPreImportData | toImport: {toImport.Childs.Count} | toRemove: {toRemove.Childs.Count}");

            return new PreImportInput
            {
                ToImport = toImport,
                ToRemove = toRemove,
            };
        }

        private async Task<SelectableObject<DiffInfo>> GetToImport(List<FObject> fobjects, List<SyncHelper> syncHelpers)
        {
            SelectableObject<DiffInfo> toImport = new SelectableObject<DiffInfo>
            {
                Object = new DiffInfo
                {
                    Id = TO_IMPORT_MENU_ID,
                    Name = TO_IMPORT_MENU_ID
                }
            };

            Dictionary<string, DiffInfo> allObjects = new Dictionary<string, DiffInfo>();

            for (int i = 0; i < syncHelpers.Count; i++)
            {
                if (monoBeh.IsCancellationRequested(TokenType.Import))
                    return toImport;

                if (i % 100 == 0)
                {
                    await Task.Yield();
                }

                SyncHelper syncHelper = syncHelpers[i];

                if (syncHelper.Data.RootFrame == null)
                {
                    DALogger.LogError($"RootFrame is null for '{syncHelper.gameObject.name}', skip.");
                    continue;
                }

                allObjects[syncHelper.Data.Id] = new DiffInfo
                {
                    Id = syncHelper.Data.Id,
                    IsFrame = syncHelper.ContainsTag(FcuTag.Frame),
                    RootFrame = syncHelper.Data.RootFrame,
                    Name = syncHelper.gameObject.name,
                    OldData = syncHelper.Data
                };
            }

            for (int i = 0; i < fobjects.Count; i++)
            {
                if (monoBeh.IsCancellationRequested(TokenType.Import))
                    return toImport;

                if (i % 100 == 0)
                {
                    await Task.Yield();
                }

                FObject fobject = fobjects[i];

                if (fobject.Data?.RootFrame == null)
                {
                    DALogger.LogError($"RootFrame is null for '{fobject.Name}', skip.");
                    continue;
                }

                if (allObjects.TryGetValue(fobject.Id, out DiffInfo diffModel))
                {
                    diffModel.NewData = fobject;
                }
                else
                {
                    diffModel = new DiffInfo();
                    diffModel.Name = fobject.Name;
                    diffModel.IsFrame = fobject.ContainsTag(FcuTag.Frame);
                    diffModel.Id = fobject.Id;
                    diffModel.RootFrame = fobject.Data.RootFrame;
                    diffModel.IsNew = true;
                    diffModel.NewData = fobject;
                }

                allObjects[fobject.Id] = diffModel;
            }

            allObjects = allObjects.Where(x => !x.Value.NewData.IsDefault()).ToDictionary(x => x.Key, x => x.Value);

            Dictionary<string, DiffInfo> allObjectsWithDiffFlags = new Dictionary<string, DiffInfo>();

            foreach (KeyValuePair<string, DiffInfo> obj in allObjects)
            {
                if (monoBeh.IsCancellationRequested(TokenType.Import))
                    return toImport;

                DiffInfo di = obj.Value;

                if (obj.Value.OldData != null && !obj.Value.NewData.IsDefault())
                {
                    if (di.OldData.HashData == di.NewData.Data.HashData)
                    {
                        di.HasFigmaDiff = false;
                    }
                    else
                    {
                        di.HasFigmaDiff = true;
                    }

                    if (di.OldData.GameObject.TryGetComponentSafe(out RectTransform rectTransform))
                    {
                        Vector2 oldSize = new Vector2(rectTransform.rect.width, rectTransform.rect.height);

                        Vector2 fobjectSize = di.NewData.Size;
                        bool hasBoundingSize = di.NewData.GetBoundingSize(out Vector2 bSize);
                        bool hasRenderSize = di.NewData.GetRenderSize(out Vector2 rSize);

                        // Local function for rounding a vector
                        Vector2 Round(Vector2 vec) => vec.Round(FcuConfig.Rounding.DiffCheckerSizeDigits);

                        // Local function for comparing vectors after rounding
                        bool IsEqual(Vector2 a, Vector2 b) => Round(a).Equals(Round(b));

                        // Local function to find a size that is different from oldSize
                        Vector2 GetDifferentSize()
                        {
                            if (!IsEqual(oldSize, fobjectSize))
                                return fobjectSize;
                            if (hasBoundingSize && !IsEqual(oldSize, bSize))
                                return bSize;
                            if (hasRenderSize && !IsEqual(oldSize, rSize))
                                return rSize;
                            // Fallback if all sizes match (should not occur in this branch)
                            return fobjectSize;
                        }

                        // If at least one of the sizes equals oldSize, execute the block with default values
                        if (IsEqual(oldSize, fobjectSize) ||
                            (hasBoundingSize && IsEqual(oldSize, bSize)) ||
                            (hasRenderSize && IsEqual(oldSize, rSize)))
                        {
                            di.Size = new TProp<Vector2, Vector2>(false, default, default);
                        }
                        else
                        {
                            // Otherwise, use the size that is different from oldSize
                            di.Size = new TProp<Vector2, Vector2>(true, oldSize, GetDifferentSize());
                        }
                    }

                    if (di.OldData.GameObject.TryGetComponentSafe(out Graphic oldGraphic))
                    {
                        if (!di.NewData.Fills.IsEmpty() && oldGraphic.color != di.NewData.Fills[0].Color)
                        {
                            di.Color = new TProp<Color, Color>(true, oldGraphic.color, di.NewData.Fills[0].Color);
                        }
                        else
                        {
                            di.Color = new TProp<Color, Color>(false, default, default);
                        }
                    }
                }

                allObjectsWithDiffFlags.Add(obj.Key, di);
            }

            allObjects = allObjectsWithDiffFlags;

            Dictionary<string, SelectableObject<DiffInfo>> selectableObjects = new Dictionary<string, SelectableObject<DiffInfo>>();

            foreach (DiffInfo obj in allObjects.Values)
            {
                if (monoBeh.IsCancellationRequested(TokenType.Import))
                    return toImport;

                if (!obj.IsFrame)
                    continue;

                selectableObjects.Add(obj.RootFrame.Id, new SelectableObject<DiffInfo>
                {
                    Object = obj,
                    Childs = new List<SelectableObject<DiffInfo>>()
                });
            }

            foreach (DiffInfo obj in allObjects.Values)
            {
                if (monoBeh.IsCancellationRequested(TokenType.Import))
                    return toImport;

                if (obj.IsFrame)
                    continue;

                selectableObjects[obj.RootFrame.Id].Childs.Add(new SelectableObject<DiffInfo>
                {
                    Object = obj,
                    Childs = new List<SelectableObject<DiffInfo>>()
                });
            }

            toImport.Childs = selectableObjects.Values.ToList();

            return toImport;
        }

        private async Task<SelectableObject<SyncData>> GetToRemove(List<FObject> fobjects, List<SyncHelper> syncHelpers)
        {
            var toRemove = new SelectableObject<SyncData>
            {
                Object = new SyncData
                {
                    Id = TO_REMOVE_MENU_ID
                }
            };

            fobjects = fobjects.Where(x => x.Data?.RootFrame != null).ToList();
            syncHelpers = syncHelpers.Where(x => x.Data?.RootFrame != null).ToList();

            SelectableObject<SyncData>[] syncHelpersByFrame = syncHelpers
                .GroupBy(x => x.Data.RootFrame)
                .Select(g => new SelectableObject<SyncData>
                {
                    Childs = g.Select(x => new SelectableObject<SyncData>
                    {
                        Object = x.Data
                    }).ToList(),
                    Object = g.First(x => x.Data.RootFrame == x.Data).Data
                }).ToArray();

            FrameGroup[] fobjectsByFrame = fobjects
                .GroupBy(x => x.Data.RootFrame)
                .Select(g => new FrameGroup
                {
                    Childs = g.Select(x => x).ToList(),
                    RootFrame = g.First()
                }).ToArray();

            for (int i = 0; i < syncHelpersByFrame.Length; i++)
            {
                if (monoBeh.IsCancellationRequested(TokenType.Import))
                    return toRemove;

                if (i % 100 == 0)
                {
                    await Task.Yield();
                }

                SelectableObject<SyncData> syncGroup = syncHelpersByFrame[i];

                SelectableObject<SyncData> selectableObj = new SelectableObject<SyncData>();
                selectableObj.Object = syncGroup.Object;
                selectableObj.Childs = new List<SelectableObject<SyncData>>();

                for (int j = 0; j < syncGroup.Childs.Count; j++)
                {
                    if (j % 100 == 0)
                    {
                        await Task.Yield();
                    }

                    SelectableObject<SyncData> onSceneObj = syncGroup.Childs[j];

                    if (onSceneObj.Object.Tags.Contains(FcuTag.Frame))
                        continue;

                    bool isMissing = true;
                    foreach (FrameGroup frameGroup in fobjectsByFrame)
                    {
                        if (frameGroup.RootFrame.Id != syncGroup.Object.Id)
                            continue;

                        foreach (FObject fobject in frameGroup.Childs)
                        {
                            if (fobject.Id == onSceneObj.Object.Id)
                            {
                                isMissing = false;
                                break;
                            }
                        }
                    }

                    if (isMissing)
                    {
                        selectableObj.Childs.Add(onSceneObj);
                    }
                }

                if (!selectableObj.Childs.IsEmpty())
                {
                    toRemove.Childs.Add(selectableObj);
                }
            }

            return toRemove;
        }
    }
}

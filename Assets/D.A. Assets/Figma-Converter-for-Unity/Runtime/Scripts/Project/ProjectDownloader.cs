using DA_Assets.Extensions;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.DAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DA_Assets.Logging;
using DA_Assets.Constants;

namespace DA_Assets.FCU
{
    [Serializable]
    public class ProjectDownloader : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        public void DownloadProject()
        {
            if (monoBeh.IsJsonNetExists() == false)
            {
                DALogger.LogError(FcuLocKey.log_cant_find_package.Localize(DAConstants.JsonNetPackageName));
                return;
            }

            monoBeh.AssetTools.StopImport(StopImportReason.Hidden);

            CancellationTokenSource cts = monoBeh.CancellationTokenController.CreateNew(TokenType.Import);
            _ = DownloadProjectAsync(cts);
        }

        private async Task DownloadProjectAsync(CancellationTokenSource cts)
        {
            monoBeh.InspectorDrawer.SelectableDocument.Childs.Clear();

            if (monoBeh.Authorizer.IsAuthed() == false)
            {
                DALogger.LogError(FcuLocKey.log_need_auth.Localize());
                monoBeh.Events.OnProjectDownloadFail?.Invoke(monoBeh);
                return;
            }

            monoBeh.Events.OnProjectDownloadStart?.Invoke(monoBeh);

            DARequest projectRequest = RequestCreator.CreateProjectRequest(
                monoBeh.RequestSender.GetRequestHeader(monoBeh.Authorizer.Token),
                monoBeh.Settings.MainSettings.ProjectUrl);

            DAResult<FigmaProject> result = await monoBeh.RequestSender.SendRequest<FigmaProject>(projectRequest);

            if (result.Success)
            {
                monoBeh.CurrentProject.FigmaProject = result.Object;
                monoBeh.CurrentProject.ProjectName = result.Object.Name;
                monoBeh.InspectorDrawer.FillSelectableFramesArray(monoBeh.CurrentProject.FigmaProject.Document);

                DALogger.Log(FcuLocKey.log_project_downloaded.Localize());

                monoBeh.Events.OnProjectDownloaded?.Invoke(monoBeh);
            }
            else
            {
                switch (result.Error.Status)
                {
                    case 403:
                        DALogger.LogError(FcuLocKey.log_need_auth.Localize());
                        break;
                    case 404:
                        DALogger.LogError(FcuLocKey.log_project_not_found.Localize());
                        break;
                    default:
                        DALogger.LogError(FcuLocKey.log_unknown_error.Localize(result.Error.Message, result.Error.Status, result.Error.Exception));
                        break;
                }

                monoBeh.Events.OnProjectDownloadFail?.Invoke(monoBeh);
            }
        }

        public async Task<List<FObject>> DownloadAllNodes(List<string> selectedIds)
        {
            List<List<FObject>> nodeChunks = new List<List<FObject>>();
            List<List<string>> idChunks = selectedIds.Split(FcuConfig.ChunkSizeGetNodes);

            foreach (List<string> chunk in idChunks)
            {
                string ids = string.Join(",", chunk);

                DARequest projectRequest = RequestCreator.CreateNodeRequest(
                    monoBeh.RequestSender.GetRequestHeader(monoBeh.Authorizer.Token),
                    monoBeh.Settings.MainSettings.ProjectUrl,
                    ids);

                _ = Task.Run(async () =>
                {
                    DAResult<FigmaProject> result = await monoBeh.RequestSender.SendRequest<FigmaProject>(projectRequest);

                    if (result.Success)
                    {
                        List<FObject> docs = new List<FObject>();

                        if (!result.Object.IsDefault())
                        {
                            if (!result.Object.Nodes.IsEmpty())
                            {
                                foreach (var item in result.Object.Nodes)
                                {
                                    if (item.Value.IsDefault())
                                        continue;

                                    docs.Add(item.Value.Document);
                                }
                            }
                        }

                        nodeChunks.Add(docs);
                    }
                    else
                    {
                        nodeChunks.Add(default);

                        switch (result.Error.Status)
                        {
                            default:
                                DALogger.LogError(FcuLocKey.log_cant_get_part_of_frames.Localize(result.Error.Message, result.Error.Status));
                                break;
                        }
                    }
                });
            }

            int tempCount = -1;

            while (FcuLogger.WriteLogBeforeEqual(nodeChunks, idChunks, FcuLocKey.log_getting_frames, nodeChunks.CountAll(), selectedIds.Count(), ref tempCount))
            {
                await Task.Delay(1000);
            }

            return nodeChunks.FromChunks();
        }
    }
}
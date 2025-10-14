using DA_Assets.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.DAI;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using DA_Assets.Logging;

namespace DA_Assets.FCU
{
    [Serializable]
    public class AssetTools : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        public async Task ReselectFcu()
        {
            GameObject tempGo = MonoBehExtensions.CreateEmptyGameObject();
            await Task.Delay(100);
            tempGo.MakeGameObjectSelectedInHierarchy();
            await Task.Delay(100);
            SelectFcu();
            tempGo.Destroy();
        }

        public void SelectFcu()
        {
            monoBeh.gameObject.MakeGameObjectSelectedInHierarchy();
        }

        [HideInInspector, SerializeField] bool needShowRateMe;
        public bool NeedShowRateMe
        {
            get
            {
                if (needShowRateMe)
                {
#if UNITY_EDITOR
                    if (UnityEditor.EditorPrefs.GetInt(FcuConfig.RATEME_PREFS_KEY, 0) == 1)
                        return false;
#else
                    return false;
#endif
                }

                return needShowRateMe;
            }
            set => needShowRateMe = value;
        }

        public void DestroyLastImportedFrames()
        {
            CancellationTokenSource cts = monoBeh.CancellationTokenController.CreateNew(TokenType.Import);
            _ = DestroyLastImportedFramesAsync(cts);
        }

        private async Task DestroyLastImportedFramesAsync(CancellationTokenSource cts)
        {
            foreach (SyncData syncData in monoBeh.CurrentProject.LastImportedFrames)
            {
                syncData.GameObject.Destroy();
            }

            monoBeh.CurrentProject.LastImportedFrames.Clear();
            await Task.Yield();
        }

        public static void CreateFcuOnScene()
        {
            GameObject go = MonoBehExtensions.CreateEmptyGameObject();

            go.TryAddComponent(out FigmaConverterUnity fcu);
            go.name = string.Format(FcuConfig.CanvasGameObjectName, fcu.Guid);

            fcu.CanvasDrawer.AddCanvasComponent();
        }

        public void StopImport(StopImportReason reason)
        {
            monoBeh.CancellationTokenController.CancelAll();

            switch (reason)
            {
                case StopImportReason.Manual:
                    DALogger.LogSuccess(FcuLocKey.log_import_stoped_manually.Localize());
                    break;
                case StopImportReason.Error:
                    DALogger.Log(FcuLocKey.log_import_stoped_because_error.Localize());
                    break;
                case StopImportReason.TaskCanceled:
                    DALogger.Log(FcuLocKey.log_import_task_canceled.Localize());
                    break;
                case StopImportReason.End:
                    DALogger.LogSuccess(FcuLocKey.log_import_complete.Localize());
                    break;
            }
        }
/*
        public void RestoreResolutionData()
        {
            if (this.ResolutionData.GameViewSizeReceived)
            {
                monoBeh.DelegateHolder.SetGameViewSize(this.ResolutionData.GameViewSize);
            }
        }*/

        internal void ShowRateMe()
        {
            int componentsCount = monoBeh.TagSetter.TagsCounter.Values.Sum();
            int importErrorCount = monoBeh.AssetTools.GetConsoleErrorCount();

            if (importErrorCount > 0 || componentsCount < 1)
            {
                needShowRateMe = false;
                return;
            }

            needShowRateMe = true;
        }

        public int GetConsoleErrorCount()
        {
#if UNITY_EDITOR
            try
            {
                Type logEntriesType = System.Type.GetType("UnityEditor.LogEntries, UnityEditor");
                if (logEntriesType == null)
                {
                    return 0;
                }

                MethodInfo getCountsByTypeMethod = logEntriesType.GetMethod("GetCountsByType", BindingFlags.Static | BindingFlags.Public);
                if (getCountsByTypeMethod == null)
                {
                    return 0;
                }

                int errorCount = 0;
                int warningCount = 0;
                int logCount = 0;
                object[] args = new object[] { errorCount, warningCount, logCount };

                getCountsByTypeMethod.Invoke(null, args);

                errorCount = (int)args[0];
                warningCount = (int)args[1];
                logCount = (int)args[2];

                return errorCount;
            }
            catch (Exception)
            {
                return 1;
            }
#else
            return 1;
#endif
        }

        public static int GetMaxFileNumber(string folderPath, string prefix, string extension)
        {
            string[] files = Directory.GetFiles(folderPath, $"{prefix}*.{extension}", SearchOption.AllDirectories);
            int maxNumber = -1;

            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                int number = ExtractFileNumber(fileName, prefix);
                if (number > maxNumber)
                {
                    maxNumber = number;
                }
            }

            return maxNumber;
        }

        private static int ExtractFileNumber(string fileName, string prefix)
        {
            if (fileName == prefix)
            {
                return 0;
            }

            char[] separators = { ' ', '-', '_' };

            foreach (char separator in separators)
            {
                if (fileName.StartsWith(prefix + separator))
                {
                    string numberPart = fileName.Substring(prefix.Length + 1);
                    if (int.TryParse(numberPart, out int number))
                    {
                        return number;
                    }
                }
            }

            return -1;
        }
    }
}
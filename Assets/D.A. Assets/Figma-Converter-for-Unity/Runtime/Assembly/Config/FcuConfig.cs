using DA_Assets.Constants;
using DA_Assets.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Singleton;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#pragma warning disable CS0649

namespace DA_Assets.FCU
{
    [CreateAssetMenu(menuName = DAConstants.Publisher + "/FCU Config")]
    public class FcuConfig : AssetConfig<FcuConfig>
    {
        [SerializeField] List<TagConfig> tags;
        public static List<TagConfig> TagConfigs => Instance.tags;

        [Header("File names")]
        [SerializeField] string webLogFileName;
        public static string WebLogFileName => Instance.webLogFileName;

        [Header("Formats")]
        [SerializeField] string dateTimeFormat1;
        public static string DateTimeFormat1 => Instance.dateTimeFormat1;

        [Header("GameObject names")]
        [SerializeField] string canvasGameObjectName;
        public static string CanvasGameObjectName => Instance.canvasGameObjectName;

        [SerializeField] string i2LocGameObjectName;
        public static string I2LocGameObjectName => Instance.i2LocGameObjectName;

        [Header("Values")]

        [SerializeField] RoundingConfig rounding;
        public static RoundingConfig Rounding => Instance.rounding;

        [SerializeField] int recentProjectsLimit = 20;
        public static int RecentProjectsLimit => Instance.recentProjectsLimit;

        [SerializeField] int figmaSessionsLimit = 10;
        public static int FigmaSessionsLimit => Instance.figmaSessionsLimit;

        [SerializeField] int logFilesLimit = 50;
        public static int LogFilesLimit => Instance.logFilesLimit;

        [SerializeField] int maxRenderSize = 4096;
        public static int MaxRenderSize => Instance.maxRenderSize;

        [SerializeField] int renderUpscaleFactor = 2;
        public static int RenderUpscaleFactor => Instance.renderUpscaleFactor;

        [SerializeField] string blurredObjectTag = "UIBlur";
        public static string BlurredObjectTag => Instance.blurredObjectTag;

        [SerializeField] string blurCameraTag = "BackgroundBlur";
        public static string BlurCameraTag => Instance.blurCameraTag;

        [SerializeField] char realTagSeparator = '-';
        public static char RealTagSeparator => Instance.realTagSeparator;

        [Tooltip("If an object has more than **N** children, **SmartTags** and **Hashes** will not be assigned to them.")]
        [SerializeField] int childParsingLimit = 512;
        public static int ChildParsingLimit => Instance.childParsingLimit;

        [Header("Api")]
        [SerializeField] int apiRequestsCountLimit = 2;
        public static int ApiRequestsCountLimit => Instance.apiRequestsCountLimit;

        [SerializeField] int apiTimeoutSec = 5;
        public static int ApiTimeoutSec => Instance.apiTimeoutSec;

        [SerializeField] int chunkSizeGetNodes;
        public static int ChunkSizeGetNodes => Instance.chunkSizeGetNodes;

        [SerializeField] int frameListDepth = 2;
        public static int FrameListDepth => Instance.frameListDepth;

        [SerializeField] string gFontsApiKey;
        public static string GoogleFontsApiKey { get => Instance.gFontsApiKey; set => Instance.gFontsApiKey = value; }

        [Header("Other")]
        [SerializeField] Sprite whiteSprite32px;
        public static Sprite WhiteSprite32px => Instance.whiteSprite32px;

        [SerializeField] Sprite missingImageTexture128px;
        public static Sprite MissingImageTexture128px => Instance.missingImageTexture128px;

        [SerializeField] TextAsset baseClass;
        public static TextAsset BaseClass => Instance.baseClass;

        [SerializeField] Material imageLinearMaterial;
        public static Material ImageLinearMaterial => Instance.imageLinearMaterial;

        [SerializeField] VectorMaterials vectorMaterials;
        public static VectorMaterials VectorMaterials => Instance.vectorMaterials;

        /////////////////////////////////////////////////////////////////////////////////
        //CONSTANTS//////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////

        public const string ProductName = "Figma Converter for Unity";
        public const string ProductNameShort = "FCU";
        public const string DestroyChilds = "Destroy childs";
        public const string SetFcuToSyncHelpers = "Set current FCU to SyncHelpers";
        public const string CompareTwoObjects = "Compare two selected objects";
        public const string DestroyLastImported = "Destroy last imported frames";
        public const string DestroySyncHelpers = "Destroy SyncHelpers";
        public const string CreatePrefabs = "Create Prefabs";
        public const string UpdatePrefabs = "Update Prefabs";
        public const string Create = "Create";
        public const string OptimizeSyncHelpers = "Optimize SyncHelpers";
        public const string GenerateScripts = "Generate scripts";

        public const float IMAGE_SCALE_MIN = 0.25f;
        public const float IMAGE_SCALE_MAX = 4f;

        public static char HierarchyDelimiter { get; } = '/';
        public static string PARENT_ID { get; } = "603951929:602259738";
        public static char AsterisksChar { get; } = '•';
        public static string DefaultLocalizationCulture { get; } = "en-US";

        public static string RATEME_PREFS_KEY { get; } = "DONT_SHOW_RATEME";
        public static string RECENT_PROJECTS_PREFS_KEY { get; } = "recentProjectsPrefsKey";
        public static string FIGMA_SESSIONS_PREFS_KEY { get; } = "FigmaSessions";

        public static string ClientId => "LaB1ONuPoY7QCdfshDbQbT";
        public static string ClientSecret => "E9PblceydtAyE7Onhg5FHLmnvingDp";
        public static string RedirectUri => "http://localhost:1923/";
        public static string OAuthUrl => "https://www.figma.com/oauth?client_id={0}&redirect_uri={1}&scope=file_read&state={2}&response_type=code";

        private static string logPath;
        public static string LogPath
        {
            get
            {
                if (logPath.IsEmpty())
                    logPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Logs");

                logPath.CreateFolderIfNotExists();

                return logPath;
            }
        }

        private static string cachePath;
        public static string CachePath
        {
            get
            {
                if (cachePath.IsEmpty())
                {
                    string tempFolder = Path.GetTempPath();
                    cachePath = Path.Combine(tempFolder, "FcuCache");
                }

                cachePath.CreateFolderIfNotExists();

                return cachePath;
            }
        }
    }
}
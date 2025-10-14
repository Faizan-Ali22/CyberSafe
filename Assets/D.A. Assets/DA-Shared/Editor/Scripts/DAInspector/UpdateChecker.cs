using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace DA_Assets.Tools
{
    public enum AssetType
    {
        fcu = 1,
        dab = 2,
        uitk_converter = 3,
        dal = 4,
        img_overflow = 5,
        uitk_linker = 6
    }

    public enum VersionType
    {
        stable = 0,
        beta = 1,
        buggy = 2
    }

    public static class UpdateChecker
    {
        // Caching developer messages by asset type and current version string
        private static Dictionary<(AssetType, string), DeveloperMessage[]> _developerMessagesCache = new Dictionary<(AssetType, string), DeveloperMessage[]>();

        // Cache for computed version display information
        private static Dictionary<(AssetType, string), VersionDisplayInfo> _versionDisplayCache = new Dictionary<(AssetType, string), VersionDisplayInfo>();

        // URL for downloading the configuration
        private static readonly string ConfigUrl = "https://da-assets.github.io/site/files/webConfig.json";

        // Configuration loaded from the server
        private static WebConfig _webConfig;
        private static bool _configLoaded = false;

        // Cached styles for link labels
        private static GUIStyle WhiteStyle;
        private static GUIStyle RedStyle;
        private static GUIStyle BlueStyle;

        // Static constructor – initializes styles and starts configuration loading
        static UpdateChecker()
        {
            InitializeStyles();
            LoadWebConfig();
        }

        private static void InitializeStyles()
        {
            WhiteStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 10,
                normal = { textColor = Color.white }
            };

            RedStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 10,
                normal = { textColor = Color.red }
            };

            BlueStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = 10,
                normal = { textColor = Color.blue }
            };
        }

        /// <summary>
        /// Asynchronously loads the configuration from a remote server.
        /// </summary>
        private static async void LoadWebConfig()
        {
            using (UnityWebRequest request = UnityWebRequest.Get(ConfigUrl))
            {
                var operation = request.SendWebRequest();
                // Wait for the request to complete
                while (!operation.isDone)
                {
                    await Task.Delay(50);
                }

#if UNITY_2020_1_OR_NEWER
                if (request.result == UnityWebRequest.Result.Success)
#else
                if (!request.isNetworkError && !request.isHttpError)
#endif
                {
                    try
                    {
                        _webConfig = JsonUtility.FromJson<WebConfig>(request.downloadHandler.text);
                        _configLoaded = true;
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("Failed to parse web config: " + ex);
                        _configLoaded = false;
                    }
                }
                else
                {
                    Debug.LogError("Failed to load web config: " + request.error);
                    _configLoaded = false;
                }
            }
        }

        /// <summary>
        /// Retrieves and caches the computed version display information for a given asset type and current version string.
        /// </summary>
        private static VersionDisplayInfo GetVersionDisplayInfo(AssetType assetType, string currentVersionStr)
        {
            AssetConfig? assetConfig = GetAssetConfiguration(assetType);
            if (assetConfig == null)
                return null;

            if (!TryParseVersion(currentVersionStr, out Version currentVersion))
                return null;

            AssetVersion? currentAssetVersion = GetCurrentAssetVersion(assetConfig.Value, currentVersionStr);
            if (!currentAssetVersion.HasValue)
                return null;

            (GUIContent currentContent, GUIStyle currentStyle) = BuildCurrentVersionInfo(currentAssetVersion.Value, currentVersionStr);

            AssetVersion? latestAssetVersion = GetLatestAssetVersion(assetConfig.Value);
            if (!latestAssetVersion.HasValue)
                return null;

            (GUIContent latestContent, GUIStyle latestStyle, bool isUpToDate) = BuildLatestVersionInfo(assetConfig.Value, currentVersion, latestAssetVersion.Value);

            return new VersionDisplayInfo
            {
                AssetConfig = assetConfig.Value,
                CurrentAssetVersion = currentAssetVersion.Value,
                LatestAssetVersion = latestAssetVersion.Value,
                CurrentContent = currentContent,
                CurrentStyle = currentStyle,
                IsUpToDate = isUpToDate,
                LatestContent = latestContent,
                LatestStyle = latestStyle,
            };
        }

        private static AssetConfig? GetAssetConfiguration(AssetType assetType)
        {
            AssetConfig assetConfig = _webConfig.Assets.FirstOrDefault(a => a.Type == assetType);
            if (string.IsNullOrEmpty(assetConfig.Name) || assetConfig.Versions == null || assetConfig.Versions.Count == 0)
            {
                return null;
            }
            return assetConfig;
        }

        private static bool TryParseVersion(string versionStr, out Version version)
        {
            try
            {
                version = new Version(versionStr);
                return true;
            }
            catch
            {
                version = null;
                return false;
            }
        }

        private static AssetVersion? GetCurrentAssetVersion(AssetConfig assetConfig, string currentVersionStr)
        {
            return assetConfig.Versions.FirstOrDefault(v =>
                v.Version.Equals(currentVersionStr, StringComparison.OrdinalIgnoreCase));
        }

        private static (GUIContent content, GUIStyle style) BuildCurrentVersionInfo(AssetVersion currentAssetVersion, string currentVersionStr)
        {
            string label = $"{currentVersionStr} [current";
            GUIStyle style = WhiteStyle;
            string tooltip = "Current version information";

            label += $", {currentAssetVersion.VersionType}";
            if (currentAssetVersion.VersionType == VersionType.buggy)
            {
                style = RedStyle;
            }
            tooltip = currentAssetVersion.Description;
            label += "]";
            GUIContent content = new GUIContent(label, tooltip);
            return (content, style);
        }

        private static AssetVersion? GetLatestAssetVersion(AssetConfig assetConfig)
        {
            return assetConfig.Versions.LastOrDefault();
        }

        private static (GUIContent content, GUIStyle style, bool isUpToDate) BuildLatestVersionInfo(AssetConfig assetConfig, Version currentVersion, AssetVersion latestAssetVersion)
        {
            Version latestVersion;
            try
            {
                latestVersion = new Version(latestAssetVersion.Version);
            }
            catch
            {
                latestVersion = currentVersion;
            }

            bool isUpToDate = currentVersion >= latestVersion;
            GUIContent content = null;
            GUIStyle style = WhiteStyle;

            if (!isUpToDate)
            {
                string label = $"{latestAssetVersion.Version} [latest";
                if (DateTime.TryParseExact(latestAssetVersion.ReleaseDate, "MMM d, yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime releaseDate))
                {
                    TimeSpan diff = DateTime.Now - releaseDate;
                    string timeAgo = FormatTimeSpan(diff);
                    label += $", {timeAgo}";
                    if (diff.TotalDays > assetConfig.OldVersionDaysCount)
                    {
                        style = BlueStyle;
                    }
                }
                label += $", {latestAssetVersion.VersionType}]";
                if (latestAssetVersion.VersionType == VersionType.buggy)
                {
                    style = RedStyle;
                }
                string tooltip = latestAssetVersion.Description;
                content = new GUIContent(label, tooltip);
            }
            return (content, style, isUpToDate);
        }


        /// <summary>
        /// Displays version information of the asset in Unity Editor as clickable links.
        /// If the current version is not the latest, information about the update is shown next to it.
        /// Clicking opens the Package Manager window for the asset.
        /// </summary>
        /// <param name="assetType">Type of the asset.</param>
        /// <param name="currentVersionStr">Current version of the asset (e.g., "1.0.0").</param>
        public static void DrawVersionLabels(AssetType assetType, string currentVersionStr)
        {
            // If the configuration is not loaded yet – display a loading message.
            if (!_configLoaded || _webConfig.Assets == null)
            {
                EditorGUILayout.LabelField("Loading version information...", EditorStyles.boldLabel);
                return;
            }

            var cacheKey = (assetType, currentVersionStr);
            if (!_versionDisplayCache.TryGetValue(cacheKey, out VersionDisplayInfo displayInfo))
            {
                displayInfo = GetVersionDisplayInfo(assetType, currentVersionStr);
                _versionDisplayCache[cacheKey] = displayInfo;
            }

            if (displayInfo == null)
            {
                EditorGUILayout.LabelField("Asset configuration not found.", EditorStyles.boldLabel);
                return;
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace(); // Add flexible space for right-alignment

            if (GUILayout.Button(displayInfo.CurrentContent, displayInfo.CurrentStyle))
            {
                UnityEditor.PackageManager.UI.Window.Open(displayInfo.AssetConfig.Name);
            }

            if (!displayInfo.IsUpToDate)
            {
                GUILayout.Label(" — ", EditorStyles.label);
                if (GUILayout.Button(displayInfo.LatestContent, displayInfo.LatestStyle))
                {
                    UnityEditor.PackageManager.UI.Window.Open(displayInfo.AssetConfig.Name);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Formats a time interval into a readable string.
        /// </summary>
        /// <param name="ts">Time interval.</param>
        /// <returns>A string like "5 minutes ago", "2 days ago", etc.</returns>
        private static string FormatTimeSpan(TimeSpan ts)
        {
            if (ts.TotalMinutes < 1)
            {
                return "just now";
            }
            else if (ts.TotalHours < 1)
            {
                int minutes = (int)ts.TotalMinutes;
                return $"{minutes} minute{(minutes == 1 ? "" : "s")} ago";
            }
            else if (ts.TotalDays < 1)
            {
                int hours = (int)ts.TotalHours;
                return $"{hours} hour{(hours == 1 ? "" : "s")} ago";
            }
            else
            {
                int days = (int)ts.TotalDays;
                return $"{days} day{(days == 1 ? "" : "s")} ago";
            }
        }

        /// <summary>
        /// Draws developer messages for the specified asset type and current version.
        /// The method retrieves (and caches) the asset configuration and current asset version,
        /// then displays any associated developer messages.
        /// </summary>
        /// <param name="assetType">Type of the asset.</param>
        /// <param name="currentVersionStr">Current version string (e.g., "1.0.0").</param>
        public static void DrawDeveloperMessages(AssetType assetType, string currentVersionStr)
        {
            if (!_configLoaded || _webConfig.Assets == null)
                return;

            var key = (assetType, currentVersionStr);
            if (!_developerMessagesCache.TryGetValue(key, out DeveloperMessage[] messages))
            {
                // Retrieve asset configuration for the specified asset type.
                AssetConfig assetConfig = _webConfig.Assets.FirstOrDefault(a => a.Type == assetType);
                if (string.IsNullOrEmpty(assetConfig.Name))
                    return;

                // Parse the current version.
                Version currentVersion;
                try
                {
                    currentVersion = new Version(currentVersionStr);
                }
                catch
                {
                    return;
                }

                // Find the current asset version in the configuration.
                AssetVersion? currentAssetVersion = assetConfig.Versions.FirstOrDefault(v =>
                {
                    try { return new Version(v.Version).Equals(currentVersion); }
                    catch { return false; }
                });

                // Gather developer messages from both the asset and the current version.
                var messageList = new List<DeveloperMessage>();
                if (!string.IsNullOrEmpty(assetConfig.DeveloperMessage.Text))
                    messageList.Add(assetConfig.DeveloperMessage);

                if (currentAssetVersion.HasValue && !string.IsNullOrEmpty(currentAssetVersion.Value.DeveloperMessage.Text))
                    messageList.Add(currentAssetVersion.Value.DeveloperMessage);

                messages = messageList.ToArray();
                _developerMessagesCache[key] = messages;
            }

            // Display the developer messages if any exist.
            if (messages.Length > 0)
            {
                EditorGUILayout.Space(15);
                foreach (var msg in messages)
                {
                    EditorGUILayout.HelpBox(msg.Text, msg.Type);
                    EditorGUILayout.Space(5);
                }
            }
        }

        public static int GetFirstVersionDaysCount(AssetType assetType)
        {
            try
            {
                if (!_configLoaded || _webConfig.Assets == null)
                {
                    return -1;
                }

                AssetConfig assetInfo = _webConfig.Assets.FirstOrDefault(x => x.Type == assetType);
                if (assetInfo.Versions == null || assetInfo.Versions.Count == 0)
                {
                    return -1;
                }

                AssetVersion firstVersion = assetInfo.Versions.First();
                DateTime firstDt = DateTime.ParseExact(firstVersion.ReleaseDate, "MMM d, yyyy", CultureInfo.InvariantCulture);
                int daysCount = (int)Math.Abs((DateTime.Now - firstDt).TotalDays);

                return daysCount;
            }
            catch
            {
                return -1;
            }
        }

        public static AssetVersion? GetCachedAssetVersion(AssetType assetType, string currentVersionStr)
        {
            var cacheKey = (assetType, currentVersionStr);
            if (_versionDisplayCache.TryGetValue(cacheKey, out VersionDisplayInfo displayInfo))
            {
                return displayInfo.CurrentAssetVersion;
            }
            return null;
        }

        private class VersionDisplayInfo
        {
            public AssetConfig AssetConfig { get; set; }
            public GUIContent CurrentContent { get; set; }
            public GUIStyle CurrentStyle { get; set; }
            public bool IsUpToDate { get; set; }
            public GUIContent LatestContent { get; set; }
            public GUIStyle LatestStyle { get; set; }
            public AssetVersion CurrentAssetVersion { get; internal set; }
            public AssetVersion LatestAssetVersion { get; internal set; }
        }

        [Serializable]
        private struct AssetConfig
        {
            [SerializeField] string name;
            public string Name => name;

            [SerializeField] AssetType assetType;
            public AssetType Type => assetType;

            [SerializeField] int oldVersionDaysCount;
            public int OldVersionDaysCount => oldVersionDaysCount;

            [SerializeField] DeveloperMessage developerMessage;
            public DeveloperMessage DeveloperMessage => developerMessage;

            [SerializeField] List<AssetVersion> versions;
            public List<AssetVersion> Versions => versions;
        }

        [Serializable]
        private struct WebConfig
        {
            [SerializeField] List<AssetConfig> assets;
            public List<AssetConfig> Assets => assets;
        }
    }

    [Serializable]
    public struct DeveloperMessage
    {
        [SerializeField] string text;
        public string Text => text;

        [SerializeField] MessageType type;
        public MessageType Type => type;
    }

    [Serializable]
    public struct AssetVersion
    {
        [SerializeField] string version;
        public string Version => version;

        [SerializeField] VersionType versionType;
        public VersionType VersionType => versionType;

        [SerializeField] string releaseDate;
        public string ReleaseDate => releaseDate;

        [SerializeField] string description;
        public string Description => description;

        [SerializeField] DeveloperMessage developerMessage;
        public DeveloperMessage DeveloperMessage => developerMessage;
    }
}
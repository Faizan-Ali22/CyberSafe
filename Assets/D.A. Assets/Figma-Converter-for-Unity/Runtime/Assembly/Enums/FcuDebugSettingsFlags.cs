using System;

namespace DA_Assets.FCU
{
    [Flags]
    public enum FcuDebugSettingsFlags
    {
        None = 0,
        LogDefault = 1 << 0,
        LogSetTag = 1 << 1,
        LogIsDownloadable = 1 << 2,
        LogTransform = 1 << 3,
        LogGameObjectDrawer = 1 << 4,
        LogComponentDrawer = 1 << 5,
        LogHashGenerator = 1 << 6,
        DebugMode = 1 << 7,
        LogError = 1 << 8,
        LogAngle = 1 << 9,
        LogMask = 1 << 10
    }
}

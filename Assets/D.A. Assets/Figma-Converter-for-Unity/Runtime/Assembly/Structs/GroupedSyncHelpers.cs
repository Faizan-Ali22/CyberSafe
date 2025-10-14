using DA_Assets.FCU.Model;
using System.Collections.Generic;

namespace DA_Assets.FCU
{
    public struct GroupedSyncHelpers
    {
        public SyncData RootFrame { get; set; }
        public List<SyncHelper> SyncHelpers { get; set; }
    }
}
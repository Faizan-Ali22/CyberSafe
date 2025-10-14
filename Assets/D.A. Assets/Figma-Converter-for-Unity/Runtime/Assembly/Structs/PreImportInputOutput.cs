using DA_Assets.FCU.Model;
using System.Collections.Generic;

namespace DA_Assets.FCU
{
    public struct PreImportInput
    {
        public SelectableObject<DiffInfo> ToImport { get; set; }
        public SelectableObject<SyncData> ToRemove { get; set; }
    }

    public struct PreImportOutput
    {
        public IEnumerable<string> ToImport { get; set; }
        public IEnumerable<SyncData> ToRemove { get; set; }
    }
}
using DA_Assets.FCU.Model;
using System.Collections.Generic;

namespace DA_Assets.FCU
{
    public struct FrameGroup
    {
        public FObject RootFrame { get; set; }
        public List<FObject> Childs { get; set; }
    }
}

using DA_Assets.FCU.Model;
using DA_Assets.Tools;
using UnityEngine;

namespace DA_Assets.FCU
{
    public class DiffInfo : IHaveId
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsFrame { get; set; }
        public SyncData RootFrame { get; set; }

        public FObject NewData { get; set; }
        public SyncData OldData { get; set; }
        public bool IsNew { get; set; }
        public bool HasFigmaDiff { get; set; }
        public TProp<Color, Color> Color { get; set; }
        public TProp<Vector2, Vector2> Size { get; set; }
    }
}
using DA_Assets.FCU.Model;
using DA_Assets.DAI;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace DA_Assets.FCU
{
    public delegate Task DrawByTag(FObject fobject, FcuTag tag, Action onDraw);
    public delegate bool GetGameViewSize(out Vector2 size);

    [Serializable]
    public class DelegateHolder : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        public Action<PreImportInput, Action<PreImportOutput>> ShowDifferenceChecker { get; set; }
        public Func<Vector2, bool> SetGameViewSize { get; set; }
        public Action<Sprite, Vector4> SetSpriteRects { get; set; }
        public Action UpdateScrollContent { get; set; }
    }
}
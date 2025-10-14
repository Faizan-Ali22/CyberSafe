using DA_Assets.FCU.Model;
using System;

namespace DA_Assets.FCU
{
    public struct ScriptData
    {
        public FObject FObject { get; set; }
        public Type ComponentType { get; set; }

        public ScriptData(FObject fobject, Type type)
        {
            this.FObject = fobject;
            this.ComponentType = type;
        }
    }
}
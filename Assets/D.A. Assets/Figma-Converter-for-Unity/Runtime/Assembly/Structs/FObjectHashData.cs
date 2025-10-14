using DA_Assets.FCU.Model;
using System.Collections.Generic;

namespace DA_Assets.FCU
{
    public struct FObjectHashData
    {
        private int indent;
        private FObject fobject;
        private List<FieldHashData> hashes;
        public List<EffectHashData> effectDatas;

        public FObjectHashData(FObject fobject, List<FieldHashData> hashes, List<EffectHashData> effectDatas, int indent)
        {
            this.fobject = fobject;
            this.hashes = hashes;
            this.indent = indent;
            this.effectDatas = effectDatas;
        }

        public int Indent => indent;
        public FObject FObject => fobject;
        public List<FieldHashData> FieldHashes => hashes;
        public List<EffectHashData> EffectDatas => effectDatas;
    }

    public struct FieldHashData
    {
        private string name;
        private object data;

        public FieldHashData(string name, object data)
        {
            this.name = name;
            this.data = data;
        }

        public string Name => name;
        public object Data => data;
    }

    public struct EffectHashData
    {
        private string name;
        private List<FieldHashData> data;

        public EffectHashData(string name, List<FieldHashData> data)
        {
            this.name = name;
            this.data = data;
        }

        public string Name => name;
        public List<FieldHashData> Data => data;
    }
}
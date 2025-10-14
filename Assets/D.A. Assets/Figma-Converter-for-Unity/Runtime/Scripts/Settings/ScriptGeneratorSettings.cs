using DA_Assets.DAI;
using System;
using System.IO;
using UnityEngine;

namespace DA_Assets.FCU.Model
{
    [Serializable]
    public class ScriptGeneratorSettings : MonoBehaviourLinkerRuntime<FigmaConverterUnity>
    {
        [SerializeField] FieldSerializationMode serializationMode = FieldSerializationMode.SyncHelpers;
        public FieldSerializationMode SerializationMode { get => serializationMode; set => SetValue(ref serializationMode, value); }

        [SerializeField] string @namespace = "MyNamespace";
        public string Namespace { get => @namespace; set => SetValue(ref @namespace, value); }

        [SerializeField] string baseClass = nameof(MonoBehaviour);
        public string BaseClass { get => baseClass; set => SetValue(ref baseClass, value); }

        [SerializeField] string outputPath = Path.Combine("Assets", "GeneratedScripts");
        public string OutputPath { get => outputPath; set => SetValue(ref outputPath, value); }

        [SerializeField] int fieldNameMaxLenght = 16;
        public int FieldNameMaxLenght { get => fieldNameMaxLenght; set => SetValue(ref fieldNameMaxLenght, value); }

        [SerializeField] int methodNameMaxLenght = 16;
        public int MethodNameMaxLenght { get => methodNameMaxLenght; set => SetValue(ref methodNameMaxLenght, value); }

        [SerializeField] int classNameMaxLenght = 16;
        public int ClassNameMaxLenght { get => classNameMaxLenght; set => SetValue(ref classNameMaxLenght, value); }
    }

    public enum FieldSerializationMode
    {
        SyncHelpers = 0,
        Attributes = 1,
        GameObjectNames = 2
    }
}
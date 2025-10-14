using System;

namespace DA_Assets.FCU.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = false)]
    public class FObjectAttribute : Attribute
    {
        /// <summary>
        /// The object's name in Figma.
        /// </summary>
        public string Name { get; }

        public FObjectAttribute(string name)
        {
            Name = name;
        }
    }
}
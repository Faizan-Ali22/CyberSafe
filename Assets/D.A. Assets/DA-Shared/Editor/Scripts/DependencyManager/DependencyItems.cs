using DA_Assets.Constants;
using DA_Assets.Singleton;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DA_Assets.DM
{
    [CreateAssetMenu(menuName = DAConstants.Publisher + "/DependencyItems")]
    [ResourcePath("")]
    public class DependencyItems : SingletonScriptableObject<DependencyItems>
    {
        [SerializeField] List<DependencyItem> items;
        public List<DependencyItem> Items => items;
    }

    [Serializable]
    public struct DependencyItem
    {
        public string Name;
        public string Type;
        public string ScriptingDefineName;
        public bool Enabled { get; set; }
    }
}

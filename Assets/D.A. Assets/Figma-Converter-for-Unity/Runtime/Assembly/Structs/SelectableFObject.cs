using DA_Assets.FCU.Model;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DA_Assets.FCU
{
    [Serializable]
    public class SelectableFObject
    {
        [SerializeField] string id;
        public string Id { get => id; set => id = value; }

        [SerializeField] string name;
        public string Name { get => name; set => name = value; }

        [SerializeField] NodeType type;
        public NodeType Type { get => type; set => type = value; }

        [SerializeField] bool selected;
        public bool Selected { get => selected; set => selected = value; }

        /// https://issuetracker.unity3d.com/issues/serialization-depth-limit-10-exceeded-at-evolutionnode-dot-icon-there-may-be-an-object-composition-cycle-in-one-or-more-of-your-serialized-classes-dot-when-opening-the-project
        [SerializeField, SerializeReference] List<SelectableFObject> childs = new List<SelectableFObject>();
        public List<SelectableFObject> Childs { get => childs; set => childs = value; }

        public void SetAllSelected(bool value)
        {
            selected = value;

            foreach (SelectableFObject child in childs)
            {
                child.SetAllSelected(value);
            }
        }
    }
}

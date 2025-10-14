using System.Collections.Generic;
using UnityEngine;

namespace DA_Assets.FCU
{
    public class SelectableObject<T>
    {
        [SerializeField] bool selected;
        public bool Selected { get => selected; set => selected = value; }

        [SerializeField] List<SelectableObject<T>> childs = new List<SelectableObject<T>>();
        public List<SelectableObject<T>> Childs { get => childs; set => childs = value; }

        public T Object { get; set; }

        public void SetAllSelected(bool value)
        {
            selected = value;

            foreach (SelectableObject<T> child in childs)
            {
                child.SetAllSelected(value);
            }
        }
    }
}
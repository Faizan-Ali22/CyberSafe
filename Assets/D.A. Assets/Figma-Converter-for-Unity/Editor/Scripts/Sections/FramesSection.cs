using DA_Assets.DAI;
using DA_Assets.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DA_Assets.FCU
{
    [Serializable]
    internal class FramesSection : MonoBehaviourLinkerEditor<FcuEditor, FigmaConverterUnity>
    {
        protected int _visibleItemCount = 10;
        protected float _itemHeight = 35;

        private Dictionary<string, InfinityScrollRectWindow<SelectableFObject>> _scrolls = new Dictionary<string, InfinityScrollRectWindow<SelectableFObject>>();

        public void UpdateScrollContent()
        {
            _scrolls.Clear();

            foreach (SelectableFObject item in monoBeh.InspectorDrawer.SelectableDocument.Childs)
            {
                var isrw = new InfinityScrollRectWindow<SelectableFObject>(_visibleItemCount, _itemHeight, scriptableObject.gui);
                _scrolls.Add(item.Id, isrw);
            }
        }

        public void DrawDocument()
        {
            SelectableFObject doc = monoBeh.InspectorDrawer.SelectableDocument;

            int selectedCount = doc.Childs.Where(x => x != null).SelectRecursive(x => x.Childs).Count(x => x.Childs.IsEmpty() && x.Selected);
            int allCount = doc.Childs.Where(x => x != null).SelectRecursive(x => x.Childs).Count(x => x.Childs.IsEmpty());
            bool isAllSelected = selectedCount == allCount;

            SetCheckboxValue(doc.Id, isAllSelected);

            gui.DrawMenu(monoBeh.InspectorDrawer.SelectableHamburgerItems, new HamburgerItem
            {
                Id = doc.Id,
                GUIContent = new GUIContent(FcuLocKey.label_frames_to_import.Localize(selectedCount, allCount), ""),
                Body = () =>
                {
                    foreach (SelectableFObject page in doc.Childs)
                    {
                        DrawPage(page);
                    }   
                },
                CheckBoxValueChanged = (id, value) => SetAllChildrenSelected(doc, value)
            });
        }

        private void DrawPage(SelectableFObject page)
        {
            int selectedCount = page.Childs.Where(x => x != null).SelectRecursive(x => x.Childs).Count(x => x.Childs.IsEmpty() && x.Selected);
            int allCount = page.Childs.Where(x => x != null).SelectRecursive(x => x.Childs).Count(x => x.Childs.IsEmpty());
            bool isAllSelected = selectedCount == allCount;

            SetCheckboxValue(page.Id, isAllSelected);

            gui.DrawMenu(monoBeh.InspectorDrawer.SelectableHamburgerItems, new HamburgerItem
            {
                Id = page.Id,
                GUIContent = new GUIContent($"{page.Name} ({selectedCount}/{allCount})", ""),
                Body = () =>
                {
                    _scrolls.TryGetValue(page.Id, out var scroll);
                    scroll.SetData(page.Childs, DrawFrame);
                    scroll.OnGUI();
                },
                CheckBoxValueChanged = (id, value) => SetAllChildrenSelected(page, value)
            });
        }

        private void DrawFrame(SelectableFObject item)
        {
            item.Selected = gui.CheckBox(new GUIContent(item.Name), item.Selected, rightSide: false, onValueChange: () =>
            {
                monoBeh.InspectorDrawer.FillSelectableFramesArray(monoBeh.CurrentProject.FigmaProject.Document);
            });
        }

        private void SetCheckboxValue(string id, bool value)
        {
            var checkBoxValue = monoBeh.InspectorDrawer.SelectableHamburgerItems.FirstOrDefault(item => item.Id == id)?.CheckBoxValue;
            if (checkBoxValue != null)
            {
                checkBoxValue.Value = value;
                checkBoxValue.Temp = value;
            }
        }

        private void SetAllChildrenSelected(SelectableFObject item, bool selected)
        {
            item.Selected = selected;
            foreach (var child in item.Childs)
            {
                SetAllChildrenSelected(child, selected);
            }
        }
    }
}
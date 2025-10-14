using UnityEngine;

namespace DA_Assets.DAI
{
    public delegate void UITabContent();

    public class UITab
    {
        public UITab(string label, string tooltip, UITabContent content, int? labelWidth = null)
        {
            this.Label = new GUIContent(label, tooltip);
            this.Content = content;
            this.LabelWidth = labelWidth;
        }

        public GUIContent Label { get; set; }
        public UITabContent Content { get; set; }
        public int? LabelWidth { get; set; }
        public bool Selected { get; set; }
    }
}
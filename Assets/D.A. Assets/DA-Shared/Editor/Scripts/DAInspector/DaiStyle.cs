using System;
using UnityEngine;

namespace DA_Assets.DAI
{
    [Serializable]
    public struct DaiStyle
    {
        // Labels
        public GUIStyle Label12px;
        public GUIStyle RedLabel10px;
        public GUIStyle BlueLabel10px;
        public GUIStyle Label10px;
        public GUIStyle CheckBoxLabel;
        public GUIStyle LinkLabel10px;
        public GUIStyle BigFieldLabel12px;
        public GUIStyle LabelCentered12px;

        [Space]

        // Buttons
        public GUIStyle OutlineButton;
        public GUIStyle SquareButton30x30;
        public GUIStyle TabButton;
        public GUIStyle LinkButton;
        public GUIStyle HamburgerButton;
        public GUIStyle HamburgerImageSubButton;
        public GUIStyle HamburgerTextSubButton;
        public GUIStyle HamburgerExpandButton;
        public GUIStyle HamburgerButtonBg;
        public GUIStyle Group6Buttons;
        public GUIStyle Group5Buttons;
        public GUIStyle TabSelector;

        [Space]

        // Backgrounds
        public GUIStyle Background;
        public GUIStyle WindowRootBg;
        public GUIStyle DAInspectorBackground;
        public GUIStyle DiffCheckerBackground;
        public GUIStyle DiffCheckerRightPanel;
        public GUIStyle DiffCheckerToImportPanel;
        public GUIStyle DiffCheckerToRemovePanel;
        public GUIStyle BoxPanel;
        public GUIStyle HambugerButtonBg;
        public GUIStyle HamburgerTabsBg;

        [Space]

        // Fields
        public GUIStyle TextField;
        public GUIStyle ActiveToggle;
        public GUIStyle ObjectField;
        public GUIStyle BigTextField;
        public GUIStyle BigDropdown;
        public GUIStyle CheckBoxField;

        [Space]

        // Progress Bars
        public GUIStyle ProgressBar;
        public GUIStyle ProgressBarBg;

        [Space]

        // Tabs
        public GUIStyle TabBg1;
        public GUIStyle TabBg2;

        [Space]

        // Separators
        public GUIStyle HorizontalSeparator;

        [Space]

        // Images
        public GUIStyle ImgStar;

        [Space]

        // Others
        public GUIStyle None;
        public GUIStyle Logo;
        public GUIStyle SectionHeader;
    }
}
using DA_Assets.Constants;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

#pragma warning disable CS0649

namespace DA_Assets.DAI
{
    [CreateAssetMenu(fileName = "DAInspector", menuName = "Custom/DAInspector")]
    [Serializable]
    public class DAInspector : ScriptableObject
    {
        private const int _hamburgerMenuItemsLimit = 100;
        private const int _defaultFieldWidth = 200;
        private Dictionary<string, GroupData> _groupDatas = new Dictionary<string, GroupData>();
        private Dictionary<string, HamburgerItem> _hamburgerItems = new Dictionary<string, HamburgerItem>();

        [SerializeField] DAI.InspectorResources _resources;
        public DAI.InspectorResources Resources => _resources;

        [SerializeField] DaiStyle _style;
        [SerializeField] ColorScheme _colorScheme;

        public DaiStyle ColoredStyle => _style;

        public Object ObjectField(GUIContent content, Object value, Type objectType, bool allowSceneObjects)
        {
            Object _value = null;

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    Label12px(content);
                    FlexibleSpace();

                    Rect position = GUILayoutUtility.GetRect(_style.ObjectField.fixedWidth, _style.ObjectField.fixedHeight);
                    Rect dropRect = position;
                    int controlID = GUIUtility.GetControlID(FocusType.Passive);

                    _value = (Object)_doObjectFieldMethod.Invoke(null, new object[]
                    {
                        position,
                        dropRect,
                        controlID,
                        value,
                        null,
                        objectType,
                        _validatorDelegate,
                        allowSceneObjects,
                        _style.ObjectField
                    });
                }
            });

            return _value;
        }

        private static MethodInfo _doObjectFieldMethod;
        private static Type _objectFieldValidatorType;
        private static Type _objectFieldValidatorOptionsType;

        public DAInspector()
        {
            FindTypes();
        }

        private void FindTypes()
        {
            if (_objectFieldValidatorType == null)
            {
                Type editorGUIType = typeof(EditorGUI);
                _objectFieldValidatorType = editorGUIType.GetNestedType("ObjectFieldValidator", BindingFlags.NonPublic);
                _objectFieldValidatorOptionsType = editorGUIType.GetNestedType("ObjectFieldValidatorOptions", BindingFlags.NonPublic);
            }

            if (_doObjectFieldMethod == null)
            {
                Type editorGUIType = typeof(EditorGUI);
                _doObjectFieldMethod = editorGUIType.GetMethod(
                    "DoObjectField",
                    BindingFlags.NonPublic | BindingFlags.Static,
                    null,
                    new Type[]
                    {
                        typeof(Rect),
                        typeof(Rect),
                        typeof(int),
                        typeof(UnityEngine.Object),
                        typeof(UnityEngine.Object),
                        typeof(Type),
                        _objectFieldValidatorType,
                        typeof(bool),
                        typeof(GUIStyle)
                    },
                    null);
            }

            if (_validatorDelegate == null)
            {
                MethodInfo myValidatorMethodInfo = typeof(DAInspector).GetMethod(nameof(ObjectFieldValidator), BindingFlags.NonPublic | BindingFlags.Static);
                _validatorDelegate = Delegate.CreateDelegate(_objectFieldValidatorType, myValidatorMethodInfo);
            }

        }

        private static Object ObjectFieldValidator(Object[] references, Type objType, SerializedProperty property, object options)
        {
            if (references != null)
            {
                foreach (Object obj in references)
                {
                    if (obj != null)
                    {
                        return obj;
                    }
                }
            }

            return null;
        }

        public void DrawSplitGroup(Group group, Action body1, Action body2)
        {
            StackFrame sf = new StackFrame(1, true);
            string methodPath = GetMethodPath(sf);
            string unicumId = $"{methodPath}-{group.InstanceId}";

            if (_groupDatas.TryGetValue(unicumId, out GroupData gd) == false)
            {
                gd = new GroupData();
                gd.SplitterPosition = group.SplitterStartPos;
                _groupDatas.Add(unicumId, gd);
            }

            group.Body = () =>
            {
                DrawGroup(new Group
                {
                    Options = new GUILayoutOption[]
                    {
                        GUILayout.Width(gd.SplitterPosition),
                        GUILayout.MaxWidth(gd.SplitterPosition),
                        GUILayout.MinWidth(gd.SplitterPosition)
                    },
                    Body = () =>
                    {
                        body1?.Invoke();
                    }
                });

                GUILayout.Box("",
                    GUILayout.Width(group.SplitterWidth),
                    GUILayout.MaxWidth(group.SplitterWidth),
                    GUILayout.MinWidth(group.SplitterWidth),
                    GUILayout.ExpandHeight(true));

                gd.SplitterRect = GUILayoutUtility.GetLastRect();

                if (group.GroupType == GroupType.Horizontal)
                {
                    EditorGUIUtility.AddCursorRect(gd.SplitterRect, MouseCursor.ResizeHorizontal);
                }
                else if (group.GroupType == GroupType.Vertical)
                {
                    EditorGUIUtility.AddCursorRect(gd.SplitterRect, MouseCursor.ResizeVertical);
                }

                DrawGroup(new Group
                {
                    Options = new GUILayoutOption[]
                    {
                        GUILayout.ExpandWidth(true)
                    },
                    Body = () =>
                    {
                        body2?.Invoke();
                    }
                });
            };

            DrawGroup(group);

            if (Event.current != null)
            {
                switch (Event.current.rawType)
                {
                    case EventType.MouseDown:
                        if (gd.SplitterRect.Contains(Event.current.mousePosition))
                        {
                            gd.IsDragging = true;
                        }
                        break;
                    case EventType.MouseDrag:
                        if (gd.IsDragging)
                        {
                            gd.SplitterPosition += Event.current.delta.x;
                        }
                        break;
                    case EventType.MouseUp:
                        if (gd.IsDragging)
                        {
                            gd.IsDragging = false;
                        }
                        break;
                }
            }
        }

        public void DrawGroup(Group group)
        {
            if (group.GroupType == GroupType.Horizontal)
            {
                if (group.Style != null)
                {
                    GUILayout.BeginHorizontal(group.Style, group.Options);
                }
                else
                {
                    GUILayout.BeginHorizontal(group.Options);
                }

                if (group.Flexible)
                    FlexibleSpace();

                group.Body.Invoke();

                if (group.Flexible)
                    FlexibleSpace();

                GUILayout.EndHorizontal();
            }
            else if (group.GroupType == GroupType.Vertical)
            {
                if (group.Style != null)
                {
                    GUILayout.BeginVertical(group.Style, group.Options);
                }
                else
                {
                    GUILayout.BeginVertical(group.Options);
                }

                if (group.Scroll)
                {
                    StackFrame sf = new StackFrame(1, true);
                    BeginScroll(group, sf);
                }

                if (group.Flexible)
                    FlexibleSpace();

                group.Body.Invoke();

                if (group.Flexible)
                    FlexibleSpace();

                if (group.Scroll)
                {
                    EndScroll();
                }

                GUILayout.EndVertical();
            }
            else if (group.GroupType == GroupType.Fade)
            {
                if (EditorGUILayout.BeginFadeGroup(group.Fade.faded))
                {
                    if (group.Flexible)
                        FlexibleSpace();

                    group.Body.Invoke();

                    if (group.Flexible)
                        FlexibleSpace();
                }

                EditorGUILayout.EndFadeGroup();
            }
            else
            {
                Debug.LogError($"Unknown group type.");
            }
        }

        public bool HamburgerButton(Rect position, GUIContent content)
        {
            bool _value = GUI.Button(position, content, _style.HamburgerButton);
            return _value;
        }

        public bool HamburgerToggle(Rect position, bool value)
        {
            bool _value = EditorGUI.Toggle(position, value, EditorStyles.toggle);
            return _value;
        }

        public bool CheckBox(GUIContent label, bool value, bool rightSide = true, Action onClick = null, Action onValueChange = null)
        {
            bool _value = false;

            DrawGroup(new Group
            {
                Style = _style.CheckBoxField,
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    if (rightSide)
                    {
                        Btn();
                    }

                    Rect rect = GUILayoutUtility.GetRect(width: 25, height: 25);

                    GUI.backgroundColor = _colorScheme.CheckBoxColor;
                    _value = EditorGUI.Toggle(
                        rect,
                        value,
                        EditorStyles.toggle);
                    GUI.backgroundColor = Color.white;

                    if (!rightSide)
                    {
                        Btn();
                    }
                }
            });

            void Btn()
            {
                if (GUILayout.Button(label, _style.CheckBoxLabel))
                {
                    if (onClick == null)
                    {
                        value = !value;
                        if (onValueChange != null)
                        {
                            onValueChange.Invoke();
                        }
                    }
                    else
                    {
                        onClick.Invoke();
                    }
                }

                GUILayout.FlexibleSpace();
            }

            return _value;
        }

        public void DrawTable(Action[,] actions)
        {
            int rows = actions.GetLength(0);
            int columns = actions.GetLength(1);

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    for (int i = 0; i < columns; i++)
                    {
                        DrawGroup(new Group
                        {
                            GroupType = GroupType.Vertical,
                            Body = () =>
                            {
                                for (int j = 0; j < rows; j++)
                                {
                                    DrawGroup(new Group
                                    {
                                        GroupType = GroupType.Horizontal,
                                        Body = actions[j, i]
                                    });
                                }
                            }
                        });
                    }
                }
            });
        }

        public int ShaderDropdown(GUIContent content, int option, string[] options, Action<int> onChange)
        {
            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    Label12px(content);
                    FlexibleSpace();

                    EditorGUI.BeginChangeCheck();
                    option = EditorGUILayout.Popup(option, options, _style.ObjectField);

                    if (EditorGUI.EndChangeCheck())
                    {
                        onChange?.Invoke(option);
                    }
                    DrawDropDownIcon();
                }
            });

            return option;
        }

        public int BigDropdown(int selectedIndex, GUIContent[] displayedOptions, Action<int> onChange, bool expand = true, int fieldWith = _defaultFieldWidth)
        {
            GUIStyle customStyle = _style.BigDropdown;
            GUIContent currentSelection = displayedOptions[selectedIndex];

            GUILayoutOption[] options;

            if (expand)
            {
                options = new GUILayoutOption[] { GUILayout.Height(customStyle.fixedHeight), GUILayout.ExpandWidth(true) };
            }
            else
            {
                Vector2 textSize = customStyle.CalcSize(new GUIContent(currentSelection.text));
                float imageWidth = 0f;

                if (currentSelection.image != null)
                {
                    imageWidth = customStyle.fixedHeight - 2;
                    imageWidth += 2f;
                }

                float calculatedWidth = textSize.x + imageWidth + 20f;
                if (calculatedWidth < fieldWith)
                {
                    calculatedWidth = fieldWith;
                }
                options = new GUILayoutOption[] { GUILayout.Height(customStyle.fixedHeight), GUILayout.Width(calculatedWidth) };
            }

            Rect dropdownRect = EditorGUILayout.GetControlRect(options);

            EditorGUI.BeginChangeCheck();
            selectedIndex = EditorGUI.Popup(dropdownRect, selectedIndex, displayedOptions, customStyle);
            if (EditorGUI.EndChangeCheck())
            {
                onChange?.Invoke(selectedIndex);
            }

            if (currentSelection.image != null)
            {
                float imageSize = customStyle.fixedHeight - 2;
                Rect imageRect = new Rect(dropdownRect.x + 1, dropdownRect.y + 1, imageSize, imageSize);
                GUI.DrawTexture(imageRect, currentSelection.image, ScaleMode.ScaleToFit);
            }

            DrawDropDownIcon(-5);

            return selectedIndex;
        }

        public int Dropdown(GUIContent content, int option, string[] displayedOptions, Action<int> onChange, int fieldWith = _defaultFieldWidth)
        {
            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    Label12px(content);
                    FlexibleSpace();
                    EditorGUI.BeginChangeCheck();
                    option = EditorGUILayout.Popup(option, displayedOptions, _style.ObjectField, GUILayout.Width(fieldWith));
                    if (EditorGUI.EndChangeCheck())
                    {
                        onChange?.Invoke(option);
                    }

                    DrawDropDownIcon();
                }
            });

            return option;
        }

        private void DrawDropDownIcon(float paddingX = 5)
        {
            Rect popupRect = GUILayoutUtility.GetLastRect();

            float iconSize = popupRect.height * 1f;
            Rect iconRect = new Rect(
                popupRect.x + popupRect.width - iconSize - paddingX, // 5 pixels right padding
                popupRect.y + (popupRect.height - iconSize) / 2 + 1, // Vertical centering
                iconSize,
                iconSize
            );

            GUIContent dropdownIcon = EditorGUIUtility.IconContent("Dropdown");
            Texture iconTexture = dropdownIcon?.image;

            if (iconTexture != null)
            {
                GUI.Label(iconRect, new GUIContent(iconTexture));
            }
            else
            {
                GUI.Label(iconRect, new GUIContent("V"));
            }
        }

        public void GenericMenu(GenericMenu menu, GUIContent content, string selectedItem)
        {
            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    Label12px(content);
                    FlexibleSpace();
                    if (Button(new GUIContent(selectedItem), _style.ObjectField, true))
                    {
                        menu.ShowAsContext();
                    }

                    DrawDropDownIcon();
                }
            });
        }

        public bool Toggle(GUIContent content, bool value, int fieldWidth = _defaultFieldWidth)
        {
            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    Label12px(content);
                    Space10();
                    FlexibleSpace();

                    GUIStyle buttonStyle = value ? _style.ActiveToggle : _style.ObjectField;

                    if (GUILayout.Button(value ? "ENABLED" : "DISABLED", buttonStyle, GUILayout.Width(_defaultFieldWidth)))
                    {
                        value = !value;
                    }
                }
            });

            return value;
        }

        public string BigTextField(string value, GUIContent content, bool password = false/*, string placeholder = "Enter text..."*/)
        {
            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    if (content != null)
                    {
                        Vector2 textSize = _style.Label12px.CalcSize(content);
                        GUILayout.Label(content, _style.BigFieldLabel12px, GUILayout.Width(textSize.x));
                        Space60();
                    }

                    GUIStyle style;

                    /*if (value == null || value == placeholder)
                    {
                        style = new GUIStyle(_coloredStyle.BigTextField);
                        style.normal.textColor = Color.gray;
                        value = placeholder;
                    }
                    else*/
                    {
                        style = _style.BigTextField;
                    }

                    if (password)
                    {
                        value = EditorGUILayout.PasswordField(value, style, GUILayout.ExpandWidth(true));
                    }
                    else
                    {
                        value = EditorGUILayout.TextField(value, style, GUILayout.ExpandWidth(true));
                    }
                }
            });

            return value;
        }

        public string TextAreaPrefs(string prefsKey, GUIContent content, string currentValue, float width)
        {
            EditorGUI.BeginChangeCheck();
            string newValue = TextArea(content, currentValue, width);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetString(prefsKey, newValue);
                return newValue;
            }
            return currentValue;
        }

        public string TextFieldPrefs(string prefsKey, GUIContent content, string currentValue)
        {
            EditorGUI.BeginChangeCheck();
            string newValue = TextField(content, currentValue);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetString(prefsKey, newValue);
                return newValue;
            }
            return currentValue;
        }


        public string TextArea(GUIContent content, string currentValue, float? width = null)
        {
            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    Label12px(content);
                    Space10();
                    FlexibleSpace();

                    if (width != null)
                    {
                        currentValue = EditorGUILayout.TextArea(currentValue, GUILayout.Width(width.Value));
                    }
                    else
                    {
                        currentValue = EditorGUILayout.TextArea(currentValue);
                    }

                }
            });

            return currentValue;
        }

        public bool SubButtonText(string label, string tooltip = null) =>
            SubButtonText(new GUIContent(label, tooltip));

        public bool SubButtonText(GUIContent content)
        {
            Vector2 size = _style.HamburgerTextSubButton.CalcSize(content);
            size += new Vector2(10, 0);
            return GUILayout.Button(content, _style.HamburgerTextSubButton, GUILayout.Width(size.x));
        }

        public bool SubButtonIcon(Texture image, string tooltip = null)
        {
            return GUILayout.Button(new GUIContent(image, tooltip), _style.HamburgerImageSubButton);
        }

        public string TextField(GUIContent content, string currentValue, GUIContent btnLabel = null, Action buttonClick = null, bool password = false, string placeholder = "Enter text...", int fieldWith = _defaultFieldWidth)
        {
            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    Label12px(content);
                    Space10();
                    FlexibleSpace();

                    Vector2 textSize = _style.TextField.CalcSize(new GUIContent(currentValue));

                    float width = textSize.x;

                    if (width > 400)
                    {
                        width = 400;
                    }
                    else if (width < fieldWith && buttonClick != null)
                    {
                        width = 160;
                    }
                    else if (width < fieldWith)
                    {
                        width = fieldWith;
                    }
                    else
                    {
                        width += 10;
                    }

                    GUIStyle style;

                    if (currentValue == null || currentValue == placeholder)
                    {
                        style = new GUIStyle(_style.TextField);
                        style.normal.textColor = Color.gray;
                        currentValue = placeholder;
                    }
                    else
                    {
                        style = _style.TextField;
                    }

                    if (password)
                    {
                        currentValue = EditorGUILayout.PasswordField(currentValue, style, GUILayout.Width(width));
                    }
                    else
                    {
                        currentValue = EditorGUILayout.TextField(currentValue, style, GUILayout.Width(width));
                    }

                    if (buttonClick != null)
                    {
                        Space10();

                        if (btnLabel.image == null)
                        {
                            if (SubButtonText(btnLabel))
                            {
                                buttonClick.Invoke();
                            }
                        }
                        else
                        {
                            if (SubButtonIcon(btnLabel.image, btnLabel.tooltip))
                            {
                                buttonClick.Invoke();
                            }
                        }
                    }
                }
            });

            return currentValue;
        }

        public float SliderField(GUIContent content, float value, float minValue, float maxValue, int fieldWith = _defaultFieldWidth)
        {
            float _value = 0;

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    Label12px(content);
                    Space10();
                    FlexibleSpace();

                    GUI.backgroundColor = _colorScheme.UnityGuiColor;
                    _value = EditorGUILayout.Slider(value, minValue, maxValue, GUILayout.Width(fieldWith));
                    GUI.backgroundColor = Color.white;
                }
            });

            return _value;
        }

        public float FloatField(GUIContent content, float value, int fieldWith = _defaultFieldWidth)
        {
            float _value = 0;

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    Label12px(content);
                    Space10();
                    FlexibleSpace();

                    _value = EditorGUILayout.FloatField(value, _style.ObjectField, GUILayout.Width(fieldWith));
                }
            });

            return _value;
        }

        public int IntField(GUIContent content, int value, int fieldWith = _defaultFieldWidth)
        {
            int _value = 0;

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    if (content!=null)
                    {
                        Label12px(content);
                        Space10();
                        FlexibleSpace();
                    }

                    _value = EditorGUILayout.IntField(value, _style.ObjectField, GUILayout.Width(fieldWith));
                }
            });

            return _value;
        }

        public Vector2 Vector2Field(GUIContent content, Vector2 value, int fieldWith = _defaultFieldWidth)
        {
            Vector2 _value = default;

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    Label12px(content);
                    Space10();
                    FlexibleSpace();

                    GUI.backgroundColor = _colorScheme.UnityGuiColor;
                    _value = EditorGUILayout.Vector2Field("", value, GUILayout.Width(fieldWith));
                    GUI.backgroundColor = Color.white;
                }
            });

            return _value;
        }

        public Color ColorField(GUIContent content, Color value, int fieldWith = _defaultFieldWidth)
        {
            Color _value = default;

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    Label12px(content);
                    Space10();
                    FlexibleSpace();

                    GUI.backgroundColor = _colorScheme.UnityGuiColor;
                    _value = EditorGUILayout.ColorField("", value, GUILayout.Width(fieldWith));
                    GUI.backgroundColor = Color.white;
                }
            });

            return _value;
        }

        public Vector2Int Vector2IntField(GUIContent content, Vector2Int value, int fieldWith = _defaultFieldWidth)
        {
            Vector2Int _value = default;

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    Label12px(content);
                    Space10();
                    FlexibleSpace();

                    GUI.backgroundColor = _colorScheme.UnityGuiColor;
                    _value = EditorGUILayout.Vector2IntField("", value, GUILayout.Width(fieldWith));
                    GUI.backgroundColor = Color.white;
                }
            });

            return _value;
        }

        public Vector4 Vector4Field(GUIContent content, Vector4 value, int fieldWith = _defaultFieldWidth)
        {
            Vector4 _value = default;

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    Label12px(content);
                    Space10();
                    FlexibleSpace();

                    GUI.backgroundColor = _colorScheme.UnityGuiColor;
                    _value = EditorGUILayout.Vector4Field("", value, GUILayout.Width(fieldWith));
                    GUI.backgroundColor = Color.white;
                }
            });

            return _value;
        }

        public int LayerField(GUIContent content, int layer)
        {
            int result = 0;

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    Label12px(content);
                    Space10();
                    FlexibleSpace();

                    Rect r = EditorGUILayout.GetControlRect(false, 20);
                    result = EditorGUI.LayerField(r, layer, _style.ObjectField);
                    Space(-5);
                }
            });

            return result;
        }

        public void Line(bool horizontal = true, int lineHeight = 1)
        {
            if (horizontal)
            {
                Rect rect = EditorGUILayout.GetControlRect(false, lineHeight);
                rect.height = lineHeight;
                rect.x -= 3;

                Color lineColor = Color.gray;
                lineColor.a = 0.25f;

                EditorGUI.DrawRect(rect, lineColor);
            }
            else
            {
                Rect rect = EditorGUILayout.GetControlRect(false, lineHeight);
                rect.width = 1;
                rect.height = lineHeight;
                rect.x += rect.width / 2;

                Color lineColor = Color.gray;
                lineColor.a = 0.25f;

                EditorGUI.DrawRect(rect, lineColor);
            }

        }

        public void TopProgressBar(float value)
        {
            Rect position = GUILayoutUtility.GetRect(0, 7, GUILayout.ExpandWidth(true));

            int controlId = GUIUtility.GetControlID(nameof(TopProgressBar).GetHashCode(), FocusType.Keyboard);

            if (Event.current.GetTypeForControl(controlId) == EventType.Repaint)
            {
                if (value > 0.0f)
                {
                    Rect barRect = new Rect(position);
                    barRect.width *= value;
                    _style.ProgressBar.Draw(barRect, false, false, false, false);
                }
            }
        }

        public bool LinkLabel(GUIContent label, GUIStyle customStyle, params GUILayoutOption[] options)
        {
            bool clicked = false;

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    Rect btnRect = GUILayoutUtility.GetRect(label, customStyle, options);
                    clicked = GUI.Button(btnRect, label, customStyle);
                }
            });

            return clicked;
        }

        public void HelpBox(string message, MessageType type)
        {
            GUI.backgroundColor = _colorScheme.UnityGuiColor;
            EditorGUILayout.HelpBox(message, type);
            GUI.backgroundColor = Color.white;
        }

        public bool SquareButton30x30(GUIContent label)
        {
            bool clicked = false;

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    GUIStyle style = _style.SquareButton30x30;
                    Rect btnRect = GUILayoutUtility.GetRect(label, style, GUILayout.ExpandWidth(true));
                    clicked = GUI.Button(btnRect, label, style);
                }
            });

            return clicked;
        }

        public bool TabButton(UITab tab)
        {
            bool clicked = false;

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    GUIStyle style = _style.TabButton;

                    if (tab.Selected)
                    {
                        GUIStyle pbarBody = _style.TabSelector;
                        GUILayout.Box(GUIContent.none, pbarBody, GUILayout.Width(4), GUILayout.ExpandHeight(true));
                    }

                    Rect btnRect = GUILayoutUtility.GetRect(tab.Label, style, GUILayout.ExpandWidth(true));
                    clicked = GUI.Button(btnRect, tab.Label, style);
                }
            });

            return clicked;
        }

        public void BeginScroll(Group group, StackFrame sf)
        {
            string methodPath = GetMethodPath(sf);
            string unicumId = $"{methodPath}-{group.InstanceId}";

            if (_groupDatas.TryGetValue(unicumId, out GroupData gd) == false)
            {
                gd = new GroupData();
                _groupDatas.Add(unicumId, gd);
            }

            GUI.backgroundColor = _colorScheme.UnityGuiColor;
            gd.ScrollPosition = EditorGUILayout.BeginScrollView(gd.ScrollPosition, false, false);
            GUI.backgroundColor = Color.white;
        }

        public void EndScroll()
        {
            EditorGUILayout.EndScrollView();
        }

        public string GetMethodPath(StackFrame frame)
        {
            var method = frame.GetMethod();
            string className = method.DeclaringType.Name;
            int lineNumber = frame.GetFileLineNumber();
            return $"{className}-{lineNumber}";
        }

        private List<HamburgerItem> _internalHambBuffer;
        private Delegate _validatorDelegate;

        public void DrawMenu(HamburgerItem menu)
        {
            if (_internalHambBuffer == null)
            {
                _internalHambBuffer = new List<HamburgerItem>();
            }

            DrawMenu(_internalHambBuffer, menu);
        }

        public void DrawMenu(List<HamburgerItem> buffer, HamburgerItem menu)
        {
            if (buffer.Count > _hamburgerMenuItemsLimit)
            {
                int itemsToRemove = buffer.Count - _hamburgerMenuItemsLimit;
                buffer.RemoveRange(0, itemsToRemove);
            }

            int index = buffer.FindIndex(x => x.Id == menu.Id);

            if (index < 0)
            {
                buffer.Add(menu);
                index = buffer.Count - 1;
            }

            GUILayout.BeginHorizontal(_style.HambugerButtonBg);

            if (buffer[index].Fade == null)
            {
                buffer[index].Fade = new AnimBool(false)
                {
                    speed = 4f
                };
            }

            if (menu.Body != null)
            {
                Texture2D t2d = buffer[index].Fade.value ? _resources.ImgExpandOpened : _resources.ImgExpandClosed;
                GUILayout.Button(t2d, _style.HamburgerExpandButton);
            }

            Rect btnRect = GUILayoutUtility.GetRect(menu.GUIContent, _style.HamburgerButton, GUILayout.ExpandWidth(true));
            btnRect.x += 15;
            btnRect.width -= 46;

            if (HamburgerButton(btnRect, menu.GUIContent))
            {
                buffer[index].Fade.target = !buffer[index].Fade.target;
            }

            Rect smallBtnRect = default;

            if (menu.OnButtonClick != null)
            {
                smallBtnRect = GUILayoutUtility.GetRect(new GUIContent(), GUI.skin.box);
                smallBtnRect.width = 20;
                smallBtnRect.height = 20;
                smallBtnRect.x -= 15;

                GUIStyle style = menu.ButtonGuiContent.image == null
                    ? _style.HamburgerTextSubButton
                    : _style.HamburgerImageSubButton;

                if (GUI.Button(smallBtnRect, menu.ButtonGuiContent, style))
                    menu.OnButtonClick.Invoke();
            }

            if (menu.CheckBoxValueChanged != null)
            {
                Rect cbRect = btnRect;
                cbRect.x += btnRect.width;
                cbRect.width = 20;

                if (smallBtnRect == default)
                    cbRect.x += 10;
                else
                    cbRect.x -= 1;

                GUI.backgroundColor = _colorScheme.CheckBoxColor;

                buffer[index].CheckBoxValue.Value = HamburgerToggle(
                    cbRect,
                    buffer[index].CheckBoxValue.Value);

                GUI.backgroundColor = Color.white;

                if (buffer[index].CheckBoxValue.Value != buffer[index].CheckBoxValue.Temp)
                {
                    buffer[index].CheckBoxValue.Temp = buffer[index].CheckBoxValue.Value;
                    menu.CheckBoxValueChanged.Invoke(menu.Id, buffer[index].CheckBoxValue.Value);
                }
            }

            GUILayout.EndHorizontal();

            if (menu.Body != null)
            {
                DrawGroup(new Group
                {
                    GroupType = GroupType.Horizontal,
                    Body = () =>
                    {
                        Space15();

                        DrawGroup(new Group
                        {
                            GroupType = GroupType.Vertical,
                            Body = () =>
                            {
                                DrawGroup(new Group
                                {
                                    GroupType = GroupType.Fade,
                                    Fade = buffer[index].Fade,
                                    Body = () =>
                                    {
                                        Space(6);
                                        menu.Body.Invoke();
                                    }
                                });
                            }
                        });

                        Space15();
                    }
                });
            }
        }

        public T EnumField<T>(GUIContent content, T @enum, bool uppercase = true, string[] itemNames = null, Action<T> onChange = null, int fieldWidth = _defaultFieldWidth)
        {
            List<int> enumValues = Enum.GetValues(@enum.GetType()).Cast<int>().ToList();

            if (itemNames == null)
            {
                itemNames = Enum.GetNames(@enum.GetType());
            }

            if (uppercase)
            {
                for (int i = 0; i < itemNames.Length; i++)
                {
                    itemNames[i] = Regex.Replace(itemNames[i], "(\\B[A-Z])", "$1").ToUpper();
                }
            }

            int result = 0;

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    Label12px(content);
                    Space10();
                    FlexibleSpace();

                    Rect popupRect = GUILayoutUtility.GetRect(_style.ObjectField.fixedWidth, _style.ObjectField.fixedHeight, GUILayout.Width(fieldWidth));
                    int _result2 = EditorGUI.Popup(popupRect, enumValues.IndexOf(Convert.ToInt32(@enum)), itemNames, _style.ObjectField);
                    result = enumValues[_result2];
                }
            });

            T _result = (T)(object)result;

            if (_result.Equals(@enum) == false)
            {
                onChange?.Invoke(_result);
            }
            DrawDropDownIcon();
            return _result;
        }

        public void DragZoneInt(Rect dragZone, ref int dragValue)
        {
            Event currentEvent = Event.current;
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            switch (currentEvent.type)
            {
                case EventType.Repaint:
                    // Change the cursor to the horizontal resize cursor when it's over the drag zone
                    if (dragZone.Contains(currentEvent.mousePosition))
                    {
                        EditorGUIUtility.AddCursorRect(dragZone, MouseCursor.ResizeHorizontal);
                    }
                    break;

                case EventType.MouseDown:
                    // Check if the mouse is within the drag zone
                    if (dragZone.Contains(currentEvent.mousePosition))
                    {
                        GUIUtility.hotControl = controlID;
                        currentEvent.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    // Check if we are dragging the hot control
                    if (GUIUtility.hotControl == controlID)
                    {
                        // Perform drag operation here, for example, adjust the dragValue
                        dragValue += (int)currentEvent.delta.x;
                        currentEvent.Use();
                    }
                    break;

                case EventType.MouseUp:
                    // End the drag operation
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                        currentEvent.Use();
                    }
                    break;
            }
        }

        public string FileField(GUIContent content, string selectedPath, GUIContent btnLabel, string folderPanelText)
        {
            TextField(
               content,
               selectedPath,
               btnLabel,
               () =>
               {
                   string _selectedPath = EditorUtility.OpenFilePanel(folderPanelText, "", "");

                   if (string.IsNullOrWhiteSpace(_selectedPath) == false)
                   {
                       if (IsPathInsideAssetsPath(_selectedPath))
                       {
                           selectedPath = _selectedPath;
                       }
                       else
                       {
                           //Console.LogError(FcuLocKey.label_inside_assets_folder.Localize());
                       }
                   }
               });

            return ToRelativePath(selectedPath);
        }

        public string FolderField(GUIContent content, string selectedPath, GUIContent btnLabel, string folderPanelText)
        {
            TextField(
               content,
               selectedPath,
               btnLabel,
               () =>
               {
                   string _selectedPath = EditorUtility.OpenFolderPanel(folderPanelText, "", "");

                   if (string.IsNullOrWhiteSpace(_selectedPath) == false)
                   {
                       if (IsPathInsideAssetsPath(_selectedPath))
                       {
                           selectedPath = _selectedPath;
                       }
                       else
                       {
                           //Console.LogError(FcuLocKey.label_inside_assets_folder.Localize());
                       }
                   }
               });

            return ToRelativePath(selectedPath);
        }

        private bool IsPathInsideAssetsPath(string path)
        {
            if (path.IndexOf(Application.dataPath, System.StringComparison.InvariantCultureIgnoreCase) == -1)
            {
                return false;
            }

            return true;
        }

        private string ToRelativePath(string absolutePath)
        {
            if (absolutePath.StartsWith(Application.dataPath))
            {
                return "Assets" + absolutePath.Substring(Application.dataPath.Length);
            }

            return absolutePath;
        }

        public void Label10px(string label, string tooltip = null, params GUILayoutOption[] options)
        {
            Label10px(new GUIContent(label, tooltip), options);
        }

        public void Label10px(GUIContent content, params GUILayoutOption[] options)
        {
            try
            {
                GUILayout.Label(content, _style.Label10px, options);
            }
            catch
            {

            }
        }

        public void Label12px(string label, string tooltip = null, params GUILayoutOption[] options)
        {
            Label12px(new GUIContent(label, tooltip), options);
        }

        public void Label12px(GUIContent content, params GUILayoutOption[] options)
        {
            if (content != null)
            {
                Vector2 textSize = _style.Label12px.CalcSize(content);
                var combinedOptions = options.Concat(new GUILayoutOption[] { GUILayout.Width(textSize.x) }).ToArray();
                GUILayout.Label(content, _style.Label12px, combinedOptions);
            }
        }

        public void RedLinkLabel10px(GUIContent content, params GUILayoutOption[] options)
        {
            GUILayout.Label(content, _style.RedLabel10px, options);
        }

        public void SectionHeader(string label, string tooltip = null)
        {
            GUILayout.Label(new GUIContent(label, tooltip), _style.SectionHeader, GUILayout.ExpandWidth(true));
        }

        public void TabHeader(string label, string tooltip = null, float paddingTop = -9)
        {
            RectOffset stylePadding = _style.SectionHeader.padding;
            float totalPaddingTop = paddingTop + stylePadding.top;
            GUILayout.BeginVertical();
            GUILayout.Space(totalPaddingTop);
            GUILayout.Label(new GUIContent(label, tooltip), _style.SectionHeader, GUILayout.ExpandWidth(true));
            GUILayout.EndVertical();
        }

        public bool Button(GUIContent label, GUIStyle style, bool expand)
        {
            bool clicked = false;

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    GUILayoutOption[] options;
                    if (expand)
                    {
                        options = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };
                    }
                    else
                    {
                        Vector2 textSize = _style.Label12px.CalcSize(label);
                        options = new GUILayoutOption[] { GUILayout.Width(textSize.x + 20) };
                    }

                    clicked = GUILayout.Button(label, style, options);
                }
            });

            return clicked;
        }


        public bool OutlineButton(string label, string tooltip = null, bool expand = false) =>
            OutlineButton(new GUIContent(label, tooltip), expand);

        public bool OutlineButton(GUIContent content, bool expand = false) =>
            Button(content, _style.OutlineButton, expand);

        public bool LinkButton(GUIContent content, bool expand) =>
            Button(content, _style.LinkButton, expand);

        public void Space60() => GUILayout.Space(60);
        public void Space30() => GUILayout.Space(30);
        public void Space15() => GUILayout.Space(15);
        public void Space10() => GUILayout.Space(10);
        public void Space5() => GUILayout.Space(5);
        public void Space(float pixels) => GUILayout.Space(pixels);
        public void FlexibleSpace() => GUILayout.FlexibleSpace();

        private SerializedProperty GetPropertyRecursive(string[] names, int index, SerializedProperty property)
        {
            if (index >= names.Length)
            {
                return property;
            }
            else
            {
                string fieldName = names[index];
                SerializedProperty rprop = property.FindPropertyRelative(fieldName);
                return GetPropertyRecursive(names, index + 1, rprop);
            }
        }

        public IEnumerable<SerializedProperty> GetChildren(SerializedProperty property)
        {
            property = property.Copy();
            var nextElement = property.Copy();
            bool hasNextElement = nextElement.NextVisible(false);
            if (!hasNextElement)
            {
                nextElement = null;
            }

            property.NextVisible(true);
            while (true)
            {
                if ((SerializedProperty.EqualContents(property, nextElement)))
                {
                    yield break;
                }

                yield return property;

                bool hasNext = property.NextVisible(false);
                if (!hasNext)
                {
                    break;
                }
            }
        }

        public void DrawChildProperty<T>(SerializedObject so, SerializedProperty parentElement, Expression<Func<T, object>> pathExpression)
        {
            SerializedProperty targetGraphic = GetChildProperty(parentElement, pathExpression);

            if (targetGraphic == null)
            {
                return;
            }

            DrawProperty(so, targetGraphic);
        }

        public SerializedProperty GetPropertyFromArray<T>(SerializedProperty arrayProperty, int elementIndex)
        {
            if (arrayProperty?.arraySize > 0 && arrayProperty.arraySize >= elementIndex + 1)
            {
                return arrayProperty.GetArrayElementAtIndex(elementIndex);
            }

            return null;
        }

        public SerializedProperty GetChildProperty<T>(SerializedProperty arrayProperty, Expression<Func<T, object>> pathExpression)
        {
            try
            {
                string[] fields = pathExpression.GetFieldsArray();
                return arrayProperty.FindPropertyRelative(fields[0]);
            }
            catch
            {
                return null;
            }
        }

        public void DrawProperty(SerializedObject so, SerializedProperty property)
        {
            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    Space15();

                    DrawGroup(new Group
                    {
                        GroupType = GroupType.Vertical,
                        Body = () =>
                        {
                            so.Update();

                            try
                            {
                                EditorGUILayout.PropertyField(property, true);
                            }
                            catch (Exception)
                            {

                            }

                            so.ApplyModifiedProperties();
                        }
                    });
                }
            });
        }

        public static string GetFieldName<T>(Expression<Func<T, object>> pathExpression)
        {
            string[] fields = pathExpression.GetFieldsArray();
            return fields.Last();
        }

        public void SerializedPropertyField<T>(SerializedObject so, Expression<Func<T, object>> pathExpression, bool? isExpanded = null)
        {
            string[] fields = pathExpression.GetFieldsArray();

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    Space(14);

                    DrawGroup(new Group
                    {
                        GroupType = GroupType.Vertical,
                        Body = () =>
                        {
                            SerializedProperty rootProperty = so.FindProperty(fields[0]);
                            SerializedProperty lastProperty = GetPropertyRecursive(fields, 1, rootProperty);

                            if (isExpanded != null)
                            {
                                lastProperty.isExpanded = (bool)isExpanded;
                            }

                            so.Update();

                            GUI.backgroundColor = _colorScheme.UnityGuiColor;
                            EditorGUI.indentLevel--;
                            EditorGUILayout.PropertyField(lastProperty, true);
                            EditorGUI.indentLevel++;
                            GUI.backgroundColor = Color.white;

                            so.ApplyModifiedProperties();
                        }
                    });
                }
            });
        }

        public void Colorize(Action action)
        {
            GUI.backgroundColor = _colorScheme.UnityGuiColor;
            action.Invoke();
            GUI.backgroundColor = Color.white;
        }

        public void VerticalSeparator()
        {
            GUILayout.Box(GUIContent.none, _style.HorizontalSeparator, GUILayout.Width(1), GUILayout.ExpandHeight(true));
        }

        public bool Button(GUIContent content, GUILayoutOption options)
        {
            GUI.backgroundColor = _colorScheme.OutlineColor;
            bool value = GUILayout.Button(content, options);
            GUI.backgroundColor = Color.white;
            return value;
        }

        public void DrawObjectFields(object target)
        {
            Type targetType = target.GetType();
            PropertyInfo[] properties = targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            PropertyHeader headerAttribute = targetType.GetCustomAttribute<PropertyHeader>();
            if (headerAttribute != null)
            {
                SectionHeader(headerAttribute.HeaderLabel.text, headerAttribute.HeaderLabel.tooltip);
                Space10();
            }

            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];

                CustomInspectorProperty attribute = property.GetCustomAttribute<CustomInspectorProperty>();
                if (attribute == null) continue;

                if (!property.CanRead || !property.CanWrite)
                    continue;

                EditorGUI.BeginChangeCheck();
                object currentValue = property.GetValue(target);
                Type valueType = currentValue?.GetType();

                object newValue = null;

                switch (attribute.Type)
                {
                    case ComponentType.Toggle:
                        newValue = Toggle(attribute.Label, (bool)currentValue);
                        break;
                    case ComponentType.EnumField:
                        newValue = EnumField(attribute.Label, Convert.ChangeType(currentValue, property.PropertyType));
                        break;
                    case ComponentType.FloatField:
                        newValue = FloatField(attribute.Label, (float)currentValue);
                        break;
                    case ComponentType.TextField:
                        newValue = TextField(attribute.Label, (string)currentValue);
                        break;
                    case ComponentType.IntField:
                        newValue = IntField(attribute.Label, (int)currentValue);
                        break;
                    case ComponentType.Vector2Field:
                        newValue = Vector2Field(attribute.Label, (Vector2)currentValue);
                        break;
                    case ComponentType.Vector2IntField:
                        newValue = Vector2IntField(attribute.Label, (Vector2Int)currentValue);
                        break;
                    case ComponentType.Vector4Field:
                        newValue = Vector4Field(attribute.Label, (Vector4)currentValue);
                        break;
                    case ComponentType.ColorField:
                        newValue = ColorField(attribute.Label, (Color)currentValue);
                        break;
                    case ComponentType.SliderField:
                        if (valueType == typeof(int))
                        {
                            newValue = SliderField(attribute.Label, (int)currentValue, attribute.MinValue, attribute.MaxValue);
                        }
                        else if (valueType == typeof(float))
                        {
                            newValue = SliderField(attribute.Label, (float)currentValue, attribute.MinValue, attribute.MaxValue);
                        }
                        else
                        {
                            HelpBox($"Missing '{attribute.Label.text}' field.", MessageType.Warning);
                        }
                        break;
                    default:
                        HelpBox($"Missing '{attribute.Label.text}' field.", MessageType.Warning);
                        break;
                }

                Space10();

                if (EditorGUI.EndChangeCheck())
                {
                    Type fieldType = property.PropertyType;

                    if (fieldType == typeof(int))
                    {
                        property.SetValue(target, Convert.ToInt32(newValue));
                    }
                    else if (fieldType == typeof(float))
                    {
                        property.SetValue(target, Convert.ToSingle(newValue));
                    }
                    else if (fieldType == typeof(double))
                    {
                        property.SetValue(target, Convert.ToDouble(newValue));
                    }
                    else if (fieldType == typeof(long))
                    {
                        property.SetValue(target, Convert.ToInt64(newValue));
                    }
                    else if (fieldType == typeof(short))
                    {
                        property.SetValue(target, Convert.ToInt16(newValue));
                    }
                    else if (fieldType == typeof(byte))
                    {
                        property.SetValue(target, Convert.ToByte(newValue));
                    }
                    else if (fieldType == typeof(decimal))
                    {
                        property.SetValue(target, Convert.ToDecimal(newValue));
                    }
                    else if (fieldType == typeof(uint))
                    {
                        property.SetValue(target, Convert.ToUInt32(newValue));
                    }
                    else if (fieldType == typeof(ulong))
                    {
                        property.SetValue(target, Convert.ToUInt64(newValue));
                    }
                    else if (fieldType == typeof(ushort))
                    {
                        property.SetValue(target, Convert.ToUInt16(newValue));
                    }
                    else if (fieldType == typeof(sbyte))
                    {
                        property.SetValue(target, Convert.ToSByte(newValue));
                    }
                    else
                    {
                        property.SetValue(target, newValue);
                    }
                }
            }
        }

        public void DrawFooter()
        {
            Space30();

            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Flexible = true,
                Body = () =>
                {
                    if (LinkButton(new GUIContent($"made by\n{DAConstants.Publisher}"), false))
                    {
                        Application.OpenURL(DAConstants.SiteLink);
                    }
                }
            });
        }

        public void DrawAssetHeader(Texture2D logo)
        {
            DrawGroup(new Group
            {
                GroupType = GroupType.Horizontal,
                Body = () =>
                {
                    GUILayout.BeginVertical(logo, _style.Logo);
                    Space60();
                    GUILayout.EndVertical();
                }
            });
        }
    }
}
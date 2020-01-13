using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using SeanLib.Core;
using System.Text.RegularExpressions;
using UnityEditor.IMGUI.Controls;
using UnityEngine.Events;
using UnityEditor.AnimatedValues;

namespace EditorPlus
{
    public static partial class OnGUIUtility
    {
        public static class Components
        {
            public static GenericMenu Menu()
            {
                return new GenericMenu();
            }
            public class FadePool
            {
                private Dictionary<string, FadeGroup> Groups = new Dictionary<string, FadeGroup>();
                public FadeGroup Get(string Title, UnityAction Repaint, bool defaultEnable = false, params GUILayoutOption[] options)
                {
                    FadeGroup group = null;
                    if(!Groups.TryGetValue(Title,out group))
                    {
                        group = new FadeGroup();
                        group.OnEnable(Repaint, defaultEnable);
                        Groups[Title] = group;
                    }
                    return group;
                }
                public void OnDisable(UnityAction Repaint)
                {
                    var en = Groups.GetEnumerator();
                    while(en.MoveNext())
                    {
                        en.Current.Value.OnDisable(Repaint);
                    }
                    Groups.Clear();
                }
            }
            public class ComponentPool<T> where T:class, new()
            {
                private Dictionary<string, T> coms = new Dictionary<string, T>();
                public T Get(string Key,out bool isNew)
                {
                    T com = null;
                    isNew = false;
                    if (!coms.TryGetValue(Key, out com))
                    {
                        com = new T();
                        coms[Key] = com;
                        isNew = true;
                    }
                    return com;
                }
                public void Clear(Action<T> onClear)
                {
                    var en = coms.GetEnumerator();
                    while (en.MoveNext())
                    {
                        onClear(en.Current.Value);
                    }
                    coms.Clear();
                }
            }


        }
        public static class Grid
        {
            public class GridContainer<T>
            {
                public enum Operation
                {
                    none,
                    shear,
                    copy
                }
                public Operation oper = Operation.none;
                public List<T> selected = new List<T>();
                public static List<T> Clipboard = new List<T>();
                public static List<T> CopyClipboard = new List<T>();
                public Func<T, int, T> onGui;
                public Func<T, int, bool> isShow;
                public Func<T> onCreate;
                public int maxColumn = 5;
                public string add = "+";
                public string del = "x";
                public Action<GenericMenu> OnShowMenu;
                public bool Selectable;
                public void OnEnable(Func<T, int, T> onGui, Func<T> onCreate, int maxColumn = 5, string add = "+", string del = "x")
                {
                    this.onGui = onGui;
                    this.onCreate = onCreate;
                    this.maxColumn = maxColumn;
                    this.add = add;
                    this.del = del;
                }
                public void OnGUI(List<T> list)
                {
                    int indexMax = list.Count - 1;
                    int row = (indexMax + maxColumn) / maxColumn;
                    int index = 0;
                    List<int> deletes = new List<int>();
                    for (int i = 0; i < row; i++)
                    {
                        int colCount = (list.Count - index) < maxColumn ? (list.Count - index) : maxColumn;
                        GUILayout.BeginHorizontal();
                        for (int j = 0; j < colCount; j++)
                        {
                            if (isShow != null && !isShow(list[index], index))
                            {
                                index++;
                                continue;
                            }
                            list[index] = onGui(list[index], index);
                            var rect = GUILayoutUtility.GetLastRect();
                            if (Selectable&&Event.current.type == EventType.MouseDown)
                            {
                                if (Event.current.button == 0)
                                {
                                    if (rect.Contains(Event.current.mousePosition))
                                    {
                                        Event.current.Use();
                                        selected.Add(list[index]);
                                    }
                                    else
                                    {
                                        if (!Event.current.control)
                                        {
                                            selected.Clear();
                                        }
                                    }
                                }
                                else
                                {
                                    if (selected.Count >= 1 && rect.Contains(Event.current.mousePosition))
                                    {
                                        Event.current.Use();
                                        var menu = new GenericMenu();
                                        if (OnShowMenu != null)
                                        {
                                            OnShowMenu(menu);
                                        }
                                        menu.ShowAsContext();
                                    }
                                }
                            }
                            if (Event.current.type == EventType.KeyDown)
                            {
                                if (Event.current.keyCode == KeyCode.Escape)
                                {
                                    oper = Operation.none;
                                    selected.Clear();
                                }
                            }
                            if (selected.Contains(list[index]))
                            {
                                EditorGUI.DrawRect(rect, new Color(0, 0, 1, 0.1f));
                            }
                            GUILayout.BeginVertical(GUILayout.Width(del.Length * 16));
                            if (GUILayout.Button(del, EditorStyles.label))
                            {
                                deletes.Add(index);
                            }
                            GUILayout.EndVertical();
                            index++;
                        }
                        GUILayout.EndHorizontal();
                    }

                    foreach (var delete in deletes)
                    {
                        list.RemoveAt(delete);
                    }

                    GUILayout.BeginHorizontal();
                    EditorGUILayout.Space();
                    if (GUILayout.Button(add, GUILayout.Width(add.Length * 16)))
                    {
                        list.Add(onCreate());
                    }
                    GUILayout.EndHorizontal();
                }
            }
            public static void GridList<T>(List<T> list, Func<T, int, T> onGui, Func<T> onCreate, int maxColumn = 5, string add = "+", string del = "x")
            {
                try
                {
                    int indexMax = list.Count - 1;
                    int row = (indexMax + maxColumn) / maxColumn;
                    int index = 0;
                    List<int> deletes = new List<int>();
                    for (int i = 0; i < row; i++)
                    {
                        int colCount = (list.Count - index) < maxColumn ? (list.Count - index) : maxColumn;
                        GUILayout.BeginHorizontal();
                        for (int j = 0; j < colCount; j++)
                        {
                            list[index] = onGui(list[index], index);
                            GUILayout.BeginVertical(GUILayout.Width(del.Length * 16));
                            if (GUILayout.Button(del, EditorStyles.label))
                            {
                                deletes.Add(index);
                            }
                            GUILayout.EndVertical();
                            index++;
                        }
                        GUILayout.EndHorizontal();
                    }

                    foreach (var delete in deletes)
                    {
                        list.RemoveAt(delete);
                    }
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.Space();
                    if (GUILayout.Button(add, GUILayout.Width(add.Length * 16)))
                    {
                        list.Add(onCreate());
                    }
                    GUILayout.EndHorizontal();
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError(e);
                }
            }
        }
        public class Zone_Divide2Horizontal
        {
            DragArea LayoutDragLine = new DragArea();
            public Rect Area0, Area1, DivideLine;
            public float Area0Size = 100f;
            public float min = 80;
            public float Max = 300;
            public GUIStyle DivideLineStyle;
            public virtual void DrawDivideLine()
            {
                EditorGUIUtility.AddCursorRect(DivideLine.WidthNew(4), MouseCursor.ResizeHorizontal);
                DivideLineStyle = DivideLineStyle ?? new GUIStyle("Tooltip");
                GUI.Box(DivideLine, "", DivideLineStyle);
            }
            public void OnGUI(Rect rect, Action RepaintAPI, Action<Rect> DrawArea0, Action<Rect> DrawArea1)
            {
                Area1 = OnGUIUtility.Layout.Divide.Divide2Horizontal(rect, out Area0, Area0Size);
                Area0.width -= 1;
                Area1.x += 1;
                DivideLine = new Rect(Area1.x - 2, Area1.y, 2, Area1.height);
                DrawDivideLine();
                Vector2 delta = LayoutDragLine.OnGui(DivideLine.WidthNew(4));
                Area0Size = Mathf.Clamp(Area0Size + delta.x, min, Max);
                if (delta.x != 0)
                {
                    RepaintAPI();
                }
                GUILayout.BeginArea(Area0);
                DrawArea0(new Rect(0, 0, Area0.width, Area0.height));
                GUILayout.EndArea();
                GUILayout.BeginArea(Area1);
                DrawArea1(new Rect(0, 0, Area1.width, Area1.height));
                GUILayout.EndArea();
            }
            public void OnGUILayout(Action RepaintAPI, Action DrawArea0, Action DrawArea1, GUIStyle style = null, params GUILayoutOption[] options)
            {
                GUILayout.BeginHorizontal(options);
                if (style != null)
                {
                    GUILayout.BeginVertical(style, GUILayout.MinWidth(min), GUILayout.MaxWidth(Max), GUILayout.Width(Area0Size), GUILayout.ExpandHeight(true));
                }
                else
                {
                    GUILayout.BeginVertical(GUILayout.MinWidth(min), GUILayout.MaxWidth(Max), GUILayout.Width(Area0Size), GUILayout.ExpandHeight(true));
                }
                DrawArea0();
                GUILayout.EndVertical();
                DivideLineStyle = DivideLineStyle ?? new GUIStyle("Tooltip");
                GUILayout.Box("", DivideLineStyle, GUILayout.Width(1), GUILayout.ExpandHeight(true));
                if (Event.current.type != EventType.Used && Event.current.type != EventType.Layout)
                {
                    DivideLine = GUILayoutUtility.GetLastRect();
                    EditorGUIUtility.AddCursorRect(DivideLine.WidthNew(4), MouseCursor.ResizeHorizontal);
                    Vector2 delta = LayoutDragLine.OnGui(DivideLine.WidthNew(4));
                    Area0Size = Mathf.Clamp(Area0Size + delta.x, min, Max);
                    if (delta.x != 0)
                    {
                        RepaintAPI();
                    }
                }

                if (style != null)
                {
                    GUILayout.BeginVertical(style, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                }
                else
                {
                    GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                }

                DrawArea1();
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
        }

        public class Zone_Divide2Vertical
        {
            DragArea LayoutDragLine = new DragArea();
            public Rect Area0, Area1, DivideLine;
            public float Area0Size = 100f;
            public float min = 80;
            public float Max = 300;
            public GUIStyle DivideLineStyle;
            public virtual void DrawDivideLine()
            {
                EditorGUIUtility.AddCursorRect(DivideLine.HeightNew(4), MouseCursor.ResizeVertical);
                DivideLineStyle = DivideLineStyle ?? new GUIStyle("Tooltip");
                GUI.Box(DivideLine, "", DivideLineStyle);
            }
            public void OnGUI(Rect rect, Action RepaintAPI, Action DrawArea0, Action DrawArea1)
            {
                Area1 = OnGUIUtility.Layout.Divide.Divide2Vertical(rect, out Area0, Area0Size);
                Area0.height -= 1;
                Area1.y += 1;
                DivideLine = new Rect(Area1.x, Area1.y - 2, Area1.width, 2);
                DrawDivideLine();

                Vector2 delta = LayoutDragLine.OnGui(DivideLine.HeightNew(4));
                Area0Size = Mathf.Clamp(Area0Size + delta.y, min, Max);
                if (delta.y != 0)
                {
                    RepaintAPI();
                }

                GUILayout.BeginArea(Area0);
                DrawArea0();
                GUILayout.EndArea();

                GUILayout.BeginArea(Area1);
                DrawArea1();
                GUILayout.EndArea();
            }
            public void OnGUILayout(Action RepaintAPI, Action DrawArea0, Action DrawArea1, GUIStyle style = null, params GUILayoutOption[] options)
            {
                if (style != null)
                {
                    GUILayout.BeginVertical(style, GUILayout.Height(Area0Size));
                }
                else
                {
                    GUILayout.BeginVertical(GUILayout.Height(Area0Size));
                }
                DrawArea0();
                GUILayout.EndVertical();
                Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(4), GUILayout.ExpandWidth(true));
                if (Event.current.type != EventType.Used && Event.current.type != EventType.Layout)
                {
                    DivideLine = rect;
                    DivideLine.y = DivideLine.yMax - 2;
                    DivideLine.height = 2;
                    DrawDivideLine();
                    Vector2 delta = LayoutDragLine.OnGui(DivideLine.HeightNew(4));
                    Area0Size = Mathf.Clamp(Area0Size + delta.y, min, Max);
                    if (delta.y != 0)
                    {
                        RepaintAPI();
                    }
                }
                GUILayout.BeginVertical();
                DrawArea1();
                GUILayout.EndVertical();
            }
        }
        public class TabGroup
        {
            public bool[] Tabs;
            public string[] TabNames;
            private Texture[] TabTextures;

            public GUIContent[] Contents;
            public int Index;
            public bool Multiple;
            public int Count
            {
                get
                {
                    return Tabs.Length;
                }
            }
            public TabGroup(string[] tabNames, Texture[] textures = null, bool[] initShow = null)
            {
                TabNames = tabNames;
                if (initShow == null)
                    Tabs = new bool[tabNames.Length];
                else
                    Tabs = initShow;
                if (Tabs.Length > 0)
                    Tabs[Index] = true;
                this.TabTextures = textures;
                Init();
            }
            public TabGroup(string[] tabNames) : this(tabNames, null, null)
            {
            }
            void Init()
            {
                Contents = new GUIContent[Tabs.Length];
                for (int i = 0; i < Tabs.Length; i++)
                {
                    Contents[i] = new GUIContent(TabNames[i], TabTextures != null ? TabTextures[i] : null);
                }
            }
            public bool IsEnable(int index)
            {
                if (index >= Tabs.Length) return false;
                return Tabs[index];
            }
            public void SetTabs(int index)
            {
                if (!Multiple)
                {
                    Tabs = new bool[Tabs.Length];
                    Index = index;
                }
                if (Tabs.Length > 0)
                    Tabs[index] = true;
            }
            public void OnGui(GUIStyle Style = null, params GUILayoutOption[] options)
            {
                for (int i = 0; i < Tabs.Length; i++)
                {
                    EditorGUI.BeginChangeCheck();
                    bool tabEnable = Tabs[i];
                    if (Style != null)
                    {
                        tabEnable = GUILayout.Toggle(Tabs[i], Contents[i], Style, options);
                    }
                    else
                    {
                        tabEnable = GUILayout.Toggle(Tabs[i], Contents[i], options);
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (!Multiple)
                        {
                            if (Tabs[i])
                            {
                                continue;
                            }
                        }
                        if (tabEnable)
                        {
                            SetTabs(i);
                        }
                    }
                }
            }
        }

        public class FoldoutGroup
        {
            public bool foldout;
            public string title;
            public FoldoutGroup(string title)
            {
                this.title = title;
            }
            public bool OnGui(GUIStyle style = null)
            {
                if (style != null)
                {
                    foldout = EditorGUILayout.Foldout(foldout, title, true, style);
                }
                else
                {
                    foldout = EditorGUILayout.Foldout(foldout, title, true);
                }
                return foldout;
            }
        }
        public class FadeGroup
        {
            public AnimBool fadeGroup;
            public void OnEnable(UnityAction Repaint, bool defaultEnable = false)
            {
                this.fadeGroup = new AnimBool(defaultEnable);
                // 注册动画监听
                this.fadeGroup.valueChanged.AddListener(Repaint);
                OnTitle = OnTitle ?? defalutTitle;
            }

            public void OnDisable(UnityAction Repaint)
            {
                // 移除动画监听
                this.fadeGroup.valueChanged.RemoveListener(Repaint);
            }
            public bool OnGuiBegin(string Title, params GUILayoutOption[] options)
            {
                return OnGuiBegin(Title, EditorStyles.foldout, options);
            }
            public bool Begin()
            {
                return EditorGUILayout.BeginFadeGroup(fadeGroup.faded);
            }
            public void End()
            {
                EditorGUILayout.EndFadeGroup();
            }
            public delegate bool TitleGUI(string Title, GUIStyle style, GUILayoutOption[] options, AnimBool extend);

            public TitleGUI OnTitle;

            private bool defalutTitle(string Title, GUIStyle style, GUILayoutOption[] options, AnimBool extend)
            {
                extend.target = GUILayout.Toggle(extend.target, Title, style, options);
                return extend.target;
            }
            public bool OnGuiBegin(string Title, GUIStyle style, params GUILayoutOption[] options)
            {
                OnTitle(Title, style, options, fadeGroup);
                OnGUIUtility.Layout.IndentBegin();
                return Begin();
            }
            public void OnGuiEnd()
            {
                OnGUIUtility.Layout.IndentEnd();
                End();
            }
        }

        /// <summary>
        /// 可拖拽区域
        /// </summary>
        public class DragArea
        {
            bool Draging;
            public Vector2 OnGui(Rect rect)
            {
                Vector2 delta = Vector2.zero;
                if (Draging)
                {
                    if (Event.current.type == EventType.MouseUp)
                    {
                        Draging = false;
                        delta = Event.current.delta / 2;
                    }
                    else
                    {
                        delta = Event.current.delta / 2;
                    }
                }
                else
                {
                    if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
                    {
                        Draging = true;
                    }
                }
                return delta;
            }
        }


        public class Search : SearchField
        {
            public class SearchFilter
            {
                public struct Filter
                {
                    /// <summary>
                    /// 关键字
                    /// </summary>
                    public string Key;
                    /// <summary>
                    /// 关键字 参数
                    /// </summary>
                    public string Value;
                }
                public List<Filter> filters = new List<Filter>();
                /// <summary>
                /// 出关键字意外的参数
                /// </summary>
                public string value = string.Empty;
                public void Refresh(string data)
                {
                    try
                    {
                        filters.Clear();
                        value = string.Empty;
                        Regex r = new Regex(@"(\S*):(\S*)\s", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                        MatchCollection ms = r.Matches(data);
                        for (int i = 0; i < ms.Count; i++)
                        {
                            Match m = ms[i];
                            String key = m.Groups[1].ToString();
                            String value = m.Groups[2].ToString();
                            filters.Add(new Filter() { Key = key, Value = value });
                            data = data.Replace(key + ":", "");
                            if (value.IsNOTNullOrEmpty())
                            {
                                data = data.Replace(value, "");
                            }
                        }
                        data = data.Replace(" ", "");
                        value = data.ToLower();
                    }
                    catch (Exception e)
                    {
                        UnityEngine.Debug.Log(e);
                        //这里就不用 那么复杂了
                    }
                }
            }
            public SearchFilter filter = new SearchFilter();
            public string Current = "";
            public string OnToolbarGUI(Rect rect)
            {
                if (CheckChange(base.OnToolbarGUI(rect, Current)))
                {
                    filter.Refresh(Current);
                }
                return Current;
            }
            public string OnToolbarGUI(params GUILayoutOption[] options)
            {
                if (CheckChange(base.OnToolbarGUI(Current, options)))
                {
                    filter.Refresh(Current);
                }
                return Current;
            }
            public string OnGUI(Rect rect, GUIStyle style, GUIStyle cancelButtonStyle, GUIStyle emptyCancelButtonStyle)
            {
                if (CheckChange(base.OnGUI(rect, Current, style, cancelButtonStyle, emptyCancelButtonStyle)))
                {
                    filter.Refresh(Current);
                }
                return Current;
            }
            public string OnGUI(Rect rect)
            {
                if (CheckChange(base.OnGUI(rect, Current)))
                {
                    filter.Refresh(Current);
                }
                return Current;
            }
            public string OnGUI(params GUILayoutOption[] options)
            {
                if (CheckChange(base.OnGUI(Current, options)))
                {
                    filter.Refresh(Current);
                }
                return Current;
            }
            private bool CheckChange(string input)
            {
                var b = input != Current;
                Changed?.Invoke(Current, input);
                Current = input;
                return b;
            }
            public Action<string, string> Changed;
            public bool GeneralValid(string input)
            {
                if (Current.IsNOTNullOrEmpty())
                {
                    return input.ToLower().Contains(Current.ToLower());
                }
                else
                {
                    return true;
                }
            }
            public bool TypeValid(object input)
            {
                if (Current.IsNullOrEmpty()) return true;
                foreach (var kvFilter in filter.filters)
                {
                    if (kvFilter.Key == "t" && input.GetType().FullName.ToLower().Contains(kvFilter.Value.ToLower()))
                    {
                        if (filter.value.IsNullOrEmpty()) return true;
                        else
                        {
                            return input.ToString().ToLower().Contains(filter.value.ToLower());
                        }
                    }
                }
                return GeneralValid(input.ToString());
            }
        }
        public class Select<T>
        {
            public string ControlId;
            public T t;
            public Select()
            {
                ControlId = Guid.NewGuid().ToString();
            }
            public bool OnGUI(List<T> list, GUIStyle style = null)
            {
                style = style ?? EditorStyles.miniButton;
                if (GUILayout.Button("Select", style))
                {
                    SelectWindow<T>.Show(list, ControlId);
                }
                if (SelectWindow<T>.CanPick(ControlId))
                {
                    t = SelectWindow<T>.GetPick();
                    return true;
                }
                return false;
            }
        }

        public class ToolBar
        {
            public class ToolBarButton
            {
                public enum ButtonPostion
                {
                    left,
                    right
                }
                public ButtonPostion postion = ButtonPostion.left;
                public Action OnClick;
                public Action OnShow;
                public GUIContent content;
                public float width = 20f;
            }
            public List<ToolBarButton> buttons;
            public string Title;
            public Rect ToolBarRect = new Rect(0, 0, 0, 20);
            public void Set(string Title, List<ToolBarButton> buttons)
            {
                this.buttons = buttons;
                this.Title = Title;
            }
            public void OnGui(GUIStyle style = null)
            {
                style = style ?? (style = EditorStyles.toolbarButton);
                EditorGUILayout.BeginHorizontal();
                if (buttons != null)
                {
                    foreach (var button in buttons)
                    {
                        if (button.postion == ToolBarButton.ButtonPostion.left)
                        {
                            if (GUILayout.Button(button.content, style, GUILayout.Width(button.width)))
                            {
                                if (button.OnClick != null)
                                {
                                    button.OnClick.Invoke();
                                }
                            }
                            if (button.OnShow != null)
                            {
                                button.OnShow.Invoke();
                            }
                        }
                    }
                }
                GUILayout.Label(Title, style, GUILayout.ExpandWidth(true));
                if (buttons != null)
                {
                    foreach (var button in buttons)
                    {
                        if (button.postion == ToolBarButton.ButtonPostion.right)
                        {
                            if (GUILayout.Button(button.content, style, GUILayout.Width(button.width)))
                            {
                                if (button.OnClick != null)
                                {
                                    button.OnClick.Invoke();
                                }
                            }
                            if (button.OnShow != null)
                            {
                                button.OnShow.Invoke();
                            }
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
                if (Event.current.type == EventType.Repaint)
                {
                    ToolBarRect = GUILayoutUtility.GetLastRect();
                }
            }
        }


    }
}
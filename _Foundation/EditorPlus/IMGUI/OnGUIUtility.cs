using SeanLib.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Internal;
using Object = UnityEngine.Object;

namespace EditorPlus
{
    public static partial class OnGUIUtility
    {
        #region Component
        public class OrderList
        {
            public void OnGui()
            {

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
            public void OnGUI(Rect rect, Action RepaintAPI, Action DrawArea0, Action DrawArea1)
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
                DrawArea0();
                GUILayout.EndArea();
                GUILayout.BeginArea(Area1);
                DrawArea1();
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
                public string value= string.Empty;
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
                    catch(Exception e)
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
                if(CheckChange(base.OnGUI(Current, options)))
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
            public Action<string,string> Changed;
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

        #endregion
        #region Layout

        public class Layout
        {
            /// <summary>
            /// 整体分拆
            /// </summary>
            public class Divide
            {
                //横向
                public static Rect FirstRect(Rect postion, int SegmentsCount)
                {
                    return new Rect(postion.x, postion.y, postion.width / SegmentsCount, postion.height);
                }
                public static Rect NextRect(Rect LastRect)
                {
                    return new Rect(LastRect.x + LastRect.width, LastRect.y, LastRect.width, LastRect.height);
                }
                //-------------------------------------
                public static Rect Golden(Rect rect, out Rect r2)
                {
                    float big = rect.width * 0.618f;
                    Rect r1 = new Rect(rect.x, rect.y, rect.width - big, rect.height);
                    r2 = new Rect(rect.x + r1.width, rect.y, big, rect.height);
                    return r1;
                }
                public static Rect Divide2Horizontal(Rect source, out Rect r1, float r1_width)
                {
                    r1_width = Mathf.Clamp(r1_width, 0f, source.width);
                    r1 = new Rect(source.x, source.y, r1_width, source.height);
                    return new Rect(source.x + r1_width, source.y, source.width - r1_width - 4, source.height);
                }
                public static Rect Divide2Vertical(Rect source, out Rect r1, float r1_height)
                {
                    r1_height = Mathf.Clamp(r1_height, 0f, source.height);
                    r1 = new Rect(source.x, source.y, source.width, r1_height);
                    return new Rect(source.x, source.y + r1_height, source.width, source.height - r1_height - 4);
                }
                //纵向
                public static Rect FirstRectVertical(Rect postion, int SegmentsCount)
                {
                    return new Rect(postion.x, postion.y, postion.width, postion.height / SegmentsCount);
                }
                public static Rect NextRectVertical(Rect LastRect)
                {
                    return new Rect(LastRect.x, LastRect.y + LastRect.height, LastRect.width, LastRect.height);
                }
            }
            /// <summary>
            /// 逐个累计
            /// </summary>
            public class Accumulative
            {
                public static Rect NextRect(Rect LastRect)
                {
                    return new Rect(LastRect.x + LastRect.width, LastRect.y, LastRect.width, LastRect.height);
                }
            }
            public static void Line()
            {
                EditorGUILayout.LabelField("", GUI.skin.box, GUILayout.Height(1), GUILayout.ExpandWidth(true));
            }
            public static void IndentEnable()
            {
                EditorGUI.indentLevel = indentLevel;
            }
            public static void IndentDisable()
            {
                indentLevel = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
            }

            public static void IndentBegin()
            {
                EditorGUI.indentLevel++;
            }
            public static void IndentEnd()
            {
                EditorGUI.indentLevel--;
            }
        }
        #endregion
        #region Tools
        public static string OpenFilePanel(string Title, string extension)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            string savePath = EditorGUILayout.TextField(Title, EditorUserSettings.GetConfigValue(Title));
            if (EditorGUI.EndChangeCheck())
            {
                EditorUserSettings.SetConfigValue(Title, savePath);
            }
            if (!File.Exists(savePath))
            {
                savePath = Application.dataPath;
                EditorUserSettings.SetConfigValue(Title, savePath);
            }
            if (GUILayout.Button("browser", GUILayout.Width(80)))
            {
                string raw = EditorUtility.OpenFilePanel(Title, savePath, extension);
                if (!string.IsNullOrEmpty(raw) && raw != savePath)
                {
                    savePath = raw;
                    EditorUserSettings.SetConfigValue(Title, savePath);
                    GUI.FocusControl("");
                }
            }
            EditorGUILayout.EndHorizontal();

            return savePath;
        }
        public static string OpenFolderPannel(string Title, string defaultName = "")
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            string OpenDir = EditorGUILayout.TextField(Title, EditorUserSettings.GetConfigValue(Title));
            if (EditorGUI.EndChangeCheck())
            {
                EditorUserSettings.SetConfigValue(Title, OpenDir);
            }
            if (!Directory.Exists(OpenDir))
            {
                OpenDir = Application.dataPath;
                EditorUserSettings.SetConfigValue(Title, OpenDir);
            }
            if (GUILayout.Button("browser", GUILayout.Width(80)))
            {
                string raw = EditorUtility.OpenFolderPanel(Title, OpenDir, defaultName);
                if (!string.IsNullOrEmpty(raw) && raw != OpenDir)
                {
                    OpenDir = raw;
                    EditorUserSettings.SetConfigValue(Title, OpenDir);
                    GUI.FocusControl("");
                }
            }
            EditorGUILayout.EndHorizontal();
            return OpenDir;
        }
        public static string SaveFolderPanel(string Title, string defaultName = "", bool useDataPathDefault = true)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            string savePath = EditorGUILayout.TextField(Title, EditorUserSettings.GetConfigValue(Title));
            if (EditorGUI.EndChangeCheck())
            {
                EditorUserSettings.SetConfigValue(Title, savePath);
            }
            if (!Directory.Exists(savePath))
            {
                if (useDataPathDefault)
                {
                    savePath = Application.dataPath;
                    EditorUserSettings.SetConfigValue(Title, savePath);
                }
            }
            if (GUILayout.Button("browser", GUILayout.Width(80)))
            {
                string raw = EditorUtility.SaveFolderPanel(Title, savePath, defaultName);
                if (!string.IsNullOrEmpty(raw) && raw != savePath)
                {
                    savePath = raw;
                    EditorUserSettings.SetConfigValue(Title, savePath);
                    GUI.FocusControl("");
                }
            }
            EditorGUILayout.EndHorizontal();

            return savePath;
        }
        public static string SaveFilePanel(string Title, string defaultName, string extension)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            string savePath = EditorGUILayout.TextField(Title, EditorUserSettings.GetConfigValue(Title));
            if (EditorGUI.EndChangeCheck())
            {
                EditorUserSettings.SetConfigValue(Title, savePath);
            }
            if (!Directory.Exists(savePath))
            {
                savePath = Application.dataPath;
                EditorUserSettings.SetConfigValue(Title, savePath);
            }
            if (GUILayout.Button("browser", GUILayout.Width(80)))
            {
                string raw = EditorUtility.SaveFilePanel(Title, savePath, defaultName, extension);
                if (!string.IsNullOrEmpty(raw) && raw != savePath)
                {
                    savePath = raw;
                    EditorUserSettings.SetConfigValue(Title, savePath);
                    GUI.FocusControl("");
                }
            }
            EditorGUILayout.EndHorizontal();

            return savePath;
        }
        public static bool OpenClose(string title, Object instance = null)
        {
            string key = instance ? instance.GetInstanceID().ToString() : title;
            bool state = EditorPrefs.GetBool(key, false);
            string text = title;
            if (state)
            {
                text = "\u25BC" + (char)0x200a + text;
            }
            else
            {
                text = "\u25BA" + (char)0x200a + text;
            }
            if (GUILayout.Button(text, GUI.skin.GetStyle("flow varPin tooltip")))
            {
                EditorPrefs.SetBool(key, !state);
            }
            state = EditorPrefs.GetBool(key, false);
            return state;
        }
        public static bool EditorPrefsFoldoutGroup(string Title, GUIStyle style = null, params GUILayoutOption[] options)
        {
            bool extend = !string.IsNullOrEmpty(EditorUserSettings.GetConfigValue(Title));
            EditorGUI.BeginChangeCheck();
            extend = EditorGUILayout.Foldout(extend, Title, true, style ?? EditorStyles.foldout);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUserSettings.SetConfigValue(Title, extend ? "True" : string.Empty);
            }
            return extend;
        }
        public static void ScriptField(string title, Type type)
        {
            ScriptField(title, type, GUI.skin.button);
        }
        /// <summary>
        /// TypeFullName
        /// </summary>
        /// <param name="Title"></param>
        /// <param name="typeFullName"></param>
        public static void ScriptField(string Title,string typeFullName)
        {
            string typeName = typeFullName;
            var ts = typeFullName.Split('.');
            if (ts.Length>0)
            {
                typeName = ts[ts.Length - 1];
            }
            ScriptField(Title, typeName, GUI.skin.button);
        }
        public static void ScriptField(string title, string typeName, GUIStyle buttonStyle)
        {
            var assetName = typeName + " t:script";
            Object editAsset = null;
            if (!scriptCache.TryGetValue(assetName, out editAsset))
            {
                string[] s = AssetDatabase.FindAssets(assetName);
                editAsset = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(s[0]));
                if (editAsset != null)
                    scriptCache[assetName] = editAsset;
            }
            GUILayout.BeginHorizontal();

            OnGUIUtility.Vision.GUIEnabled(false);
            EditorGUILayout.ObjectField(title, editAsset, typeof(Object), true);
            OnGUIUtility.Vision.GUIEnabled(true);
            if (GUILayout.Button("Edit", buttonStyle, GUILayout.MaxWidth(200)))
            {
                AssetDatabase.OpenAsset(editAsset);
            }
            GUILayout.EndHorizontal();
        }
        public static void ScriptField(string title, Type type, GUIStyle style)
        {
            ScriptField(title, type.Name, style);
        }
        public static string SearchField(string value, params GUILayoutOption[] options)
        {
            MethodInfo info = typeof(EditorGUILayout).GetMethod("ToolbarSearchField", BindingFlags.NonPublic | BindingFlags.Static, null, new System.Type[] { typeof(string), typeof(GUILayoutOption[]) }, null);
            if (info != null)
            {
                value = (string)info.Invoke(null, new object[] { value, options });
            }
            return value;
        }
        public static Component SelectComponentField<T>(Component c, string title, params GUILayoutOption[] options) where T : Component
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            var go = EditorGUILayout.ObjectField(c ? c.gameObject : null, typeof(GameObject), true, options) as GameObject;
            if (EditorGUI.EndChangeCheck())
            {
                if (go)
                {
                    SelectComponentWindow.Show<T>(go, c.GetInstanceID().ToString());
                }
            }
            EditorGUILayout.LabelField(c ? c.GetType().Name : "Null");
            EditorGUILayout.EndHorizontal();
            if (SelectComponentWindow.CanPick(c.GetInstanceID().ToString()))
            {
                return SelectComponentWindow.GetPick();
            }
            else
            {
                return c;
            }
        }
        public static Component SelectComponentField<T>(Rect postion, Component c, string title) where T : Component
        {
            Rect r2;
            Rect r1 = Layout.Divide.Golden(postion, out r2);
            Rect r2_1 = Layout.Divide.FirstRect(r2, 2);
            Rect r2_2 = Layout.Divide.NextRect(r2_1);
            EditorGUI.LabelField(r1, title);
            EditorGUI.BeginChangeCheck();
            var g = EditorGUI.ObjectField(r2_1, c ? c.gameObject : null, typeof(GameObject), true) as GameObject;
            if (EditorGUI.EndChangeCheck())
            {
                if (g)
                {
                    SelectComponentWindow.Show<T>(g, c.GetInstanceID().ToString());
                }
            }
            EditorGUI.LabelField(r2_2, c ? c.GetType().Name : "Null");
            if (SelectComponentWindow.CanPick(c.GetInstanceID().ToString()))
            {
                return SelectComponentWindow.GetPick();
            }
            else
            {
                return c;
            }
        }
        private static void SetComponent(ref Component oc)
        {
        }
        public static void GradientField(string label, Gradient value, params GUILayoutOption[] options)
        {
            if (GradientField_label_value_paramsOptions == null)
            {
                Type tyEditorGUILayout = typeof(EditorGUILayout);
                GradientField_label_value_paramsOptions = tyEditorGUILayout.GetMethod("GradientField", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(string), typeof(Gradient), typeof(GUILayoutOption[]) }, null);
            }
            GradientField_label_value_paramsOptions.Invoke(null, new object[] { label, value, options });
        }

        public static GenericMenu Menu()
        {
            if (GUILayout.Button("", Styles.Menu))
            {
                return new GenericMenu();
            }
            return null;
        }
        [Obsolete("Use Vision.BeginColor")]
        public static void BeginColor(Color color)
        {
            Vision.BeginBackGroundColor(color);
        }

        [Obsolete("Use Vision.EndColor")]
        public static void EndColor()
        {
            Vision.EndBackGroundColor();
        }

        public static bool ColorButton(Color color, string text)
        {
            Vision.BeginBackGroundColor(color);
            var b = GUILayout.Button(text);
            Vision.EndBackGroundColor();
            return b;
        }
        public static void Header(string head)
        {
            GUILayout.Label(head, EditorStyles.boldLabel);
            GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
        }
        #endregion
        public class Debug
        {
            public static void HolderBox()
            {
                GUILayout.Box("HoldRect", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            }
            public static void ButtonTest()
            {
                GUILayout.Button("Test", GUILayout.Width(300), GUILayout.Height(300), GUILayout.ExpandWidth(false));
            }
            public static void ShowRect(Rect rect,Color color)
            {
                EditorGUI.DrawRect(rect, color);
            }
        }
        #region Cache
        static Dictionary<string, Object> scriptCache = new Dictionary<string, Object>();
        static MethodInfo GradientField_label_value_paramsOptions;
        static int indentLevel;
        #endregion
        public static class Property
        {
            public static bool DefaultPropertyField(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
            {
                if (!includeChildren)
                {
                    return EditorGUI.PropertyField(position, property, label);
                }
                else
                {
                    Vector2 iconSize = EditorGUIUtility.GetIconSize();
                    bool enabled = GUI.enabled;
                    int indentLevel = EditorGUI.indentLevel;
                    int num2 = indentLevel - property.depth;
                    SerializedProperty serializedProperty = property.Copy();
                    SerializedProperty endProperty = serializedProperty.GetEndProperty();
                    position.height = GetSinglePropertyHeight(serializedProperty, label);
                    EditorGUI.indentLevel = serializedProperty.depth + num2;
                    bool enterChildren = DefaultPropertyField(position, serializedProperty, label, false) && serializedProperty.hasVisibleChildren;
                    position.y += position.height + 2f;
                    while (serializedProperty.NextVisible(enterChildren) && !SerializedProperty.EqualContents(serializedProperty, endProperty))
                    {
                        EditorGUI.indentLevel = serializedProperty.depth + num2;
                        position.height = EditorGUI.GetPropertyHeight(serializedProperty, null, false);
                        EditorGUI.BeginChangeCheck();
                        enterChildren = DefaultPropertyField(position, serializedProperty, null, false) && serializedProperty.hasVisibleChildren;
                        if (EditorGUI.EndChangeCheck())
                        {
                            break;
                        }
                        position.y += position.height + 2f;
                    }
                    GUI.enabled = enabled;
                    EditorGUIUtility.SetIconSize(iconSize);
                    EditorGUI.indentLevel = indentLevel;
                    return false;
                }
            }
            public static float GetSinglePropertyHeight(SerializedProperty property, GUIContent label)
            {
                float result;
                if (property == null)
                {
                    result = EditorGUIUtility.singleLineHeight;
                }
                else
                {
                    result = EditorGUI.GetPropertyHeight(property.propertyType, label);
                }
                return result;
            }

            public static object GetValue<T>(SerializedProperty prop)
            {
                return prop.GetValue<T>();
            }
        }
        public static class Colors
        {
            //原色
            public static Color light = new Color(0.9f, 0.9f, 0.9f);
            public static Color dark = new Color(0.8f, 0.8f, 0.8f);
            public static Color red = new Color(0.9f, 0.5f, 0.5f);
            public static Color red_light = new Color(1f, 0.8f, 0.8f);
            public static Color green = new Color(0.5f, 0.9f, 0.5f);
            public static Color green_light = new Color(0.8f, 1f, 0.8f);
            public static Color blue = new Color(0.5f, 0.5f, 0.9f);
            public static Color blue_light = new Color(0.8f, 0.8f, 1f);

            //混合色
            public static Color purple = new Color(0.8f, 0.1f, 0.8f);
            public static Color purple_light = new Color(0.8f, 0.3f, 0.8f);

            public static Color yellow = new Color(0.8f, 0.8f, 0.1f);
            public static Color yellow_light = new Color(0.8f, 0.8f, 0.3f);
        }
    }
}
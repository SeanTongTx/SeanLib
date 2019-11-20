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
            public static void Header(string head)
            {
                GUILayout.Label(head, EditorStyles.boldLabel);
                GUILayout.Box("", GUILayout.Height(1), GUILayout.ExpandWidth(true));
            }
            public static void Line(float height=1)
            {
                GUILayout.Box("", GUI.skin.box, GUILayout.Height(height), GUILayout.ExpandWidth(true));
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

            public static void IndentBegin(int depth=1)
            {
                EditorGUI.indentLevel += depth;
            }
            public static void IndentEnd(int depth = 1)
            {
                EditorGUI.indentLevel-= depth;
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
        public static bool Foldout(string Title,GUIStyle style=null, params GUILayoutOption[] options)
        {
            bool extend = !string.IsNullOrEmpty(EditorUserSettings.GetConfigValue(Title));
            EditorGUI.BeginChangeCheck();
            extend =GUILayout.Toggle(extend, Title, style ?? EditorStyles.foldout, options);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUserSettings.SetConfigValue(Title, extend ? "True" : string.Empty);
            }
            return extend;
        }
        public static bool EditorPrefsFoldoutGroup(string Title, GUIStyle style = null)
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
        [Obsolete("Use OnGUIUtility.Layout.Header instand ")]
        public static void Header(string head)
        {
            Layout.Header(head);
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
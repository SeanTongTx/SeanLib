using SeanLib.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorPlus
{
    public class EditorMarkDownDrawer
    {
        //MD styles
        static class MDStyles
        {
            public static GUIStyle Title1;
            public static GUIStyle Title2;
            public static GUIStyle Title3;
            public static GUIStyle FontI;
            public static GUIStyle FontB;
            public static GUIStyle FontBI;
            public static GUIStyle Link;
            public static GUIStyle Font;
            public static GUIStyle Area;
            static MDStyles()
            {
                Title1 = new GUIStyle(OnGUIUtility.Fonts.RichText);
                Title1.fontStyle = FontStyle.Bold;
                Title1.fontSize = (int)EditorGUIUtility.singleLineHeight * 3;
                Title1.normal.textColor = EditorGUIUtility.isProSkin?Color.white: Color.black;

                Title2 = new GUIStyle(OnGUIUtility.Fonts.RichText);
                Title2.fontStyle = FontStyle.Bold;
                Title2.fontSize = (int)EditorGUIUtility.singleLineHeight * 2;
                Title2.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
                

                Title3 = new GUIStyle(OnGUIUtility.Fonts.RichText);
                Title3.fontStyle = FontStyle.Bold;
                Title3.fontSize = (int)EditorGUIUtility.singleLineHeight * 1;
                Title3.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;

                Font = new GUIStyle(OnGUIUtility.Fonts.RichText);

                FontI = new GUIStyle(OnGUIUtility.Fonts.RichText);
                FontI.fontStyle = FontStyle.Italic;

                FontB = new GUIStyle(OnGUIUtility.Fonts.RichText);
                FontB.fontStyle = FontStyle.Bold;

                FontBI = new GUIStyle(OnGUIUtility.Fonts.RichText);
                FontBI.fontStyle = FontStyle.BoldAndItalic;

                Link = new GUIStyle("PR PrefabLabel");

                Area = new GUIStyle(EditorStyles.textArea);
                Area.richText = true;
            }
        }
        public static void DrawDoc(MarkDownDoc doc, OnGUIUtility.Search search = null, Action<string> PageChange = null)
        {
            foreach (var data in doc.datas)
            {
                if (search != null && !string.IsNullOrEmpty(search.Current))
                {
                    if (!data.keyValue.ToLower().Contains(search.Current.ToLower()))
                    {
                        continue;
                    }
                }
                DrawData(data, PageChange);
            }
        }
        private static void DrawData(MarkDownData data, Action<string> PageChange)
        {
            switch (data.type)
            {
                case KeyType.foldin:
                    data.doc.InFoldout = true;
                    data.doc.CurrentFoldout = OnGUIUtility.Foldout(data.keyValue, MDStyles.Title3, GUILayout.ExpandWidth(true),GUILayout.Height(EditorGUIUtility.singleLineHeight * 1.3f));// OnGUIUtility.EditorPrefsFoldoutGroup(data.keyValue);
                    OnGUIUtility.Layout.IndentBegin(2);
                    break;
                case KeyType.foldout:
                    data.doc.InFoldout = false;
                    OnGUIUtility.Layout.IndentEnd(2);
                    break;
            }
            if (data.doc.InFoldout &&!data.doc.CurrentFoldout)
            {
                return;
            }
            switch (data.type)
            {
                case KeyType.text:
                    EditorGUILayout.SelectableLabel(data.Data, MDStyles.Font, GUILayout.ExpandWidth(true), GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    break;
                case KeyType.code:
                    EditorGUILayout.TextArea(data.Data, MDStyles.Area);
                    break;
                case KeyType.qa:
                    EditorGUILayout.SelectableLabel(RichTextHelper.Color(data.keyValue, Color.green), MDStyles.Font, GUILayout.ExpandWidth(true), GUILayout.Height(EditorGUIUtility.singleLineHeight));

                    EditorGUILayout.TextArea(data.Data, MDStyles.Area);
                    break;
                case KeyType.font:
                    GUIStyle fontstyle = data.keyValue == "B" ? MDStyles.FontB : data.keyValue == "I" ? MDStyles.FontI : data.keyValue == "BI" ? MDStyles.FontBI : MDStyles.Font;
                    EditorGUILayout.SelectableLabel(data.Data, fontstyle, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    break;
                case KeyType.title:
                    int size = int.Parse(data.keyValue);
                    GUIStyle style = EditorStyles.boldLabel;
                    switch (size)
                    {
                        case 1: style = MDStyles.Title3; break;
                        case 2: style = MDStyles.Title2; break;
                        case 3: style = MDStyles.Title1; break;
                    }
                    EditorGUILayout.SelectableLabel(data.Data, style, GUILayout.Height(EditorGUIUtility.singleLineHeight * size * 1.3f), GUILayout.MinWidth(500));
                    if (size == 3)
                    {
                        OnGUIUtility.Layout.Line();
                    }
                    break;
                case KeyType.link:
                    if (GUILayout.Button(data.Data, MDStyles.Link))
                    {
                        EditorGUIUtility.systemCopyBuffer = data.Data;
                        Application.OpenURL(data.Data);
                    }
                    break;
                case KeyType.image:
                    if (data.texture)
                    {
                        int w = data.texture.width;
                        int h = data.texture.height;
                        if (w > 1024)
                        {
                            h = (int)((float)h / w * 1024f);
                            w = 1024;
                        }
                        if (h > 1024)
                        {
                            w = (int)((float)w / h * 1024f);
                            h = 1024;
                        }
                        GUILayout.Box("", GUILayout.Width(w), GUILayout.Height(h));
                        if (Event.current.type == EventType.Repaint)
                        {
                            Rect rect = GUILayoutUtility.GetLastRect();

                            GUI.DrawTexture(rect, data.texture);
                        }
                    }
                    break;
                case KeyType.separator:
                    OnGUIUtility.Layout.Line();
                    break;
                case KeyType.page:
                    Color c = GUI.color;
                    GUI.color = Color.cyan;
                    if (GUILayout.Button("▶" + data.Data, MDStyles.Title3, GUILayout.ExpandWidth(false), GUILayout.Height(EditorGUIUtility.singleLineHeight * 1.3f)))
                    {
                        PageChange?.Invoke(data.Data);
                    }
                    GUI.color = c;
                    break;
                case KeyType.table:
                    GUILayout.BeginHorizontal();
                    string[] datas = data.Data.Split('|');
                    foreach (var d in datas)
                    {
                        if (d.IsNOTNullOrEmpty())
                        {
                            if (d == "--:")
                            {
                                OnGUIUtility.Layout.Line();
                            }
                            else
                            {
                                EditorGUILayout.SelectableLabel(d, MDStyles.Font, GUILayout.ExpandWidth(true), GUILayout.Height(EditorGUIUtility.singleLineHeight));
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
                    break;
            }
        }
    }
}
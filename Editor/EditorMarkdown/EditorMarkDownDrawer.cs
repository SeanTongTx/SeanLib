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
        public static void DrawDoc(MarkDownDoc doc, Action<string> PageChange = null,Action<string>DocChange=null,Action Repaint=null)
        {
            foreach (var data in doc.datas)
            {
                DrawData(data, PageChange, DocChange, Repaint);
                if (Event.current.type == EventType.Repaint)
                {
                    data.drawRect = GUILayoutUtility.GetLastRect();
                }
            }
        }
        private static void DrawData(MarkDownData data, Action<string> PageChange, Action<string> DocChange,Action Repaint)
        {
            var doc = data.doc;
            switch (data.type)
            {
                case KeyType.foldin:
                    data.doc.InFoldout = true;
                    data.doc.CurrentFoldout = OnGUIUtility.Foldout(data.keyValue, doc.Styles.Foldout, GUILayout.ExpandWidth(true),GUILayout.Height(EditorGUIUtility.singleLineHeight * 1.3f));// OnGUIUtility.EditorPrefsFoldoutGroup(data.keyValue);
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
                    Rect position = GUILayoutUtility.GetRect(EditorGUIUtility.TrTextContent(data.Data), doc.Styles.Font,GUILayout.ExpandHeight(true));
                    OnGUIUtility.Layout.IndentDisable();
                    EditorGUI.SelectableLabel(position, data.Data, doc.Styles.Font);
                    OnGUIUtility.Layout.IndentEnable();
                    break;
                case KeyType.code:
                    OnGUIUtility.Vision.BeginBackGroundColor(ColorPalette.Get(doc.ColorSetting, "Area_BackGround", EditorGUIUtility.isProSkin ? Color.white : Color.black));
                    EditorGUILayout.TextArea(data.Data, doc.Styles.Area);
                    OnGUIUtility.Vision.EndBackGroundColor();
                    break;
                case KeyType.qa:
                    OnGUIUtility.Vision.BeginBackGroundColor(ColorPalette.Get(doc.ColorSetting, "Area_BackGround", EditorGUIUtility.isProSkin ? Color.white : Color.black));

                    EditorGUILayout.SelectableLabel(RichTextHelper.Color(data.keyValue, Color.green), doc.Styles.Font, GUILayout.ExpandWidth(true), GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    EditorGUILayout.TextArea(data.Data, doc.Styles.Area);
                    OnGUIUtility.Vision.EndBackGroundColor();
                    break;
                case KeyType.font:
                    GUIStyle fontstyle = data.keyValue == "B" ? doc.Styles.FontB : data.keyValue == "I" ? doc.Styles.FontI : data.keyValue == "BI" ? doc.Styles.FontBI : doc.Styles.Font;
                    EditorGUILayout.SelectableLabel(data.Data, fontstyle, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    break;
                case KeyType.title:
                    int size = int.Parse(data.keyValue);
                    GUIStyle style = EditorStyles.boldLabel;
                    switch (size)
                    {
                        case 1: style = doc.Styles.Title3; break;
                        case 2: style = doc.Styles.Title2; break;
                        case 3: style = doc.Styles.Title1; break;
                    }
                    EditorGUILayout.SelectableLabel(data.Data, style, GUILayout.Height(EditorGUIUtility.singleLineHeight * size * 1.3f), GUILayout.MinWidth(500));
                    if (size == 3)
                    {
                        OnGUIUtility.Layout.Line();
                    }
                    break;
                case KeyType.link:
                    if (GUILayout.Button(data.Data, doc.Styles.Link))
                    {
                        EditorGUIUtility.systemCopyBuffer = data.Data;
                        Application.OpenURL(data.Data);
                    }
                    if(Event.current.type==EventType.Repaint)
                    {
                       var last= GUILayoutUtility.GetLastRect();
                        EditorGUIUtility.AddCursorRect(last, MouseCursor.Link);
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
                    else if(data.gifDrawer!=null&& Repaint!=null)
                    {
                        data.gifDrawer.OnGUI(Repaint);
                    }
                    break;
                case KeyType.separator:
                    OnGUIUtility.Layout.Line();
                    break;
                case KeyType.page:
                    if (GUILayout.Button("▶" + data.Data, doc.Styles.Page, GUILayout.ExpandWidth(false), GUILayout.Height(EditorGUIUtility.singleLineHeight * 1.3f)))
                    {
                        PageChange?.Invoke(data.Data);
                    }
                    break;
                case KeyType.doc:
                    if (GUILayout.Button("▶" + data.Data, doc.Styles.Doc, GUILayout.ExpandWidth(false), GUILayout.Height(EditorGUIUtility.singleLineHeight * 1.3f)))
                    {
                        DocChange?.Invoke(data.Data);
                    }
                    break;
                case KeyType.table:
                    GUILayout.BeginHorizontal();
                    foreach (var item in data.subdatas)
                    {
                        //OnGUIUtility.Debug.ButtonTest();
                        DrawData(item, PageChange, DocChange, Repaint);
                        //OnGUIUtility.Debug.DrawLastRect(Color.white);
                    }
                    GUILayout.EndHorizontal();
                    break;
            }
        }
    }
}
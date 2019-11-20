﻿using ServiceTools.Reflect;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;

namespace EditorPlus
{
    public class SeanLibIndex : TreeView
    {
        public List<SeanLibEditor> editors = new List<SeanLibEditor>();
        public SeanLibIndex(TreeViewState state) : base(state)
        {
        }
        public SeanLibEditor GetEditor(int id)
        {
            SeanLibIndexItem item = FindItem(id, rootItem) as SeanLibIndexItem;
            if (item != null)
            {
                return item.editor;
            }
            return null;
        }
        protected override TreeViewItem BuildRoot()
        {
            var root = new SeanLibIndexItem { id = 0, depth = -1, displayName = "SeanLib" };
            List<List<SeanLibIndexItem>> Map = new List<List<SeanLibIndexItem>>();
            int id = 1;
            foreach (var editor in editors)
            {
                CustomSeanLibEditor att = ReflecTool.GetAttribute<CustomSeanLibEditor>(editor.GetType());
                string[] pathes = att.Path.Split('/');
                if (Map.Count < pathes.Length)
                {
                    int c = pathes.Length - Map.Count;
                    for (int i = 0; i < c; i++)
                    {
                        Map.Add(new List<SeanLibIndexItem>());
                    }
                }
                SeanLibIndexItem parent = root;
                for (int i = 0; i < pathes.Length; i++)
                {
                    string node = pathes[i];
                    var existItem = Map[i].Find(e => e.displayName == node && e.parent == parent);
                    if (existItem == null)
                    {
                        existItem = new SeanLibIndexItem { id = id++, displayName = node };
                        Map[i].Add(existItem);
                        if (i == 0)
                        {
                            root.AddChild(existItem);
                        }
                        else
                        {
                            var parentItem = Map[i - 1].Find(e => e.displayName == pathes[i - 1]);
                            parentItem.AddChild(existItem);
                        }
                        if (i == pathes.Length - 1)
                        {
                            existItem.editor = editor;
                        }
                    }
                    else
                    {
                        if (i == pathes.Length - 1)
                        {
                            existItem.editor = editor;
                        }
                    }
                    parent = existItem;
                }
            }
            SetupDepthsFromParentsAndChildren(root);
            return root;
        }
        public void RefreshTreeData(SeanLibManager drawer)
        {
            var editorTypes = AssemblyTool.FindTypesInCurrentDomainWhereAttributeIs<CustomSeanLibEditor>();
            editorTypes.Sort((l, r) =>
            {
                return ReflecTool.GetAttribute<CustomSeanLibEditor>(l).order - ReflecTool.GetAttribute<CustomSeanLibEditor>(r).order;
            });
            editors = new List<SeanLibEditor>(editorTypes.Count);
            foreach (var item in editorTypes)
            {
                SeanLibEditor editor = ReflecTool.Instantiate(item) as SeanLibEditor;
                //editor.OnEnable(drawer);
                editors.Add(editor);
            }
            Reload();
        }
    }
    public class SeanLibIndexItem : TreeViewItem
    {
        public SeanLibEditor editor;
    }
    public abstract class SeanLibEditor
    {
        public SeanLibManager window;
        /// <summary>
        /// window postion
        /// </summary>
        public Rect position;
        public IMGUIContainer EditorContent_IMGUI;
        protected virtual string UXML => string.Empty;

        protected StyleSheet editorContent_styles;
        protected virtual bool UseIMGUI => true;
        public static class styles
        {
            public static GUIStyle ExtendArea;
            public static GUIStyle ExtendGroup;
            public static GUIStyle Title;
            public static GUIStyle Box;
            static styles()
            {
                ExtendArea = new GUIStyle("RL Background");
                ExtendGroup = new GUIStyle("CN Box");
                Title = new GUIStyle("OL Title");
                Box = new GUIStyle("OL box NoExpand");
            }
        }
        public virtual void OnEnable(SeanLibManager drawer)
        {
            window = drawer;
            if(UseIMGUI) EnableIMGUI();
            else EnableUIElements();
           
        }
        public virtual void EnableUIElements()
        {
            if (!string.IsNullOrEmpty(UXML))
            {
                var editorContent = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(PathTools.RelativeAssetPath(this.GetType(),UXML+".uxml"));
                editorContent_styles = AssetDatabase.LoadAssetAtPath<StyleSheet>(PathTools.RelativeAssetPath(this.GetType(), UXML+".uss"));
                window.EditorContent.styleSheets.Add(editorContent_styles);
                editorContent.CloneTree(window.EditorContent);
            }
        }
        public virtual void EnableIMGUI()
        {
            EditorContent_IMGUI = new IMGUIContainer(OnGUI);
            EditorContent_IMGUI.name = "EditorContent_IMGUI";
            EditorContent_IMGUI.style.flexGrow = 1;
            window.EditorContent.Add(EditorContent_IMGUI);
        }
        public virtual void OnGUI()
        {
        }
        public virtual void OnDisable()
        {
            window.EditorContent.Clear();
            if (UseIMGUI)
            {

            }
            else
            {
                if(editorContent_styles)
                {
                    window.EditorContent.styleSheets.Remove(editorContent_styles);
                    editorContent_styles = null;
                }
            }
            window = null;
        }
    }

    public class CustomSeanLibEditor : Attribute
    {
        public string Path;
        public int order;
        public CustomSeanLibEditor(string path)
        {
            this.Path = path;
        }
    }
    public interface IEditorGUI
    {
        void OnEnable(SeanLibEditor editorRoot);
        void OnGUI();
        void OnDisable();
    }

}
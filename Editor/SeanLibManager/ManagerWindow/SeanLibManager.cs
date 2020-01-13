
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace EditorPlus
{
    public abstract class SeanLibManager : EditorWindow
    {
        [SerializeField]
        protected TreeViewState indexState;
        protected SeanLibIndex libIndex;
        protected virtual string IndexKey => "SeanLibIndex";
        protected virtual ElementsFileAsset FileAsset => new ElementsFileAsset()
        {
             BaseType= typeof(SeanLibManager),
             UXML= "../SeanLibManagerWindow.uxml",
             USS= "../SeanLibManagerWindow.uss"
        };
        /// <summary>
        /// 编辑器目录
        /// </summary>
        public VisualElement EditorIndex;
        internal IMGUIContainer EditorIndexContent_IMGUI;

        /// <summary>
        /// 编辑器内容
        /// </summary>
        public VisualElement EditorContent;
        public SeanLibEditor CurrentEditor { get; private set; }
        protected virtual void OnEnable()
        {
            if (indexState == null)
            {
                indexState = new TreeViewState();
            }
            libIndex = new SeanLibIndex(indexState);
            libIndex.RefreshTreeData(this);
            libIndex.SetSelection(new List<int>() { EditorPrefs.GetInt(IndexKey, 1) });

            VisualElement root = rootVisualElement;
            
            var visualTree = AssetDBHelper.LoadAsset<VisualTreeAsset>(FileAsset.BaseType, FileAsset.UXML);
            var styleSheet = AssetDBHelper.LoadAsset<StyleSheet>(FileAsset.BaseType, FileAsset.USS);
            root.styleSheets.Add(styleSheet);
            visualTree.CloneTree(root);

            //目录
            EditorIndex = root.Q<VisualElement>("EditorIndex");
            EditorIndexContent_IMGUI = EditorIndex.Q<IMGUIContainer>("EditorIndexContent_IMGUI");
            EditorIndexContent_IMGUI.onGUIHandler = () => { libIndex.OnGUI(EditorIndexContent_IMGUI.contentRect); };
            //内容
            EditorContent = root.Q<VisualElement>("EditorContent");
        }
        Vector2 v;
        private void OnGUI()
        {
            if (EditorApplication.isCompiling)
            {
                ShowNotification(new GUIContent("Compiling..."));
                rootVisualElement.SetEnabled(false);
                return;
            }
            else
            {
                rootVisualElement.SetEnabled(true);
                CheckEditorChange();
            }
        }
        private void CheckEditorChange()
        {
            if (indexState.selectedIDs.Count != 0)
            {
                //Draw one editor
                var selectEditor = libIndex.GetEditor(indexState.selectedIDs[0]);
                if (CurrentEditor == selectEditor)
                {
                    return;
                }
                else
                {
                    if (CurrentEditor != null)
                    {
                        CurrentEditor.OnDisable();
                    }
                    CurrentEditor = selectEditor;
                    EditorPrefs.SetInt(IndexKey, indexState.selectedIDs[0]);
                    if (selectEditor != null)
                    {
                        selectEditor.position = EditorContent.contentRect;
                        selectEditor.OnEnable(this);
                    }
                }
            }
        }
        private void OnDisable()
        {
            if(CurrentEditor!=null)
            {
                CurrentEditor.OnDisable();
            }
        }

        public void SelectIndex(int id)
        {
            indexState.selectedIDs.Clear();
            indexState.selectedIDs.Add(id);
        }
        public SeanLibIndexItem SeachIndex(string path)
        {
            string[] pathes = path.Split('/');
            TreeViewItem temp = null;
            for (int i = 0; i < pathes.Length; i++)
            {
                string node = pathes[i];
                if (i == 0)
                {
                    temp = FindRow(node);
                }
                else
                {
                    if (temp != null)
                    {
                        temp = temp.children.Find(e => e.displayName == node);
                    }
                }
            }
            return temp == null ? null : (temp as SeanLibIndexItem);
        }
        TreeViewItem FindRow(string Node)
        {
            var rows = libIndex.GetRows();
            foreach (var row in rows)
            {
                if (row.displayName == Node)
                {
                    return row;
                }
            }
            return null;
        }
    }
}
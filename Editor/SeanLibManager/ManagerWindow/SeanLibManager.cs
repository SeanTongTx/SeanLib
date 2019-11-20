
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace EditorPlus
{
    public class SeanLibManager : EditorWindow
    {
        [MenuItem("Window/SeanLib/Manager &#1")]
        public static void ShowWindow()
        {
            SeanLibManager w = GetWindow<SeanLibManager>();
            w.titleContent = new GUIContent("SeanLibManager");
            w.Show();
        }
        [SerializeField]
        TreeViewState indexState;
        SeanLibIndex libIndex;

        /// <summary>
        /// 编辑器目录
        /// </summary>
        public VisualElement EditorIndex;
        internal IMGUIContainer EditorIndexContent_IMGUI;

        /// <summary>
        /// 编辑器内容
        /// </summary>
        public VisualElement EditorContent;
        private SeanLibEditor editor;
        private void OnEnable()
        {
            if (indexState == null)
            {
                indexState = new TreeViewState();
            }
            libIndex = new SeanLibIndex(indexState);
            libIndex.RefreshTreeData(this);
            libIndex.SetSelection(new List<int>() { EditorPrefs.GetInt("SeanLibIndex", 1) });

            VisualElement root = rootVisualElement;
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(PathTools.RelativeAssetPath(this.GetType(), "../SeanLibManagerWindow.uxml"));
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(PathTools.RelativeAssetPath(this.GetType(), "../SeanLibManagerWindow.uss"));
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
                if (editor == selectEditor)
                {
                    return;
                }
                else
                {
                    if (editor != null)
                    {
                        editor.OnDisable();
                    }
                    editor = selectEditor;
                    EditorPrefs.SetInt("SeanLibIndex", indexState.selectedIDs[0]);
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
            if(editor!=null)
            {
                editor.OnDisable();
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
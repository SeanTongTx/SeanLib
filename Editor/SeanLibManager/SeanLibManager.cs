
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;
namespace EditorPlus
{
    public class SeanLibManager : EditorWindow
    {

        [MenuItem("Window/SeanLib &#1")]
        public static void ShowWindow()
        {
            SeanLibManager w = GetWindow<SeanLibManager>();
            w.Show();
        }
        static class styles
        {
            public static GUIStyle Area;
            static styles()
            {
                styles.Area = new GUIStyle("RL Background");
            }
        }

        #region Layout
        public OnGUIUtility.Zone_Divide2Horizontal RootLayout = new OnGUIUtility.Zone_Divide2Horizontal();
        #endregion
        [SerializeField]
        TreeViewState indexState;
        SeanLibIndex libIndex;
        private void OnEnable()
        {
            if (indexState == null)
            {
                indexState = new TreeViewState();
            }
            libIndex = new SeanLibIndex(indexState);
            libIndex.RefreshTreeData(this);
            libIndex.SetSelection(new List<int>() { EditorPrefs.GetInt("SeanLibIndex", 1) });
            this.wantsMouseMove = true;
        }
        Vector2 v;
        private void OnGUI()
        {
            if (EditorApplication.isCompiling)
            {
                ShowNotification(new GUIContent("Compiling..."));
                return;
            }
            RootLayout.OnGUI(new Rect(Vector2.zero, position.size), Repaint, DrawLibIndex, DrawEditor);
        }
        private void OnDisable()
        {
            foreach (var item in libIndex.editors)
            {
                if (item.enable)
                    item.OnDisable();
            }
        }

        protected virtual void DrawLibIndex(Rect position)
        {
            libIndex.OnGUI(RootLayout.Area0);
        }
        protected virtual void DrawEditor(Rect position)
        {
            if (indexState.selectedIDs.Count != 0)
            {
                //Draw one editor
                var editor = libIndex.GetEditor(indexState.selectedIDs[0]);
                editor.position = position;
                if (Event.current.type == EventType.Repaint)
                {
                    EditorPrefs.SetInt("SeanLibIndex", indexState.selectedIDs[0]);
                }
                if (editor != null)
                {
                    if (editor.enable == false)
                    {
                        editor.OnEnable(this);
                    }
                    editor.OnGUI();
                }
                else
                {
                    GUILayout.Label("No Drawable editor");
                }
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
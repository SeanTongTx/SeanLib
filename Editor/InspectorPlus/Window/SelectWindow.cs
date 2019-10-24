using SeanLib.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace EditorPlus
{
    public class SelectWindow<T> : PopupWindowContent
    {
        protected OnGUIUtility.Search searchField = new OnGUIUtility.Search();
        private static SelectWindow<T> defaultContent = new SelectWindow<T>();
        public static SelectWindow<T> instance;
        public static void Show(List<T> list, string controlId)
        {
            Show(list, controlId, new Vector2(0, 0));
        }
        public static void Show(List<T> list, string controlId,Vector2 size)
        {
            instance = defaultContent;
            instance.ControlId = controlId;
            instance.List = list;
            instance.size = size;
            try
            {
                PopupWindow.Show(new Rect(Event.current.mousePosition.DeltaX(-100),Vector2.zero), instance);
            }
            catch
            {
                //  EditorGUIUtility.ExitGUI();
            }
        }
        public override Vector2 GetWindowSize()
        {
            if(size!=Vector2.zero)
            {
                return size;
            }
            return base.GetWindowSize();
        }
        protected static bool canPick;
        public static bool CanPick(string controlId)
        {
            return canPick && controlId == instance.ControlId;
        }
        public static T GetPick()
        {
            var c = instance.Selected;
            instance.Selected = default(T);
            instance.ControlId = string.Empty;
            canPick = false;
            return c;
        }
        public static int GetPickIndex()
        {
            int i = instance.SelectedIndex;
            instance.ControlId = string.Empty;
            canPick = false;
            return i;
        }

        public string ControlId;
        public List<T> List;
        public T Selected;
        public int SelectedIndex;
        public Vector2 size;
        protected Vector2 v;
        protected static class Styles
        {
            public static GUIStyle Selection;
            static Styles()
            {
                Selection = new GUIStyle("OL Title");
            }
        }
        public override void OnGUI(Rect rect)
        {
            var searching = searchField.OnToolbarGUI();
            if (List != null)
            {
                v = EditorGUILayout.BeginScrollView(v);
                for (int i = 0; i < List.Count; i++)
                {
                    var item = List[i];
                    if (item == null)
                    {
                        if (GUILayout.Button("null", Styles.Selection))
                        {
                            instance.Selected = default(T);
                            instance.SelectedIndex = i;
                            canPick = true;
                            this.editorWindow.Close();
                            return;
                        }
                    }
                    else if (searchField.GeneralValid(item.ToString()))
                    {
                        if (GUILayout.Button(item.ToString(), Styles.Selection))
                        {
                            instance.Selected = item;
                            instance.SelectedIndex = i;
                            canPick = true;
                            this.editorWindow.Close();
                            return;
                        }
                    }
                }
                EditorGUILayout.EndScrollView();
            }
        }
        public override void OnOpen()
        {
            base.OnOpen();
            canPick = false;
        }
        public override void OnClose()
        {
            base.OnClose();
        }
    }
}
using SeanLib.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace EditorPlus
{
    public class SelectWindow<T> : PopupWindowContent
    {
        public struct CallBack
        {
            public Action<T, int> OnSelected;
            public Func<Vector2> WindowSize;
            public Action<T, int> DrawSelection;
            public void Merge(CallBack option)
            {
                OnSelected = option.OnSelected == null ? OnSelected : option.OnSelected;
                WindowSize = option.WindowSize == null ? WindowSize : option.WindowSize;
                DrawSelection = option.DrawSelection == null ? DrawSelection : option.DrawSelection;
            }
        }
        #region Default
        private static SelectWindow<T> defaultContent = new SelectWindow<T>();
        private static CallBack defaultCallback = new CallBack() { WindowSize = defaultSize, DrawSelection=DrawSelection};
        public static void DrawSelection(T item,int index)
        {
            if (item == null)
            {
                if (GUILayout.Button("null", Styles.Selection))
                {
                    instance.Select(default(T), index);
                    instance.editorWindow.Close();
                    return;
                }
            }
            else if (instance.searchField.GeneralValid(item.ToString()))
            {
                if (GUILayout.Button(item.ToString(), Styles.Selection))
                {
                    instance.Select(item, index);
                    instance.editorWindow.Close();
                    return;
                }
            }
        }
        public static Vector2 defaultSize() { return new Vector2(200, 300); }
        #endregion
        #region API
        public static SelectWindow<T> instance;
        public static void Show(List<T> list, string controlId)
        {
            Show(list, controlId,new CallBack());
        }
        public static void Show(List<T> list, string controlId, CallBack callback)
        {
            instance = defaultContent;
            instance.OnEnable(list, controlId, callback);
            try
            {
                PopupWindow.Show(new Rect(Event.current.mousePosition.DeltaX(-100), Vector2.zero), instance);
            }
            catch
            {
                //  EditorGUIUtility.ExitGUI();
            }
        }
        #endregion
        #region IMGUI
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
        #endregion

        public OnGUIUtility.Search searchField = new OnGUIUtility.Search();
        public string ControlId;
        public List<T> List;

        public T Selected;
        public int SelectedIndex;

        protected CallBack callback;
        protected Vector2 v;
        public static class Styles
        {
            public static GUIStyle Selection;
            static Styles()
            {
                Selection = new GUIStyle("OL Title");
            }
        }
        public virtual void OnEnable(List<T> list, string controlId, CallBack optional)
        {
            CallBack instanceCallBack = defaultCallback;
            instanceCallBack.Merge(optional);
            instance.ControlId = controlId;
            instance.List = list;
            instance.callback= instanceCallBack;
        }
        public virtual void Select(T t, int index)
        {
            instance.Selected = t;
            instance.SelectedIndex = index;
            instance.callback.OnSelected?.Invoke(t, index);
            instance.callback.OnSelected = null;
            canPick = true;
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
                    callback.DrawSelection(item, i);
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
        public override Vector2 GetWindowSize()
        {
            return callback.WindowSize();
        }
    }
}
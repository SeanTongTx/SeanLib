using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorPlus
{
    public class ObjectWindow : EditorWindow
    {
        public Action<object> OnGui;
        public Action OnChange;
        public object current;
        public static ObjectWindow Show(string title, object obj, Action<object> OnGUI)
        {
            var win = GetWindow<ObjectWindow>();
            win.current = obj;
            win.OnGui = OnGUI;
            win.titleContent = new GUIContent(title);
            win.Show();
            return win;
        }
        public static ObjectWindow Show(object obj, Action<object> OnGUI)
        {
            var win = GetWindow<ObjectWindow>();
            win.current = obj;
            win.OnGui = OnGUI;
            win.Show();
            return win;
        }

        Vector2 scroll;
        private void OnGUI()
        {
            if (OnGui == null)
            {
                EditorGUILayout.LabelField("需要GUI方法", OnGUIUtility.Fonts.Error);
                return;
            }
            if (current == null)
            {
                EditorGUILayout.LabelField("Object = null", OnGUIUtility.Fonts.Error);
                return;
            }
            scroll = EditorGUILayout.BeginScrollView(scroll);
            EditorGUI.BeginChangeCheck();
            OnGui(current);
            if (EditorGUI.EndChangeCheck())
            {
            }
            EditorGUILayout.EndScrollView();
        }

    }
}
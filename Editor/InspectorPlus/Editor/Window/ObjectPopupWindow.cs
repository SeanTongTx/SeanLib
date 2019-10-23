using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorPlus
{
    public class ObjectPopupWindow : PopupWindowContent
    {
        private static ObjectPopupWindow instance = new ObjectPopupWindow();
        public static void Show(object obj, Vector2 size, string title, Action OnChange = null)
        {
            instance.Title = title;
            instance.Size = size;
            instance.current = obj;
            instance.OnChange = OnChange;
            PopupWindow.Show(new Rect(Event.current.mousePosition, new Vector2(0, 0)), instance);
        }
        public static void Show(object obj)
        {
            Show(obj, new Vector2(400, 300), "Current");
        }
        public static void Show(object obj, Action OnClose)
        {
            instance.onClose = OnClose;
            Show(obj);
        }
        public static void Show(object obj, Action OnOpen, Action OnClose)
        {
            instance.onOpen = OnOpen;
            instance.onClose = OnClose;
            Show(obj);
        }
        public static void Show(object obj, Action<Rect> OnGUI)
        {
            instance.OnGui = OnGUI;
            Show(obj, new Vector2(400, 300), "Current");
        }
        public override Vector2 GetWindowSize()
        {
            return Size;
        }
        public string Title;
        public object current;
        public Vector2 Size;
        public Action OnChange;
        public Action onOpen;
        public Action onClose;
        public Action<Rect> OnGui;
        Vector2 scroll;
        public override void OnGUI(Rect rect)
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);

            EditorGUI.BeginChangeCheck();
            if (OnGui == null)
            {
                FieldDrawerUtil.ObjectField(Title, current, current.GetType(), null);
            }
            else
            {
                OnGui(rect);
            }
            if (EditorGUI.EndChangeCheck())
            {
                if (OnChange != null) OnChange();
            }
            EditorGUILayout.EndScrollView();
        }
        public override void OnOpen()
        {
            if (onOpen != null)
            {
                onOpen();
            }
            base.OnOpen();
        }
        public override void OnClose()
        {
            if (onClose != null)
            {
                onClose();
            }
            base.OnClose();
        }
    }
}

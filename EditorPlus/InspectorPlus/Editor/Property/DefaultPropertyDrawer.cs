using SeanLib.Core;
using ServiceTools.Reflect;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EditorPlus
{
    [CustomPropertyDrawer(typeof(InspectorPlus.InspectorPlusAttribute), true)]
    public class DefaultPropertyDrawer : PropertyDrawer
    {
        public Dictionary<Type, InspectorPlus.InspectorPlusAttribute> Atts = new Dictionary<Type, InspectorPlus.InspectorPlusAttribute>();
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Atts.Clear();
            var atts = fieldInfo.GetCustomAttributes(typeof(InspectorPlus.InspectorPlusAttribute), true);
            foreach (var item in atts)
            {
                Atts[item.GetType()] = item as InspectorPlus.InspectorPlusAttribute;
            }
            Rect rect = position;
            if (Event.current.type == EventType.Repaint)
            {
                var line = TryGetAttibute<InspectorPlus.Line>();
                if (line != null)
                {
                    switch (line.m_type)
                    {
                        case InspectorPlus.Line.LineType.Normal:
                            EditorGUI.DrawRect(new Rect(position.position, new Vector2(position.width, 2)), Color.grey);
                            rect = rect.Delta(new Vector2(0, 2));
                            break;
                        default:
                            break;
                    }
                }
            }
            var read = TryGetAttibute<InspectorPlus.ReadOnly>();
            if (read != null)
            {
                OnGUIUtility.Vision.GUIGlobleEnable(false);
            }
            var popup = TryGetAttibute<InspectorPlus.PopupObject>();
            if (popup != null)
            {
                object value = PropertyDrawerTools.GetPropertyInstance(property, fieldInfo);
                string title = popup.title.IsNullOrEmpty() ? label.text : popup.title;
                if (GUI.Button(position, title))
                {
                    property.serializedObject.Update();
                    ObjectPopupWindow.Show(value, new Vector2(popup.width, popup.heigth), title, () =>
                    {
                    //this may help in prefab mode
                    PrefabUtility.RecordPrefabInstancePropertyModifications(property.serializedObject.targetObject);
                        property.serializedObject.ApplyModifiedProperties();

                        EditorUtility.SetDirty(property.serializedObject.targetObject);
                        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                    });
                }
            }
            else
            {
                OnDraw(rect, property, label);
            }
            if (read != null)
            {
                OnGUIUtility.Vision.GUIGlobleEnable(true);
            }
        }
        protected T TryGetAttibute<T>() where T : InspectorPlus.InspectorPlusAttribute
        {
            InspectorPlus.InspectorPlusAttribute att = null;
            Atts.TryGetValue(typeof(T), out att);
            return att as T;
        }
        protected virtual void OnDraw(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }

    }
}
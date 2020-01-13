using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EditorPlus;
using System;

namespace EditorPlus
{
    public static partial class OnGUIUtility
    {
        public static class GUIProperty
        {
            public static Rect BeginIndented(Rect position)
            {
                position = EditorGUI.IndentedRect(position);
                Layout.IndentDisable();
                return position;
            }
            public static void EndIndented()
            {
               Layout.IndentEnable();
            }
            public static bool DefaultPropertyField(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
            {
                return ProperyField(position, property, label, includeChildren, EditorGUI.PropertyField);
            }
            public static bool ProperyField(Rect position, SerializedProperty property, GUIContent label, bool includeChildren,Func<Rect, SerializedProperty, GUIContent,bool>propertyDrawer)
            {
                if (!includeChildren)
                {
                    return propertyDrawer(position, property, label);
                }
                else
                {
                    Vector2 iconSize = EditorGUIUtility.GetIconSize();
                    bool enabled = GUI.enabled;
                    int indentLevel = EditorGUI.indentLevel;
                    int num2 = indentLevel - property.depth;
                    SerializedProperty serializedProperty = property.Copy();
                    SerializedProperty endProperty = serializedProperty.GetEndProperty();
                    position.height = GetSinglePropertyHeight(serializedProperty, label);
                    EditorGUI.indentLevel = serializedProperty.depth + num2;
                    bool enterChildren = ProperyField(position, serializedProperty, label, false, propertyDrawer) && serializedProperty.hasVisibleChildren;
                    position.y += position.height + 2f;
                    while (serializedProperty.NextVisible(enterChildren) && !SerializedProperty.EqualContents(serializedProperty, endProperty))
                    {
                        EditorGUI.indentLevel = serializedProperty.depth + num2;
                        position.height = EditorGUI.GetPropertyHeight(serializedProperty, null, false);
                        EditorGUI.BeginChangeCheck();
                        enterChildren = ProperyField(position, serializedProperty, label, false, propertyDrawer) && serializedProperty.hasVisibleChildren;
                        if (EditorGUI.EndChangeCheck())
                        {
                            break;
                        }
                        position.y += position.height + 2f;
                    }
                    GUI.enabled = enabled;
                    EditorGUIUtility.SetIconSize(iconSize);
                    EditorGUI.indentLevel = indentLevel;
                    return false;
                }
            }

            public static float GetSinglePropertyHeight(SerializedProperty property, GUIContent label,bool includeChildren=true)
            {
                float result;
                if (property == null)
                {
                    result = EditorGUIUtility.singleLineHeight;
                }
                else
                {
                    result = EditorGUI.GetPropertyHeight(property, label, includeChildren);
                }
                return result;
            }

            public static object GetValue<T>(SerializedProperty prop)
            {
                return prop.GetValue<T>();
            }
        }
    }
}

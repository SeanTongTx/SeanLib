using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Reflection;
using SeanLib.Core;

namespace EditorPlus
{
    [CustomPropertyDrawer(typeof(InspectorPlus.SetProperty))]
    public class SetPropertyDrawer : DefaultPropertyDrawer
    {
        protected override void OnDraw(Rect position, SerializedProperty property, GUIContent label)
        {
            // Rely on the default inspector GUI
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, label);

            // Update only when necessary
            InspectorPlus.SetProperty setProperty = attribute as InspectorPlus.SetProperty;
            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
                // The propertyPath may reference something that is a child field of a field on this Object, so it is necessary
                // to find which object is the actual parent before attempting to set the property with the current value.
                object parent = GetParentObjectOfProperty(property.propertyPath, property.serializedObject.targetObject);
                Type type = parent.GetType();
                PropertyInfo pi = type.GetProperty(setProperty.Name);
                if (pi == null)
                {
                    Debug.LogError("Invalid property name: " + setProperty.Name + "\nCheck your [SetProperty] attribute");
                }
                else
                {
                    // Use FieldInfo instead of the SerializedProperty accessors as we'd have to deal with every 
                    // SerializedPropertyType and use the correct accessor
                    pi.SetValue(parent, fieldInfo.GetValue(parent), null);
                }
            }
        }

        private object GetParentObjectOfProperty(string path, object obj)
        {
            string[] fields = path.Split('.');

            // We've finally arrived at the final object that contains the property
            if (fields.Length == 1)
            {
                return obj;
            }

            // We may have to walk public or private fields along the chain to finding our container object, so we have to allow for both
            FieldInfo fi = obj.GetType().GetField(fields[0], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            obj = fi.GetValue(obj);

            // Keep searching for our object that contains the property
            return GetParentObjectOfProperty(string.Join(".", fields, 1, fields.Length - 1), obj);
        }
    }
}
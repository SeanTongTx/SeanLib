
using ServiceTools.Reflect;
using System;
using System.Reflection;


using UnityEditor;

using UnityEngine;

using Object = UnityEngine.Object;

namespace EditorPlus
{
    public class PropertyDrawerTools
    {
        public static T GetAttribute<T>(SerializedProperty property) where T : PropertyAttribute
        {
            Object obj = property.serializedObject.targetObject;
            if (obj == null)
            {
                return null;
            }
            FieldInfo field = obj.GetType().GetField(property.name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (field == null)
            {
                return null;
            }
            return field.GetAttribute<T>();
        }
        public static object GetPropertyInstance(SerializedProperty property, FieldInfo field)
        {
            try
            {
                return property.GetValue();
            }
            catch 
            {
                return null;
            }
        }
        public static T GetPropertyInstance<T>(SerializedProperty property, FieldInfo field) where T : class
        {
            try
            {
                return property.GetValue<T>() as T;
            }
            catch
            {
                return default(T);
            }
        }
        public static T GetPropertyInstance<T>(SerializedProperty property) where T : class
        {
            try
            {
                return property.GetValue<T>() as T;
            }
            catch
            {
                return default(T);
            }
        }

    }
}
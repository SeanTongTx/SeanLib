using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using ServiceTools.Reflect;
using System.Reflection;

[InitializeOnLoad]
public class FieldDrawerUtil
{
    static Dictionary<Type, FieldDrawer> fieldDrawers;
    static FieldDrawerUtil()
    {
        fieldDrawers = new Dictionary<Type, FieldDrawer>();
        List<Type> drawers = AssemblyTool.FindTypesInCurrentDomainWhereAttributeIs<CustomFieldDrawer>();
        foreach (var item in drawers)
        {
            if (item.IsSubclassOf(typeof(FieldDrawer)))
            {
                CustomFieldDrawer att = item.GetAttribute<CustomFieldDrawer>();
                fieldDrawers[att.m_type] = ReflecTool.Instantiate(item) as FieldDrawer;
            }
        }
    }
    public static FieldDrawer Get(Type type)
    {
        var v = type.IsGenericType ? type.GetGenericTypeDefinition() : null;

        var enumerator = fieldDrawers.GetEnumerator();
        Type t = null;
        while (enumerator.MoveNext())
        {
            if (type.Equals(enumerator.Current.Key))
            {
                return enumerator.Current.Value;
            }
            else if (type.IsSubclassOf(enumerator.Current.Key))
            {
                if (t == null || enumerator.Current.Key.IsSubclassOf(t))
                {
                    t = enumerator.Current.Key;
                }
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == enumerator.Current.Key)
            {
                if (t == null || enumerator.Current.Key.IsSubclassOf(t))
                {
                    t = enumerator.Current.Key;
                }
            }
        }
        if (t != null)
        {
            return fieldDrawers[t];
        }
        return null;
    }

    public static string TypeString(Type type, out bool isCollection)
    {
        isCollection = false;
        if (type == null)
        {
            return "UnHandledType";
        }
        //泛类型
        //list dictionary
        if (type.IsGenericType)
        {
            Type[] inters = type.GetInterfaces();
            if (inters.First(e => e == typeof(IList)) != null)
            {
                //List
                Type[] args = type.GetGenericArguments();
                string argstr = args[0].Name;
                isCollection = true;
                return string.Format("List<{0}>", argstr);
            }
            else if (inters.First(e => e == typeof(IDictionary)) != null)
            {

                Type[] args = type.GetGenericArguments();
                string argstr0 = args[0].Name;
                string argstr1 = args[1].Name;
                isCollection = true;
                return string.Format("Dictionary<{0},{1}>", argstr0, argstr1);
            }
        }
        else if (type.IsArray)
        {
            isCollection = true;
        }
        return type.Name;
    }


    public static object ValueTypeField(string title, ValueType value, List<PropertyAttribute> CustomAttributes = null)
    {
        if (value is Enum)
        {
            return EditorGUILayout.EnumPopup(title, (Enum)value);
        }
        else if (value is int)
        {
            if (CustomAttributes != null)
            {
                RangeAttribute range = CustomAttributes.Find(e => e is RangeAttribute) as RangeAttribute;
                if (range != null)
                {
                    return EditorGUILayout.IntSlider(title, (int)value, (int)range.min, (int)range.max);
                }
            }
            return EditorGUILayout.DelayedIntField(title, (int)value);
        }
        else if (value is float)
        {
            return EditorGUILayout.DelayedFloatField(title, (float)value);
        }
        else if (value is double)
        {
            return EditorGUILayout.DelayedDoubleField(title, (double)value);
        }
        else if (value is bool)
        {
            return EditorGUILayout.Toggle(title, (bool)value);
        }
        else if (value is long)
        {
            return EditorGUILayout.LongField(title, (long)value);
        }
        else if (value is Color)
        {
            return EditorGUILayout.ColorField(title, (Color)value);
        }
        else if (value is Vector2)
        {
            return EditorGUILayout.Vector2Field(title, (Vector2)value);
        }
        else if (value is Vector3)
        {
            return EditorGUILayout.Vector3Field(title, (Vector3)value);
        }
        else if (value is Vector4)
        {
            return EditorGUILayout.Vector4Field(title, (Vector4)value);
        }
        else if (value is Rect)
        {
            return EditorGUILayout.RectField(title, (Rect)value);
        }
        else if (value is Quaternion)
        {
            Vector3 euler = ((Quaternion)value).eulerAngles;
            euler.x = (float)Math.Round(euler.x, 2);
            euler.y = (float)Math.Round(euler.y, 2);
            euler.z = (float)Math.Round(euler.z, 2);
            euler = EditorGUILayout.Vector3Field(title, euler);
            return Quaternion.Euler(euler);
        }
        else
        {
            EditorGUILayout.LabelField(title, "UnHandledType:" + value.GetType());
            return value;
        }
    }

    public static object ObjectField(string title,object value)
    {
        if(value==null)
        {
            return null;
        }
        return ObjectField(title, value, value.GetType(),null);
    }
    public static object ObjectField(string title, object value, Type type,FieldInfo info,object owner=null)
    {
        return ObjectField(title, value, type, info, owner,null);
    }
    public static object ObjectField(string title, object value, Type type, FieldInfo info, object owner = null, params GUILayoutOption[] options)
    {
        FieldDrawer drawer = FieldDrawerUtil.Get(type);
        if (drawer != null)
        {
            FieldDrawer instance = drawer.Clone();
            instance.owner = owner;
            instance.OnEnbale(title, value, type, info);
            return instance.OnGUI(options);
        }
        else
        {
            EditorGUILayout.LabelField(title, "Cant get Drawer", options);
        }
        return value;
    }
}

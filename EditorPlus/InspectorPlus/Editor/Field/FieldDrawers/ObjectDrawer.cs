
using EditorPlus;
using SeanLib.Core;
using ServiceTools.Reflect;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
[CustomFieldDrawer(typeof(object))]
public class ObjectDrawer : FieldDrawer
{
    public override object OnGUI(params GUILayoutOption[] options)
    {
        if (!type.IsClass)
        {
            EditorGUILayout.LabelField("not class type");
            return instance;
        }
        if (instance == null)
        {
            //try set defalut
            instance = TypeHelper.DefaultValue(type);
        }
        if (instance == null)
        {
            EditorGUILayout.LabelField(type + "cant serialize");
            return instance;
        }
        if (OnGUIUtility.EditorPrefsFoldoutGroup(this.Title))
        {
            OnGUIUtility.Layout.IndentBegin();
            List<FieldInfo> fields = new List<FieldInfo>();
            fields.AddRange(type.GetPublicFields());
            fields.AddRange(type.GetPrivateFields(typeof(SerializeField)));
            foreach (var field in fields)
            {
                object value = field.GetValue(instance);
                var fieldValue = FieldDrawerUtil.ObjectField(field.Name, value, field.FieldType, field, this, options);
                field.SetValue(instance, fieldValue);
            }
            OnGUIUtility.Layout.IndentEnd();
        }
        return instance;
    }
}
 
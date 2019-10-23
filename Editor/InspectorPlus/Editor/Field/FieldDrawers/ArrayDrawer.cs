using EditorPlus;
using SeanLib.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomFieldDrawer(typeof(Array))]
public class ArrayDrawer : FieldDrawer
{
    public override object OnGUI(params GUILayoutOption[] options)
    {
        EditorGUILayout.BeginVertical();
        if (OnGUIUtility.EditorPrefsFoldoutGroup(Title))
        {
            OnGUIUtility.Layout.IndentBegin();
              Type dataType = instance.GetType().GetElementType();

            Array a = (Array)instance;
            int count = EditorGUILayout.IntField("Size", a.Length);
            count = Mathf.Max(0, count);
            if (count > a.Length)
            {
                int offset = count - a.Length;
                Array temp = Array.CreateInstance(dataType, count);
                a.CopyTo(temp, 0);
                for (int i = a.Length; i < a.Length + offset; i++)
                {
                    temp.SetValue(TypeHelper.DefaultValue(dataType), i);
                }
                instance = temp;
            }
            else if (count < a.Length)
            {
                Array temp = Array.CreateInstance(dataType, count);
                for (int i = 0; i < count; i++)
                {
                    temp.SetValue(a.GetValue(i), i);
                }
                instance = temp;
            }
            a = (Array)instance;
            for (int i = 0; i < a.Length; i++)
            {
                object data = a.GetValue(i);
                a.SetValue(FieldDrawerUtil.ObjectField(i.ToString(), data, dataType, this.fieldInfo, this), i);
                //a.SetValue(FieldDrawerUtil.ValueField(i.ToString(), data, dataType), i);
            }
            OnGUIUtility.Layout.IndentEnd();
        }
        EditorGUILayout.EndVertical();
        return instance;
    }
}

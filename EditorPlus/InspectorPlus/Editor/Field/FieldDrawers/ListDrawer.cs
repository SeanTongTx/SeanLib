
using EditorPlus;
using SeanLib.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomFieldDrawer(typeof(List<>))]
public class ListDrawer : FieldDrawer
{
    public override object OnGUI(params GUILayoutOption[] options)
    {
        EditorGUILayout.BeginVertical();
        if (OnGUIUtility.EditorPrefsFoldoutGroup(Title))
        {
            OnGUIUtility.Layout.IndentBegin();
              Type dataType = instance.GetType().GetGenericArguments()[0];
            IList list = (IList)instance;
            int count = EditorGUILayout.IntField("Size", list.Count);
            count = Mathf.Max(0, count);
            if (count > list.Count)
            {
                int offset = count - list.Count;
                for (int i = 0; i < offset; i++)
                {
                    list.Add(TypeHelper.DefaultValue(dataType));
                }
            }
            else if (count < list.Count)
            {
                for (int i = list.Count - 1; i >= count; i--)
                {
                    list.RemoveAt(i);
                }
            }
            for (int i = 0; i < list.Count; i++)
            {
                object data = list[i];

                list[i] = FieldDrawerUtil.ObjectField(i.ToString(), data, dataType, this.fieldInfo, this);
            }
            OnGUIUtility.Layout.IndentEnd();
        }
        EditorGUILayout.EndVertical();
        return instance;
    }
}

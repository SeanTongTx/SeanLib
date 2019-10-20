using EditorPlus;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomFieldDrawer(typeof(string))]
public class StringDrawer : FieldDrawer
{
    public override object OnGUI(params GUILayoutOption[] options)
    {
        if (this.customAttribute != null)
        {
            MultilineAttribute multiline = customAttribute.Find(e => e is MultilineAttribute) as MultilineAttribute;
            if (multiline != null)
            {
                GUILayout.BeginVertical();
                EditorGUILayout.LabelField(Title);
                OnGUIUtility.Layout.IndentBegin();
                int lines = Mathf.Max(3, multiline.lines);
                string str = EditorGUILayout.TextArea((string)instance, GUILayout.Height(lines * 16));
                OnGUIUtility.Layout.IndentEnd();
                GUILayout.EndVertical();
                return str;
            }
        }
        return EditorGUILayout.DelayedTextField(Title, (string)instance, options);
    }
}

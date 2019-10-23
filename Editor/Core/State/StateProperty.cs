
using EditorPlus;
using SeanLib.Core;
using System;

using UnityEditor;

using UnityEngine;
[CustomPropertyDrawer(typeof(State), true)]
public class StateProperty : DefaultPropertyDrawer
{
    protected override void OnDraw(Rect position, SerializedProperty property, GUIContent label)
    {
        State target = PropertyDrawerTools.GetPropertyInstance<State>(property,this.fieldInfo);
        if (target != null && target.BindEnum != null)
        {
            string[] enumstr = Enum.GetNames(target.BindEnum.GetType());
            Undo.RecordObject(property.serializedObject.targetObject, "StateChange");
            target.Current = EditorGUI.MaskField(position, label, target.Current, enumstr);
        }
        else
        {
            EditorGUI.PropertyField(position, property.FindPropertyRelative("Current"));
        }
    }
}

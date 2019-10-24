using SeanLib.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace EditorPlus
{
    [CustomPropertyDrawer(typeof(InspectorPlus.Range))]
    public class RangeDrawer : DefaultPropertyDrawer
    {
        protected override void OnDraw(Rect position, SerializedProperty property, GUIContent label)
        {
            InspectorPlus.Range att = this.attribute as InspectorPlus.Range;
            if (property.propertyType == SerializedPropertyType.Integer)
            {
                property.intValue = EditorGUI.IntSlider(position, label, property.intValue, (int)att.Min, (int)att.Max);
            }
            else if (property.propertyType == SerializedPropertyType.Float)
            {
                property.floatValue = EditorGUI.Slider(position, label, property.floatValue, att.Min, att.Max);
            }
            else if (property.type == "RangeFloat")
            {
                Undo.RegisterCompleteObjectUndo(property.serializedObject.targetObject, "RangeFloat changed");
                SerializedProperty start = property.FindPropertyRelative("start");
                SerializedProperty length = property.FindPropertyRelative("length");
                float s =  start.floatValue;
                float e= s+length.floatValue;
                EditorGUI.BeginChangeCheck();
                position = OnGUIUtility.GUIProperty.BeginIndented(position);
                { 
                    float delta = Mathf.Max(0, position.width - 380);
                    Rect r = Rect.zero;
                    position = OnGUIUtility.Layout.Divide.Divide2Horizontal(position, out r, 80 + delta / 2);
                    GUI.Label(r, label);
                    position = OnGUIUtility.Layout.Divide.Divide2Horizontal(position, out r, 40 + delta / 6);
                    s=EditorGUI.FloatField(r,s);
                    position.x += 1;
                    position.width -= 2;
                    float w = 40 + +delta / 6;
                    position = OnGUIUtility.Layout.Divide.Divide2Horizontal(position, out r, position.width - (w));
                    EditorGUI.MinMaxSlider(r, ref s, ref e, att.Min, att.Max);
                    e = EditorGUI.FloatField(position, e);
                }
                OnGUIUtility.GUIProperty.EndIndented();
                if (EditorGUI.EndChangeCheck())
                {
                    start.floatValue = s;
                    length.floatValue = e - s;
                }
            }
            else{
                base.OnDraw(position, property, label);
            }
        }
    }
}
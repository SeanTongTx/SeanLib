using SeanLib.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace EditorPlus
{
    [CustomPropertyDrawer(typeof(InspectorPlus.TypeName))]
    public class TypeNameDrawer : DefaultPropertyDrawer
    {
        protected override void OnDraw(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                var go = (property.serializedObject.targetObject as Component).gameObject;
                Rect newpos = position.WidthNew(position.width - 16f * 3f);
                Rect r1 = new Rect(newpos.xMax, newpos.y, 16f * 3, newpos.height);
                if (GUI.Button(r1, "Select"))
                {
                    SelectComponentWindow.Show<Component>(go, property.propertyPath);
                }
                if (SelectComponentWindow.CanPick(property.propertyPath))
                {
                    var com = SelectComponentWindow.GetPick();
                    if (com)
                    {
                        property.stringValue = com.GetType().Name;
                    }
                    else
                    {
                        property.stringValue = "";
                    }
                    property.serializedObject.ApplyModifiedProperties();
                }
                EditorGUI.PropertyField(newpos, property, label);
            }
            else
                base.OnDraw(position, property, label);
        }
    }
}
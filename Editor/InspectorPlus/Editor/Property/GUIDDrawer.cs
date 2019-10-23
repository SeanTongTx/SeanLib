using SeanLib.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorPlus
{
    [CustomPropertyDrawer(typeof(InspectorPlus.GUID))]
    public class GUIDDrawer : DefaultPropertyDrawer
    {
        protected override void OnDraw(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                Rect newpos = position.WidthNew(position.width - 16f * 3f);
                Rect r1 = new Rect(newpos.xMax, newpos.y, 16f * 3, newpos.height);
                if (GUI.Button(r1, "New"))
                {
                    property.stringValue = Guid.NewGuid().ToString();
                }
                EditorGUI.PropertyField(newpos, property, label);
            }
            else
                base.OnDraw(position, property, label);
        }
    }
}
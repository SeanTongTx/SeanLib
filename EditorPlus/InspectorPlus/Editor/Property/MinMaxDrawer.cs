using SeanLib.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace EditorPlus
{
    [CustomPropertyDrawer(typeof(InspectorPlus.MinValueAttribute))]
    public class MinDrawer : DefaultPropertyDrawer
    {
        protected override void OnDraw(Rect position, SerializedProperty property, GUIContent label)
        {
            InspectorPlus.MinValueAttribute att = this.attribute as InspectorPlus.MinValueAttribute;
            if (property.propertyType == SerializedPropertyType.Float)
            {
                if (property.floatValue < att.Value)
                {
                    property.floatValue = att.Value;
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
            else if (property.propertyType == SerializedPropertyType.Integer)
            {
                if (property.intValue < (int)att.Value)
                {
                    property.intValue = (int)att.Value;
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
            base.OnDraw(position, property, label);
        }
    }
    [CustomPropertyDrawer(typeof(InspectorPlus.MaxValueAttribute))]
    public class MaxDrawer : DefaultPropertyDrawer
    {
        protected override void OnDraw(Rect position, SerializedProperty property, GUIContent label)
        {
            InspectorPlus.MaxValueAttribute att = this.attribute as InspectorPlus.MaxValueAttribute;
            if (property.propertyType == SerializedPropertyType.Float)
            {
                if (property.floatValue > att.Value)
                {
                    property.floatValue = att.Value;
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
            else if (property.propertyType == SerializedPropertyType.Integer)
            {
                if (property.intValue > (int)att.Value)
                {
                    property.intValue = (int)att.Value;
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
            base.OnDraw(position, property, label);
        }
    }
}
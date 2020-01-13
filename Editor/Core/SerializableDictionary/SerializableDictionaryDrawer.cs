using UnityEditor;
using UnityEngine;
using EditorPlus;
using System;
using SeanLib.Core;

[CustomPropertyDrawer(typeof(ISerializableDictionary), true)]
public class SerializableDictionaryDrawer : DefaultPropertyDrawer
{
    protected override void OnDraw(Rect position, SerializedProperty property, GUIContent label)
    {

        var keys= property.FindPropertyRelative("keys");
        var values = property.FindPropertyRelative("values");
        var keys_size = property.FindPropertyRelative("keys.Array.size");
        var values_size = property.FindPropertyRelative("values.Array.size");

        //EditorGUI.DrawRect(position, Color.green);
        //
        position.height = OnGUIUtility.GUIProperty.GetSinglePropertyHeight(property, label,false);
        //EditorGUI.DrawRect(position, Color.white);
        if (EditorGUI.PropertyField(position, property, false))
        {
            position=EditorGUI.IndentedRect(position);
            position.y += position.height+2;

            OnGUIUtility.Layout.IndentEnable();
            EditorGUI.BeginChangeCheck();
            position.height = OnGUIUtility.GUIProperty.GetSinglePropertyHeight(keys_size, label, false);
            var newsize=EditorGUI.DelayedIntField(position,"Size",keys_size.intValue);
            position.y += position.height + 2;
            if (EditorGUI.EndChangeCheck())
            {
                keys_size.intValue = Mathf.Max(0, newsize);
                keys.arraySize = keys_size.intValue;
                values.arraySize = keys_size.intValue;
                return;
            }
            OnGUIUtility.Layout.IndentDisable();


            //elements
            for (int i = 0; i < keys_size.intValue; i++)
            {
                var keyProperty = keys.GetArrayElementAtIndex(i);
                var propertyHeight= OnGUIUtility.GUIProperty.GetSinglePropertyHeight(keyProperty, null, keyProperty.hasChildren);
                propertyHeight =Mathf.Max(propertyHeight,OnGUIUtility.GUIProperty.GetSinglePropertyHeight(keyProperty, null, keyProperty.hasChildren));

                Rect r0 = Rect.zero;
                Rect r1= OnGUIUtility.Layout.Divide.Divide2Horizontal(position, out r0, 32);
                //index
                GUI.Label(r0,i.ToString());

                r1 = OnGUIUtility.Layout.Divide.FirstRect(r1, 2);
                //Key
                EditorGUI.PropertyField(r1, keyProperty, EditorGUIUtility.TrTempContent(string.Empty), keyProperty.hasChildren);

                Rect r2 = OnGUIUtility.Layout.Divide.NextRect(r1);
                //value
                var valueProperty = values.GetArrayElementAtIndex(i);
                EditorGUI.PropertyField(r2, valueProperty, EditorGUIUtility.TrTempContent(string.Empty), keyProperty.hasChildren);
                //OnGUIUtility.GUIProperty.DefaultPropertyField(r2, valueProperty, null, valueProperty.hasChildren);

                position.y += position.height + 2;
            }
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (TryGetAttibute<InspectorPlus.HideInInspector>() != null)
        {
            return 0;
        }
        if(property.hasChildren)
        {
            float height = EditorGUIUtility.singleLineHeight+2;//property+

            var keys = property.FindPropertyRelative("keys");
            var values = property.FindPropertyRelative("values");
            var keys_size = property.FindPropertyRelative("keys.Array.size");
            var values_size = property.FindPropertyRelative("values.Array.size");
            if(property.isExpanded)
            {
                height += EditorGUIUtility.singleLineHeight + 2;// size
                for (int i = 0; i < keys_size.intValue; i++)
                {
                    var keyProperty = keys.GetArrayElementAtIndex(i);
                    height += OnGUIUtility.GUIProperty.GetSinglePropertyHeight(keyProperty, null, keyProperty.hasChildren);
                    height += 2;
                }
            }
           
            return height;
        }
        else
        {
            return OnGUIUtility.GUIProperty.GetSinglePropertyHeight(property, label, false);
        }
    }
}
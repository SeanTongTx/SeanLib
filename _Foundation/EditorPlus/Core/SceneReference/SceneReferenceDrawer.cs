using EditorPlus;
using SeanLib.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomPropertyDrawer(typeof(SceneReference), true)]
[CustomPropertyDrawer(typeof(InspectorPlus.SceneRefAtt), true)]
public class SceneReferenceDrawer : DefaultPropertyDrawer
{
    protected override void OnDraw(Rect position, SerializedProperty property, GUIContent label)
    {
        var GUID = property.FindPropertyRelative("GUID");
        var DyamicName = property.FindPropertyRelative("DyamicName");
        var Dynamic = property.FindPropertyRelative("Dynamic");
        var TypeName = property.FindPropertyRelative("TypeName");
        SceneReference instance = SerializedPropExtension.GetValue<SceneReference>(property) as SceneReference;
        //instance.RefObject= OnGUIUtility.SelectComponentField<ReferenceObject>(position, instance.RefObject, instance.GUID);
        EditorGUI.BeginChangeCheck();
        InspectorPlus.SceneRefAtt att = TryGetAttibute<InspectorPlus.SceneRefAtt>();
        OnGUIUtility.Vision.BeginBackGroundColor(instance.Dynamic ? OnGUIUtility.Colors.blue : Color.cyan);
        if(att==null)
        {
            att = new InspectorPlus.SceneRefAtt() { ShowDetail = true, ShowFieldStr = true };
        }
        int PropertiesCount = 0;
        if (att.ShowFieldStr)PropertiesCount++;
        if (att.ShowRefType||instance.Dynamic) PropertiesCount++;

        Rect r1 = Rect.zero;
        Rect current = position;
        float delta =Mathf.Max(0, position.width - 360);
        if (att.ShowFieldStr)
        {
            current = OnGUIUtility.Layout.Divide.Divide2Horizontal(current, out r1, PropertiesCount==1?160+ delta : 120+ delta/2);
            EditorGUI.LabelField(r1, label.text + "(" + instance.Identity + ")");
        }
        if (instance.Dynamic)
        {
            Rect r3 = Rect.zero;
            current = OnGUIUtility.Layout.Divide.Divide2Horizontal(current, out r3, 65 + delta / 2);
            DyamicName.stringValue = GUI.TextField(r3, DyamicName.stringValue);
        }
        else
        {
            if (att.ShowRefType)
            {
                Rect r3 = Rect.zero;
                current = OnGUIUtility.Layout.Divide.Divide2Horizontal(current, out r3, 65 + delta / 2);
                if (GUI.Button(r3, instance.Type))
                {
                    if (instance.RefObj)
                    {
                        SelectComponentWindow.Show<Component>(instance.RefObj.gameObject, property.propertyPath);
                    }
                }
                if (SelectComponentWindow.CanPick(property.propertyPath))
                {
                    Undo.RecordObject(property.serializedObject.targetObject, "SceneReference");
                    var com = SelectComponentWindow.GetPick();
                    if (com != null)
                    {
                        instance.Type = com.GetType().Name;
                    }
                    else
                    {
                        instance.Type = "";
                    }
                }
            }
        }

        ObjectField(property, instance, current, att.ShowDetail);
        OnGUIUtility.Vision.EndBackGroundColor();
        if (EditorGUI.EndChangeCheck())
        {
            PrefabUtility.RecordPrefabInstancePropertyModifications(property.serializedObject.targetObject);

        }
    }
    Rect ObjectField(SerializedProperty property, SceneReference instance, Rect postion,bool showDetail, string label=null)
    {
        OnGUIUtility.Layout.IndentDisable();
        postion = postion.Delta(new Vector2(2, 0));
        Rect r4 = Rect.zero;
        if(showDetail)
        {
            r4 = OnGUIUtility.Layout.Divide.Divide2Horizontal(postion, out postion, postion.width - 16);
        }
        EditorGUI.BeginChangeCheck();
        if (!ReferenceRoot.Instance)
        {
            if(label.IsNOTNullOrEmpty())
            {
                instance.RefObj = EditorGUI.ObjectField(postion, label, instance.RefObj, typeof(ReferenceObject), true) as ReferenceObject;
            }
            else
            {
                instance.RefObj = EditorGUI.ObjectField(postion, instance.RefObj, typeof(ReferenceObject), true) as ReferenceObject;
            }
        }
        else
        {
            EditorGUI.BeginChangeCheck();

            UnityEngine.Object obj = null;
            if (label.IsNOTNullOrEmpty())
            {
                obj = EditorGUI.ObjectField(postion, label, instance.RefObj, typeof(UnityEngine.Object), true);
            }
            else
            {
                obj= EditorGUI.ObjectField(postion, instance.RefObj, typeof(UnityEngine.Object), true);
            }
            if (EditorGUI.EndChangeCheck())
            {
                if (obj)
                {
                    if (obj is ReferenceObject)
                    {
                        instance.RefObj = obj as ReferenceObject;
                    }
                    else if (obj is GameObject)
                    {
                        instance.RefObj = ReferenceRoot.AddReference(obj as GameObject, instance);
                    }
                }
                else
                {
                    instance.RefObj = null;
                }
            }
        }
        if (EditorGUI.EndChangeCheck())
        {
            if (instance.RefObj)
            {
                instance.RefObj.TryInit();
                instance.Identity = instance.RefObj.Data.GUID;
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }
            else
            {
                if (!string.IsNullOrEmpty(instance.Identity))
                {
                    instance.Identity = string.Empty;
                    instance.Type = string.Empty;
                    instance.RefObj = null;
                }
            }
        }
        if (GUI.Button(r4, "", new GUIStyle("WinBtnRestore")))
        {
            ObjectPopupWindow.Show(instance, new Vector2(300, 300), "ScenReference", () =>
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                PrefabUtility.RecordPrefabInstancePropertyModifications(property.serializedObject.targetObject);
            });
        }
        OnGUIUtility.Layout.IndentEnable();
        return postion;
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}

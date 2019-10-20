using EditorPlus;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(SceneTimelineRebinder))]
public class SceneTimelineRebinderEditor : MonoBehaviourInspector
{
    SerializedProperty rebinding;
    private void OnEnable()
    {
        rebinding = serializedObject.FindProperty("rebinding");
    }

    public override void OnInspectorGUI()
    {
        SceneTimelineRebinder rebinder = target as SceneTimelineRebinder;
        serializedObject.Update();
        Undo.RecordObject(target, "SceneTimelineRebinder");
        rebinding.isExpanded = EditorGUILayout.Foldout(rebinding.isExpanded, "Rebindings");
        if (rebinding.isExpanded)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginVertical();
            for (int i = 0; i < rebinder.rebinding.Count; i++)
            {
                var item = rebinder.rebinding[i];
                EditorGUILayout.BeginHorizontal();
                OnGUIUtility.Vision.GUIEnabled(false);
                EditorGUILayout.ObjectField(item.Key, typeof(UnityEngine.Object), true,GUILayout.MaxWidth(170));
                OnGUIUtility.Vision.GUIEnabled(true);
                SerializedProperty itemProperty = rebinding.GetArrayElementAtIndex(i).FindPropertyRelative("Value");
                EditorGUILayout.PropertyField(itemProperty);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
        base.OnInspectorGUI();
    }
}

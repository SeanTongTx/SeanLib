using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(MeshIndexViewer))]
public class MeshIndexViewerEditor : MonoBehaviourInspector
{
    SerializedProperty index, count;
    private void OnEnable()
    {
        index = serializedObject.FindProperty("index");
        count = serializedObject.FindProperty("count");
    }
    private void OnSceneGUI()
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.red;
        MeshIndexViewer viewer = target as MeshIndexViewer;
        for (int i = 0, imax = viewer.verticesList.Count; i < imax; ++i)
        {
            if(i>=(index.intValue-1)*count.intValue&&i<=index.intValue*count.intValue)
            {
                Vector3 vPos = viewer.transform.TransformPoint(viewer.verticesList[i]);
                Handles.SphereHandleCap(0, vPos, Quaternion.identity, 0.005f,EventType.Repaint);
                Handles.Label(vPos, i.ToString(), style);
            }
        }
    }
}

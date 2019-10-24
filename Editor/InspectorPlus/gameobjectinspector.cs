using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
/* 作为示范 
[CustomEditor(typeof(GameObject))]
public class GameObjectInspector : InternalInspector<GameObject>
{
    public override string InternalClassFullName
    {
        get
        {
            return "UnityEditor.GameObjectInspector";
        }
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}
*/
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomFieldDrawer(typeof(UnityEngine.Object))]
public class UnityObjectDrawer : FieldDrawer
{
    public override object OnGUI(params GUILayoutOption[] options)
    {
        //这里直接指定type类型 unity editor会崩溃 所以 UnityEngine.Object 将就了
        //2018.2.16 突然又好了 就改回来吧
        return EditorGUILayout.ObjectField(Title, instance as UnityEngine.Object,type, true);
    }
}

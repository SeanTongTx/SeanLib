using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EditorPlus;
namespace EditorPlus
{
    public static partial class OnGUIUtility
    {
        public static class GUIProperty
        {
            public static Rect BeginIndented(Rect position)
            {
                position = EditorGUI.IndentedRect(position);
                Layout.IndentDisable();
                return position;
            }
            public static void EndIndented()
            {
               Layout.IndentEnable();
            }
        }
    }
}

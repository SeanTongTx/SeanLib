using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EditorPlus
{
    public static partial class OnGUIUtility
    {
        public static class Vision
        {
            private static bool GlobleEnable = true;
            static Stack<Color> backgrounds = new Stack<Color>();
            public static void GUIEnabled(bool enable)
            {
                if (!GlobleEnable)
                {
                    GUI.enabled = false;
                    return;
                }
                GUI.enabled = enable;
            }
            public static void GUIGlobleEnable(bool enable)
            {
                GUI.enabled = enable;
                GlobleEnable = enable;
            }

            public static void BeginBackGroundColor(Color color)
            {
                backgrounds.Push(GUI.backgroundColor);
                GUI.backgroundColor = color;
            }
            public static void EndBackGroundColor()
            {
                GUI.backgroundColor = backgrounds.Pop();
            }
        }
    }
}
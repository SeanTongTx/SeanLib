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
            static Stack<Color> background_Colors = new Stack<Color>();
            static Stack<Color> contents_colors = new Stack<Color>();
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
                background_Colors.Push(GUI.backgroundColor);
                GUI.backgroundColor = color;
            }
            public static void EndBackGroundColor()
            {
                GUI.backgroundColor = background_Colors.Pop();
            }
            public static void BeginColor(Color color)
            {
                contents_colors.Push(GUI.color);
                GUI.color = color;
            }
            public static void EndColor()
            {
                GUI.color = contents_colors.Pop();
            }
        }
    }
}
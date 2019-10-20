using System;

using UnityEngine;
using System.Collections;

namespace EditorPlus
{
    public class RichTextHelper
    {
        public static string Bold(string str)
        {
            return "<b>" + str + "</b>";
        }
        public static string Italics(string str)
        {
            return "<i>" + str + "</i>";
        }

        public static string Size(string str, int size = 20)
        {
            return "<size=" + size + ">" + str + "</size>";
        }

        public static string Color(string str, string colorText = "white")
        {
            return "<color=" + colorText + ">" + str + "</color>";
        }
        public static string Color(string str, Color color)
        {
            int r = (int)(color.r * 255);
            int g = (int)(color.g * 255);
            int b = (int)(color.b * 255);
            int a = (int)(color.a * 255);
            string R, G, B, A;
            R = Convert.ToString(r, 16);
            R = R.Length == 1 ? "0" + R : R;
            G = Convert.ToString(g, 16);
            G = G.Length == 1 ? "0" + G : G;
            B = Convert.ToString(b, 16);
            B = B.Length == 1 ? "0" + B : B;
            A = Convert.ToString(a, 16);
            A = A.Length == 1 ? "0" + A : A;
            string colorstr = "#" + R + G + B + A;
            return "<color=" + colorstr + ">" + str + "</color>";
        }
    }
}
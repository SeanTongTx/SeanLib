using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorPlus
{
    public static partial class OnGUIUtility
    {
        public static class Styles
        {
            public static GUIStyle Menu;
            public static GUIStyle ExtendArea;
            public static GUIStyle ExtendGroup;
            public static GUIStyle Title;
            public static GUIStyle Box;
            public static GUIStyle HelpBox;
            public static GUIStyle ErrorLabel;
            static Styles()
            {
                Menu = new GUIStyle("Icon.TrackOptions");
                ExtendArea = new GUIStyle("RL Background");
                ExtendGroup = new GUIStyle("CN Box");
                Title = new GUIStyle("OL Title");
                Box = new GUIStyle("OL box NoExpand");
                HelpBox = new GUIStyle(EditorStyles.helpBox);
                ErrorLabel = new GUIStyle("ErrorLabel");
            }
        }
        public static class Fonts
        {
            public static GUIStyle Bold;
            public static GUIStyle Italics;
            public static GUIStyle Bold_Italics;
            public static GUIStyle RichText;
            public static GUIStyle Error;
            public static GUIStyle ColorText(Color color)
            {
                var colortxt = new GUIStyle(EditorStyles.label);
                colortxt.normal.textColor = color;
                return colortxt;
            }
            static Fonts()
            {
                Bold = new GUIStyle(EditorStyles.label);
                Bold.fontStyle = FontStyle.Bold;
                Italics = new GUIStyle(EditorStyles.label);
                Italics.fontStyle = FontStyle.Italic;
                Bold_Italics = new GUIStyle(EditorStyles.label);
                Bold_Italics.fontStyle = FontStyle.BoldAndItalic;
                RichText = new GUIStyle(EditorStyles.label);
                RichText.richText = true;
                Error = new GUIStyle(Bold);
                Error.normal.textColor = Color.red;
            }
        }
    }
}
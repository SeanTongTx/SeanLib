
using EditorPlus;
using SeanLib.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Collections.Generic;

namespace SeanLib.CodeTemplate
{
    [CustomSeanLibEditor("EditorPlus")]
    public class EditorPlusSettings : SeanLibEditor
    {
        protected override bool UseIMGUI => false;
        protected override ElementsFileAsset FileAsset => new ElementsFileAsset()
        {
            BaseType = this.GetType(),
            UXML = "../EditorPlusSettings.uxml",
            USS = "../EditorPlusSettings.uss"
        };
        List<SerializedObject> ColorPaletteObjs = new List<SerializedObject>();
        Vector2 v,v1;
        public override void SetupUIElements()
        {
            base.SetupUIElements();
            //ColorPalettes
            ColorPaletteObjs.Clear();
            var ColorPaletteAssets= AssetDBHelper.LoadAssets<ColorPalette>(" t:ColorPalette");


            var ColorPalettesRoot = new Foldout() { text = "ColorPalettes" };
            var DocHub = new Foldout() { text = "DocHub" };
            EditorContent_Elements.Add(ColorPalettesRoot);
            EditorContent_Elements.Add(DocHub);

            foreach (var item in ColorPaletteAssets)
            {
                ColorPaletteObjs.Add(new SerializedObject(item));
            }
            IMGUIContainer OnGUIColorPalette = new IMGUIContainer(() =>
            {
                v = EditorGUILayout.BeginScrollView(v);
                foreach (var sobj in ColorPaletteObjs)
                {
                    sobj.Update();
                    EditorGUI.BeginChangeCheck();
                    {
                        EditorGUILayout.ObjectField(sobj.targetObject, typeof(ColorPalette), false);
                        if (EditorGUIUtility.isProSkin)
                        {
                            EditorGUILayout.PropertyField(sobj.FindProperty("Dark"));
                        }
                        else
                        {
                            EditorGUILayout.PropertyField(sobj.FindProperty("Light"));
                        }
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button("Default"))
                            {
                                (sobj.targetObject as ColorPalette).SetDefault();
                                AssetDatabase.SaveAssets();
                            }
                        }
                        GUILayout.EndHorizontal();
                    }
                    if (EditorGUI.EndChangeCheck())
                    {
                        sobj.ApplyModifiedProperties();
                    }
                }
                EditorGUILayout.EndScrollView();
            });
            ColorPalettesRoot.Add(OnGUIColorPalette);
            IMGUIContainer OnGUIDocHub = new IMGUIContainer(() =>
            {
                v1 = EditorGUILayout.BeginScrollView(v1);
                EditorGUI.BeginChangeCheck();
                var autoGif= EditorGUILayout.Toggle("自动播放Gif", EditorUserSettings.GetConfigValue("DocAutoPlayGif") == "true");
                if(EditorGUI.EndChangeCheck())
                {
                    EditorUserSettings.SetConfigValue("DocAutoPlayGif", autoGif ? "true" : "false");
                }
                EditorGUILayout.EndScrollView();
            });
            DocHub.Add(OnGUIDocHub);
        }
    }
}
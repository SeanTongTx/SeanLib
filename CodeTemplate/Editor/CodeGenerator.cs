using EditorPlus;
using SeanLib.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class CodeGenerator : SeanLibEditor
{
    public string Dir;
    public List<CodeTemplate> templates = new List<CodeTemplate>();
    public override void OnGUI()
    {
        base.OnGUI();
        Dir = OnGUIUtility.SaveFolderPanel("SaveDirectory");
        OnDraw();
        if(GUILayout.Button("Generate"))
        {
            OnGenerate();
        }

    }
    public Dictionary<string, string> KeyValues = new Dictionary<string, string>();
    public override void OnEnable(SeanLibWindow drawer)
    {
        base.OnEnable(drawer);
        foreach (var template in templates)
        {
            foreach (var key in template.KeyWords)
            {
                KeyValues[key] = "";
            }
        }
    }
    public virtual void OnDraw()
    {
        foreach (var template in templates)
        {
            GUILayout.BeginVertical(SeanLibEditor.styles.Box);
            EditorGUILayout.LabelField(template.TemplateName,EditorStyles.boldLabel);
            foreach (var key in template.KeyWords)
            {
                KeyValues[key]=EditorGUILayout.TextField(key, KeyValues[key]);
            }
            GUILayout.EndVertical();
            EditorGUILayout.Space();
        }
    }
    public virtual void OnGenerate()
    {
        foreach (var item in KeyValues)
        {
            if(item.Value.IsNullOrEmpty())
            {
                throw new System.Exception("Values error");
            }
        }
        foreach (var template in templates)
        {
            template.GenerateFile(KeyValues, Dir);
        }
        AssetDatabase.Refresh();
    }
}

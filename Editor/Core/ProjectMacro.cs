using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
[InitializeOnLoad]
public class ProjectMacro
{

    static ProjectMacro()
    {
        var Macro = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        if (!Macro.Contains("EDITORPLUS"))
        {
            Macro += "EDITORPLUS;";
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, Macro);
        }
    }
}

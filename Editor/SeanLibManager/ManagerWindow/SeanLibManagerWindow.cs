using EditorPlus;
using UnityEditor;
using UnityEngine;

public class SeanLibManagerWindow: SeanLibManager
{
    [MenuItem("Window/SeanLib/Manager &#1")]
    public static void ShowWindow()
    {
        SeanLibManagerWindow w = GetWindow<SeanLibManagerWindow>();
        w.titleContent = new GUIContent("SeanLibManager");
        w.Show();
    }
}
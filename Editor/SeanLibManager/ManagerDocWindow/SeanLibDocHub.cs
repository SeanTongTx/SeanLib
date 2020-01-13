using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using EditorPlus;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;

public class SeanLibDocHub : SeanLibManager
{
    [MenuItem("Window/SeanLib/Doc &#h")]
    public static SeanLibDocHub ShowDocWindow()
    {
        SeanLibDocHub wnd = GetWindow<SeanLibDocHub>();
        wnd.titleContent = new GUIContent("Documents");
        return wnd;
    }
    protected override ElementsFileAsset FileAsset => new ElementsFileAsset()
    {
        BaseType = this.GetType(),
        UXML = "../SeanLibDocHub.uxml",
        USS = "../SeanLibDocHub.uss"
    };
    protected override string IndexKey => "SeanLibDocIndex";
    protected override void OnEnable()
    {
        if (indexState == null)
        {
            indexState = new TreeViewState();
        }
        libIndex = new SeanLibIndex(indexState,true);
        libIndex.RefreshTreeData(this);
        libIndex.SetSelection(new List<int>() { EditorPrefs.GetInt(IndexKey, 1) });

        VisualElement root = rootVisualElement;

        var visualTree = AssetDBHelper.LoadAsset<VisualTreeAsset>(FileAsset.BaseType, FileAsset.UXML);
        var styleSheet = AssetDBHelper.LoadAsset<StyleSheet>(FileAsset.BaseType, FileAsset.USS);
        root.styleSheets.Add(styleSheet);
        visualTree.CloneTree(root);

        //目录
        EditorIndex = root.Q<VisualElement>("EditorIndex");
        EditorIndexContent_IMGUI = EditorIndex.Q<IMGUIContainer>("EditorIndexContent_IMGUI");
        EditorIndexContent_IMGUI.onGUIHandler = () => { libIndex.OnGUI(EditorIndexContent_IMGUI.contentRect); };
        //内容
        EditorContent = root.Q<VisualElement>("EditorContent");
    }
}
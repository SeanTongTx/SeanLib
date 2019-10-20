using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;

public class UIElementsDemo : EditorWindow
{
    [MenuItem("Window/UIElementsDemo")]
    public static void ShowExample()
    {
        UIElementsDemo wnd = GetWindow<UIElementsDemo>();
        wnd.titleContent = new GUIContent("UIElementsDemo");
    }

    public void OnEnable()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        root.name = "Base";
        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/SeanLib/EditorPlus/Editor/Demo/UIElementsDemo/UIElementsDemo.uxml");
        VisualElement labelFromUXML = visualTree.CloneTree();
        labelFromUXML.AddToClassList("Root");
        labelFromUXML.Q<IMGUIContainer>("BottomIMGUI").onGUIHandler = OnBottomIMGUI;
        root.Add(labelFromUXML);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/SeanLib/EditorPlus/Editor/Demo/UIElementsDemo/UIElementsDemo.uss");
        /*VisualElement labelWithStyle = new Label("Hello World! With Style");
        labelWithStyle.styleSheets.Add(styleSheet);
        root.Add(labelWithStyle);*/
        root.styleSheets.Add(styleSheet);
    }

    private void OnBottomIMGUI()
    {
        if (GUILayout.Button("aaaaa"))
        {
            var sheet = EditorGUIUtility.Load("StyleSheets / UIElementsSamples / UIElementsSamples.uss") as StyleSheet;
            Debug.Log(sheet);
        }
    }
}
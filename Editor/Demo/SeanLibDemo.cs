using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;

namespace EditorPlus
{
    [CustomSeanLibEditor("EditorPlus/Demo")]
    public class SeanLibDemo : SeanLibEditor
    {
        OnGUIUtility.Search search = new OnGUIUtility.Search();
        OnGUIUtility.Zone_Divide2Horizontal zone_Horizon = new OnGUIUtility.Zone_Divide2Horizontal();
        OnGUIUtility.Zone_Divide2Horizontal SubZone_Horizon = new OnGUIUtility.Zone_Divide2Horizontal();
        OnGUIUtility.Zone_Divide2Vertical SubZOne_Vertical = new OnGUIUtility.Zone_Divide2Vertical();
        GUIGifDrawer gifDrawer = new GUIGifDrawer();
        GUIGifDrawer gifDrawer1 = new GUIGifDrawer();
        protected override ElementsFileAsset FileAsset => new ElementsFileAsset()
        {
            BaseType = this.GetType(),
            USS = "../UIElementsDemo/UIElementsDemo.uss",
            UXML = "../UIElementsDemo/UIElementsDemo.uxml"
        };
        protected override bool UseIMGUI => false;
        public override void OnEnable(SeanLibManager drawer)
        {
            base.OnEnable(drawer);
            zone_Horizon.Area0Size = 200;
            zone_Horizon.min = 60;
            zone_Horizon.Max = 800;
            #region 列表Demo

            // Create some list of data, here simply numbers in interval [1, 1000]
            const int itemCount = 1000;
            var items = new List<string>(itemCount);
            for (int i = 1; i <= itemCount; i++)
                items.Add(i.ToString());

            // The "makeItem" function will be called as needed
            // when the ListView needs more items to render
            Func<VisualElement> makeItem = () => new Button();

            // As the user scrolls through the list, the ListView object
            // will recycle elements created by the "makeItem"
            // and invoke the "bindItem" callback to associate
            // the element with the matching data item (specified as an index in the list)
            Action<VisualElement, int> bindItem = (e, i) =>
            {
                var btn = (e as Button);
                btn.text = items[i];
                btn.clickable.clicked += () => Debug.Log(i);
            };
            var listView = window.EditorContent.Q<ListView>();
            listView.makeItem = makeItem;
            listView.bindItem = bindItem;
            listView.itemsSource = items;
            listView.selectionType = SelectionType.Multiple;

            // Callback invoked when the user double clicks an item
            listView.onItemChosen += obj => Debug.Log(obj);

            // Callback invoked when the user changes the selection inside the ListView
            listView.onSelectionChanged += objects => Debug.Log(objects);
            #endregion
            #region button
            // Action to perform when button is pressed.
            // Toggles the text on all buttons in 'container'.
            Action action = () =>
            {
                Debug.Log("Button click");
            };

            // Get a reference to the Button from UXML and assign it its action.
            var uxmlButton = window.EditorContent.Q<Button>("the-uxml-button");
            uxmlButton.clickable.clicked += () => action();
            #endregion
            var DemoIMGUI= window.EditorContent.Q<IMGUIContainer>("IMGUI");
            DemoIMGUI.onGUIHandler = OnGUI;
            gifDrawer.LoadGIF(PathTools.Asset2File(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("gif t:texture")[0])));
            gifDrawer.Play();

            gifDrawer1.LoadGIF(PathTools.Asset2File(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("gif t:texture")[0])));
            gifDrawer1.Play();
        }
        List<string> strlist = new List<string>() { "asd", "ss", "zxz", "cvx1", "svzxc", "asd", "ss", "zxz", "cvx1", "svzxc", "asd", "ss", "zxz", "cvx1", "svzxc" };

        float min = 0, max = 0;
        Vector2 v;
        public override void OnGUI()
        {
            base.OnGUI();
            Title("ScriptField");
            v = GUILayout.BeginScrollView(v);
            OnGUIUtility.ScriptField("this Script", GetType());
            Title("EditorPrefsFoldoutGroup");
            if (OnGUIUtility.EditorPrefsFoldoutGroup("FoldoutGroup"))
            {
                GUILayout.Label("FoldoutContent");
                GUILayout.Label("FoldoutContent");
                GUILayout.Label("FoldoutContent");
            }
            Title("ObjectPopupWindow");
            if (GUILayout.Button("ObjectPopupWindow.Show"))
            {
                ObjectPopupWindow.Show(this);
            }
            Title("SelectWindow");
            if (GUILayout.Button("SelectWindow<T>.Show"))
            {
                SelectWindow<string>.Show(strlist, "1");
            }
            if (SelectWindow<string>.CanPick("1"))
            {
                var t = SelectWindow<string>.GetPick();
                Debug.Log(t);
            }
            Title("OnGUIUtility.Search");
            var s = search.OnToolbarGUI();

            Title("OnGUIUtility.Zone_Divide2Horizontal");
            zone_Horizon.OnGUILayout(window.Repaint, () =>
             {
                 GUILayout.Button("1");
                 GUILayout.Button("1");
                 GUILayout.Button("1");
                 GUILayout.Button("1");
                 OnGUIUtility.Debug.HolderBox();
             },
             () =>
             {
                 SubZone_Horizon.OnGUILayout(window.Repaint, () =>
                 {
                     GUILayout.Button("2");
                     GUILayout.Button("2");
                     OnGUIUtility.Debug.HolderBox();
                     GUILayout.Button("2");
                     SubZOne_Vertical.OnGUILayout(window.Repaint, () =>
                     {
                         GUILayout.Button("3");
                         GUILayout.Button("3");
                         OnGUIUtility.Debug.HolderBox();
                         GUILayout.Button("3");

                     },
                     () =>
                     {
                         GUILayout.Button("3");
                         OnGUIUtility.Debug.HolderBox();
                         GUILayout.Button("3");
                     });
                 },
                 () =>
                 {
                     GUILayout.Button("2");
                     OnGUIUtility.Debug.HolderBox();
                     GUILayout.Button("2");
                 });
             });
            EditorGUILayout.MinMaxSlider("111",ref min, ref max, 0, 100);
            Title("SeanlibEditor.Styles");
            GUILayout.Button("SeanLibEditor.styles.Area", SeanLibEditor.styles.ExtendArea, GUILayout.Width(200));
            GUILayout.Button("SeanLibEditor.styles.Group", SeanLibEditor.styles.ExtendGroup, GUILayout.Width(200));
            GUILayout.Button("SeanLibEditor.styles.Title", SeanLibEditor.styles.Title, GUILayout.Width(200));
            GUILayout.EndScrollView();
            Title("GUIGifDrawer.OnGUI");
            gifDrawer.OnGUI(this.window.Repaint);
            gifDrawer1.OnGUI(this.window.Repaint);

        }
        public override void OnDisable()
        {
            base.OnDisable();
            gifDrawer.Clean();
        }
        public void Title(string title)
        {
            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            EditorGUILayout.LabelField(title);
            OnGUIUtility.Layout.Line();
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
namespace EditorPlus
{
    [CustomSeanLibEditor("EditorPlus/Demo")]
    public class SeanLibDemo : SeanLibEditor
    {
        OnGUIUtility.Search search = new OnGUIUtility.Search();
        OnGUIUtility.OrderList orderList = new OnGUIUtility.OrderList();
        OnGUIUtility.Zone_Divide2Horizontal zone_Horizon = new OnGUIUtility.Zone_Divide2Horizontal();
        OnGUIUtility.Zone_Divide2Horizontal SubZone_Horizon = new OnGUIUtility.Zone_Divide2Horizontal();
        OnGUIUtility.Zone_Divide2Vertical SubZOne_Vertical = new OnGUIUtility.Zone_Divide2Vertical();
        public override void OnEnable(SeanLibWindow drawer)
        {
            base.OnEnable(drawer);
            zone_Horizon.Area0Size = 200;
            zone_Horizon.min = 60;
            zone_Horizon.Max = 800;
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

            Title("OnGUIUtility.OrderList");
            orderList.OnGui();

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
        }
        public void Title(string title)
        {
            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            EditorGUILayout.LabelField(title);
            OnGUIUtility.Layout.Line();
        }
    }
}
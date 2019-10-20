using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using ServiceTools.Reflect;
using System;

namespace EditorPlus
{

    [CustomSeanLibEditor("Home", order = -1)]
    public class HomePage : EditorMarkDownWindow
    {
        public override string RelativePath => "../../../Doc";
        static class Styles
        {
            public static GUIStyle PackageTitle;
            public static GUIStyle ImportButton;
        }
        string[] packages;
        string[] plugins;
        AnimBool[] packageExtends;
        AnimBool[] pluginExtends;
        MarkDownDoc[] docs;
        Vector2 HomeScroll;
        string PackStorageDir;
        string LibDir;

        ListRequest request;
        PackageCollection packagesCollect;
        public override void OnEnable(SeanLibWindow drawer)
        {
            base.OnEnable(drawer);
            packages = null;
            plugins = null;
            request = UnityEditor.PackageManager.Client.List();
        }
        protected override void SetupLayout()
        {
            base.SetupLayout();
            Styles.PackageTitle = Styles.PackageTitle ?? new GUIStyle("OL Title");
            Styles.ImportButton = Styles.ImportButton ?? new GUIStyle("OL Title");
            // Styles.ImportButton = Styles.ImportButton ?? new GUIStyle("ToolbarButtonFlat");
        }
        private bool CheckPackages()
        {
            if (packagesCollect == null)
            {
                if (request.IsCompleted)
                {
                    if (request.Status == StatusCode.Failure)
                    {
                        UnityEngine.Debug.LogError(request.Error);
                    }
                    packagesCollect = request.Result;
                }
                else
                {
                    this.window.ShowNotification(new GUIContent("Collecting Packages..."));
                    return true;
                }
            }
            return false;
        }
        private void CheckDocs()
        {
            if (EditorGUI.EndChangeCheck() || packages == null || plugins == null)
            {
                packages = Directory.GetFiles(PackStorageDir, "*.unitypackage", SearchOption.AllDirectories);
                plugins = Directory.GetFiles(LibDir,"package.json",SearchOption.AllDirectories);
                //Packages
                if (packageExtends != null)
                {
                    foreach (var item in packageExtends)
                    {
                        item.valueChanged.RemoveListener(window.Repaint);
                    }
                }
                packageExtends = new AnimBool[packages.Length];
                for (int i = 0; i < packages.Length; i++)
                {
                    packageExtends[i] = new AnimBool(false);
                    packageExtends[i].valueChanged.AddListener(window.Repaint);
                }
                //Plugins
                if (pluginExtends != null)
                {
                    foreach (var item in pluginExtends)
                    {
                        item.valueChanged.RemoveListener(window.Repaint);
                    }
                }
                pluginExtends = new AnimBool[plugins.Length];
                for (int i = 0; i < plugins.Length; i++)
                {
                    pluginExtends[i] = new AnimBool(false);
                    pluginExtends[i].valueChanged.AddListener(window.Repaint);
                }
                //DOC
                var mdlist = new List<MarkDownDoc>();
                for (int i = 0; i < packages.Length; i++)
                {
                    var packageName = Path.GetFileNameWithoutExtension(packages[i]);
                    var docDir = PackStorageDir + "/docs/" + packageName + "/";
                    var introPath = docDir + "Introduction.md";
                    if (File.Exists(introPath))
                    {
                        string rawDoc = File.ReadAllText(introPath);
                        var doc = new MarkDownDoc(docDir, rawDoc);
                        mdlist.Add(doc);
                    }
                    else
                    {
                        mdlist.Add(null);
                    }
                }
                for (int i = 0; i < plugins.Length; i++)
                {
                    var packageName = Directory.GetParent(plugins[i]).Name;
                    var docDir = LibDir + "/" + packageName + "/";
                    var introPath = docDir + "README.md";
                    if (File.Exists(introPath))
                    {
                        string rawDoc = File.ReadAllText(introPath);
                        var doc = new MarkDownDoc(docDir, rawDoc);
                        mdlist.Add(doc);
                    }
                    else
                    {
                        mdlist.Add(null);
                    }
                }
                docs = mdlist.ToArray();

            }
        }
        public override void OnGUI()
        {
            SetupLayout();
            if (CheckPackages()) return;
            HomeScroll = GUILayout.BeginScrollView(HomeScroll);
            MarkDownDoc readme = null;
            if (Docs.TryGetValue("Index", out readme))
            {
                EditorMarkDownDrawer.DrawDoc(readme);
            }
            EditorGUI.BeginChangeCheck();
            PackStorageDir = OnGUIUtility.OpenFolderPannel("SeanLibPackageStorage");
            LibDir = OnGUIUtility.OpenFolderPannel("SeanLibDirectory");
            CheckDocs();
            //ShowDoc
            for (int i = 0; i < packages.Length; i++)
            {
                var package = packages[i];
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(Path.GetFileNameWithoutExtension(package), Styles.PackageTitle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false)))
                {
                    bool b = !packageExtends[i].target;
                    foreach (var extend in packageExtends)
                    {
                        extend.target = false;
                    }
                    packageExtends[i].target = b;
                }
                if (GUILayout.Button("Import", Styles.ImportButton, GUILayout.Width(80)))
                {
                    AssetDatabase.ImportPackage(package, true);
                    //Process.Start(package);
                }
                EditorGUILayout.EndHorizontal();
                if (EditorGUILayout.BeginFadeGroup(packageExtends[i].faded))
                {
                    EditorGUILayout.BeginVertical();
                    if (docs[i] != null)
                    {
                        EditorMarkDownDrawer.DrawDoc(docs[i]);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Cant find MD document", EditorStyles.boldLabel);
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndFadeGroup();
            }
            for (int i = 0; i < plugins.Length; i++)
            {
                var plugin = plugins[i];
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(Directory.GetParent(plugin).Name, Styles.PackageTitle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false)))
                {
                    bool b = !pluginExtends[i].target;
                    foreach (var extend in pluginExtends)
                    {
                        extend.target = false;
                    }
                    pluginExtends[i].target = b;
                }
                if (GUILayout.Button("Import", Styles.ImportButton, GUILayout.Width(80)))
                {
                    Type type = typeof(UnityEditor.PackageManager.Client).Assembly.GetType("UnityEditor.PackageManager.UI.Package");
                    var method = type.GetMethod("AddFromLocalDisk", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                    method.Invoke(null, new object[] { plugin  });
                    //TODO:
                }
                EditorGUILayout.EndHorizontal();
                if (EditorGUILayout.BeginFadeGroup(pluginExtends[i].faded))
                {
                    EditorGUILayout.BeginVertical();
                    if (docs[packages.Length + i] != null)
                    {
                        EditorMarkDownDrawer.DrawDoc(docs[packages.Length + i]);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Cant find MD document", EditorStyles.boldLabel);
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndFadeGroup();
            }
            GUILayout.EndScrollView();
        }
        public override void OnDisable()
        {
            if (packageExtends != null)
            {
                foreach (var item in packageExtends)
                {
                    item.valueChanged.RemoveListener(window.Repaint);
                }
            }
            if (pluginExtends != null)
            {
                foreach (var item in pluginExtends)
                {
                    item.valueChanged.RemoveListener(window.Repaint);
                }
            }
            base.OnDisable();
        }
    }
}
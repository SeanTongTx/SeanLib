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
using SeanLib.Core;

namespace EditorPlus
{

    [CustomSeanLibEditor("Home", order = -1)]
    public class HomePage : EditorMarkDownWindow
    {
        private bool coding = true;
        public override string RelativePath => "../../../Doc";
        static class Styles
        {
            public static GUIStyle PackageTitle;
            public static GUIStyle ImportButton;
            static Styles()
            {
                PackageTitle = new GUIStyle("OL Title");
                PackageTitle.richText = true;
                ImportButton = new GUIStyle("OL Title");
            }
        }
        string[] packages;
        string[] plugins;
        AnimBool[] packageExtends;
        AnimBool[] pluginExtends;

        Vector2 HomeScroll;
        string PackStorageDir;
        string LocalLibDir;
        MarkDownDoc[] docs;
        public override void OnEnable(SeanLibManager drawer)
        {
            base.OnEnable(drawer);
            packages = null;
            plugins = null;
            ReadRemotePluginsFile();
            RemotePlistEditor.OnEnable((info, index) =>
            {
                OnGUIUtility.Vision.BeginBackGroundColor(index%2==0?OnGUIUtility.Colors.dark: OnGUIUtility.Colors.light);
                {
                    EditorGUILayout.BeginVertical();
                    if (OnGUIUtility.Foldout(info.Name, Styles.PackageTitle, GUILayout.ExpandWidth(true)))
                    {
                        info.Name = EditorGUILayout.DelayedTextField("Name", info.Name);
                        info.URL = EditorGUILayout.DelayedTextField("URL", info.URL);
                    }
                    EditorGUILayout.EndVertical();
                }
                OnGUIUtility.Vision.EndBackGroundColor();
                return info;
            },
            () =>
            {
                return new PluginInfo();
            },1,"+Plugin");
        }
        private void GUIToolBar()
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.Label(Title, EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
            if (GUILayout.Button(EditorGUIUtility.IconContent("RotateTool"), EditorStyles.toolbarButton, GUILayout.Width(26)))
            {
                ReadRemotePluginsFile();
            }
            GUILayout.EndHorizontal();
        }
        private void ShowIndexDoc()
        {
            MarkDownDoc readme = null;
            if (Docs.TryGetValue("Index", out readme))
            {
                EditorMarkDownDrawer.DrawDoc(readme);
            }
        }
        public override void OnGUI()
        {
            SetupLayout();
            GUIToolBar();
            HomeScroll = GUILayout.BeginScrollView(HomeScroll);
            ShowIndexDoc();
            EditorGUI.BeginChangeCheck();
            {
                PackStorageDir = OnGUIUtility.OpenFolderPannel(".unitypackage:");
                LocalLibDir = OnGUIUtility.OpenFolderPannel("LocalPlugins:");
                OnGUIUtility.Vision.GUIEnabled(false);
                var pluginlistPath = PathTools.RelativeAssetPath(this.GetType(), RelativePath);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("RemotePlugins:", pluginlistPath + "/plugins.json");
                OnGUIUtility.Vision.GUIEnabled(true);
                var newEdit= GUILayout.Toggle(EditRemotePlugins, "Edit", GUI.skin.button, GUILayout.Width(80));
                if(!newEdit&& EditRemotePlugins!= newEdit)
                {
                    if(EditorUtility.DisplayDialog("cancel", "This will cancel unsaved changes ","ok","cancel"))
                    {
                        ReadRemotePluginsFile();
                        EditRemotePlugins = newEdit;
                    }
                }
                else
                {
                    EditRemotePlugins = newEdit;
                }
                EditorGUILayout.EndHorizontal();
            }
            if (EditorGUI.EndChangeCheck())
            {
            }
            if (EditRemotePlugins)
            {
                GUIRemotePlugins();
                GUILayout.EndScrollView();
                return;
            }
            //CheckDocs();
            //ShowDoc
            OnGUIUtility.Layout.Line();
            ShowPackages();
            OnGUIUtility.Layout.Line();
            ShowLocalPlugins();
            OnGUIUtility.Layout.Line();
            ShowRemotePlugins();
            /* for (int i = 0; i < packages.Length; i++)
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
             }*/
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
            if (RemotepluginExtends != null)
            {
                foreach (var item in RemotepluginExtends)
                {
                    item.valueChanged.RemoveListener(window.Repaint);
                }
            }
            
            base.OnDisable();
        }
        #region packages

        private void ShowPackages()
        {
            foreach (var item in remoteplugins)
            {
                EditorGUILayout.LabelField(item.Name);
            }
        }
        #endregion
        #region Local

        private void ShowLocalPlugins()
        {
            foreach (var item in remoteplugins)
            {
                EditorGUILayout.LabelField(item.Name);
            }
        }
        #endregion

        #region RemotePlugin

        Vector2 remoteScroll;
        PluginList remoteplugins;
        bool EditRemotePlugins;
        AnimBool[] RemotepluginExtends;

        OnGUIUtility.Grid.GridContainer<PluginInfo> RemotePlistEditor = new OnGUIUtility.Grid.GridContainer<PluginInfo>();
        private void ShowRemotePlugins()
        {
            for (int i = 0; i < remoteplugins.Plugins.Count; i++)
            {
                var item = remoteplugins.Plugins[i];
                item.type = PluginType.Remote;
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button(item.Name+ MarkDownConst.GetVersionRichText((int)item.version), Styles.PackageTitle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false)))
                    {
                        bool b = !RemotepluginExtends[i].target;
                        foreach (var extend in RemotepluginExtends)
                        {
                            extend.target = false;
                        }
                        RemotepluginExtends[i].target = b;
                    }
                    if(!string.IsNullOrEmpty(item.URL)&& GUILayout.Button("Import", Styles.ImportButton, GUILayout.Width(80)))
                    {
                        var packageId = item.Name.ToLower();
                        Client.Add(item.URL);
                    }
                }
                EditorGUILayout.EndHorizontal();
                if (EditorGUILayout.BeginFadeGroup(RemotepluginExtends[i].faded))
                {
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField("Name:", item.Name, EditorStyles.boldLabel);
                    EditorGUILayout.LabelField("Storage:", item.type.ToString());
                    EditorGUILayout.LabelField("URL:", item.URL);
                    EditorGUILayout.LabelField("Version:",MarkDownConst.GetVersionRichText((int)item.version),OnGUIUtility.Fonts.RichText);
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndFadeGroup();
            }
        }
        private void WriteRemotePluginsFile()
        {
            var pluginlistPath = PathTools.RelativeAssetPath(this.GetType(), RelativePath);
            DirectoryInfo info = new DirectoryInfo(pluginlistPath);
            FileTools.WriteAllText(info + "/plugins.json", JsonUtility.ToJson(remoteplugins));
        }
        private void ReadRemotePluginsFile()
        {
            var pluginlistPath = PathTools.RelativeAssetPath(this.GetType(), RelativePath);
            DirectoryInfo info = new DirectoryInfo(pluginlistPath);
            remoteplugins = JsonUtility.FromJson<PluginList>(FileTools.ReadAllText(info + "/plugins.json"));
            if (RemotepluginExtends != null)
            {
                foreach (var item in RemotepluginExtends)
                {
                    item.valueChanged.RemoveListener(window.Repaint);
                }
            }
            RemotepluginExtends = new AnimBool[remoteplugins.Plugins.Count];
            for (int i = 0; i < RemotepluginExtends.Length; i++)
            {
                RemotepluginExtends[i] = new AnimBool(false);
                RemotepluginExtends[i].valueChanged.AddListener(window.Repaint);
            }
        }
        private void GUIRemotePlugins()
        {
            EditorGUILayout.HelpBox(@"This window will help u edit remotePlugins config.
               these changes store in where which your foundation package storaged.
               1. SeanLib Import by SeanLib.unitypackage .Then these config will store in your project Assets.
               2. SeanLib Import by package.json in Local file.Then these config will store in folder your localPlugin storaged.
               3. SeanLib Import by package.json/URL Online.Then these config will store in your project Libary", MessageType.Warning);
            RemotePlistEditor.OnGUI(remoteplugins.Plugins);
            remoteScroll = EditorGUILayout.BeginScrollView(remoteScroll);
            EditorGUILayout.EndScrollView();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("save", GUILayout.Width(80)))
            {
                WriteRemotePluginsFile();
                AssetDatabase.Refresh();
                ReadRemotePluginsFile();
                EditRemotePlugins = false;
            }
            if (GUILayout.Button("cancel", GUILayout.Width(80)))
            {
                ReadRemotePluginsFile();
                EditRemotePlugins = false;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }
        #endregion

        private void CheckDocs()
        {
            if (EditorGUI.EndChangeCheck() || packages == null || plugins == null)
            {
                packages = Directory.GetFiles(PackStorageDir, "*.unitypackage", SearchOption.AllDirectories);
                plugins = Directory.GetFiles(LocalLibDir, "package.json", SearchOption.AllDirectories);
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
                    var docDir = LocalLibDir + "/" + packageName + "/";
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
    }
}
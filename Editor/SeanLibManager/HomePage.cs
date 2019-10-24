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
        Vector2 HomeScroll;
        public override void OnEnable(SeanLibManager drawer)
        {
            base.OnEnable(drawer);
            PackStorageDir = EditorUserSettings.GetConfigValue(".unitypackage:");
            LocalLibDir = EditorUserSettings.GetConfigValue("LocalPlugins:");
            ReadRemotePluginsFile();
            ReadPackageFold();
            ReadLocalPlugin();

            RemotePlistEditor.OnEnable((info, index) =>
            {
                OnGUIUtility.Vision.BeginBackGroundColor(index % 2 == 0 ? OnGUIUtility.Colors.dark : OnGUIUtility.Colors.light);
                {
                    EditorGUILayout.BeginVertical();
                    if (OnGUIUtility.Foldout(info.Name, Styles.PackageTitle, GUILayout.ExpandWidth(true)))
                    {
                        info.Name = EditorGUILayout.DelayedTextField("Name", info.Name);
                        info.URL = EditorGUILayout.TextField("URL", info.URL);
                        info.version = (PluginVersion)EditorGUILayout.EnumPopup("Version", info.version);
                    }
                    EditorGUILayout.EndVertical();
                }
                OnGUIUtility.Vision.EndBackGroundColor();
                return info;
            },
            () =>
            {
                return new PluginInfo();
            }, 1, "+Plugin");
        }
        private void GUIToolBar()
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.Label(Title, EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
            if (GUILayout.Button(EditorGUIUtility.IconContent("RotateTool"), EditorStyles.toolbarButton, GUILayout.Width(26)))
            {
                ReadPackageFold();
                ReadRemotePluginsFile();
                ReadLocalPlugin();
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
                ReadPackageFold();
            }
            if (EditRemotePlugins)
            {
                GUIRemotePlugins();
                GUILayout.EndScrollView();
                return;
            }
            //CheckDocs();
            //ShowDoc
            OnGUIUtility.Layout.Header("Packages");
            ShowPackages();
            OnGUIUtility.Layout.Header("Local");
            ShowLocalPlugins();
            OnGUIUtility.Layout.Header("Remote");
            ShowRemotePlugins();
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

        PluginList PackagePlugins = new PluginList();
        string PackStorageDir=string.Empty;
        AnimBool[] packageExtends;
        private void ReadPackageFold()
        {
            if (PackStorageDir.IsNullOrEmpty()) return;
            PackagePlugins.Plugins.Clear();
            string[] packages = Directory.GetFiles(PackStorageDir, "*.unitypackage", SearchOption.AllDirectories);
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
            //DOC
            for (int i = 0; i < packages.Length; i++)
            {
                PluginInfo plugin = new PluginInfo();
                
                var packageName = Path.GetFileNameWithoutExtension(packages[i]);
                var docDir = PackStorageDir + "/docs/" + packageName + "/";
                var introPath = docDir + "README.md";
                if (File.Exists(introPath))
                {
                    string rawDoc = File.ReadAllText(introPath);
                    plugin.doc = new MarkDownDoc(docDir, rawDoc);                    
                }
                plugin.type = PluginType.Packages;
                //plugin.version = PluginVersion.Release;
                plugin.Name = packageName;
                plugin.URL = packages[i];
                PackagePlugins.Plugins.Add(plugin);
            }
        }

        private void ShowPackages()
        {
            for (int i = 0; i < PackagePlugins.Plugins.Count; i++)
             {
                 var package = PackagePlugins.Plugins[i];
                 EditorGUILayout.BeginHorizontal();
                 if (GUILayout.Button(package.Name, Styles.PackageTitle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false)))
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
                     AssetDatabase.ImportPackage(package.URL, true);
                     //Process.Start(package);
                 }
                 EditorGUILayout.EndHorizontal();
                 if (EditorGUILayout.BeginFadeGroup(packageExtends[i].faded))
                 {
                     EditorGUILayout.BeginVertical();
                     if (package.doc!=null)
                     {
                         EditorMarkDownDrawer.DrawDoc(package.doc);
                     }
                     else
                     {
                         EditorGUILayout.LabelField("Cant find MD document", EditorStyles.boldLabel);
                     }
                     EditorGUILayout.EndVertical();
                 }
                 EditorGUILayout.EndFadeGroup();
             }
        }
        #endregion
        #region Local

        PluginList LocalPlugins = new PluginList();
        string LocalLibDir;
        AnimBool[] pluginExtends;
        private void ReadLocalPlugin()
        {
            string[] plugins= Directory.GetFiles(LocalLibDir, "package.json", SearchOption.AllDirectories);
            LocalPlugins.Plugins.Clear();
            if (pluginExtends != null)
            {
                foreach (var item in pluginExtends)
                {
                    item.valueChanged.RemoveListener(window.Repaint);
                }
            }
            pluginExtends = new AnimBool[plugins.Length];
            for (int i = 0; i < pluginExtends.Length; i++)
            {
                pluginExtends[i] = new AnimBool(false);
                pluginExtends[i].valueChanged.AddListener(window.Repaint);
            }
            for (int i = 0; i < plugins.Length; i++)
            {
                PluginInfo plugin = new PluginInfo();
                var pluginURL = plugins[i].Replace(@"/",@"\");
                var dir = Directory.GetParent(pluginURL);
                var pluginName = dir.Name;
                var pluginDocPath = dir.FullName + "/README.md";
                if (File.Exists(pluginDocPath))
                {
                    string rawDoc = File.ReadAllText(pluginDocPath);
                    plugin.doc = new MarkDownDoc(dir.FullName+"/", rawDoc);
                }
                plugin.Name = pluginName;
                plugin.URL = pluginURL;
                plugin.type = PluginType.Local;
                //plugin.version
                LocalPlugins.Plugins.Add(plugin);
            }
        }
        private void ShowLocalPlugins()
        {
            for (int i = 0; i < LocalPlugins.Plugins.Count; i++)
            {
                var package = LocalPlugins.Plugins[i];
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(package.Name, Styles.PackageTitle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(false)))
                {
                    bool b = !pluginExtends[i].target;
                    foreach (var extend in pluginExtends)
                    {
                        extend.target = false;
                    }
                    pluginExtends[i].target = b;
                }
                if (!string.IsNullOrEmpty(package.URL) && GUILayout.Button("Import", Styles.ImportButton, GUILayout.Width(80)))
                {
                    if (EditorUtility.DisplayDialog(@"Import LocalPlugin", @"导入本地插件时，插件不会复制到项目内，而是存储在原本的目录中，通过绝对路径引用。
因此不能将项目分享给其他用户！", "这是本地项目", "这是合作项目"))
                    {
#if UNITY_2019_2_OR_NEWER
                        var Internal_packageType = typeof(Client).Assembly.GetType("UnityEditor.PackageManager.UI.Package");
                        var addLocalMethod = Internal_packageType.GetMethod("AddFromLocalDisk", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                        addLocalMethod.Invoke(null, new object[] { package.URL });
#endif
                    }
                }
                EditorGUILayout.EndHorizontal();
                if (EditorGUILayout.BeginFadeGroup(pluginExtends[i].faded))
                {
                    EditorGUILayout.BeginVertical();
                    if (package.doc != null)
                    {
                        EditorMarkDownDrawer.DrawDoc(package.doc);
                    }
                    else
                    {
                        EditorGUILayout.LabelField("Cant find MD document", EditorStyles.boldLabel);
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndFadeGroup();
            }
        }
#endregion

#region RemotePlugin

        Vector2 remoteScroll;
        PluginList remoteplugins;
        bool EditRemotePlugins;
        AnimBool[] RemotepluginExtends;
        static AddRequest request;
        static string ProgressingPlugin;
        float fake;
        string[] hints = new string[] { "这个读条完全是假的", "API里没有进度信息", "要爬多久还不是得看网有多快",
            "一次下载一个GitHub都很卡","关了窗口他也在爬","开始了就停不下来","居然做了真做出了这个读条，感觉真蠢",
        "加载过一次会有缓存，快很多",};
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
                    if (request != null && request.Status == StatusCode.InProgress)
                    {
                        if (item.Name == ProgressingPlugin)
                        {
                            var rect = GUILayoutUtility.GetLastRect();
                            fake += (Time.deltaTime / 200f);
                            if(fake>=hints.Length)
                            {
                                fake = 0;
                            }
                            EditorGUI.ProgressBar(rect, fake-(float)((int)fake),hints[(int)fake]);
                            window.Repaint();
                        }
                        OnGUIUtility.Vision.GUIEnabled(false);
                    }
                    if (!string.IsNullOrEmpty(item.URL)&& GUILayout.Button("Import", Styles.ImportButton, GUILayout.Width(80)))
                    {
                        ProgressingPlugin = item.Name;
                        fake = UnityEngine.Random.Range(0, hints.Length);
                        request = Client.Add(item.URL);
                    }
                    OnGUIUtility.Vision.GUIEnabled(true);
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
            remoteScroll = EditorGUILayout.BeginScrollView(remoteScroll);
            RemotePlistEditor.OnGUI(remoteplugins.Plugins);
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

    }
}
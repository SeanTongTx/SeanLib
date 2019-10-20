using SeanLib.Core;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace EditorPlus
{
    public abstract class EditorMarkDownWindow : SeanLibEditor
    {
        //文档查找
        public virtual string RelativePath => "../../Doc";
        public virtual string Title => this.GetType().Name;
        public virtual bool EditScript { get { return true; } }
        public virtual bool SearchField { get { return false; } }
        public Dictionary<string, MarkDownDoc> Docs = new Dictionary<string, MarkDownDoc>();
        public Stack<string> DocPages = new Stack<string>();
        public string CurrentDocName;

        private OnGUIUtility.Search search = new OnGUIUtility.Search();
        public OnGUIUtility.Search Search
        {
            get
            {
                return SearchField ? search : null;
            }
        }

        public override void OnEnable(SeanLibWindow drawer)
        {
            base.OnEnable(drawer);
            var docAssetDir = PathTools.RelativeAssetPath(this.GetType(), RelativePath);
            //因为有一些文档放在Package目录中 而Package又不一定在项目文件夹下 所以这个方法无效
            /* var scriptFilePath = PathTools.Asset2File(docAssetDir);
             List<string> dirs = new List<string>(Directory.GetDirectories(Path.GetDirectoryName(scriptFilePath),"",SearchOption.AllDirectories));// new List<string>(Directory.GetDirectories(Application.dataPath, PathRoot, SearchOption.AllDirectories));
             string DirRoot = Application.dataPath;
             if (dirs.Count > 0)
             {
                 dirs.Sort((l, r) => { return l.Length - r.Length; });

                 DirRoot = LoadPath.replaceToUnity(dirs[0]) + "/";
             }

             List<string> docPath = new List<string>();
             foreach (var dir in dirs)
             {
                 docPath.AddRange(Directory.GetFiles(dir, "*.md", SearchOption.AllDirectories));
             }
             docPath.Sort((l, r) =>
             {
                 var lDir = LoadPath.replaceToUnity(Path.GetDirectoryName(l).Replace(DirRoot, ""));
                 var rDir = LoadPath.replaceToUnity(Path.GetDirectoryName(r).Replace(DirRoot, ""));
                 int lCount = lDir.Split('/').Length;
                 int rCount = rDir.Split('/').Length;
                 return lCount - rCount;
             });

             foreach (var path in docPath)
             {
                 var docDir = Path.GetDirectoryName(path) + "/";
                 var docName = Path.GetFileNameWithoutExtension(path);
                 string rawDoc = File.ReadAllText(path);
                 var doc = new MarkDownDoc(docDir, rawDoc);
                 if (Docs.ContainsKey(docName))
                 {
                     var subDir = LoadPath.replaceToUnity(docDir).Replace(DirRoot, "");
                     Docs[subDir + docName] = doc;
                 }
                 else
                 {
                     Docs[docName] = doc;
                 }
             }*/
            var subAssets = AssetDatabase.FindAssets("", new string[] { docAssetDir });
            List<string> docPath = new List<string>();
            foreach (var item in subAssets)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(item);
                if (Path.GetExtension(assetPath) == ".md")
                {
                    docPath.Add(assetPath);
                }
            }
            foreach (var aPath in docPath)
            {
                var rawDoc = AssetDatabase.LoadAssetAtPath(aPath, typeof(TextAsset)) as TextAsset;
                var docName = aPath.Replace(docAssetDir + "/", "").Replace(".md", "");

                var doc = new MarkDownDoc(aPath.Substring(0, aPath.LastIndexOf('/')), rawDoc.text);
                Docs[docName] = doc;
            }
        }
        Vector2 v;
        public override void OnGUI()
        {
            base.OnGUI();
            MarkDownDoc doc = null;
            if (string.IsNullOrEmpty(CurrentDocName))
            {
                if (Docs.TryGetValue("Index", out doc))
                {
                    CurrentDocName = "Index";
                }
            }
            else
            {
                doc = Docs[CurrentDocName];
            }
            if (doc != null)
            {
                ToolBar();
                v = GUILayout.BeginScrollView(v);
                EditorMarkDownDrawer.DrawDoc(doc, Search, changePage);
                GUILayout.EndScrollView();
            }
            else
            {
                EditorGUILayout.SelectableLabel("Can't find Index.md Document at :" + PathTools.RelativeAssetPath(this.GetType(), RelativePath));
                EditorGUILayout.SelectableLabel("First you should create default page");
                if (GUILayout.Button("CreateReadMe"))
                {
                    if (EditorUtility.DisplayDialog("CreateIndex.md", "After create .md file,you have to reopen this editorwindow.", "OK", "Cancel"))
                    {
                        //Template
                        string[] assets = AssetDatabase.FindAssets("TMP_ReadMe");
                        TextAsset text = AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath(assets[0]));
                        //

                        var DirRoot = PathTools.RelativeAssetPath(this.GetType(), RelativePath);
                        var FileDirRoot = Directory.GetParent(DirRoot).FullName + @"\" + Path.GetFileName(DirRoot);
                        if (!Directory.Exists(FileDirRoot))
                        {
                            Directory.CreateDirectory(FileDirRoot);
                        }

                        FileTools.WriteAllText(FileDirRoot + "/Index.md", text.text);

                        AssetDatabase.Refresh();
                        window.Close();
                    }
                }
            }
            if (EditScript)
            {
                OnGUIUtility.ScriptField("Editor Script", this.GetType());
            }
        }
        public void ToolBar()
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            if (DocPages.Count > 0 && GUILayout.Button("<", EditorStyles.toolbarButton, GUILayout.Width(20)))
            {
                CurrentDocName = DocPages.Pop();
            }
            GUILayout.Label(Title, EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
            if (SearchField)
            {
                Search.OnToolbarGUI(GUILayout.MaxWidth(160));
            }
            if (GUILayout.Button("H", EditorStyles.toolbarButton, GUILayout.Width(20)))
            {
                changePageWithDocName("Index");
            }
            if (GUILayout.Button("E", EditorStyles.toolbarButton, GUILayout.Width(20)))
            {
                var doc = Docs[CurrentDocName];
                var assetPath = doc.AssetDir + "/" + CurrentDocName + ".md";
                // assetPath = "Assets" + assetPath.Replace(Application.dataPath, "");
                var obj = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
                AssetDatabase.OpenAsset(obj);
                //Process.Start();
            }
            if (Docs.Count > 1)
            {
                if (GUILayout.Button(CurrentDocName, EditorStyles.toolbarPopup, GUILayout.Width(100)))
                {
                    GenericMenu menu = new GenericMenu();
                    foreach (var kv in Docs)
                    {
                        menu.AddItem(new GUIContent(kv.Key), false, (page) =>
                        {
                            changePageWithDocName(page as string);
                        }, kv.Key);
                    }
                    //menu.AddSeparator("");
                    menu.ShowAsContext();
                }
            }
            GUILayout.EndHorizontal();
        }
        private void changePageWithDocName(string DocName)
        {
            if (DocName == CurrentDocName) return;
            MarkDownDoc doc = null;
            if (Docs.TryGetValue(DocName, out doc))
            {
                DocPages.Push(CurrentDocName);
                CurrentDocName = DocName;
            }
        }
        private void changePage(string pageName)
        {
            var reLocatePageName = GetRelocatePageName(pageName);
            if (reLocatePageName == CurrentDocName) return;
            MarkDownDoc doc = null;
            if (Docs.TryGetValue(reLocatePageName, out doc))
            {
                DocPages.Push(CurrentDocName);
                CurrentDocName = reLocatePageName;
            }
        }
        private string GetRelocatePageName(string pageName)
        {
            var docAssetRoot = PathTools.RelativeAssetPath(this.GetType(), RelativePath);
            MarkDownDoc current = Docs[CurrentDocName];
            return (current.AssetDir + "/" + pageName).Replace(docAssetRoot + "/", "");
        }
    }
}
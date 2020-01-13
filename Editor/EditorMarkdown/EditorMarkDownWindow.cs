using SeanLib.Core;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace EditorPlus
{
    public abstract class EditorMarkDownWindow : SeanLibEditor
    {
        public virtual string ColorSettings => "Color_DocHub";
        //文档查找
        public virtual string RelativePath => "../../Doc";
        public virtual string Title => this.GetType().Name;
        public virtual bool EditScript { get { return true; } }
        public virtual bool SearchField { get { return false; } }

        public Dictionary<string, MarkDownDoc> Docs = new Dictionary<string, MarkDownDoc>();
        public Stack<string> DocPages = new Stack<string>();
        public string CurrentDocName;
        public MarkDownDoc Current => string.IsNullOrEmpty(CurrentDocName) ? null : Docs[CurrentDocName];
        #region  AutoRefresh
        public bool AutoRefresh;
        #endregion
        #region IMGUI
        MarkDownData searched;
        private OnGUIUtility.Search search = new OnGUIUtility.Search();
        public OnGUIUtility.Search Search
        {
            get
            {
                return SearchField ? search : null;
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
                if (CurrentDocName.IsNOTNullOrEmpty())
                {
                    var searching = Search.OnToolbarGUI(GUILayout.MaxWidth(160));
                    if (searching.IsNOTNullOrEmpty())
                    {
                        if (GUILayout.Button("»", EditorStyles.toolbarButton, GUILayout.Width(16)))
                        {
                            SearchNext();
                        }
                    }
                }
            }
            if (GUILayout.Button("P", EditorStyles.toolbarButton, GUILayout.Width(20)))
            {
                var doc = Docs[CurrentDocName];
                var index = CurrentDocName.LastIndexOf("/") + 1;
                var assetPath = doc.AssetDir + "/" + CurrentDocName.Substring(index, CurrentDocName.Length - index) + ".md";
                // assetPath = "Assets" + assetPath.Replace(Application.dataPath, "");
                var obj = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
                EditorGUIUtility.PingObject(obj);
            }
            if (GUILayout.Button("H", EditorStyles.toolbarButton, GUILayout.Width(20)))
            {
                changeDoc("Index");
            }
            if (GUILayout.Button("E", EditorStyles.toolbarButton, GUILayout.Width(20)))
            {
                var doc = Docs[CurrentDocName];
                var index = CurrentDocName.LastIndexOf("/") + 1;
                var assetPath = doc.AssetDir + "/" + CurrentDocName.Substring(index, CurrentDocName.Length - index) + ".md";
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
                            changeDoc(page as string);
                        }, kv.Key);
                    }
                    //menu.AddSeparator("");
                    menu.ShowAsContext();
                }
            }
            GUILayout.EndHorizontal();
        }
        Vector2 v {
            get
            {
                return Current == null ? Vector2.zero : Current.ReadingPoint;
            }
            set
            {
                if(Current!=null)
                {
                    Current.ReadingPoint = value;
                }
            }
        }
        Rect DrawRect;
        public override void OnGUI()
        {
            base.OnGUI();
            //CheckDoc
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
                if (Event.current.type == EventType.Layout)
                {
                    if(DocPages.Count>0)
                    {
                        var oldDoc = Docs[DocPages.Peek()];
                        if (oldDoc.datas.Count > 0)
                        {
                            oldDoc.Release();
                        }
                    }
                    if (doc.datas.Count == 0)
                    {
                        doc.Load();
                    }
                    else
                    {
                        if (AutoRefresh&& doc.RawDoc.text != doc.rawDoc)
                        {
                            doc.Release();
                            doc.rawDoc = doc.RawDoc.text;
                            doc.Load();
                        }
                    }
                }

            }
            //DrawDoc
            if (doc != null)
            {
                //ToolBar
                ToolBar();
                //DocContent
                EditorGUI.DrawRect(DrawRect, ColorPalette.Get(ColorSettings, "BackGround", Color.white));
                v = GUILayout.BeginScrollView(v);
                {
                    if (searched != null)
                    {
                        if (Event.current.type == EventType.Repaint)
                        {
                            EditorGUI.DrawRect(searched.drawRect, ColorPalette.Get(ColorSettings, "HightLight", Color.white));
                        }
                        if (Search.Current.IsNullOrEmpty())
                        {
                            searched = null;
                        }
                    }
                    EditorMarkDownDrawer.DrawDoc(doc, changePage, changeDoc, window.Repaint);
                }
                GUILayout.EndScrollView();
                if (Event.current.type == EventType.Repaint) { DrawRect = GUILayoutUtility.GetLastRect(); }
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
            //PingDocScript
            if (EditScript)
            {
                GUILayout.BeginHorizontal();
                {
                    AutoRefresh = GUILayout.Toggle(AutoRefresh, "自动刷新");
                    OnGUIUtility.ScriptField("Editor Script", this.GetType());
                }
                GUILayout.EndHorizontal(); 
            }

        }
        #endregion
        protected override bool UseIMGUI => true;
        protected override ElementsFileAsset FileAsset => new ElementsFileAsset()
        {
            BaseType = typeof(EditorMarkDownWindow),
            UXML = "../EditorMDDoc/EditorMDDoc.uxml",
            USS = "../EditorMDDoc/EditorMDDoc.uss",
        };
        public bool IsDocHub => window is SeanLibDocHub;
        public override void OnEnable(SeanLibManager drawer)
        {
            base.OnEnable(drawer);
            var docAssetDir = PathTools.RelativeAssetPath(this.GetType(), RelativePath);
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
                //需要手动加载
                var doc = new MarkDownDoc(aPath.Substring(0, aPath.LastIndexOf('/')), rawDoc,false);
                if(this.window is SeanLibDocHub)
                {
                    doc.ColorSetting = ColorSettings;
                }
                Docs[docName] = doc;
            }
            if(SearchField)
            {
                Search.downOrUpArrowKeyPressed += () => { SearchNext(); };
            }
        }
        public override void OnDisable()
        {
            base.OnDisable();
            foreach (var item in Docs)
            {
                item.Value.Release();
            }
            Docs.Clear();
        }
        /// <summary>
        /// 按照文档名换页
        /// </summary>
        /// <param name="DocName"></param>
        public void changeDoc(string DocName)
        {
            if (DocName.IsNullOrEmpty()||DocName == CurrentDocName) return;
            MarkDownDoc doc = null;
            if (Docs.TryGetValue(DocName, out doc))
            {
                if (!string.IsNullOrEmpty(CurrentDocName))
                {
                    DocPages.Push(CurrentDocName);
                }
                CurrentDocName = DocName;
            }
        }
        /// <summary>
        /// 按照页名换页(当前文档的相对目录)
        /// </summary>
        /// <param name="DocName"></param>
        public void changePage(string pageName)
        {
            var reLocatePageName = GetRelocatePageName(pageName);
            if (reLocatePageName == CurrentDocName) return;
            changeDoc(reLocatePageName);
        }
        private string GetRelocatePageName(string pageName)
        {
            var docAssetRoot = PathTools.RelativeAssetPath(this.GetType(), RelativePath);
            MarkDownDoc current = Docs[CurrentDocName];
            return (current.AssetDir + "/" + pageName).Replace(docAssetRoot + "/", "");
        }
        private void SearchNext()
        {
            var doc = Docs[CurrentDocName];
            var found = searched;
            bool located = found == null;
            foreach (var data in doc.datas)
            {
                var cur = data;
                if (located)
                {
                    if (Search.GeneralValid(cur.Data))
                    {
                        found = cur;
                        break;
                    }
                    foreach (var sub in cur.subdatas)
                    {
                        if (Search.GeneralValid(sub.Data))
                        {
                            found = cur;
                            goto BK;
                        }
                    }
                }
                else
                {
                    located = cur == searched;
                    if (!located)
                    {
                        foreach (var sub in cur.subdatas)
                        {
                            located = sub == searched;
                        }
                    }
                }
            }
            BK:
            searched = found == searched ? null : found;
            if (searched != null)
            {
                this.v = searched.drawRect.position.DeltaY(-position.height / 2);
            }
        }
    }
}
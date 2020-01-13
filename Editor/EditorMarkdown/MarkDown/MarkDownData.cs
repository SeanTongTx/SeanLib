using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using SeanLib.Core;

namespace EditorPlus
{
    public class MarkDownDoc
    {
        public class MDStyles
        {
            public GUIStyle Title1;
            public GUIStyle Title2;
            public GUIStyle Title3;
            public GUIStyle FontI;
            public GUIStyle FontB;
            public GUIStyle FontBI;
            public GUIStyle Link;
            public GUIStyle Font;
            public GUIStyle Area;
            public GUIStyle Page;
            public GUIStyle Doc;
            public GUIStyle Foldout;
            public MDStyles(string ColorSetting)
            {
                Title1 = new GUIStyle(OnGUIUtility.Fonts.RichText);
                Title1.fontStyle = FontStyle.Bold;
                Title1.fontSize = (int)EditorGUIUtility.singleLineHeight * 3;
                Title1.normal.textColor = ColorPalette.Get(ColorSetting, "Title1", EditorGUIUtility.isProSkin ? Color.white : Color.black);
                Title1.hover.textColor = Title1.normal.textColor;

                Title2 = new GUIStyle(OnGUIUtility.Fonts.RichText);
                Title2.fontStyle = FontStyle.Bold;
                Title2.fontSize = (int)EditorGUIUtility.singleLineHeight * 2;
                Title2.normal.textColor = ColorPalette.Get(ColorSetting, "Title2", EditorGUIUtility.isProSkin ? Color.white : Color.black);
                Title2.hover.textColor = Title2.normal.textColor;

                Title3 = new GUIStyle(OnGUIUtility.Fonts.RichText);
                Title3.fontStyle = FontStyle.Bold;
                Title3.fontSize = (int)EditorGUIUtility.singleLineHeight * 1;
                Title3.normal.textColor = ColorPalette.Get(ColorSetting, "Title3", EditorGUIUtility.isProSkin ? Color.white : Color.black);
                Title3.hover.textColor = Title3.normal.textColor;
                Title3.active.textColor= Title3.normal.textColor;

                Foldout = new GUIStyle(EditorStyles.foldout);
                Foldout.richText = true;
                Foldout.normal.textColor = ColorPalette.Get(ColorSetting, "Foldout", EditorGUIUtility.isProSkin ? Color.white : Color.black);
                Foldout.hover.textColor = Foldout.normal.textColor;
                Foldout.active.textColor = Foldout.normal.textColor;
                Foldout.focused.textColor = Foldout.normal.textColor;
                Foldout.onActive.textColor = Foldout.normal.textColor;
                Foldout.onHover.textColor = Foldout.normal.textColor;
                Foldout.onFocused.textColor = Foldout.normal.textColor;
                Foldout.onNormal.textColor = Foldout.normal.textColor;


                Page = new GUIStyle(Title3);
                Page.normal.textColor = ColorPalette.Get(ColorSetting, "Page", EditorGUIUtility.isProSkin ? Color.cyan : Color.cyan / 2);

                Doc = new GUIStyle(Title3);
                Doc.normal.textColor = ColorPalette.Get(ColorSetting, "Doc", EditorGUIUtility.isProSkin ? Color.green : Color.green / 2);

                Font = new GUIStyle(OnGUIUtility.Fonts.RichText);
                Font.alignment = TextAnchor.MiddleLeft;
                Font.normal.textColor= ColorPalette.Get(ColorSetting, "Font", EditorGUIUtility.isProSkin ? Color.white : Color.black);
                Font.hover.textColor = Font.normal.textColor;
                Font.stretchWidth = false;
                Font.wordWrap = true;

                FontI = new GUIStyle(Font);
                FontI.fontStyle = FontStyle.Italic;

                FontB = new GUIStyle(Font);
                FontB.fontStyle = FontStyle.Bold;

                FontBI = new GUIStyle(Font);
                FontBI.fontStyle = FontStyle.BoldAndItalic;

                Link = new GUIStyle("PR PrefabLabel");
                Link.normal.textColor = ColorPalette.Get(ColorSetting, "Link", EditorGUIUtility.isProSkin ? OnGUIUtility.Colors.blue_light : OnGUIUtility.Colors.blue);

                Area = new GUIStyle(EditorStyles.textArea);
                Area.richText = true;
                Area.normal.textColor = ColorPalette.Get(ColorSetting, "Area", EditorGUIUtility.isProSkin ? Color.white : Color.black);
                Area.hover.textColor = Area.normal.textColor;
                Area.active.textColor = Area.normal.textColor;
                Area.focused.textColor= Area.normal.textColor;
            }
        }
        public string AssetDir;
        public string rawDoc;
        public TextAsset RawDoc;
        public List<MarkDownData> datas = new List<MarkDownData>();
        string _ColorSetting = "Color_EditorMDDoc";
        public string ColorSetting
        {
            get
            {
                return _ColorSetting;
            }
            set
            {
                if(value!=_ColorSetting)
                {
                    _ColorSetting = value;
                    Styles = new MDStyles(_ColorSetting);
                }
            }
        }
        public MDStyles Styles = new MDStyles("Color_EditorMDDoc");
        //缓存
        public Vector2 ReadingPoint = Vector2.zero;
        public bool InFoldout = false;
        public bool CurrentFoldout = true;
        MarkDownData stackData = null;
        public MarkDownDoc(string DocDir, TextAsset rawDoc, bool autoLoad = true):this(DocDir,rawDoc.text,autoLoad)
        {
            this.RawDoc = rawDoc;
        }
        public MarkDownDoc(string DocDir, string RawDocString,bool autoLoad=true)
        {
            this.AssetDir = DocDir;
            this.rawDoc = RawDocString;
            if (autoLoad)
            {
                Load();
            }
        }
        /// <summary>
        /// 加载并生成数据
        /// </summary>
        public void Load()
        {
            if (datas.Count > 0) Release();
            string[] lines = rawDoc.Split(Environment.NewLine.ToCharArray());
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line)) continue;
                MarkDownData data = new MarkDownData() { doc = this };
                KeyType stackKey = stackData == null ? KeyType.text : stackData.type;
                switch (stackKey)
                {
                    case KeyType.code:
                        {
                            data.Load(line, this);
                            if (data.type == KeyType.code)
                            {
                                stackData = null;
                            }
                            else
                            {
                                //补上换行符
                                stackData.Data += data.Data + Environment.NewLine;
                            }
                        }
                        break;
                    case KeyType.qa:
                        {
                            data.Load(line, this);
                            if (data.type == KeyType.qa)
                            {
                                stackData = null;
                            }
                            else
                            {
                                //补上换行符
                                stackData.Data += data.Data + Environment.NewLine;
                            }
                        }
                        break;
                    default:
                        data.Load(line, this);
                        datas.Add(data);
                        switch (data.type)
                        {
                            case KeyType.code:
                                stackData = data;
                                break;
                            case KeyType.qa:
                                stackData = data;
                                break;
                        }
                        break;
                }
            }
        }
        /// <summary>
        /// 释放资源
        /// 并不释放源文件。可以在需要的时候重新加载
        /// </summary>
        public void Release()
        {
            foreach (var item in datas)
            {
                item.Release();
            }
            datas.Clear();
            stackData = null;
            InFoldout = false;
            CurrentFoldout = true;
        }
    }
    public enum KeyType
    {
        text,
        font,
        title,
        link,
        image,
        separator,
        page,
        table,
        code,
        qa,
        foldin,
        foldout,
        doc
    }
    public class MarkDownData
    {
        public KeyType type;
        public Texture2D texture;
        public GUIGifDrawer gifDrawer;
        public string keyValue = string.Empty;
        public string Data = string.Empty;
        public MarkDownDoc doc;
        public List<MarkDownData> subdatas = new List<MarkDownData>();
        public Rect drawRect;
        public void Load(string RawString, MarkDownDoc doc)
        {
            this.doc = doc;
            if (RawString.IsNullOrEmpty()) return;
            if (RawString.Replace(Environment.NewLine, "") == "***")
            {
                this.type = KeyType.separator;
            }
            else if (RawString[0] == '*')
            {
                this.type = KeyType.font;
                if (RawString.IndexOf("***") == 0)
                {
                    //斜体加粗
                    keyValue = "BI";
                    Data = RawString.Substring(3, RawString.Length - 6);
                }
                else if (RawString.IndexOf("**") == 0)
                {
                    //加粗
                    keyValue = "B";
                    Data = RawString.Substring(2, RawString.Length - 4);
                }
                else if (RawString.IndexOf("*") == 0)
                {
                    //斜体
                    keyValue = "I";
                    Data = RawString.Substring(1, RawString.Length - 2);
                }
            }
            else if (RawString[0] == '#')
            {
                this.type = KeyType.title;
                if (RawString.IndexOf("###") == 0)
                {
                    //1号标题
                    keyValue = "1";
                    Data = RawString.Substring(3, RawString.Length - 3);
                }
                else if (RawString.IndexOf("##") == 0)
                {
                    //3级标题
                    keyValue = "2";
                    Data = RawString.Substring(2, RawString.Length - 2);
                }
                else if (RawString.IndexOf("#") == 0)
                {
                    //3级标题
                    keyValue = "3";
                    Data = RawString.Substring(1, RawString.Length - 1);
                }
            }
            else if (RawString[0] == '!')
            {
                this.type = KeyType.image;
                int start = RawString.IndexOf("(") + 1;
                int end = RawString.LastIndexOf(")");
                Data = RawString.Substring(start, RawString.Length - start - (RawString.Length - end));
                var ImgAssetPath = doc.AssetDir + "/" + Data;
                if (Path.GetExtension(ImgAssetPath) == ".gif")
                {
                    gifDrawer = new GUIGifDrawer();
                    gifDrawer.LoadGIF(PathTools.Asset2File(ImgAssetPath));
                    if(EditorUserSettings.GetConfigValue("DocAutoPlayGif") != "true")
                    {
                        gifDrawer.Controller = true;
                    }
                    gifDrawer.Play();
                }
                else
                {

                    if (PathTools.IsAssetPath(ImgAssetPath))
                    {
                        this.texture =AssetDatabase.LoadAssetAtPath(ImgAssetPath, typeof(Texture2D)) as Texture2D;
                    }
                    else if (File.Exists(ImgAssetPath))
                    {
                        byte[] img = File.ReadAllBytes(ImgAssetPath);
                        this.texture = new Texture2D(1024, 1024);
                        texture.LoadImage(img);
                    }
                }
            }
            else if (RawString[0] == '(')
            {
                this.type = KeyType.link;
                int start = RawString.IndexOf("(") + 1;
                int end = RawString.LastIndexOf(")");
                Data = RawString.Substring(start, RawString.Length - start - (RawString.Length - end));
            }
            else if(RawString.IndexOf(">>")==0)
            {
                this.type = KeyType.doc;
                Data = RawString.Remove(0, 2);
            }
            else if (RawString[0] == '>')
            {
                this.type = KeyType.page;
                Data = RawString.Remove(0, 1);
            }
            else if (RawString[0] == '|')
            {
                this.type = KeyType.table;
                string[] datas = RawString.Split('|');
                for (int i = 1; i < datas.Length; i++)
                {
                    if (datas[i] == "--:")
                    {
                        this.type = KeyType.separator;
                        break;
                    }
                    MarkDownData subdata = new MarkDownData();
                    subdata.Load(datas[i], this.doc);
                    this.subdatas.Add(subdata);
                }
            }
            else if (RawString.IndexOf("```") == 0)
            {
                this.type = KeyType.code;
                keyValue = RawString.Substring(3, RawString.Length - 3);
            }
            else if (RawString.IndexOf("QA") == 0)
            {
                this.type = KeyType.qa;
                keyValue = RawString.Substring(2, RawString.Length - 2);
            }
            else if (RawString.IndexOf("/{") == 0)
            {
                this.type = KeyType.foldin;
                keyValue = RawString.Substring(2, RawString.Length - 2);
            }
            else if (RawString.IndexOf("/}") == 0)
            {
                this.type = KeyType.foldout;
                keyValue = RawString.Substring(2, RawString.Length - 2);
            }
            else
            {
                Data = RawString;
            }

        }
        public void Release()
        {
            if(gifDrawer!=null)
            {
                gifDrawer.Clean();
                gifDrawer = null;
            }
            if(texture)
            {
                texture = null;
            }
            foreach (var item in subdatas)
            {
                item.Release();
            }
            subdatas.Clear();
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace EditorPlus
{
    public class MarkDownDoc
    {
        public string AssetDir;
        public List<MarkDownData> datas = new List<MarkDownData>();
        public MarkDownDoc(string DocDir, string RawDocString)
        {
            this.AssetDir = DocDir;
            Parse(RawDocString);
        }
        MarkDownData stackData = null;
        public void Parse(string RawString)
        {
            string[] lines = RawString.Split(Environment.NewLine.ToCharArray());
            foreach (var line in lines)
            {
                MarkDownData data = new MarkDownData();
                if (string.IsNullOrEmpty(line)) continue;
                KeyType stackKey = stackData == null ? KeyType.text : stackData.type;
                switch (stackKey)
                {
                    case KeyType.code:
                        {
                            data.Parse(line, this);
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
                            data.Parse(line, this);
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
                        data.Parse(line, this);
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
    }

    public class MarkDownData
    {
        public KeyType type;
        public Texture2D texture;
        public string keyValue = string.Empty;
        public string Data = string.Empty;
        public void Parse(string RawString, MarkDownDoc doc)
        {
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
                if (PathTools.IsAssetPath(ImgAssetPath))
                {
                    this.texture = AssetDatabase.LoadAssetAtPath(ImgAssetPath, typeof(Texture2D)) as Texture2D;
                }
                else if (File.Exists(ImgAssetPath))
                {
                    byte[] img = File.ReadAllBytes(ImgAssetPath);
                    this.texture = new Texture2D(512, 512);
                    texture.LoadImage(img);
                }
            }
            else if (RawString[0] == '(')
            {
                this.type = KeyType.link;
                int start = RawString.IndexOf("(") + 1;
                int end = RawString.LastIndexOf(")");
                Data = RawString.Substring(start, RawString.Length - start - (RawString.Length - end));
            }
            else if (RawString[0] == '>')
            {
                this.type = KeyType.page;
                Data = RawString.Remove(0, 1);
            }
            else if (RawString[0] == '|')
            {
                this.type = KeyType.table;
                Data = RawString;
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
            else
            {
                Data = RawString;
            }

        }
    }
}
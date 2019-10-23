using EditorPlus;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public static class PathTools
{
    /// <summary>
    /// ../../../path
    /// </summary>
    /// <param name="type"></param>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string RelativeAssetPath(string currentPath, string relativePath)
    {
        Regex r = new Regex(@"(\.\./)");
        var matches = r.Matches(relativePath);

        int count = matches.Count;
        if (count <= 0)
        {
            return relativePath;
        }
        var paths = currentPath.Split('/');
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < paths.Length - count; i++)
        {
            sb.Append(paths[i]).Append("/");
        }
        var fronPath = sb.ToString();
        var backPath = relativePath.Replace("../", "");
        return fronPath + backPath;
    }
    public static string RelativeAssetPath(Type type, string relativePath)
    {
        return RelativeAssetPath(ScriptAssetPath(type), relativePath);
    }
    /// <summary>
    /// 类名要和文件名一致
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string ScriptAssetPath(Type type)
    {
        var assetName = type.Name + " t:script";
        string[] ans = AssetDatabase.FindAssets(assetName);
        foreach (var item in ans)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(item);
            if (Path.GetFileNameWithoutExtension(assetPath)== type.Name)
            {
                return assetPath;
            }
        }
        return null;
    }
    /// <summary>
    /// 仅对项目文件夹下的资源有效
    /// 如果资源目录是package下的话 是找不到的
    /// </summary>
    /// <param name="assetpath"></param>
    /// <returns></returns>
    public static string Asset2File(string assetpath)
    {
        if (IsAssetPath(assetpath))
        {
            var FilePath = Application.dataPath + "/" + assetpath.Replace("Assets/", "");
#if UNITY_STANDALONE_WIN
            FilePath = FilePath.Replace("/", @"\");
            return FilePath;
#else
        return FilePath;
#endif
        }
        return assetpath;
    }
    /// <summary>
    /// 仅对项目文件夹下资源有效
    /// 如果资源目录是package下的话 是找不到的
    /// </summary>
    /// <param name="filepath"></param>
    /// <returns></returns>
    public static string File2Asset(string filepath)
    {
        if (IsAssetPath(filepath))
        {
            filepath = filepath.Replace(@"\", "/");
            var AssetPath = filepath.Replace(Application.dataPath, "Assets");

#if UNITY_STANDALONE_WIN
            AssetPath = AssetPath.Replace(@"\", "/");
            return AssetPath;
#else
        return AssetPath;
#endif
        }
        return filepath;
    }
    public static bool IsAssetPath(string path)
    {
        return path.StartsWith("Assets")||path.StartsWith("Packages");
    }
    /// <summary>
    /// 获得资源在Resource中的路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetResourcePath(string path)
    {
        string assetPath = path;
        if (!IsAssetPath(path))
        {
            assetPath = (File2Asset(assetPath));
        }
        int start = assetPath.IndexOf("Resources/");
        if (start <= 0)
        {
            return null;
        }
        return Path.GetFileNameWithoutExtension(assetPath.Substring(start + 10, path.Length - start - 10));
    }
/*
    public static List<string> GetSubAssetsPath(string AssetPathDir)
    {
    }*/
}

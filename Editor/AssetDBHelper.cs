using System;
using System.Collections.Generic;
using UnityEditor;
namespace EditorPlus
{
    public static class AssetDBHelper
    {
        /// <summary>
        /// Load Searched Asset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="search"></param>
        /// <returns></returns>
        public static T LoadAsset<T>(string search) where T : UnityEngine.Object
        {
            var templatePath = AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets(search)[0]);
            var TemplateAsset = AssetDatabase.LoadAssetAtPath<T>(templatePath);
            return TemplateAsset;
        }
        public static T LoadAsset<T>(Type baseType, string relatefilePath) where T : UnityEngine.Object
        {
            var path = PathTools.RelativeAssetPath(baseType, relatefilePath);
            return AssetDatabase.LoadAssetAtPath<T>(path);
        }
        public static List<T> LoadAssets<T>(string search) where T : UnityEngine.Object
        {
            List<T> ts = new List<T>();
            var assetPathes = AssetDatabase.FindAssets(search);
            foreach (var item in assetPathes)
            {
                var path = AssetDatabase.GUIDToAssetPath(item);
                var asset = AssetDatabase.LoadAssetAtPath<T>(path);
                ts.Add(asset);
            }
            return ts;
        }
        public static bool TryLoadAsset<T>(string search, out T result) where T : UnityEngine.Object
        {
            result = null;
            var assets = AssetDatabase.FindAssets(search);
            if (assets.Length == 0)
            {
                return false;
            }
            else
            {
                var templatePath = AssetDatabase.GUIDToAssetPath(assets[0]);
                result = AssetDatabase.LoadAssetAtPath<T>(templatePath);
                return true;
            }
        }
    }
}
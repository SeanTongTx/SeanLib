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
    }
}
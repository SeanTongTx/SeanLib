using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeanLib.Core
{
    /// <summary>
    /// 键值对数值工具
    /// 使用的值数值和键数值，在位序上要求是对应的
    /// </summary>
    public static class KeyValueHelper
    {
        /// <summary>
        /// 删除值
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        /// <param name="key"></param>
        public static void DeleteKey(List<string> keys, List<string> values, string key)
        {
            int index = keys.FindIndex(e => (e == key));
            if (index >= 0)
            {
                keys.RemoveAt(index);
                values.RemoveAt(index);
            }
        }
        /// <summary>
        /// 添加值
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Add(List<string> keys, List<string> values, string key, string value)
        {
            int index = keys.FindIndex(e => (e == key));
            if (index >= 0)
            {
                values[index] = value;
            }
            else
            {
                keys.Add(key);
                values.Add(value);
            }
        }
        /// <summary>
        /// 获得值
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetValue(List<string> keys, List<string> values, string key)
        {
            int index = keys.FindIndex(e => (e == key));
            if (index >= 0)
            {
                return values[index];
            }
            return null;
        }
        /// <summary>
        /// 根据key设置值
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetValue(List<string> keys, List<string> values, string key,string value)
        {
            int index = keys.FindIndex(e => (e == key));
            if (index >= 0)
            {
                values[index]=value;
            }
        }
        /// <summary>
        /// 遍历这些键值对
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="values"></param>
        /// <param name="action"></param>
        public static void ForEachKeyValue(List<string> keys, List<string> values,Action<string, string> action)
        {
            for (int i =0; i < keys.Count; i++)
            {
                action(keys[i], values[i]);
            }
        }
    }
}
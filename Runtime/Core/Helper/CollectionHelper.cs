using System;
using System.Collections;
using System.Collections.Generic;

namespace SeanLib.Core
{
    public static class CollectionHelper
    {
        /// <summary>
        /// 获得所有满足条件的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <param name="meet"></param>
        /// <returns></returns>
        public static T[] FindAll<T>(this T[] array, Func<T, bool> meet)
        {
            List<T> list=new List<T>();
            foreach (var element in array)
            {
                if (meet(element))
                {
                    list.Add(element);
                }
            }
            return list.ToArray();
        }
        /// <summary>
        /// 获得所有满足的项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listsrc"></param>
        /// <param name="meet">条件判断</param>
        /// <returns>满足的项</returns>
        public static List<T> WhereValue<T>(this List<T> listsrc, Func<T, bool> meet)
        {
            List<T> list=new List<T>();
            foreach (var item in listsrc)
            {
                if (meet(item)) list.Add(item);
            }
            return list;
        }
        /// <summary>
        /// 获得第一个满足的项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listsrc"></param>
        /// <param name="meet">条件判断</param>
        /// <returns>满足的项</returns>
        public static T FirstOrDefaultValue<T>(this IEnumerable<T> listsrc, Func<T, bool> meet)
        {
            foreach (var item in listsrc)
            {
                if (meet(item)) return item;
            }
            return default(T);
        }
        /// <summary>
        /// 是否有值，Equals判断
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listsrc"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool AnyValue<T>(this IEnumerable<T> listsrc, T value)
        {
            foreach (var t in listsrc)
            {
                if (Equals(t, value))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 是否所有值都是，Equals判断
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listsrc"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool AllValue<T>(this IEnumerable<T> listsrc, T value)
        {
            foreach (var t in listsrc)
            {
                if (!Equals(t, value))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 是否有值，条件方法判断
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listsrc"></param>
        /// <param name="meet">条件方法</param>
        /// <returns></returns>
        public static bool AnyValue<T>(this IEnumerable<T> listsrc, Func<T, bool> meet)
        {
            foreach (var t in listsrc)
            {
                if (meet(t))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 是否所有值都是，条件方法判断
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listsrc"></param>
        /// <param name="meet">条件方法</param>
        /// <returns></returns>
        public static bool AllValue<T>(this IEnumerable<T> listsrc, Func<T, bool> meet)
        {
            foreach (var t in listsrc)
            {
                if (!meet(t))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 是否有值，条件方法判断
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listsrc"></param>
        /// <param name="meet">条件方法</param>
        /// <returns></returns>
        public static bool ContainsValue<T>(this IEnumerable<T> listsrc, Func<T, bool> meet)
        {
            foreach (var t in listsrc)
            {
                if (meet(t))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 是否有值，Equals判断
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listsrc"></param>
        /// <returns></returns>
        public static bool ContainsValue<T>(this IEnumerable<T> listsrc, T value)
        {
            foreach (var t in listsrc)
            {
                if (Equals(t, value))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 是否有值，Equals判断
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listsrc"></param>
        /// <returns></returns>
        public static bool ContainsValue<T>(this T[] listsrc, T value)
        {
            foreach (var t in listsrc)
            {
                if (Equals(t, value))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 浅拷贝到另一个List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listsrc"></param>
        /// <returns></returns>
        public static List<T> CopyList<T>(this List<T> listsrc)
        {
            List<T> list = new List<T>();
            for (int i = 0; i < listsrc.Count; i++)
            {
                list.Add(listsrc[i]);
            }
            return list;
        }
        /// <summary>
        /// 浅拷贝到另一个List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listsrc"></param>
        /// <returns></returns>
        public static List<T> CopyIEnumerableToList<T>(this IEnumerable<T> listsrc)
        {
            List<T> list = new List<T>();
            foreach (var item in listsrc)
            {
                list.Add(item);
            }
            return list;
        }
        /// <summary>
        /// 移除指定位序的元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <param name="indexs">位序组</param>
        public static void RemoveAts<T>(this List<T> src, List<int> indexs)
        {
            indexs.Sort();
            for (int i = 0; i < indexs.Count; i++)
            {
                indexs[i] = indexs[i] - i;
            }
            for (int i = 0; i < indexs.Count; i++)
            {
                src.RemoveAt(indexs[i]);
            }
        }
        /// <summary>
        /// 合并到新的数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <param name="src2"></param>
        /// <returns></returns>
        public static T[] Add<T>(this T[] src, T[] src2)
        {
            T[] strs = new T[src.Length + src2.Length];

            int index = 0;
            for (int i = 0; i < src.Length; i++)
            {
                strs[index++] = src[i];
            }
            for (int i = 0; i < src2.Length; i++)
            {
                strs[index++] = src2[i];
            }
            return strs;
        }
        /// <summary>
        /// 追加一些值
        /// </summary>
        /// <param name="target"></param>
        /// <param name="src"></param>
        public static void Append(this IDictionary target, IDictionary src)
        {
            foreach (object key in src.Keys)
            {
                try
                {
                    target.Add(key, src[key]);
                }
                catch
                {

                }
            }
        }

        /// <summary>
        /// 在this，却不在subtrahend的key
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TValueB"></typeparam>
        /// <param name="target"></param>
        /// <param name="subtrahend"></param>
        /// <returns></returns>
        public static List<TKey> Subtract<TKey, TValue, TValueB>(this Dictionary<TKey, TValue> target, Dictionary<TKey, TValueB> subtrahend)
        {
            List<TKey> list = new List<TKey>();
            foreach (var key in target.Keys)
            {
                if (!subtrahend.ContainsKey(key))
                    list.Add(key);
            }
            return list;
        }

        /// <summary>
        /// 在this，也在other的key
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TValueB"></typeparam>
        /// <param name="target"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static List<TKey> Intersection<TKey, TValue, TValueB>(this Dictionary<TKey, TValue> target, Dictionary<TKey, TValueB> other)
        {
            List<TKey> list = new List<TKey>();
            foreach (var key in target.Keys)
            {
                if (other.ContainsKey(key))
                    list.Add(key);
            }
            return list;
        }

        /// <summary>
        /// 在this中寻找也在contrast，但是值不同的key
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TValueB"></typeparam>
        /// <param name="target"></param>
        /// <param name="contrast"></param>
        /// <returns></returns>
        public static List<TKey> DifferentThan<TKey, TValue, TValueB>(this Dictionary<TKey, TValue> target, Dictionary<TKey, TValueB> contrast)
        {
            List<TKey> list = new List<TKey>();
            foreach (var key in target.Keys)
            {
                if (contrast.ContainsKey(key) && contrast[key].Equals(target[key]))
                    list.Add(key);
            }
            return list;
        }
        /// <summary>
        /// 对比数组，获得哪些内容被添加，删除，和没变化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oldList"></param>
        /// <param name="newList"></param>
        /// <param name="addList">newList里新增加的内容</param>
        /// <param name="deleteList">newList里被删除的内容</param>
        /// <param name="holdList">newList里没有变化的内容</param>
        /// <param name="isEqual">判断是同一个元素</param>
        public static void GetChangeList<T>(List<T> oldList, List<T> newList, out List<T> addList,
            out List<T> deleteList, out List<T> holdList,Func<T,T,bool> isEqual)
        {
            addList=new List<T>();
            deleteList=new List<T>();
            holdList=new List<T>();
            foreach (var oldElm in oldList)
            {
                bool found = newList.Count != 0 && newList.AnyValue(newElm => isEqual(oldElm, newElm));
                if(found) holdList.Add(oldElm);
                else deleteList.Add(oldElm);
            }
            foreach (var newElm in newList)
            {
                bool found = oldList.Count != 0 && oldList.AnyValue(oldElm => isEqual(oldElm, newElm));
                if (!found) addList.Add(newElm);
            }
        }
        /// <summary>
        /// 获得第一个key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static T GetFirstKey<T, V>(this IDictionary<T, V> dic)
        {
            foreach (var key in dic.Keys)
            {
                return key;
            }
            return default(T);
        }
        /// <summary>
        /// 获得第一个value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dic"></param>
        /// <returns></returns>
        public static V GetFirstValue<T, V>(this IDictionary<T, V> dic)
        {
            foreach (var value in dic.Values)
            {
                return value;
            }
            return default(V);
        }
    }
}

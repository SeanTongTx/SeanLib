using System;
using System.Collections.Generic;

namespace SeanLib.Core
{

    /// <summary>
    /// 简单的对象池
    /// 线程不安全
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPool<T> where T : class
    {
        public Dictionary<T, bool> objectStore = new Dictionary<T, bool>();
        public Func<T,T> CreateFunc;
        public Action<T> ResetAction;
        public Action<T> ClearAction;

        public Func<T,T,bool> SelectFunc; 
        public int Capacity = 100;
        public ObjectPool(Func<T,T> createFunc, Action<T> reset, Action<T> clear=null)
        {
            CreateFunc = createFunc;
            ResetAction = reset;
            ClearAction = clear;
        }

        public List<T> GetUsingObjects()
        {
            List<T> ret = new List<T>();
           var enumerator= objectStore.GetEnumerator();
            while(enumerator.MoveNext())
            {
                ret.Add(enumerator.Current.Key);
            }
            return ret;
        }
        public T Withdraw(T template = default(T))
        {
            T objectCanUse = default(T);

            var enumerator = objectStore.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Value == false && (this.SelectFunc != null ? this.SelectFunc(enumerator.Current.Key, template) : true))
                {
                    objectCanUse = enumerator.Current.Key;
                    break;
                }
            }
            if (objectCanUse == default(T))
            {
                if (objectStore.Count >= Capacity)
                {
                    ClearNoUse();
                }
                if (objectStore.Count >= Capacity)
                {
                    return objectCanUse;
                }
                objectCanUse = CreateFunc(template);
            }
            if (objectCanUse != default(T))
            {
                objectStore[objectCanUse] = true;
            }
            return objectCanUse;
        }

        public void Reclaim()
        {
            List<T> Deletobject = new List<T>();
            foreach (KeyValuePair<T, bool> keyValuePair in objectStore)
            {
                Deletobject.Add(keyValuePair.Key);
            }
            foreach (T t in Deletobject)
            {
                Store(t);
            }
        }
        public void Store(T obj)
        {
            if (objectStore.Count >= Capacity)
            {
                ClearNoUse();
            }
            ResetAction(obj);
            objectStore[obj] = false;
        }

        public void ClearNoUse()
        {
            List<T> Deletobject = new List<T>();
            foreach (KeyValuePair<T, bool> valuePair in objectStore)
            {
                if (valuePair.Value == false)
                {
                    Deletobject.Add(valuePair.Key);
                }
            }
            foreach (T t in Deletobject)
            {
                objectStore.Remove(t);
                if (ClearAction != null)
                {
                    ClearAction(t);
                }
            }
            Deletobject.Clear();
        }
    }

}

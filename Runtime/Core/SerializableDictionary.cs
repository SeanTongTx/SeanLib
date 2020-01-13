

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface ISerializableDictionary{ }
public class IEnumerator<TKey, TValue> : IEnumerator<KeyValuePair<TKey, TValue>>
{
    SerializableDictionary<TKey, TValue> dic;
    int index=-1;

    public IEnumerator(SerializableDictionary<TKey, TValue> dic)
    {
        this.dic = dic;
    }

    public KeyValuePair<TKey, TValue> Current => new KeyValuePair<TKey, TValue>((dic.Keys as List<TKey>)[index], (dic.Values as List<TValue>)[index]);

    object IEnumerator.Current => Current;
    public void Dispose()
    {
        throw new System.NotImplementedException();
    }

    public bool MoveNext()
    {
        index += 1;
        return dic.Count > index;
    }

    public void Reset()
    {
        index = -1;
    }
}
public abstract class SerializableDictionary<TKey,TValue>:IDictionary<TKey,TValue>, ISerializableDictionary
{
    [SerializeField]
    private List<TKey> keys = new List<TKey>();
    [SerializeField]
    private List<TValue> values = new List<TValue>();
    private Dictionary<TKey, int> _index = new Dictionary<TKey, int>();
    public TValue this[TKey key]
    {
        get
        {
            CheckCache();
            return values[_index[key]];
        }
        set
        {
            Add(key, value);
        }
    }
    private void CheckCache()
    {
        if(_index.Count!=keys.Count)
        {
            _index.Clear();
            for (int i = 0; i < keys.Count; i++)
            {
                _index[keys[i]] = i;
            }
        }
    }

    public ICollection<TKey> Keys => keys;

    public ICollection<TValue> Values => values;

    public int Count => _index.Count;

    public bool IsReadOnly => throw new System.NotImplementedException();

    public void Add(TKey key, TValue value)
    {
        CheckCache();
        _index[key] = keys.Count;
        keys.Add(key);
        values.Add(value);
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    public void Clear()
    {
        _index.Clear();
        keys.Clear();
        values.Clear();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        CheckCache();
        return _index.ContainsKey(item.Key);
    }

    public bool ContainsKey(TKey key)
    {
        CheckCache();
        return _index.ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        CheckCache();
        return new IEnumerator<TKey, TValue>(this);
    }

    public bool Remove(TKey key)
    {
        CheckCache();
        int i = -1;
        if(_index.TryGetValue(key,out i))
        {
            values.RemoveAt(i);
            keys.RemoveAt(i);
            return true;
        }
        return false;
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
       return Remove(item.Key);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        CheckCache();
        int i = -1;
        if (_index.TryGetValue(key, out i))
        {
            value = values[i];
            return true;
        }
        value = default(TValue);
        return false;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
[Serializable]
public class StringString: SerializableDictionary<string,string>
{
}
[Serializable]
public class StringInt : SerializableDictionary<string, int>
{
}
[Serializable]
public class IntString : SerializableDictionary<int, string>
{
}
[Serializable]
public class StringColor : SerializableDictionary<string, Color>
{
}
#pragma warning disable CS1591

using System;
using System.Collections.Generic;

namespace SystemExtensionLib.Utils;

public class OrderedDoubleStringKeyDictionary<TValue>
{
    private readonly Dictionary<(string, string), TValue> dict = new();

    private readonly List<(string, string)> keys = new();

    public void Add(string key1, string key2, TValue value)
    {
        var pair = (key1, key2);
        if (dict.ContainsKey(pair))
            throw new ArgumentException("Key already exists");

        dict[pair] = value;
        keys.Add(pair);
    }

    public TValue this[string key1, string key2]
    {
        get => dict[(key1, key2)];
        set
        {
            var pair = (key1, key2);
            if (!dict.ContainsKey(pair))
                keys.Add(pair);
            dict[pair] = value;
        }
    }

    public TValue this[int index] => dict[keys[index]];

    public IEnumerable<KeyValuePair<(string, string), TValue>> GetOrderedItems()
    {
        foreach (var k in keys)
            yield return new KeyValuePair<(string, string), TValue>(k, dict[k]);
    }

    public IEnumerable<TValue> GetValuesByFirstKey(string key1)
    {
        foreach (var pair in keys)
        {
            if (pair.Item1 == key1)
                yield return dict[pair];
        }
    }

    public IEnumerable<TValue> GetValuesBySecondKey(string key2)
    {
        foreach (var pair in keys)
        {
            if (pair.Item2 == key2)
                yield return dict[pair];
        }
    }

    public bool ContainsKey(string key1, string key2) => dict.ContainsKey((key1, key2));

    public bool ContainsValue(TValue value) => dict.ContainsValue(value);

    public bool Remove(string key1, string key2)
    {
        var pair = (key1, key2);
        if (!dict.Remove(pair)) return false;
        keys.Remove(pair);
        return true;
    }

    public int IndexOf(string key1, string key2)
    {
        var pair = (key1, key2);
        return keys.IndexOf(pair);
    }

    public void Insert(int index, string key1, string key2, TValue value)
    {
        var pair = (key1, key2);
        if (dict.ContainsKey(pair))
            throw new ArgumentException("Key already exists");

        dict[pair] = value;
        keys.Insert(index, pair);
    }

    public int Count => keys.Count;
}

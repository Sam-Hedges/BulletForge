using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

// Empty base class to be used with Unity's serialization system.
public class SerializableDictionary
{
}

/// <summary>
/// A dictionary that can be serialized with Unity's built-in system. This class provides functionality to bridge
/// Unity's serialization mechanism with the generic dictionary pattern.
/// </summary>
/// <typeparam name="TKey">The type of keys in the dictionary. This type needs to be serializable by Unity.</typeparam>
/// <typeparam name="TValue">The type of values in the dictionary. This type also needs to be serializable by Unity.</typeparam>
[Serializable]
public class SerializableDictionary<TKey, TValue> : SerializableDictionary, IDictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    // A list that stores key-value pairs for serialization.
    [SerializeField]
    private List<SerializableKeyValuePair> list = new List<SerializableKeyValuePair>();

    // Represents a serializable key-value pair.
    [Serializable]
    public struct SerializableKeyValuePair
    {
        public TKey Key;
        public TValue Value;

        public SerializableKeyValuePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        // Sets the value for this pair.
        public void SetValue(TValue value)
        {
            Value = value;
        }
    }

    // A lazy-loaded dictionary mapping keys to their positions in the list.
    private Dictionary<TKey, uint> KeyPositions => _keyPositions.Value;
    private Lazy<Dictionary<TKey, uint>> _keyPositions;

    // Default constructor.
    public SerializableDictionary()
    {
        _keyPositions = new Lazy<Dictionary<TKey, uint>>(MakeKeyPositions);
    }

    // Constructor that initializes the dictionary from another dictionary.
    public SerializableDictionary(IDictionary<TKey, TValue> dictionary)
    {
        _keyPositions = new Lazy<Dictionary<TKey, uint>>(MakeKeyPositions);

        if (dictionary == null)
        {
            throw new ArgumentException("The passed dictionary is null.");
        }

        foreach (KeyValuePair<TKey, TValue> pair in dictionary)
        {
            Add(pair.Key, pair.Value);
        }
    }

    // Creates the key-positions dictionary from the list.
    private Dictionary<TKey, uint> MakeKeyPositions()
    {
        int numEntries = list.Count;
        Dictionary<TKey, uint> result = new Dictionary<TKey, uint>(numEntries);

        for (int i = 0; i < numEntries; ++i)
        {
            result[list[i].Key] = (uint) i;
        }

        return result;
    }

    // Required for Unity's serialization system but unused.
    public void OnBeforeSerialize()
    {
    }

    // Callback after Unity's deserialization. Refreshes the key positions.
    public void OnAfterDeserialize()
    {
        _keyPositions = new Lazy<Dictionary<TKey, uint>>(MakeKeyPositions);
    }

    #region IDictionary

    // Indexer to get or set items within the dictionary.
    public TValue this[TKey key]
    {
        get => list[(int) KeyPositions[key]].Value;
        set
        {
            if (KeyPositions.TryGetValue(key, out uint index))
            {
                list[(int) index].SetValue(value);
            }
            else
            {
                KeyPositions[key] = (uint) list.Count;
                list.Add(new SerializableKeyValuePair(key, value));
            }
        }
    }

    public ICollection<TKey> Keys => list.Select(tuple => tuple.Key).ToArray();
    public ICollection<TValue> Values => list.Select(tuple => tuple.Value).ToArray();

    // Adds a key-value pair to the dictionary.
    public void Add(TKey key, TValue value)
    {
        if (KeyPositions.ContainsKey(key))
        {
            throw new ArgumentException("An element with the same key already exists in the dictionary.");
        }
        else
        {
            KeyPositions[key] = (uint) list.Count;
            list.Add(new SerializableKeyValuePair(key, value));
        }
    }

    // Checks if the dictionary contains a specific key.
    public bool ContainsKey(TKey key) => KeyPositions.ContainsKey(key);

    // Removes an item with the specified key from the dictionary.
    public bool Remove(TKey key)
    {
        if (KeyPositions.TryGetValue(key, out uint index))
        {
            Dictionary<TKey, uint> kp = KeyPositions;

            kp.Remove(key);
            list.RemoveAt((int) index);

            int numEntries = list.Count;
            for (uint i = index; i < numEntries; i++)
            {
                kp[list[(int) i].Key] = i;
            }

            return true;
        }

        return false;
    }

    // Tries to get the value associated with the specified key.
    public bool TryGetValue(TKey key, out TValue value)
    {
        if (KeyPositions.TryGetValue(key, out uint index))
        {
            value = list[(int) index].Value;
            return true;
        }

        value = default;
        return false;
    }

    #endregion

    #region ICollection

    // Gets the number of elements in the dictionary.
    public int Count => list.Count;
    public bool IsReadOnly => false;

    // Adds a KeyValuePair to the dictionary.
    public void Add(KeyValuePair<TKey, TValue> kvp) => Add(kvp.Key, kvp.Value);

    // Removes all items from the dictionary.
    public void Clear()
    {
        list.Clear();
        KeyPositions.Clear();
    }

    // Checks if the dictionary contains a specific key-value pair.
    public bool Contains(KeyValuePair<TKey, TValue> kvp) => KeyPositions.ContainsKey(kvp.Key);

    // Copies the dictionary elements to an array starting at a specific array index.
    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        int numKeys = list.Count;
        if (array.Length - arrayIndex < numKeys)
        {
            throw new ArgumentException("arrayIndex");
        }

        for (int i = 0; i < numKeys; ++i, ++arrayIndex)
        {
            SerializableKeyValuePair entry = list[i];
            array[arrayIndex] = new KeyValuePair<TKey, TValue>(entry.Key, entry.Value);
        }
    }

    // Removes a specific key-value pair from the dictionary.
    public bool Remove(KeyValuePair<TKey, TValue> kvp) => Remove(kvp.Key);
    
    #endregion

    #region IEnumerable

    // Returns an enumerator for the dictionary.
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return list.Select(ToKeyValuePair).GetEnumerator();

        KeyValuePair<TKey, TValue> ToKeyValuePair(SerializableKeyValuePair skvp)
        {
            return new KeyValuePair<TKey, TValue>(skvp.Key, skvp.Value);
        }
    }

    // Returns an enumerator for the dictionary (non-generic version).
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    #endregion
}
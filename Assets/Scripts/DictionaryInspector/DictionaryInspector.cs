using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DictionaryInspector<TKey, TValue> : Dictionary<TKey,TValue>, ISerializationCallbackReceiver
{
    [SerializeField] private List<DictionaryCell> dictionaryCells = new List<DictionaryCell>();
    
    void ISerializationCallbackReceiver.OnBeforeSerialize() { }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        Clear();
        foreach (var cell in dictionaryCells)
            this[cell.key] = cell.value;
    }

    [Serializable]
    private struct DictionaryCell
    {
        public TKey key;
        public TValue value;

        public DictionaryCell(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }
    }
}
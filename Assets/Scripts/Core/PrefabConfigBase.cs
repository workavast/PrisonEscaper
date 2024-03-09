using System;
using System.Collections.Generic;
using SerializableDictionaryExtension;
using UnityEngine;

namespace Core
{
    public class PrefabConfigBase<TEnum, TBase> : ScriptableObject
        where TEnum : Enum
    {
        [SerializeField] private SerializableDictionary<TEnum, TBase> data;

        public IReadOnlyDictionary<TEnum, TBase> Data => data;
    }
}
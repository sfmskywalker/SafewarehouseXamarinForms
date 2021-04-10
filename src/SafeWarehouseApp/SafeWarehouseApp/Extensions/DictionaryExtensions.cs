using System;
using System.Collections.Generic;

namespace SafeWarehouseApp.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue? TryGet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue>? defaultValueProvider = default) =>
            !dictionary.ContainsKey(key) ? defaultValueProvider != null ? defaultValueProvider() : default : dictionary[key];
    }
}